using MassTransit;
using System.Diagnostics;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Messaging.Models;

namespace TrafficCourts.Workflow.Service.Sagas;

public partial class VerifyEmailAddressStateMachine : MassTransitStateMachine<VerifyEmailAddressState>
{
    private readonly ILogger<VerifyEmailAddressStateMachine> _logger;
    private readonly TimeProvider _clock;

    #region States
    
    /// <summary>
    /// Indicates the email verification process is in progress for this dispute.
    /// </summary>
    public State Active { get; private set; }

    #endregion

    #region Events
    
    public Event<DisputeCreated> DisputeCreatedEvent { get; private set; }
    public Event<RequestEmailVerification> RequestEmailVerificationEvent { get; private set; }
    public Event<CheckEmailVerificationTokenRequest> CheckEmailVerificationTokenRequestEvent { get; private set; }
    public Event<ResendEmailVerificationEmail> ResendEmailVerificationEmailEvent { get; private set; }
    
    #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public VerifyEmailAddressStateMachine(TimeProvider clock, ILogger<VerifyEmailAddressStateMachine> logger)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InstanceState(x => x.CurrentState);

        // map the message properties to message correlation id
        Event(() => DisputeCreatedEvent, x => x.CorrelateById(context => context.Message.NoticeOfDisputeGuid));
        Event(() => RequestEmailVerificationEvent, x => x.CorrelateById(context => context.Message.NoticeOfDisputeGuid));
        Event(() => ResendEmailVerificationEmailEvent, x => x.CorrelateById(context => context.Message.NoticeOfDisputeGuid));

        Event(() => CheckEmailVerificationTokenRequestEvent, x =>
        {
            x.CorrelateById(context => context.Message.NoticeOfDisputeGuid);
            x.OnMissingInstance(m => m.ExecuteAsync(context =>
            {
                LogNotFound(context);
                return SendResponse(context, CheckEmailVerificationTokenStatus.NotFound, _clock.GetUtcNow());
            }));
        });

        // once a dispute has been created, set the saga to the active state
        Initially(
            When(DisputeCreatedEvent)
                .Then(CreateTokenAndSendVerificationEmail)
                .TransitionTo(Active));

        // while active, support email address validation
        During(Active,
            When(RequestEmailVerificationEvent)
                .Then(RecreateTokenIfRequired)
                .ThenAsync(SendVerificationEmail),
            When(ResendEmailVerificationEmailEvent)
                .ThenAsync(SendVerificationEmail),
            When(CheckEmailVerificationTokenRequestEvent)
                .ThenAsync(CheckToken)
        );

        // TODO: determine events that should finalize and delete this instance

        SetCompletedWhenFinalized();
    }

    private async void CreateTokenAndSendVerificationEmail(BehaviorContext<VerifyEmailAddressState, DisputeCreated> context)
    {
        LogSubmitNoticeOfDispute(context);

        var state = context.Saga;

        // always save the ticket number
        state.TicketNumber = context.Message.TicketNumber;
        state.Token = Guid.NewGuid(); // always generate a unique id, even if not given an email
        state.IsUpdateEmailVerification = false;

        // cant send email address, if one is not supplied, they may have opted out of email communications
        if (string.IsNullOrEmpty(context.Message.EmailAddress))
        {
            LogNoEmailAddress(context);
            return;
        }

        state.EmailAddress = context.Message.EmailAddress;

        // TCVP-1529 Saving a dispute should send a verification email to the Disputant.
        await SendVerificationEmail(context);
    }

    /// <summary>
    /// Recreates the email verification token if the email address has changed since the validation
    /// process started.
    /// </summary>
    private void RecreateTokenIfRequired(BehaviorContext<VerifyEmailAddressState, RequestEmailVerification> context)
    {
        var state = context.Saga;

        if (!string.Equals(state.EmailAddress, context.Message.EmailAddress, StringComparison.OrdinalIgnoreCase))
        {
            LogEmailAddressChanged(context);

            // email address changed, use a new token
            state.EmailAddress = context.Message.EmailAddress;
            state.IsUpdateEmailVerification = context.Message.IsUpdateEmailVerification;
            state.Token = Guid.NewGuid();
        }
    }

    private async Task SendVerificationEmail<TMessage>(BehaviorContext<VerifyEmailAddressState, TMessage> context) where TMessage : class
    {
        var state = context.Saga;

        var message = new SendEmailVerificationEmail
        {
            NoticeOfDisputeGuid = state.CorrelationId,
            EmailAddress = state.EmailAddress!,
            TicketNumber = state.TicketNumber!,
            Token = state.Token
        };

        LogSendingEmail(message);

        await context.Publish(message, context.CancellationToken);
    }

    private async Task CheckToken(BehaviorContext<VerifyEmailAddressState, CheckEmailVerificationTokenRequest> context)
    {
        // save the start time cause the time on both messages below should be the same
        DateTimeOffset now = _clock.GetUtcNow();

        var state = context.Saga;

        if (context.Message.Token != state.Token)
        {
            await SendResponse(context, CheckEmailVerificationTokenStatus.Invalid, now);
            return;
        }

        state.Verified = true;
        state.VerifiedAt = now;

        // respond the request to check the token
        await SendResponse(context, CheckEmailVerificationTokenStatus.Valid, now);

        await PublishEmailVerificationSuccessful(context);
    }

    private async Task PublishEmailVerificationSuccessful<TMessage>(BehaviorContext<VerifyEmailAddressState, TMessage> context) where TMessage : class
    {
        var state = context.Saga;

        Debug.Assert(state.Verified);
        Debug.Assert(state.VerifiedAt is not null);

        var message = new EmailVerificationSuccessful
        {
            NoticeOfDisputeGuid = state.CorrelationId,
            TicketNumber = state.TicketNumber!,
            EmailAddress = state.EmailAddress!,
            VerifiedAt = state.VerifiedAt.Value,
            IsUpdateEmailVerification = state.IsUpdateEmailVerification
        };

        await context.Publish(message, context.CancellationToken).ConfigureAwait(false);
    }

    private async Task SendResponse(ConsumeContext<CheckEmailVerificationTokenRequest> context, CheckEmailVerificationTokenStatus status, DateTimeOffset when)
    {
        var message = new CheckEmailVerificationTokenResponse
        {
            CheckedAt = when,
            Status = status
        };

        await context.RespondAsync(message).ConfigureAwait(false);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "No email associated with dispute. The disputant may have opted out of email communications, will not send email verification", EventName = "NoEmailAddress")]
    private partial void LogNoEmailAddress(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        BehaviorContext<VerifyEmailAddressState, DisputeCreated> context);

    [LoggerMessage(Level = LogLevel.Information, Message = "Notice of dispute is being submitted", EventName = "DisputeCreated")]
    private partial void LogSubmitNoticeOfDispute(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        BehaviorContext<VerifyEmailAddressState, DisputeCreated> context);

    [LoggerMessage(Level = LogLevel.Information, Message = "Resend email verification", EventName = "ResendEmailVerificationEmail")]
    private partial void Log(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        BehaviorContext<VerifyEmailAddressState, ResendEmailVerificationEmail> context);

    [LoggerMessage(Level = LogLevel.Information, Message = "Count not find saga instance")]
    private partial void LogNotFound(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        ConsumeContext<CheckEmailVerificationTokenRequest> context);

    [LoggerMessage(Level = LogLevel.Information, Message = "Email address changed on email verification request, generating new token")]
    private partial void LogEmailAddressChanged(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        BehaviorContext<VerifyEmailAddressState, RequestEmailVerification> context);

    [LoggerMessage(Level = LogLevel.Information, Message = "Sending email to disputant")]
    private partial void LogSendingEmail(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        SendEmailVerificationEmail message);
}

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
    /// Indicates the email verification process is  in progress for this dispute.
    /// </summary>
    public State Active { get; private set; }

    #endregion

    #region Events
    /// <summary>
    /// Raised when the initial notice of dispute is submitted.
    /// </summary>
    public Event<SubmitNoticeOfDispute> SubmitNoticeOfDispute { get; private set; }

    /// <summary>
    /// Raised when sending email verification is requested, will create a new token if required and request
    /// an email to be set to the disputant.
    /// </summary>
    public Event<RequestEmailVerification> RequestEmailVerification { get; private set; }
    /// <summary>
    /// Raised when sending email verification email could not be completed.
    /// </summary>
    public Event<SendEmailVerificationFailed> SendEmailVerificationFailed { get; private set; }

    public Event<CheckEmailVerificationTokenRequest> CheckEmailVerificationToken { get; private set; }

    public Event<NoticeOfDisputeSubmitted> NoticeOfDisputeSubmitted { get; private set; }

    public Event<ResendEmailVerificationEmail> ResendEmailVerificationEmail { get; private set; }
    #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public VerifyEmailAddressStateMachine(TimeProvider clock, ILogger<VerifyEmailAddressStateMachine> logger)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InstanceState(x => x.CurrentState);

        // map the message properties to message correlation id
        Event(() => SubmitNoticeOfDispute, x => x.CorrelateById(context => context.Message.NoticeOfDisputeGuid));
        Event(() => NoticeOfDisputeSubmitted, x => x.CorrelateById(context => context.Message.NoticeOfDisputeGuid));
        Event(() => RequestEmailVerification, x => x.CorrelateById(context => context.Message.NoticeOfDisputeGuid));
        Event(() => ResendEmailVerificationEmail, x => x.CorrelateById(context => context.Message.NoticeOfDisputeGuid));
        Event(() => SendEmailVerificationFailed, x => x.CorrelateById(context => context.Message.NoticeOfDisputeGuid));

        Event(() => CheckEmailVerificationToken, x =>
        {
            x.CorrelateById(context => context.Message.NoticeOfDisputeGuid);
            x.OnMissingInstance(m => m.ExecuteAsync(context =>
            {
                LogNotFound(context);
                return SendResponse(context, CheckEmailVerificationTokenStatus.NotFound);
            }));
        });

        Initially(
            // when ever a new notice of dispute is requested to be submitted, create the instance
            When(SubmitNoticeOfDispute)
                .Then(Log)
                .Then(CreateTokenAndSendVerificationEmail)
                .TransitionTo(Active));

        During(Active,
            When(RequestEmailVerification)
                .Then(Log)
                .Then(RecreateTokenIfRequired)
                .ThenAsync(SendVerificationEmail),
            When(ResendEmailVerificationEmail)
                .Then(Log)
                .ThenAsync(SendVerificationEmail),
            When(SendEmailVerificationFailed)
                .Then(Log),
            When(CheckEmailVerificationToken)
                .Then(Log)
                .ThenAsync(CheckToken),
            When(NoticeOfDisputeSubmitted)
                .Then(Log)
                .ThenAsync(HandleNoticeOfDisputeSubmitted)
        );

        // TODO: determine events that should finalize and delete this instance

        SetCompletedWhenFinalized();
    }

    private async void CreateTokenAndSendVerificationEmail(BehaviorContext<VerifyEmailAddressState, SubmitNoticeOfDispute> context)
    {
        var state = context.Saga;

        // always save the ticket number
        state.TicketNumber = context.Message.TicketNumber;

        // cant send email address, if one is not supplied, they may have opted out of email communications
        if (string.IsNullOrEmpty(context.Message.EmailAddress))
        {
            LogNoEmailAddress(context);
            return;
        }

        state.EmailAddress = context.Message.EmailAddress;

        state.IsUpdateEmailVerification = false;
        state.Token = Guid.NewGuid();

        // TCVP-1529 Saving a dispute should send a verification email to the Disputant.
        await SendVerificationEmail(context);
    }

    private void CreateToken(BehaviorContext<VerifyEmailAddressState, RequestEmailVerification> context)
    {
        var state = context.Saga;
        state.TicketNumber = context.Message.TicketNumber;
        state.EmailAddress = context.Message.EmailAddress;
        state.IsUpdateEmailVerification = context.Message.IsUpdateEmailVerification;
        state.Token = Guid.NewGuid();
    }

    /// <summary>
    /// Recreates the email verification token if the email address has changed since the validation
    /// process started.
    /// </summary>
    private void RecreateTokenIfRequired(BehaviorContext<VerifyEmailAddressState, RequestEmailVerification> context)
    {
        if (!string.Equals(context.Saga.EmailAddress, context.Message.EmailAddress, StringComparison.OrdinalIgnoreCase))
        {
            CreateToken(context); // email address changed, use a new token
        }
    }

    private async Task SendVerificationEmail<TMessage>(BehaviorContext<VerifyEmailAddressState, TMessage> context) where TMessage : class
    {
        var state = context.Saga;

        await context.PublishWithLog(_logger, new SendEmailVerificationEmail
        {
            NoticeOfDisputeGuid = state.NoticeOfDisputeGuid,
            EmailAddress = state.EmailAddress!,
            TicketNumber = state.TicketNumber!,
            Token = state.Token
        }, context.CancellationToken);
    }

    private async Task CheckToken(BehaviorContext<VerifyEmailAddressState, CheckEmailVerificationTokenRequest> context)
    {
        // save the start time cause the time on both messages below should be the same
        DateTimeOffset now = _clock.GetUtcNow();

        var state = context.Saga;

        if (context.Message.Token != state.Token)
        {
            await SendResponse(context, false, now);
            return;
        }

        state.Verified = true;
        state.VerifiedAt = now;

        // respond the request to check the token
        await SendResponse(context, true, now);

        await PublishEmailVerificationSuccessful(context);
    }

    private async Task HandleNoticeOfDisputeSubmitted(BehaviorContext<VerifyEmailAddressState, NoticeOfDisputeSubmitted> context)
    {
        if (context.Saga.Verified)
        {
            await PublishEmailVerificationSuccessful(context);
        }
    }

    private async Task PublishEmailVerificationSuccessful<TMessage>(BehaviorContext<VerifyEmailAddressState, TMessage> context) where TMessage : class
    {
        var state = context.Saga;

        Debug.Assert(state.Verified);
        Debug.Assert(state.VerifiedAt is not null);

        await context.PublishWithLog(_logger, new EmailVerificationSuccessful
        {
            NoticeOfDisputeGuid = state.NoticeOfDisputeGuid,
            TicketNumber = state.TicketNumber!,
            EmailAddress = state.EmailAddress!,
            VerifiedAt = state.VerifiedAt.Value,
            IsUpdateEmailVerification = state.IsUpdateEmailVerification
        }, context.CancellationToken).ConfigureAwait(false);

    }

    private async Task SendResponse(ConsumeContext<CheckEmailVerificationTokenRequest> context, CheckEmailVerificationTokenStatus status)
    {
        await context.RespondAsync(new CheckEmailVerificationTokenResponse
        {
            CheckedAt = _clock.GetUtcNow(),
            Status = status
        }).ConfigureAwait(false);
    }

    private async Task SendResponse(BehaviorContext<VerifyEmailAddressState, CheckEmailVerificationTokenRequest> context, bool valid, DateTimeOffset when)
    {
        await context.RespondAsync(new CheckEmailVerificationTokenResponse
        {
            CheckedAt = when,
            Status = valid ? CheckEmailVerificationTokenStatus.Valid : CheckEmailVerificationTokenStatus.Invalid
        }).ConfigureAwait(false);
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "No email associated with dispute. The disputant may have opted out of email communications, will not send email verification")]
    private partial void LogNoEmailAddress(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        BehaviorContext<VerifyEmailAddressState, SubmitNoticeOfDispute> context);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Notice of dispute is being submitted")]
    private partial void Log(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        BehaviorContext<VerifyEmailAddressState, SubmitNoticeOfDispute> context);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Email verification started")]
    private partial void Log(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        BehaviorContext<VerifyEmailAddressState, RequestEmailVerification> context);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Resend email verification")]
    private partial void Log(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        BehaviorContext<VerifyEmailAddressState, ResendEmailVerificationEmail> context);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Checking verification token")] 
    private partial void Log(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        BehaviorContext<VerifyEmailAddressState, CheckEmailVerificationTokenRequest> context);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Notice of dispute submitted and requires email verification")]
    private partial void Log(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        BehaviorContext<VerifyEmailAddressState, NoticeOfDisputeSubmitted> context);

    [LoggerMessage(Level = LogLevel.Information, Message = "Send email verification failed")]
    private partial void Log(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        BehaviorContext<VerifyEmailAddressState, SendEmailVerificationFailed> context);

    [LoggerMessage(Level = LogLevel.Information, Message = "Count not find saga instance")]
    private partial void LogNotFound(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        ConsumeContext<CheckEmailVerificationTokenRequest> context);
}

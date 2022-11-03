using MassTransit;
using NodaTime;
using System.Diagnostics;
using System.Security.Cryptography;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Messaging.Models;

namespace TrafficCourts.Workflow.Service.Sagas;

public class VerifyEmailAddressSagaStateMachine : MassTransitStateMachine<VerifyEmailAddressSagaState>
{
    private readonly ILogger<VerifyEmailAddressSagaStateMachine> _logger;
    private readonly IClock _clock;

    #region States
    /// <summary>
    /// Indicates the email verification process is  in progress for this dispute.
    /// </summary>
    public State Active { get; private set; }

    #endregion

    #region Events
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
    public Event<EmailVerificationSuccessful> EmailVerificationSuccessful { get; private set; }

    public Event<NoticeOfDisputeSubmitted> NoticeOfDisputeSubmitted { get; private set; }

    public Event<ResendEmailVerificationEmail> ResendEmailVerificationEmail { get; private set; }
    #endregion

    public VerifyEmailAddressSagaStateMachine(IClock clock, ILogger<VerifyEmailAddressSagaStateMachine> logger)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InstanceState(state => state.CurrentState, Active);

        // TODO: CorrelationId is the key in Redis, so need to ensure the CorrelationId used is unique for this saga

        Event(() => RequestEmailVerification, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));
        Event(() => ResendEmailVerificationEmail, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));
        Event(() => SendEmailVerificationFailed, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));
        Event(() => EmailVerificationSuccessful, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));
        Event(() => NoticeOfDisputeSubmitted, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));

        Event(() => CheckEmailVerificationToken, x =>
        {
            x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid));
            x.OnMissingInstance(m => m.ExecuteAsync(context =>
            {
                _logger.LogInformation("Count not find an instance for {NoticeOfDisputeGuid}", context.Message.NoticeOfDisputeGuid);
                return SendResponse(context, CheckEmailVerificationTokenStatus.NotFound);
            }));
        });

        Initially(
            When(RequestEmailVerification)
                .Then(context => _logger.LogDebug("Email verification started"))
                .Then(CreateToken)
                .ThenAsync(SendVerificationEmail)
                .TransitionTo(Active),
            When(NoticeOfDisputeSubmitted)
                .If(context => context.Message.RequiresEmailVerification,
                    x => x.Then(context => _logger.LogDebug("Notice of dispute submitted and requires email verification"))
                          .ThenAsync(HandleNoticeOfDisputeSubmitted)
                          .TransitionTo(Active))
        );

        During(Active,
            When(ResendEmailVerificationEmail)
                .Then(context => _logger.LogDebug("Resend email verification"))
                .ThenAsync(SendVerificationEmail),
            When(RequestEmailVerification)
                .Then(context => _logger.LogDebug("Email verification restarted"))
                .Then(RecreateTokenIfRequired)
                .ThenAsync(SendVerificationEmail),
            When(SendEmailVerificationFailed)
                .Then(context => _logger.LogInformation("Send email verification failed {Reason}", context.Message.Reason)),
            When(CheckEmailVerificationToken)
                .Then(context => _logger.LogDebug("Checking verification token"))
                .ThenAsync(CheckToken),
            When(NoticeOfDisputeSubmitted)
                .Then(context => _logger.LogDebug("Notice of dispute submitted and requires email verification"))
                .ThenAsync(HandleNoticeOfDisputeSubmitted),
            When(EmailVerificationSuccessful)
                .Then(context => _logger.LogDebug("Email verification completed"))
        );

        // TODO: determine events that should finalize and delete this instance

        SetCompletedWhenFinalized();
    }

    /// <summary>
    /// Creates a state machine specific correlation id based on the incoming notice of dispute id.
    /// This is required because redis persisence uses the correlationId as the key to store state.
    /// Two different sagas using the same key would clobber each other's state.
    /// </summary>
    /// <typeparam name="TStateMachine"></typeparam>
    /// <param name="NoticeOfDisputeGuid"></param>
    /// <returns></returns>
    internal static Guid ToCorrelationId(Guid NoticeOfDisputeGuid)
    {
        var buffer = new byte[16 * 2];

        Array.Copy(SagaGuid.ToByteArray(), 0, buffer, 0, 16);
        Array.Copy(NoticeOfDisputeGuid.ToByteArray(), 0, buffer, 16, 16);

        Guid correlationId = new Guid(MD5.HashData(buffer));
        return correlationId;
    }

    /// <summary>
    /// Saga specific guid to combine with the notice of dispute id to create a unique saga instance
    /// This value must not be used by other sagas. This value must not be changed once state has
    /// been persisted otherwise the existing state will be orphaned.
    /// </summary>
    private static readonly Guid SagaGuid = new Guid("0e413bec-0794-4a54-8501-443264a0c74b");

    private void CreateToken(BehaviorContext<VerifyEmailAddressSagaState, RequestEmailVerification> context)
    {
        var state = context.Saga;
        state.NoticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid;
        state.EmailAddress = context.Message.EmailAddress;
        state.TicketNumber = context.Message.TicketNumber;
        state.Token = Guid.NewGuid();

        if (context.Message.DisputeId is not null)
        {
            if (state.DisputeId is null)
            {
                state.DisputeId = context.Message.DisputeId;
            }
            else if (state.DisputeId != context.Message.DisputeId)
            {
                // dont allow changing of the dispute id
                _logger.LogWarning("Cannot change dispute id, current = {DisputeId}, requested = {RequestedDisputeId}. Dispute id will not be changed",
                    state.DisputeId, context.Message.DisputeId);
            }
        }
    }

    /// <summary>
    /// Recreates the email verification token if the email address has changed since the validation
    /// process started.
    /// </summary>
    private void RecreateTokenIfRequired(BehaviorContext<VerifyEmailAddressSagaState, RequestEmailVerification> context)
    {
        if (!string.Equals(context.Saga.EmailAddress, context.Message.EmailAddress, StringComparison.OrdinalIgnoreCase))
        {
            CreateToken(context); // email address changed, use a new token
        }
    }

    private async Task SendVerificationEmail(BehaviorContext<VerifyEmailAddressSagaState, RequestEmailVerification> context)
    {
        var state = context.Saga;

        await context.PublishWithLog(_logger, new SendEmailVerificationEmail
        {
            NoticeOfDisputeGuid = state.NoticeOfDisputeGuid,
            EmailAddress = state.EmailAddress,
            TicketNumber = state.TicketNumber,
            Token = state.Token
        }, context.CancellationToken);
    }

    private async Task SendVerificationEmail(BehaviorContext<VerifyEmailAddressSagaState, ResendEmailVerificationEmail> context)
    {
        var state = context.Saga;

        await context.PublishWithLog(_logger, new SendEmailVerificationEmail
        {
            NoticeOfDisputeGuid = state.NoticeOfDisputeGuid,
            EmailAddress = state.EmailAddress,
            TicketNumber = state.TicketNumber,
            Token = state.Token
        }, context.CancellationToken);
    }
    private async Task CheckToken(BehaviorContext<VerifyEmailAddressSagaState, CheckEmailVerificationTokenRequest> context)
    {
        // save the start time cause the time on both messages below should be the same
        DateTimeOffset now = _clock.GetCurrentInstant().ToDateTimeOffset();

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

        // only send EmailVerificationSuccessful after we have
        // confirmation the Notice of dispute has been submitted
        if (state.DisputeId is not null)
        {
            await PublishEmailVerificationSuccessful(context, state.DisputeId.Value);
        }
    }

    private async Task HandleNoticeOfDisputeSubmitted(BehaviorContext<VerifyEmailAddressSagaState, NoticeOfDisputeSubmitted> context)
    {
        var state = context.Saga;

        state.DisputeId = context.Message.DisputeId;

        if (state.Verified)
        {
            await PublishEmailVerificationSuccessful(context, state.DisputeId.Value);
        }
    }

    private async Task PublishEmailVerificationSuccessful<TMessage>(BehaviorContext<VerifyEmailAddressSagaState, TMessage> context, long disputeId) where TMessage : class
    {
        var state = context.Saga;

        Debug.Assert(state.Verified);
        Debug.Assert(state.VerifiedAt is not null);

        await context.PublishWithLog(_logger, new EmailVerificationSuccessful
        {
            DisputeId = disputeId,
            NoticeOfDisputeGuid = state.NoticeOfDisputeGuid,
            TicketNumber = state.TicketNumber,
            EmailAddress = state.EmailAddress,
            VerifiedAt = state.VerifiedAt.Value
        }, context.CancellationToken);

    }

    private async Task SendResponse(ConsumeContext<CheckEmailVerificationTokenRequest> context, CheckEmailVerificationTokenStatus status)
    {
        await context.RespondAsync(new CheckEmailVerificationTokenResponse
        {
            CheckedAt = _clock.GetCurrentInstant().ToDateTimeOffset(),
            Status = status
        });
    }

    private async Task SendResponse(BehaviorContext<VerifyEmailAddressSagaState, CheckEmailVerificationTokenRequest> context, bool valid, DateTimeOffset when)
    {
        await context.RespondAsync(new CheckEmailVerificationTokenResponse
        {
            CheckedAt = when,
            Status = valid ? CheckEmailVerificationTokenStatus.Valid : CheckEmailVerificationTokenStatus.Invalid
        });
    }
}

using MassTransit;
using NodaTime;
using System.Diagnostics;
using System.Security.Cryptography;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Messaging.Models;

namespace TrafficCourts.Workflow.Service.Sagas;

public class DisputeUpdateRequestSagaStateMachine : MassTransitStateMachine<DisputeUpdateRequestSagaState>
{
    private readonly ILogger<DisputeUpdateRequestSagaStateMachine> _logger;
    private readonly IClock _clock;

    #region States
    /// <summary>
    /// Indicates the email verification process is  in progress for this dispute.
    /// </summary>
    public State Active { get; private set; }

    #endregion

    #region Events
    /// <summary>
    /// Raised when disputant has been authenticated to retrieve a dispute for update
    /// </summary>
    public Event<DisputeUpdateRequestGetDispute> DisputeUpdateRequestGetDispute { get; private set; }

    /// <summary>
    /// Raised when checking the update request token
    /// </summary>
    public Event<CheckDisputeUpdateRequestToken> CheckDisputeUpdateRequestToken { get; private set; }

    /// <summary>
    /// Raised when the dispute update request for authorization has been granted
    /// </summary>
    public Event<DisputeUpdateRequestSuccessful> DisputeUpdateRequestSuccessful { get; private set; }

    /// <summary>
    /// Raised when the disputant has been authorized to submit a dispute update request
    /// </summary>
    public Event<DisputeUpdateRequestSubmitted> DisputeUpdateRequestSubmitted { get; private set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DisputeUpdateRequestSagaStateMachine(IClock clock, ILogger<DisputeUpdateRequestSagaStateMachine> logger)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InstanceState(state => state.CurrentState, Active);

        // TODO: CorrelationId is the key in Redis, so need to ensure the CorrelationId used is unique for this saga

        Event(() => DisputeUpdateRequestGetDispute, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));
        Event(() => DisputeUpdateRequestSubmitted, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));

        Event(() => CheckDisputeUpdateRequestToken, x =>
        {
            x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid));
            x.OnMissingInstance(m => m.ExecuteAsync(context =>
            {
                _logger.LogInformation("Count not find an instance for {NoticeOfDisputeGuid}", context.Message.NoticeOfDisputeGuid);
                return SendResponse(context, CheckDisputeUpdateRequestTokenStatus.NotFound);
            }));
        });

        Initially(
            When(DisputeUpdateRequestGetDispute)
                .Then(context => _logger.LogDebug("Dispute update request saga started"))
                .Then(CreateToken)
                .TransitionTo(Active)
        );

        During(Active,
            When(CheckDisputeUpdateRequestToken)
                .Then(context => _logger.LogDebug("Checking dispute update request verification token"))
                .ThenAsync(CheckToken),
            When(DisputeUpdateRequestSubmitted)
                .Then(context => _logger.LogDebug("Dispute update request submitted and requires access granted to save"))
                .ThenAsync(HandleDisputeUpdateRequestSubmitted),
            When(DisputeUpdateRequestSuccessful)
                .Then(context => _logger.LogDebug("Dispute update request saga completed"))
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
    private static readonly Guid SagaGuid = new Guid("1ed3adc4-0cbe-41ea-ba71-f7f434d07cdc");

    private void CreateToken(BehaviorContext<DisputeUpdateRequestSagaState, DisputeUpdateRequestGetDispute> context)
    {
        var state = context.Saga;
        state.NoticeOfDisputeGuid = context.Message.NoticeOfDisputeGuid;
        state.TicketNumber = context.Message.TicketNumber;
        state.DisputeId = context.Message.DisputeId;
        state.BCSCToken = context.Message.BCSCToken;
        state.Token = Guid.NewGuid();

        if (state.DisputeId != context.Message.DisputeId)
        {
            // dont allow changing of the dispute id
            _logger.LogWarning("Cannot change dispute id, current = {DisputeId}, requested = {RequestedDisputeId}. Dispute id will not be changed",
                state.DisputeId, context.Message.DisputeId);
        }
    }

    private async Task SendVerificationEmail(BehaviorContext<DisputeUpdateRequestSagaState, DisputeUpdateAuthorizationRequest> context)
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

    private async Task SendVerificationEmail(BehaviorContext<DisputeUpdateRequestSagaState, ResendEmailVerificationEmail> context)
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
    private async Task CheckToken(BehaviorContext<DisputeUpdateRequestSagaState, CheckDisputeUpdateRequestTokenRequest> context)
    {
        // save the start time cause the time on both messages below should be the same
        DateTimeOffset now = _clock.GetCurrentInstant().ToDateTimeOffset();

        var state = context.Saga;

        if (context.Message.Token != state.Token)
        {
            await SendResponse(context, false, now);
            return;
        }

        state.DisputeUpdateRequestAccessGranted = true;
        state.DisputeUpdateRequestAccessGrantedAt = now;

        // respond the request to check the token
        await SendResponse(context, true, now);
        await PublishDisputeUpdateRequestSuccessful(context, state.Token);
    }

    private async Task HandleDisputeUpdateRequestSubmitted(BehaviorContext<DisputeUpdateRequestSagaState, DisputeUpdateRequestSubmitted> context)
    {
        var state = context.Saga;

        state.Token = context.CancellationToken;???

        if (state.DisputeUpdateRequestAccessGranted)
        {
            await PublishDisputeUpdateRequestSuccessful(context, state.Token);
        }
    }

    private async Task PublishDisputeUpdateRequestSuccessful<TMessage>(BehaviorContext<DisputeUpdateRequestSagaState, TMessage> context, Guid token) where TMessage : class
    {
        var state = context.Saga;

        Debug.Assert(state.DisputeUpdateRequestAccessGranted);
        Debug.Assert(state.DisputeUpdateRequestAccessGrantedAt is not null);

        await context.PublishWithLog(_logger, new DisputeUpdateRequestSuccessful
        {
            DisputeId = disputeId,
            NoticeOfDisputeGuid = state.NoticeOfDisputeGuid,
            TicketNumber = state.TicketNumber,
            EmailAddress = state.EmailAddress,
            VerifiedAt = state.VerifiedAt.Value,
            IsUpdateEmailVerification = state.IsUpdateEmailVerification
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

    private async Task SendResponse(BehaviorContext<DisputeUpdateRequestSagaState, DisputeUpdateRequestToken> context, bool valid, DateTimeOffset when)
    {
        await context.RespondAsync(new DisputeUpdateRequestTokenResponse
        {
            CheckedAt = when,
            Status = valid ? CheckDisputeUpdateRequestTokenStatus.Valid : CheckDisputeUpdateRequestTokenStatus.Invalid
        });
    }
}

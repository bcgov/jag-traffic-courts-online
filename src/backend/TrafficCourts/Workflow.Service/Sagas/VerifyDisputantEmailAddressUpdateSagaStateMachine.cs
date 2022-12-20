using MassTransit;
using NodaTime;
using System.Diagnostics;
using System.Security.Cryptography;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Messaging.Models;

namespace TrafficCourts.Workflow.Service.Sagas;

public class VerifyDisputantEmailAddressUpdateSagaStateMachine : MassTransitStateMachine<VerifyDisputantEmailAddressUpdateSagaState>
{
    private readonly ILogger<VerifyDisputantEmailAddressUpdateSagaStateMachine> _logger;

    #region States
    /// <summary>
    /// Indicates the email verification process is in progress for this update request.
    /// </summary>
    public State Active { get; private set; }

    #endregion

    #region Events
    public Event<UpdateEmailVerificationSuccessful> EmailVerificationSuccessful { get; private set; }

    public Event<DisputantUpdateRequestSubmitted> DisputantUpdateRequestSubmitted { get; private set; }
    #endregion

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public VerifyDisputantEmailAddressUpdateSagaStateMachine(ILogger<VerifyDisputantEmailAddressUpdateSagaStateMachine> logger)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InstanceState(state => state.CurrentState, Active);

        // TODO: CorrelationId is the key in Redis, so need to ensure the CorrelationId used is unique for this saga

        Event(() => EmailVerificationSuccessful, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));
        Event(() => DisputantUpdateRequestSubmitted, x => x.CorrelateById(context => ToCorrelationId(context.Message.NoticeOfDisputeGuid)));

        Initially(
            When(DisputantUpdateRequestSubmitted)
                .If(context => context.Message.RequiresEmailVerification,
                    x => x.Then(context => _logger.LogDebug("Disputant update request submitted and requires email verification"))
                          .ThenAsync(HandleDisputantUpdateRequestSubmitted)
                          .TransitionTo(Active))
        );

        During(Active,
            When(DisputantUpdateRequestSubmitted)
                .Then(context => _logger.LogDebug("Disputant update request submitted and requires email verification"))
                .ThenAsync(HandleDisputantUpdateRequestSubmitted),
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
    private static readonly Guid SagaGuid = new Guid("7856cc8d-68e8-4811-8077-775605d146e4");

    private async Task HandleDisputantUpdateRequestSubmitted(BehaviorContext<VerifyDisputantEmailAddressUpdateSagaState, DisputantUpdateRequestSubmitted> context)
    {
        var state = context.Saga;

        state.DisputeId = context.Message.DisputeId;

        if (state.Verified)
        {
            await PublishEmailVerificationSuccessful(context, state.DisputeId.Value);
        }
    }

    private async Task PublishEmailVerificationSuccessful<TMessage>(BehaviorContext<VerifyDisputantEmailAddressUpdateSagaState, TMessage> context, long disputeId) where TMessage : class
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
            VerifiedAt = state.VerifiedAt.Value,
            IsUpdateEmailVerification = state.IsUpdateEmailVerification
        }, context.CancellationToken);

    }
}

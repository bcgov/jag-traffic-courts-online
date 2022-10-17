using MassTransit;

namespace TrafficCourts.Workflow.Service.Sagas;

/// <summary>
/// Represents the common state machine state for Redis based Sagas.
/// </summary>
public abstract class BaseStateMachineState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public int Version { get; set; }
}

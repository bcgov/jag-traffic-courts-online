namespace TrafficCourts.Coms.Client;

public abstract class TimestampUserData
{
    /// <summary>
    /// The subject id of the current user if request was authenticated with a Bearer token (ex. JWT), or a 'nil' uuid if request was authenticated via Basic auth
    /// </summary>
    public Guid CreatedBy { get; set; } = default!;

    /// <summary>
    /// Time when this record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = default!;

    /// <summary>
    /// The subject id of the current user if request was authenticated with a Bearer token (ex. JWT), or a 'nil' uuid if request was authenticated via Basic auth
    /// </summary>
    public Guid UpdatedBy { get; set; } = default!;

    /// <summary>
    /// Time when this record was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; } = default!;
}

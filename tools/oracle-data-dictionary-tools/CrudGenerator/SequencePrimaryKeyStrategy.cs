namespace CrudGenerator;

/// <summary>
/// Use a sequence to generate the primary key
/// </summary>
public class SequencePrimaryKeyStrategy : PrimaryKeyStrategy
{
    public SequencePrimaryKeyStrategy(string sequence)
    {
        Sequence = sequence;
    }
    public string Sequence { get; }
}

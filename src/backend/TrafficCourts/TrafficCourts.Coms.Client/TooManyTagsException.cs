namespace TrafficCourts.Coms.Client;


/// <summary>
/// The exception that is thrown when there are too many tags. There can only a total of 10 tags.
/// </summary>
public class TooManyTagsException : TagException
{
    public int TagCount { get; private set; }

    public TooManyTagsException(int count) : base("Too many tags supplied. You can only have up to 10 tags") 
    { 
        TagCount = count;
    }
}

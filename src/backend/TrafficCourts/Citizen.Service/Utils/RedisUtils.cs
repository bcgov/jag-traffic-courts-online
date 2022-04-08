using StackExchange.Redis;
using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Utils;

public class RedisUtils
{

    public static Statute ToStatute(HashEntry[] fields)
    {
        // Note this does not work - results in C# error: Evaluation of native methods in this context is not supported.
        // fields.FirstOrDefault(x => x.Name == "act").Value

        // Manually find and extract all key/value pairs and map to Statute property
        decimal code = 0;
        string act = "";
        string section = "";
        string description = "";

        foreach (var field in fields)
        {
            if ("code".Equals(field.Name))
            {
                code = Convert.ToDecimal(field.Value);
            }
            else if ("act".Equals(field.Name))
            {
                act = field.Value;
            }
            else if ("section".Equals(field.Name))
            {
                section = field.Value;
            }
            else if ("description".Equals(field.Name))
            {
                description = field.Value;
            }
        }

        return new(code, act, section, description);
    }

}

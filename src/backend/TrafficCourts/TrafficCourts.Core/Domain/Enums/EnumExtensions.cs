using System.ComponentModel;
using System.Reflection;

namespace TrafficCourts.Domain.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            // Get the type of the enum
            Type type = value.GetType();

            // Get the member information for the specific enum value
            MemberInfo[] memberInfo = type.GetMember(value.ToString());

            if (memberInfo.Length > 0)
            {
                // Look for the DescriptionAttribute on that member
                var attribute = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();

                if (attribute != null)
                {
                    return attribute.Description;
                }
            }

            // Fallback to standard string representation if no attribute exists
            return value.ToString();
        }
    }
}

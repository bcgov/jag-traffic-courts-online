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

        /// <summary>
        /// Finds the enum value whose description (see <see cref="GetDescription(Enum)"/>) exactly matches <paramref name="description"/>.
        /// </summary>
        public static bool TryParseDescription<TEnum>(string? description, out TEnum value) where TEnum : struct, Enum
        {
            foreach (TEnum candidate in Enum.GetValues<TEnum>())
            {
                if (string.Equals(candidate.GetDescription(), description, StringComparison.Ordinal))
                {
                    value = candidate;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}

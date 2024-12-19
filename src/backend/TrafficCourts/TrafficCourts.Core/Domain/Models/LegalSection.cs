using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TrafficCourts.Domain.Models
{
    /// <summary>
    /// Represents a Legistics Paragraphing.
    /// </summary>
    /// <remarks>
    /// Paragraphing is a typological device for arranging legislative text. It involves dividing 
    /// a sentence into grammatical units and arranging them as separate blocks of text.
    /// <see cref="https://www.justice.gc.ca/eng/rp-pr/csj-sjc/legis-redact/legistics/p3p1.html"/>
    /// </remarks>
    public class LegalSection
    {
        public string Section { get; private set; } = string.Empty;
        public string Subsection { get; private set; } = string.Empty;
        public string Paragraph { get; private set; } = string.Empty;
        public string Subparagraph { get; private set; } = string.Empty;

        private static readonly string[] _separator = ["(", ")"];

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            if (Section != string.Empty)
            {
                buffer.Append(Section); // 123
            }

            if (Subsection != string.Empty)
            {
                buffer.Append($"({Subsection})"); // (1)
            }

            if (Paragraph != string.Empty)
            {
                buffer.Append($"({Paragraph})"); // (a)
            }

            if (Subparagraph != string.Empty)
            {
                buffer.Append($"({Subparagraph})"); // (i)
            }

            return buffer.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">A string containing a legal section to parse.</param>
        /// <param name="legalSection">Will be not null </param>
        /// <exception cref="System.ArgumentNullException"><paramref name="s"/> is null</exception>
        /// <returns><c>true</c> if <paramref name="s"/>s was parsed successfully; otherwise, false.</returns>
        public static bool TryParse(string s, [NotNullWhen(true)] out LegalSection? legalSection)
        {
            ArgumentNullException.ThrowIfNull(s);

            s = s.Trim();

            if (s.Length != 0)
            {
                if (char.IsDigit(s[0]))
                {
                    var parts = s.Split(_separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    // have to have at least a Section
                    if (parts.Length >= 1)
                    {
                        LegalSection result = new()
                        {
                            Section = parts[0]
                        };

                        if (parts.Length >= 2)
                        {
                            // Subsection is optional and always a digit if specified
                            if (char.IsDigit(parts[1][0]))
                            {
                                result.Subsection = parts[1];
                            }
                            else
                            {
                                // no subsection, so push the remaining fields into paragraph and subparagraph
                                result.Paragraph = parts[1];
                                if (parts.Length >= 3)
                                {
                                    result.Subparagraph = parts[2];
                                }

                                legalSection = result;
                                return true;
                            }

                            if (parts.Length >= 3)
                            {
                                result.Paragraph = parts[2];

                                if (parts.Length >= 4)
                                    result.Subparagraph = parts[3];
                            }
                        }

                        legalSection = result;
                        return true;
                    }
                }
            }

            legalSection = default;
            return false;
        }
    }
}

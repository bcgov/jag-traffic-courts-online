using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common
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
        public string Subparagrah { get; private set; } = string.Empty;

        private static readonly string[] _separator = ["(", ")"];

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Section) && !string.IsNullOrEmpty(Subsection) && !string.IsNullOrEmpty(Paragraph) && !string.IsNullOrEmpty(Subparagrah))
            {
                return $"{Section}({Subsection})({Paragraph})({Subparagrah})";
            }
            if (!string.IsNullOrEmpty(Section) && !string.IsNullOrEmpty(Subsection) && !string.IsNullOrEmpty(Paragraph))
            {
                return $"{Section}({Subsection})({Paragraph})";
            }
            else if (!string.IsNullOrEmpty(Section) && !string.IsNullOrEmpty(Subsection))
            {
                return $"{Section}({Subsection})";
            }
            else if (!string.IsNullOrEmpty(Section))
            {
                return Section;
            }
            else
            {
                return string.Empty;
            }
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
                            result.Subsection = parts[1];

                            if (parts.Length >= 3)
                            {
                                result.Paragraph = parts[2];

                                if (parts.Length >= 4)
                                    result.Subparagrah = parts[3];
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

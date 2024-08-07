namespace TrafficCourts.Domain.Models;

public class LegalSectionTest
{
    [Fact]
    public void DefaultValues()
    {
        LegalSection actual = new LegalSection();
        Assert.Equal(string.Empty, actual.Section);
        Assert.Equal(string.Empty, actual.Subsection);
        Assert.Equal(string.Empty, actual.Paragraph);
        Assert.Equal(string.Empty, actual.Subparagrah);
    }

    [Theory]
    [InlineData("4.21", "4.21", "", "", "")]            // section only
    [InlineData("4.21(5)", "4.21", "5", "", "")]        // section and subsection only
    [InlineData("4.21(5)(a)(i)", "4.21", "5", "a", "i")]// full
    [InlineData(" 4.21(5)", "4.21", "5", "", "")]       // leading space
    [InlineData("4.21(5) ", "4.21", "5", "", "")]       // trailing space
    [InlineData("4.21 (5)", "4.21", "5", "", "")]       // internal space
    public void TryParseWithValidInput(string formatted, string section, string subsection, string paragraph, string subparagrah)
    {
        Assert.True(LegalSection.TryParse(formatted, out LegalSection? actual));
        Assert.NotNull(actual);
        Assert.Equal(section, actual.Section);
        Assert.Equal(subsection, actual.Subsection);
        Assert.Equal(paragraph, actual.Paragraph);
        Assert.Equal(subparagrah, actual.Subparagrah);
    }

    [Theory]
    [InlineData("")]            // empty string
    [InlineData(" ")]            // just whitespace
    [InlineData("a4.21(5)")]    // must start with a digit
    public void TryParseWithInvalidInput(string formatted)
    {
        Assert.False(LegalSection.TryParse(formatted, out LegalSection? actual));
        Assert.Null(actual);
    }

    [Theory]
    [InlineData("4.21")]            // section
    [InlineData("4.21(5)")]         // section, subsection
    [InlineData("4.21(5)(a)")]      // section, subsection paragraph
    [InlineData("4.21(5)(a)(i)")]   // section, subsection, paragraph and subparagrah
    public void ToStringFormatsCorrectly(string formatted)
    {
        Assert.True(LegalSection.TryParse(formatted, out LegalSection? actual));
        Assert.Equal(formatted, actual.ToString());
    }

    [Fact]
    public void TryParseThrowsWhenPassedNull()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => LegalSection.TryParse(null!, out LegalSection? actual));
        Assert.Equal("s", actual.ParamName);
    }
}

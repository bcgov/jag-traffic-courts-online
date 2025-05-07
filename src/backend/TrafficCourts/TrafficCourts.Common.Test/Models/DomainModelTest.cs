using System.Text;
using Xunit;

namespace TrafficCourts.Common.Test.Models;

public class DomainModelTest
{
    [Theory]
    [InlineData(123, "ACT", "10", null, null, null, "10", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", "1", null, null, "10(1)", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", "1", "a", null, "10(1)(a)", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", "1", "a", "i", "10(1)(a)(i)", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", null, "a", "i", "10(a)(i)", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", null, null, "i", "10(i)", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", null, null, null, "10", null, "Full Description")]
    [InlineData(123, "ACT", "10", null, null, null, "10", "Short Description", null)]
    [InlineData(123, "ACT", "10", "", null, null, "10", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", " ", null, null, "10", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", null, "", null, "10", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", null, " ", null, "10", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", null, null, "", "10", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", null, null, " ", "10", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", "", "", "", "10", "Short Description", "Full Description")]
    [InlineData(123, "ACT", "10", " ", " ", " ", "10", "Short Description", "Full Description")]
    public void ToDomainModel_ValidInput_ReturnsExpectedModel(
    int statId,
    string actCd,
    string section,
    string? subSection,
    string? paragraph,
    string? subParagraph,
    string expectedFormattedCode,
    string? shortDescription,
    string? fullDescription)
    {
        // Arrange
        var statute = new TrafficCourts.OrdsDataService.Justin.Statute
        {
            stat_id = statId,
            act_cd = actCd,
            stat_section_txt = section,
            stat_sub_section_txt = subSection!,
            stat_paragraph_txt = paragraph,
            stat_sub_paragraph_txt = subParagraph,
            stat_short_description_txt = shortDescription!,
            stat_description_txt = fullDescription!
        };

        var buffer = new StringBuilder();

        // Act
        var result = statute.ToDomainModel(buffer);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statId.ToString(), result.Id);
        Assert.Equal(actCd, result.ActCode);
        Assert.Equal(section, result.SectionText);
        Assert.Equal(subSection ?? string.Empty, result.SubsectionText);
        Assert.Equal(paragraph ?? string.Empty, result.ParagraphText);
        Assert.Equal(subParagraph ?? string.Empty, result.SubparagraphText);
        Assert.Equal(expectedFormattedCode, result.Code);
        Assert.Equal(shortDescription ?? fullDescription, result.ShortDescriptionText);
        Assert.Equal(fullDescription, result.DescriptionText);
    }
}

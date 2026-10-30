using System.ComponentModel;
using TrafficCourts.Domain.Enums;

namespace TrafficCourts.Core.Test
{
    public class EnumExtensionsTests
    {
        private enum TestEnum
        {
            [Description("With Description")]
            WithDescription = 1,

            WithoutDescription = 2,
        }

        public static TheoryData<UploadedDocumentType> AllUploadedDocumentTypes => new(Enum.GetValues<UploadedDocumentType>());

        // These values are persisted in COMS document metadata, changing them will break matching on existing documents.
        [Theory]
        [InlineData(UploadedDocumentType.Other, "Other")]
        [InlineData(UploadedDocumentType.Adjournment, "Adjournment")]
        [InlineData(UploadedDocumentType.WrittenReasons, "Written Reasons")]
        [InlineData(UploadedDocumentType.CertifiedExtract, "Certified Extract")]
        public void UploadedDocumentType_GetDescription_ShouldReturnPersistedValue(UploadedDocumentType value, string expected)
        {
            Assert.Equal(expected, value.GetDescription());
        }

        [Fact]
        public void UploadedDocumentType_Descriptions_ShouldBeUnique()
        {
            var descriptions = Enum.GetValues<UploadedDocumentType>().Select(x => x.GetDescription()).ToList();

            Assert.Equal(descriptions.Count, descriptions.Distinct(StringComparer.Ordinal).Count());
        }

        [Theory]
        [MemberData(nameof(AllUploadedDocumentTypes))]
        public void TryParseDescription_ShouldRoundTripUploadedDocumentType(UploadedDocumentType value)
        {
            bool parsed = EnumExtensions.TryParseDescription(value.GetDescription(), out UploadedDocumentType actual);

            Assert.True(parsed);
            Assert.Equal(value, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Unknown")]
        [InlineData("adjournment")]
        [InlineData("ADJOURNMENT")]
        [InlineData(" Adjournment")]
        [InlineData("Adjournment ")]
        [InlineData("WrittenReasons")]
        [InlineData("CertifiedExtract")]
        [InlineData("2")]
        public void TryParseDescription_ShouldReturnFalseForUnmatchedValue(string? description)
        {
            bool parsed = EnumExtensions.TryParseDescription(description, out UploadedDocumentType actual);

            Assert.False(parsed);
            Assert.Equal(default, actual);
        }

        [Fact]
        public void GetDescription_ShouldReturnDescriptionAttribute()
        {
            Assert.Equal("With Description", TestEnum.WithDescription.GetDescription());
        }

        [Fact]
        public void GetDescription_ShouldFallBackToNameWithoutDescriptionAttribute()
        {
            Assert.Equal("WithoutDescription", TestEnum.WithoutDescription.GetDescription());
        }

        [Fact]
        public void TryParseDescription_ShouldMatchNameWhenNoDescriptionAttribute()
        {
            bool parsed = EnumExtensions.TryParseDescription("WithoutDescription", out TestEnum actual);

            Assert.True(parsed);
            Assert.Equal(TestEnum.WithoutDescription, actual);
        }
    }
}

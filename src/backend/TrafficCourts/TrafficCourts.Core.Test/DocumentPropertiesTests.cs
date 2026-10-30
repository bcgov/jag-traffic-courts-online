using TrafficCourts.Domain.Enums;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Core.Test
{
    public class DocumentPropertiesTests
    {
        private const string DocumentTypeTag = "document-type";
        private const string DocumentStatusTag = "document-status";

        private static readonly Dictionary<string, string> NoMetadata = new();

        // Documents without a status tag pre-date document status, so an adjournment is assumed to already be resolved.
        [Theory]
        [InlineData(UploadedDocumentType.Adjournment, DocumentStatus.Resolved)]
        [InlineData(UploadedDocumentType.Other, DocumentStatus.Filed)]
        [InlineData(UploadedDocumentType.WrittenReasons, DocumentStatus.Filed)]
        [InlineData(UploadedDocumentType.CertifiedExtract, DocumentStatus.Filed)]
        public void Constructor_WithoutStatusTag_ShouldInferStatusFromDocumentType(UploadedDocumentType documentType, DocumentStatus expected)
        {
            var tags = new Dictionary<string, string> { [DocumentTypeTag] = documentType.GetDescription() };

            DocumentProperties actual = new(NoMetadata, tags);

            Assert.Equal(documentType.GetDescription(), actual.DocumentType);
            Assert.Equal(expected, actual.DocumentStatus);
        }

        [Theory]
        [InlineData("Unknown")]
        [InlineData("adjournment")]
        [InlineData("Adjournment ")]
        public void Constructor_WithoutStatusTag_UnrecognizedDocumentTypeShouldBeFiled(string documentType)
        {
            var tags = new Dictionary<string, string> { [DocumentTypeTag] = documentType };

            DocumentProperties actual = new(NoMetadata, tags);

            Assert.Equal(DocumentStatus.Filed, actual.DocumentStatus);
        }

        [Fact]
        public void Constructor_WithoutTypeOrStatusTag_ShouldBeFiled()
        {
            DocumentProperties actual = new(NoMetadata, new Dictionary<string, string>());

            Assert.Null(actual.DocumentType);
            Assert.Equal(DocumentStatus.Filed, actual.DocumentStatus);
        }

        [Theory]
        [InlineData(UploadedDocumentType.Adjournment, DocumentStatus.Pending)]
        [InlineData(UploadedDocumentType.Adjournment, DocumentStatus.Filed)]
        [InlineData(UploadedDocumentType.Other, DocumentStatus.Resolved)]
        [InlineData(UploadedDocumentType.CertifiedExtract, DocumentStatus.Pending)]
        public void Constructor_WithStatusTag_ShouldUseTaggedStatus(UploadedDocumentType documentType, DocumentStatus tagged)
        {
            var tags = new Dictionary<string, string>
            {
                [DocumentTypeTag] = documentType.GetDescription(),
                [DocumentStatusTag] = tagged.ToString(),
            };

            DocumentProperties actual = new(NoMetadata, tags);

            Assert.Equal(tagged, actual.DocumentStatus);
        }

        [Theory]
        [InlineData(UploadedDocumentType.Adjournment, DocumentStatus.Resolved)]
        [InlineData(UploadedDocumentType.Other, DocumentStatus.Filed)]
        public void Constructor_WithUnparseableStatusTag_ShouldInferStatusFromDocumentType(UploadedDocumentType documentType, DocumentStatus expected)
        {
            var tags = new Dictionary<string, string>
            {
                [DocumentTypeTag] = documentType.GetDescription(),
                [DocumentStatusTag] = "NotAStatus",
            };

            DocumentProperties actual = new(NoMetadata, tags);

            Assert.Equal(expected, actual.DocumentStatus);
        }
    }
}

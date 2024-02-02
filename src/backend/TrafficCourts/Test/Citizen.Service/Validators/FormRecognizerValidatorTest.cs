using Moq;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators;

public class FormRecognizerValidatorTest
{
    [Fact]
    public void TestSanitize_MVAStatuteExists()
    {
        // isMVA should be overwritten to _selected if the section text references a valid MVA Statute

        // Given
        Statute statute = new Statute("19590", "MVA", "100", "1", "a", "", "MVA 100(1)(a)", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        _statuteLookupService.Setup(x => x.GetBySectionAsync("100(1)(a)")).ReturnsAsync(statute);
        FormRecognizerValidator formRecognizerValidator = new FormRecognizerValidator(_statuteLookupService.Object);

        OcrViolationTicket violationTicket = new ();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVA, new Field("unknown"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field("100(1)(a)"));

        // When
        formRecognizerValidator.Sanitize(violationTicket);

        // Then
        Assert.Equal(Field._selected, violationTicket.Fields[OcrViolationTicket.OffenceIsMVA].Value);
    }

    [Fact]
    public void TestSanitize_MVAStatuteDoesntExist()
    {
        // isMVA should be overwritten to _unselected if the section text is blank or does not reference a valid MVA Statute.

        // Given
        Statute statute = new Statute("19590", "MVA", "100", "1", "a", "", "MVA 100(1)(a)", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        _statuteLookupService.Setup(x => x.GetBySectionAsync("100(1)(a)")).ReturnsAsync(statute);
        FormRecognizerValidator formRecognizerValidator = new FormRecognizerValidator(_statuteLookupService.Object);

        OcrViolationTicket violationTicket = new ();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVA, new Field("unknown"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field("777"));

        // When
        formRecognizerValidator.Sanitize(violationTicket);

        // Then
        Assert.Equal(Field._unselected, violationTicket.Fields[OcrViolationTicket.OffenceIsMVA].Value);
    }
    
    [Fact]
    public void TestSanitize_MVARStatuteExists()
    {
        // isMVAR should be overwritten to _selected if the section text references a valid MVA Statute

        // Given
        Statute statute = new Statute("19590", "MVAR", "100", "1", "a", "", "MVAR 100(1)(a)", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        _statuteLookupService.Setup(x => x.GetBySectionAsync("100(1)(a)")).ReturnsAsync(statute);
        FormRecognizerValidator formRecognizerValidator = new FormRecognizerValidator(_statuteLookupService.Object);

        OcrViolationTicket violationTicket = new ();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVAR, new Field("unknown"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field("100(1)(a)"));

        // When
        formRecognizerValidator.Sanitize(violationTicket);

        // Then
        Assert.Equal(Field._selected, violationTicket.Fields[OcrViolationTicket.OffenceIsMVAR].Value);
    }

    [Fact]
    public void TestSanitize_MVARStatuteDoesntExist()
    {
        // isMVA should be overwritten to _unselected if the section text is blank or does not reference a valid MVA Statute.

        // Given
        Statute statute = new Statute("19590", "MVAR", "100", "1", "a", "", "MVAR 100(1)(a)", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        _statuteLookupService.Setup(x => x.GetBySectionAsync("100(1)(a)")).ReturnsAsync(statute);
        FormRecognizerValidator formRecognizerValidator = new FormRecognizerValidator(_statuteLookupService.Object);

        OcrViolationTicket violationTicket = new ();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVAR, new Field("unknown"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field("777"));

        // When
        formRecognizerValidator.Sanitize(violationTicket);

        // Then
        Assert.Equal(Field._unselected, violationTicket.Fields[OcrViolationTicket.OffenceIsMVAR].Value);
    }
}
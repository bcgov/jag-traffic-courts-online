using Microsoft.Extensions.Logging;
using Moq;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Domain.Models;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators;

public class FormRecognizerValidatorTest
{
    [Fact]
    public async void TestSanitize_MVAStatuteExists()
    {
        // isMVA should be overwritten to _selected if the section text references a valid MVA Statute

        // Given
        Statute statute = new Statute("19590", "MVA", "100", "1", "a", "", "MVA 100(1)(a)", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        _statuteLookupService.Setup(x => x.GetBySectionAsync("100(1)(a)")).ReturnsAsync(statute);
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new ();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVA, new Field("unknown"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field("100(1)(a)"));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.Equal(Field._selected, violationTicket.Fields[OcrViolationTicket.OffenceIsMVA].Value);
    }

    [Fact]
    public async void TestSanitize_MVAStatuteDoesntExist()
    {
        // isMVA should be overwritten to _unselected if the section text is blank or does not reference a valid MVA Statute.

        // Given
        Statute statute = new Statute("19590", "MVA", "100", "1", "a", "", "MVA 100(1)(a)", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        _statuteLookupService.Setup(x => x.GetBySectionAsync("100(1)(a)")).ReturnsAsync(statute);
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new ();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVA, new Field("unknown"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field("777"));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.Equal(Field._unselected, violationTicket.Fields[OcrViolationTicket.OffenceIsMVA].Value);
    }
    
    [Fact]
    public async void TestSanitize_MVARStatuteExists()
    {
        // isMVAR should be overwritten to _selected if the section text references a valid MVA Statute

        // Given
        Statute statute = new Statute("19590", "MVAR", "100", "1", "a", "", "MVAR 100(1)(a)", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        _statuteLookupService.Setup(x => x.GetBySectionAsync("100(1)(a)")).ReturnsAsync(statute);
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new ();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVAR, new Field("unknown"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field("100(1)(a)"));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.Equal(Field._selected, violationTicket.Fields[OcrViolationTicket.OffenceIsMVAR].Value);
    }

    [Fact]
    public async void TestSanitize_MVARStatuteDoesntExist()
    {
        // isMVA should be overwritten to _unselected if the section text is blank or does not reference a valid MVA Statute.

        // Given
        Statute statute = new Statute("19590", "MVAR", "100", "1", "a", "", "MVAR 100(1)(a)", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        _statuteLookupService.Setup(x => x.GetBySectionAsync("100(1)(a)")).ReturnsAsync(statute);
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new ();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVAR, new Field("unknown"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field("777"));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.Equal(Field._unselected, violationTicket.Fields[OcrViolationTicket.OffenceIsMVAR].Value);
    }

    [Fact]
    public async void TestSanitize_DLNumberFromProvince()
    {
        // Given
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.DriverLicenceNumber, new Field(""));
        violationTicket.Fields.Add(OcrViolationTicket.DriverLicenceProvince, new Field("BC 1234567"));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.Equal("BC", violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince].Value);
        Assert.Equal("1234567", violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].Value);
    }

    [Fact]
    public async void TestSanitize_ProvinceFromDLNumber()
    {
        // Given
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.DriverLicenceNumber, new Field("BC1234567"));
        violationTicket.Fields.Add(OcrViolationTicket.DriverLicenceProvince, new Field(""));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.Equal("BC", violationTicket.Fields[OcrViolationTicket.DriverLicenceProvince].Value);
        Assert.Equal("1234567", violationTicket.Fields[OcrViolationTicket.DriverLicenceNumber].Value);
    }

    [Fact]
    public async void TestSanitize_EmptySectionAndTicketAmountForAllCounts()
    {
        // Given
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new();

        violationTicket.Fields.Add(OcrViolationTicket.Count1Description, new Field("description1"));
        violationTicket.Fields.Add(OcrViolationTicket.Count1ActRegs, new Field(""));
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, new Field(""));
        violationTicket.Fields.Add(OcrViolationTicket.Count1TicketAmount, new Field(""));

        violationTicket.Fields.Add(OcrViolationTicket.Count2Description, new Field("description2"));
        violationTicket.Fields.Add(OcrViolationTicket.Count2ActRegs, new Field(""));
        violationTicket.Fields.Add(OcrViolationTicket.Count2Section, new Field(""));
        violationTicket.Fields.Add(OcrViolationTicket.Count2TicketAmount, new Field(""));

        violationTicket.Fields.Add(OcrViolationTicket.Count3Description, new Field("description3"));
        violationTicket.Fields.Add(OcrViolationTicket.Count3ActRegs, new Field(""));
        violationTicket.Fields.Add(OcrViolationTicket.Count3Section, new Field(""));
        violationTicket.Fields.Add(OcrViolationTicket.Count3TicketAmount, new Field(""));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.True(string.IsNullOrEmpty(violationTicket.Fields[OcrViolationTicket.Count1Description].Value));
        Assert.True(string.IsNullOrEmpty(violationTicket.Fields[OcrViolationTicket.Count2Description].Value));
        Assert.True(string.IsNullOrEmpty(violationTicket.Fields[OcrViolationTicket.Count3Description].Value));
    }

    [Fact]
    public async void TestSanitize_WhitespaceRemoved()
    {
        // Given
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.DetachmentLocation, new Field(" \t \n some_text \t "));
        violationTicket.Fields.Add(OcrViolationTicket.HearingLocation, new Field(" \t \r\n some_text \t "));
        violationTicket.Fields.Add(OcrViolationTicket.ViolationTicketTitle, new Field(" \t \n some_text \t "));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.Equal(" some_text ", violationTicket.Fields[OcrViolationTicket.DetachmentLocation].Value);
        Assert.Equal(" some_text ", violationTicket.Fields[OcrViolationTicket.HearingLocation].Value);
        Assert.Equal(" some_text ", violationTicket.Fields[OcrViolationTicket.ViolationTicketTitle].Value);
    }    

    // Given a Violation Ticket with a count that has a Ticket Amount field with a trailing hyphen, ensure the hyphen is removed
    [Fact]
    public async void TestSanitize_TicketAmountStripHyphens()
    {
        // Given
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<FormRecognizerValidator>>();
        FormRecognizerValidator formRecognizerValidator = new(_statuteLookupService.Object, _logger.Object);

        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.Count1TicketAmount, new Field("10 0-"));
        violationTicket.Fields.Add(OcrViolationTicket.Count2TicketAmount, new Field("2-22-"));
        violationTicket.Fields.Add(OcrViolationTicket.Count3TicketAmount, new Field("-3 33"));

        // When
        await formRecognizerValidator.SanitizeAsync(violationTicket);

        // Then
        Assert.Equal("100", violationTicket.Fields[OcrViolationTicket.Count1TicketAmount].Value);
        Assert.Equal("222", violationTicket.Fields[OcrViolationTicket.Count2TicketAmount].Value);
        Assert.Equal("333", violationTicket.Fields[OcrViolationTicket.Count3TicketAmount].Value);
    }
    
}
using Xunit.Abstractions;
using Oracle = TrafficCourts.OracleDataApi.Client.V1;

namespace TrafficCourts.OracleDataApi.Test;

public class EnumMappingTests : DomainModelMappingTest
{
    public EnumMappingTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void EnumMapping()
    {
        #region DisputeAppearanceLessThan14DaysYn
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeAppearanceLessThan14DaysYn.UNKNOWN, _sut.Map<Domain.Models.DisputeAppearanceLessThan14DaysYn>(Oracle.DisputeAppearanceLessThan14DaysYn.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeAppearanceLessThan14DaysYn.Y, _sut.Map<Domain.Models.DisputeAppearanceLessThan14DaysYn>(Oracle.DisputeAppearanceLessThan14DaysYn.Y));
        Assert.Equal(Domain.Models.DisputeAppearanceLessThan14DaysYn.N, _sut.Map<Domain.Models.DisputeAppearanceLessThan14DaysYn>(Oracle.DisputeAppearanceLessThan14DaysYn.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeAppearanceLessThan14DaysYn.UNKNOWN, _sut.Map<Oracle.DisputeAppearanceLessThan14DaysYn>(Domain.Models.DisputeAppearanceLessThan14DaysYn.UNKNOWN));
        Assert.Equal(Oracle.DisputeAppearanceLessThan14DaysYn.Y, _sut.Map<Oracle.DisputeAppearanceLessThan14DaysYn>(Domain.Models.DisputeAppearanceLessThan14DaysYn.Y));
        Assert.Equal(Oracle.DisputeAppearanceLessThan14DaysYn.N, _sut.Map<Oracle.DisputeAppearanceLessThan14DaysYn>(Domain.Models.DisputeAppearanceLessThan14DaysYn.N));
        #endregion DisputeAppearanceLessThan14DaysYn

        #region DisputeContactTypeCd
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeContactTypeCd.UNKNOWN, _sut.Map<Domain.Models.DisputeContactTypeCd>(Oracle.DisputeContactTypeCd.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeContactTypeCd.INDIVIDUAL, _sut.Map<Domain.Models.DisputeContactTypeCd>(Oracle.DisputeContactTypeCd.INDIVIDUAL));
        Assert.Equal(Domain.Models.DisputeContactTypeCd.LAWYER, _sut.Map<Domain.Models.DisputeContactTypeCd>(Oracle.DisputeContactTypeCd.LAWYER));
        Assert.Equal(Domain.Models.DisputeContactTypeCd.OTHER, _sut.Map<Domain.Models.DisputeContactTypeCd>(Oracle.DisputeContactTypeCd.OTHER));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeContactTypeCd.UNKNOWN, _sut.Map<Oracle.DisputeContactTypeCd>(Domain.Models.DisputeContactTypeCd.UNKNOWN));
        Assert.Equal(Oracle.DisputeContactTypeCd.INDIVIDUAL, _sut.Map<Oracle.DisputeContactTypeCd>(Domain.Models.DisputeContactTypeCd.INDIVIDUAL));
        Assert.Equal(Oracle.DisputeContactTypeCd.LAWYER, _sut.Map<Oracle.DisputeContactTypeCd>(Domain.Models.DisputeContactTypeCd.LAWYER));
        Assert.Equal(Oracle.DisputeContactTypeCd.OTHER, _sut.Map<Oracle.DisputeContactTypeCd>(Domain.Models.DisputeContactTypeCd.OTHER));
        #endregion DisputeContactTypeCd

        #region DisputeCountPleaCode
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeCountPleaCode.UNKNOWN, _sut.Map<Domain.Models.DisputeCountPleaCode>(Oracle.DisputeCountPleaCode.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeCountPleaCode.G, _sut.Map<Domain.Models.DisputeCountPleaCode>(Oracle.DisputeCountPleaCode.G));
        Assert.Equal(Domain.Models.DisputeCountPleaCode.N, _sut.Map<Domain.Models.DisputeCountPleaCode>(Oracle.DisputeCountPleaCode.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeCountPleaCode.UNKNOWN, _sut.Map<Oracle.DisputeCountPleaCode>(Domain.Models.DisputeCountPleaCode.UNKNOWN));
        Assert.Equal(Oracle.DisputeCountPleaCode.G, _sut.Map<Oracle.DisputeCountPleaCode>(Domain.Models.DisputeCountPleaCode.G));
        Assert.Equal(Oracle.DisputeCountPleaCode.N, _sut.Map<Oracle.DisputeCountPleaCode>(Domain.Models.DisputeCountPleaCode.N));
        #endregion DisputeCountPleaCode

        #region DisputeCountRequestCourtAppearance
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeCountRequestCourtAppearance.UNKNOWN, _sut.Map<Domain.Models.DisputeCountRequestCourtAppearance>(Oracle.DisputeCountRequestCourtAppearance.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeCountRequestCourtAppearance.Y, _sut.Map<Domain.Models.DisputeCountRequestCourtAppearance>(Oracle.DisputeCountRequestCourtAppearance.Y));
        Assert.Equal(Domain.Models.DisputeCountRequestCourtAppearance.N, _sut.Map<Domain.Models.DisputeCountRequestCourtAppearance>(Oracle.DisputeCountRequestCourtAppearance.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeCountRequestCourtAppearance.UNKNOWN, _sut.Map<Oracle.DisputeCountRequestCourtAppearance>(Domain.Models.DisputeCountRequestCourtAppearance.UNKNOWN));
        Assert.Equal(Oracle.DisputeCountRequestCourtAppearance.Y, _sut.Map<Oracle.DisputeCountRequestCourtAppearance>(Domain.Models.DisputeCountRequestCourtAppearance.Y));
        Assert.Equal(Oracle.DisputeCountRequestCourtAppearance.N, _sut.Map<Oracle.DisputeCountRequestCourtAppearance>(Domain.Models.DisputeCountRequestCourtAppearance.N));
        #endregion DisputeCountRequestCourtAppearance

        #region DisputeCountRequestReduction
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeCountRequestReduction.UNKNOWN, _sut.Map<Domain.Models.DisputeCountRequestReduction>(Oracle.DisputeCountRequestReduction.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeCountRequestReduction.Y, _sut.Map<Domain.Models.DisputeCountRequestReduction>(Oracle.DisputeCountRequestReduction.Y));
        Assert.Equal(Domain.Models.DisputeCountRequestReduction.N, _sut.Map<Domain.Models.DisputeCountRequestReduction>(Oracle.DisputeCountRequestReduction.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeCountRequestReduction.UNKNOWN, _sut.Map<Oracle.DisputeCountRequestReduction>(Domain.Models.DisputeCountRequestReduction.UNKNOWN));
        Assert.Equal(Oracle.DisputeCountRequestReduction.Y, _sut.Map<Oracle.DisputeCountRequestReduction>(Domain.Models.DisputeCountRequestReduction.Y));
        Assert.Equal(Oracle.DisputeCountRequestReduction.N, _sut.Map<Oracle.DisputeCountRequestReduction>(Domain.Models.DisputeCountRequestReduction.N));
        #endregion DisputeCountRequestReduction

        #region DisputeCountRequestTimeToPay
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeCountRequestTimeToPay.UNKNOWN, _sut.Map<Domain.Models.DisputeCountRequestTimeToPay>(Oracle.DisputeCountRequestTimeToPay.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeCountRequestTimeToPay.Y, _sut.Map<Domain.Models.DisputeCountRequestTimeToPay>(Oracle.DisputeCountRequestTimeToPay.Y));
        Assert.Equal(Domain.Models.DisputeCountRequestTimeToPay.N, _sut.Map<Domain.Models.DisputeCountRequestTimeToPay>(Oracle.DisputeCountRequestTimeToPay.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeCountRequestTimeToPay.UNKNOWN, _sut.Map<Oracle.DisputeCountRequestTimeToPay>(Domain.Models.DisputeCountRequestTimeToPay.UNKNOWN));
        Assert.Equal(Oracle.DisputeCountRequestTimeToPay.Y, _sut.Map<Oracle.DisputeCountRequestTimeToPay>(Domain.Models.DisputeCountRequestTimeToPay.Y));
        Assert.Equal(Oracle.DisputeCountRequestTimeToPay.N, _sut.Map<Oracle.DisputeCountRequestTimeToPay>(Domain.Models.DisputeCountRequestTimeToPay.N));
        #endregion DisputeCountRequestTimeToPay

        #region DisputeDisputantDetectedOcrIssues
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeDisputantDetectedOcrIssues.UNKNOWN, _sut.Map<Domain.Models.DisputeDisputantDetectedOcrIssues>(Oracle.DisputeDisputantDetectedOcrIssues.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeDisputantDetectedOcrIssues.Y, _sut.Map<Domain.Models.DisputeDisputantDetectedOcrIssues>(Oracle.DisputeDisputantDetectedOcrIssues.Y));
        Assert.Equal(Domain.Models.DisputeDisputantDetectedOcrIssues.N, _sut.Map<Domain.Models.DisputeDisputantDetectedOcrIssues>(Oracle.DisputeDisputantDetectedOcrIssues.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeDisputantDetectedOcrIssues.UNKNOWN, _sut.Map<Oracle.DisputeDisputantDetectedOcrIssues>(Domain.Models.DisputeDisputantDetectedOcrIssues.UNKNOWN));
        Assert.Equal(Oracle.DisputeDisputantDetectedOcrIssues.Y, _sut.Map<Oracle.DisputeDisputantDetectedOcrIssues>(Domain.Models.DisputeDisputantDetectedOcrIssues.Y));
        Assert.Equal(Oracle.DisputeDisputantDetectedOcrIssues.N, _sut.Map<Oracle.DisputeDisputantDetectedOcrIssues>(Domain.Models.DisputeDisputantDetectedOcrIssues.N));
        #endregion DisputeDisputantDetectedOcrIssues

        #region DisputeInterpreterRequired
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeInterpreterRequired.UNKNOWN, _sut.Map<Domain.Models.DisputeInterpreterRequired>(Oracle.DisputeInterpreterRequired.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeInterpreterRequired.Y, _sut.Map<Domain.Models.DisputeInterpreterRequired>(Oracle.DisputeInterpreterRequired.Y));
        Assert.Equal(Domain.Models.DisputeInterpreterRequired.N, _sut.Map<Domain.Models.DisputeInterpreterRequired>(Oracle.DisputeInterpreterRequired.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeInterpreterRequired.UNKNOWN, _sut.Map<Oracle.DisputeInterpreterRequired>(Domain.Models.DisputeInterpreterRequired.UNKNOWN));
        Assert.Equal(Oracle.DisputeInterpreterRequired.Y, _sut.Map<Oracle.DisputeInterpreterRequired>(Domain.Models.DisputeInterpreterRequired.Y));
        Assert.Equal(Oracle.DisputeInterpreterRequired.N, _sut.Map<Oracle.DisputeInterpreterRequired>(Domain.Models.DisputeInterpreterRequired.N));
        #endregion DisputeInterpreterRequired

        #region DisputeListItemDisputantDetectedOcrIssues
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN, _sut.Map<Domain.Models.DisputeListItemDisputantDetectedOcrIssues>(Oracle.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeListItemDisputantDetectedOcrIssues.Y, _sut.Map<Domain.Models.DisputeListItemDisputantDetectedOcrIssues>(Oracle.DisputeListItemDisputantDetectedOcrIssues.Y));
        Assert.Equal(Domain.Models.DisputeListItemDisputantDetectedOcrIssues.N, _sut.Map<Domain.Models.DisputeListItemDisputantDetectedOcrIssues>(Oracle.DisputeListItemDisputantDetectedOcrIssues.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN, _sut.Map<Oracle.DisputeListItemDisputantDetectedOcrIssues>(Domain.Models.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN));
        Assert.Equal(Oracle.DisputeListItemDisputantDetectedOcrIssues.Y, _sut.Map<Oracle.DisputeListItemDisputantDetectedOcrIssues>(Domain.Models.DisputeListItemDisputantDetectedOcrIssues.Y));
        Assert.Equal(Oracle.DisputeListItemDisputantDetectedOcrIssues.N, _sut.Map<Oracle.DisputeListItemDisputantDetectedOcrIssues>(Domain.Models.DisputeListItemDisputantDetectedOcrIssues.N));
        #endregion DisputeListItemDisputantDetectedOcrIssues

        #region DisputeListItemJjDisputeStatus
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.UNKNOWN, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.NEW, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.NEW));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.IN_PROGRESS, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.IN_PROGRESS));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.DATA_UPDATE, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.DATA_UPDATE));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.CONFIRMED, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.CONFIRMED));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.REQUIRE_COURT_HEARING, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.REQUIRE_COURT_HEARING));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.REQUIRE_MORE_INFO, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.REQUIRE_MORE_INFO));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.ACCEPTED, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.ACCEPTED));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.REVIEW, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.REVIEW));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.CONCLUDED, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.CONCLUDED));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.CANCELLED, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.CANCELLED));
        Assert.Equal(Domain.Models.DisputeListItemJjDisputeStatus.HEARING_SCHEDULED, _sut.Map<Domain.Models.DisputeListItemJjDisputeStatus>(Oracle.DisputeListItemJjDisputeStatus.HEARING_SCHEDULED));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.UNKNOWN, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.UNKNOWN));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.NEW, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.NEW));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.IN_PROGRESS, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.IN_PROGRESS));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.DATA_UPDATE, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.DATA_UPDATE));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.CONFIRMED, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.CONFIRMED));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.REQUIRE_COURT_HEARING, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.REQUIRE_COURT_HEARING));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.REQUIRE_MORE_INFO, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.REQUIRE_MORE_INFO));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.ACCEPTED, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.ACCEPTED));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.REVIEW, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.REVIEW));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.CONCLUDED, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.CONCLUDED));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.CANCELLED, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.CANCELLED));
        Assert.Equal(Oracle.DisputeListItemJjDisputeStatus.HEARING_SCHEDULED, _sut.Map<Oracle.DisputeListItemJjDisputeStatus>(Domain.Models.DisputeListItemJjDisputeStatus.HEARING_SCHEDULED));
        #endregion DisputeListItemJjDisputeStatus

        #region DisputeListItemRequestCourtAppearanceYn
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeListItemRequestCourtAppearanceYn.UNKNOWN, _sut.Map<Domain.Models.DisputeListItemRequestCourtAppearanceYn>(Oracle.DisputeListItemRequestCourtAppearanceYn.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeListItemRequestCourtAppearanceYn.Y, _sut.Map<Domain.Models.DisputeListItemRequestCourtAppearanceYn>(Oracle.DisputeListItemRequestCourtAppearanceYn.Y));
        Assert.Equal(Domain.Models.DisputeListItemRequestCourtAppearanceYn.N, _sut.Map<Domain.Models.DisputeListItemRequestCourtAppearanceYn>(Oracle.DisputeListItemRequestCourtAppearanceYn.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeListItemRequestCourtAppearanceYn.UNKNOWN, _sut.Map<Oracle.DisputeListItemRequestCourtAppearanceYn>(Domain.Models.DisputeListItemRequestCourtAppearanceYn.UNKNOWN));
        Assert.Equal(Oracle.DisputeListItemRequestCourtAppearanceYn.Y, _sut.Map<Oracle.DisputeListItemRequestCourtAppearanceYn>(Domain.Models.DisputeListItemRequestCourtAppearanceYn.Y));
        Assert.Equal(Oracle.DisputeListItemRequestCourtAppearanceYn.N, _sut.Map<Oracle.DisputeListItemRequestCourtAppearanceYn>(Domain.Models.DisputeListItemRequestCourtAppearanceYn.N));
        #endregion DisputeListItemRequestCourtAppearanceYn

        #region DisputeListItemStatus
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeListItemStatus.UNKNOWN, _sut.Map<Domain.Models.DisputeListItemStatus>(Oracle.DisputeListItemStatus.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeListItemStatus.NEW, _sut.Map<Domain.Models.DisputeListItemStatus>(Oracle.DisputeListItemStatus.NEW));
        Assert.Equal(Domain.Models.DisputeListItemStatus.VALIDATED, _sut.Map<Domain.Models.DisputeListItemStatus>(Oracle.DisputeListItemStatus.VALIDATED));
        Assert.Equal(Domain.Models.DisputeListItemStatus.PROCESSING, _sut.Map<Domain.Models.DisputeListItemStatus>(Oracle.DisputeListItemStatus.PROCESSING));
        Assert.Equal(Domain.Models.DisputeListItemStatus.REJECTED, _sut.Map<Domain.Models.DisputeListItemStatus>(Oracle.DisputeListItemStatus.REJECTED));
        Assert.Equal(Domain.Models.DisputeListItemStatus.CANCELLED, _sut.Map<Domain.Models.DisputeListItemStatus>(Oracle.DisputeListItemStatus.CANCELLED));
        Assert.Equal(Domain.Models.DisputeListItemStatus.CONCLUDED, _sut.Map<Domain.Models.DisputeListItemStatus>(Oracle.DisputeListItemStatus.CONCLUDED));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeListItemStatus.UNKNOWN, _sut.Map<Oracle.DisputeListItemStatus>(Domain.Models.DisputeListItemStatus.UNKNOWN));
        Assert.Equal(Oracle.DisputeListItemStatus.NEW, _sut.Map<Oracle.DisputeListItemStatus>(Domain.Models.DisputeListItemStatus.NEW));
        Assert.Equal(Oracle.DisputeListItemStatus.VALIDATED, _sut.Map<Oracle.DisputeListItemStatus>(Domain.Models.DisputeListItemStatus.VALIDATED));
        Assert.Equal(Oracle.DisputeListItemStatus.PROCESSING, _sut.Map<Oracle.DisputeListItemStatus>(Domain.Models.DisputeListItemStatus.PROCESSING));
        Assert.Equal(Oracle.DisputeListItemStatus.REJECTED, _sut.Map<Oracle.DisputeListItemStatus>(Domain.Models.DisputeListItemStatus.REJECTED));
        Assert.Equal(Oracle.DisputeListItemStatus.CANCELLED, _sut.Map<Oracle.DisputeListItemStatus>(Domain.Models.DisputeListItemStatus.CANCELLED));
        Assert.Equal(Oracle.DisputeListItemStatus.CONCLUDED, _sut.Map<Oracle.DisputeListItemStatus>(Domain.Models.DisputeListItemStatus.CONCLUDED));
        #endregion DisputeListItemStatus

        #region DisputeListItemSystemDetectedOcrIssues
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeListItemSystemDetectedOcrIssues.UNKNOWN, _sut.Map<Domain.Models.DisputeListItemSystemDetectedOcrIssues>(Oracle.DisputeListItemSystemDetectedOcrIssues.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeListItemSystemDetectedOcrIssues.Y, _sut.Map<Domain.Models.DisputeListItemSystemDetectedOcrIssues>(Oracle.DisputeListItemSystemDetectedOcrIssues.Y));
        Assert.Equal(Domain.Models.DisputeListItemSystemDetectedOcrIssues.N, _sut.Map<Domain.Models.DisputeListItemSystemDetectedOcrIssues>(Oracle.DisputeListItemSystemDetectedOcrIssues.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeListItemSystemDetectedOcrIssues.UNKNOWN, _sut.Map<Oracle.DisputeListItemSystemDetectedOcrIssues>(Domain.Models.DisputeListItemSystemDetectedOcrIssues.UNKNOWN));
        Assert.Equal(Oracle.DisputeListItemSystemDetectedOcrIssues.Y, _sut.Map<Oracle.DisputeListItemSystemDetectedOcrIssues>(Domain.Models.DisputeListItemSystemDetectedOcrIssues.Y));
        Assert.Equal(Oracle.DisputeListItemSystemDetectedOcrIssues.N, _sut.Map<Oracle.DisputeListItemSystemDetectedOcrIssues>(Domain.Models.DisputeListItemSystemDetectedOcrIssues.N));
        #endregion DisputeListItemSystemDetectedOcrIssues

        #region DisputeRepresentedByLawyer
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeRepresentedByLawyer.UNKNOWN, _sut.Map<Domain.Models.DisputeRepresentedByLawyer>(Oracle.DisputeRepresentedByLawyer.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeRepresentedByLawyer.Y, _sut.Map<Domain.Models.DisputeRepresentedByLawyer>(Oracle.DisputeRepresentedByLawyer.Y));
        Assert.Equal(Domain.Models.DisputeRepresentedByLawyer.N, _sut.Map<Domain.Models.DisputeRepresentedByLawyer>(Oracle.DisputeRepresentedByLawyer.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeRepresentedByLawyer.UNKNOWN, _sut.Map<Oracle.DisputeRepresentedByLawyer>(Domain.Models.DisputeRepresentedByLawyer.UNKNOWN));
        Assert.Equal(Oracle.DisputeRepresentedByLawyer.Y, _sut.Map<Oracle.DisputeRepresentedByLawyer>(Domain.Models.DisputeRepresentedByLawyer.Y));
        Assert.Equal(Oracle.DisputeRepresentedByLawyer.N, _sut.Map<Oracle.DisputeRepresentedByLawyer>(Domain.Models.DisputeRepresentedByLawyer.N));
        #endregion DisputeRepresentedByLawyer

        #region DisputeRequestCourtAppearanceYn
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeRequestCourtAppearanceYn.UNKNOWN, _sut.Map<Domain.Models.DisputeRequestCourtAppearanceYn>(Oracle.DisputeRequestCourtAppearanceYn.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeRequestCourtAppearanceYn.Y, _sut.Map<Domain.Models.DisputeRequestCourtAppearanceYn>(Oracle.DisputeRequestCourtAppearanceYn.Y));
        Assert.Equal(Domain.Models.DisputeRequestCourtAppearanceYn.N, _sut.Map<Domain.Models.DisputeRequestCourtAppearanceYn>(Oracle.DisputeRequestCourtAppearanceYn.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeRequestCourtAppearanceYn.UNKNOWN, _sut.Map<Oracle.DisputeRequestCourtAppearanceYn>(Domain.Models.DisputeRequestCourtAppearanceYn.UNKNOWN));
        Assert.Equal(Oracle.DisputeRequestCourtAppearanceYn.Y, _sut.Map<Oracle.DisputeRequestCourtAppearanceYn>(Domain.Models.DisputeRequestCourtAppearanceYn.Y));
        Assert.Equal(Oracle.DisputeRequestCourtAppearanceYn.N, _sut.Map<Oracle.DisputeRequestCourtAppearanceYn>(Domain.Models.DisputeRequestCourtAppearanceYn.N));
        #endregion DisputeRequestCourtAppearanceYn

        #region DisputeResultDisputeStatus
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeResultDisputeStatus.UNKNOWN, _sut.Map<Domain.Models.DisputeResultDisputeStatus>(Oracle.DisputeResultDisputeStatus.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeResultDisputeStatus.NEW, _sut.Map<Domain.Models.DisputeResultDisputeStatus>(Oracle.DisputeResultDisputeStatus.NEW));
        Assert.Equal(Domain.Models.DisputeResultDisputeStatus.VALIDATED, _sut.Map<Domain.Models.DisputeResultDisputeStatus>(Oracle.DisputeResultDisputeStatus.VALIDATED));
        Assert.Equal(Domain.Models.DisputeResultDisputeStatus.PROCESSING, _sut.Map<Domain.Models.DisputeResultDisputeStatus>(Oracle.DisputeResultDisputeStatus.PROCESSING));
        Assert.Equal(Domain.Models.DisputeResultDisputeStatus.REJECTED, _sut.Map<Domain.Models.DisputeResultDisputeStatus>(Oracle.DisputeResultDisputeStatus.REJECTED));
        Assert.Equal(Domain.Models.DisputeResultDisputeStatus.CANCELLED, _sut.Map<Domain.Models.DisputeResultDisputeStatus>(Oracle.DisputeResultDisputeStatus.CANCELLED));
        Assert.Equal(Domain.Models.DisputeResultDisputeStatus.CONCLUDED, _sut.Map<Domain.Models.DisputeResultDisputeStatus>(Oracle.DisputeResultDisputeStatus.CONCLUDED));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeResultDisputeStatus.UNKNOWN, _sut.Map<Oracle.DisputeResultDisputeStatus>(Domain.Models.DisputeResultDisputeStatus.UNKNOWN));
        Assert.Equal(Oracle.DisputeResultDisputeStatus.NEW, _sut.Map<Oracle.DisputeResultDisputeStatus>(Domain.Models.DisputeResultDisputeStatus.NEW));
        Assert.Equal(Oracle.DisputeResultDisputeStatus.VALIDATED, _sut.Map<Oracle.DisputeResultDisputeStatus>(Domain.Models.DisputeResultDisputeStatus.VALIDATED));
        Assert.Equal(Oracle.DisputeResultDisputeStatus.PROCESSING, _sut.Map<Oracle.DisputeResultDisputeStatus>(Domain.Models.DisputeResultDisputeStatus.PROCESSING));
        Assert.Equal(Oracle.DisputeResultDisputeStatus.REJECTED, _sut.Map<Oracle.DisputeResultDisputeStatus>(Domain.Models.DisputeResultDisputeStatus.REJECTED));
        Assert.Equal(Oracle.DisputeResultDisputeStatus.CANCELLED, _sut.Map<Oracle.DisputeResultDisputeStatus>(Domain.Models.DisputeResultDisputeStatus.CANCELLED));
        Assert.Equal(Oracle.DisputeResultDisputeStatus.CONCLUDED, _sut.Map<Oracle.DisputeResultDisputeStatus>(Domain.Models.DisputeResultDisputeStatus.CONCLUDED));
        #endregion DisputeResultDisputeStatus

        #region DisputeResultJjDisputeHearingType
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeResultJjDisputeHearingType.UNKNOWN, _sut.Map<Domain.Models.DisputeResultJjDisputeHearingType>(Oracle.DisputeResultJjDisputeHearingType.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeHearingType.COURT_APPEARANCE, _sut.Map<Domain.Models.DisputeResultJjDisputeHearingType>(Oracle.DisputeResultJjDisputeHearingType.COURT_APPEARANCE));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeHearingType.WRITTEN_REASONS, _sut.Map<Domain.Models.DisputeResultJjDisputeHearingType>(Oracle.DisputeResultJjDisputeHearingType.WRITTEN_REASONS));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeResultJjDisputeHearingType.UNKNOWN, _sut.Map<Oracle.DisputeResultJjDisputeHearingType>(Domain.Models.DisputeResultJjDisputeHearingType.UNKNOWN));
        Assert.Equal(Oracle.DisputeResultJjDisputeHearingType.COURT_APPEARANCE, _sut.Map<Oracle.DisputeResultJjDisputeHearingType>(Domain.Models.DisputeResultJjDisputeHearingType.COURT_APPEARANCE));
        Assert.Equal(Oracle.DisputeResultJjDisputeHearingType.WRITTEN_REASONS, _sut.Map<Oracle.DisputeResultJjDisputeHearingType>(Domain.Models.DisputeResultJjDisputeHearingType.WRITTEN_REASONS));
        #endregion DisputeResultJjDisputeHearingType

        #region DisputeResultJjDisputeStatus
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.UNKNOWN, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.NEW, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.NEW));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.IN_PROGRESS, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.IN_PROGRESS));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.DATA_UPDATE, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.DATA_UPDATE));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.CONFIRMED, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.CONFIRMED));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.REQUIRE_COURT_HEARING, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.REQUIRE_COURT_HEARING));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.REQUIRE_MORE_INFO, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.REQUIRE_MORE_INFO));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.ACCEPTED, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.ACCEPTED));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.REVIEW, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.REVIEW));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.CONCLUDED, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.CONCLUDED));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.CANCELLED, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.CANCELLED));
        Assert.Equal(Domain.Models.DisputeResultJjDisputeStatus.HEARING_SCHEDULED, _sut.Map<Domain.Models.DisputeResultJjDisputeStatus>(Oracle.DisputeResultJjDisputeStatus.HEARING_SCHEDULED));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.UNKNOWN, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.UNKNOWN));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.NEW, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.NEW));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.IN_PROGRESS, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.IN_PROGRESS));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.DATA_UPDATE, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.DATA_UPDATE));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.CONFIRMED, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.CONFIRMED));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.REQUIRE_COURT_HEARING, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.REQUIRE_COURT_HEARING));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.REQUIRE_MORE_INFO, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.REQUIRE_MORE_INFO));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.ACCEPTED, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.ACCEPTED));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.REVIEW, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.REVIEW));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.CONCLUDED, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.CONCLUDED));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.CANCELLED, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.CANCELLED));
        Assert.Equal(Oracle.DisputeResultJjDisputeStatus.HEARING_SCHEDULED, _sut.Map<Oracle.DisputeResultJjDisputeStatus>(Domain.Models.DisputeResultJjDisputeStatus.HEARING_SCHEDULED));
        #endregion DisputeResultJjDisputeStatus

        #region DisputeSignatoryType
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeSignatoryType.U, _sut.Map<Domain.Models.DisputeSignatoryType>(Oracle.DisputeSignatoryType.U));
        Assert.Equal(Domain.Models.DisputeSignatoryType.D, _sut.Map<Domain.Models.DisputeSignatoryType>(Oracle.DisputeSignatoryType.D));
        Assert.Equal(Domain.Models.DisputeSignatoryType.A, _sut.Map<Domain.Models.DisputeSignatoryType>(Oracle.DisputeSignatoryType.A));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeSignatoryType.U, _sut.Map<Oracle.DisputeSignatoryType>(Domain.Models.DisputeSignatoryType.U));
        Assert.Equal(Oracle.DisputeSignatoryType.D, _sut.Map<Oracle.DisputeSignatoryType>(Domain.Models.DisputeSignatoryType.D));
        Assert.Equal(Oracle.DisputeSignatoryType.A, _sut.Map<Oracle.DisputeSignatoryType>(Domain.Models.DisputeSignatoryType.A));
        #endregion DisputeSignatoryType

        #region DisputeStatus
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeStatus.UNKNOWN, _sut.Map<Domain.Models.DisputeStatus>(Oracle.DisputeStatus.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeStatus.NEW, _sut.Map<Domain.Models.DisputeStatus>(Oracle.DisputeStatus.NEW));
        Assert.Equal(Domain.Models.DisputeStatus.VALIDATED, _sut.Map<Domain.Models.DisputeStatus>(Oracle.DisputeStatus.VALIDATED));
        Assert.Equal(Domain.Models.DisputeStatus.PROCESSING, _sut.Map<Domain.Models.DisputeStatus>(Oracle.DisputeStatus.PROCESSING));
        Assert.Equal(Domain.Models.DisputeStatus.REJECTED, _sut.Map<Domain.Models.DisputeStatus>(Oracle.DisputeStatus.REJECTED));
        Assert.Equal(Domain.Models.DisputeStatus.CANCELLED, _sut.Map<Domain.Models.DisputeStatus>(Oracle.DisputeStatus.CANCELLED));
        Assert.Equal(Domain.Models.DisputeStatus.CONCLUDED, _sut.Map<Domain.Models.DisputeStatus>(Oracle.DisputeStatus.CONCLUDED));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeStatus.UNKNOWN, _sut.Map<Oracle.DisputeStatus>(Domain.Models.DisputeStatus.UNKNOWN));
        Assert.Equal(Oracle.DisputeStatus.NEW, _sut.Map<Oracle.DisputeStatus>(Domain.Models.DisputeStatus.NEW));
        Assert.Equal(Oracle.DisputeStatus.VALIDATED, _sut.Map<Oracle.DisputeStatus>(Domain.Models.DisputeStatus.VALIDATED));
        Assert.Equal(Oracle.DisputeStatus.PROCESSING, _sut.Map<Oracle.DisputeStatus>(Domain.Models.DisputeStatus.PROCESSING));
        Assert.Equal(Oracle.DisputeStatus.REJECTED, _sut.Map<Oracle.DisputeStatus>(Domain.Models.DisputeStatus.REJECTED));
        Assert.Equal(Oracle.DisputeStatus.CANCELLED, _sut.Map<Oracle.DisputeStatus>(Domain.Models.DisputeStatus.CANCELLED));
        Assert.Equal(Oracle.DisputeStatus.CONCLUDED, _sut.Map<Oracle.DisputeStatus>(Domain.Models.DisputeStatus.CONCLUDED));
        #endregion DisputeStatus

        #region DisputeSystemDetectedOcrIssues
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeSystemDetectedOcrIssues.UNKNOWN, _sut.Map<Domain.Models.DisputeSystemDetectedOcrIssues>(Oracle.DisputeSystemDetectedOcrIssues.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeSystemDetectedOcrIssues.Y, _sut.Map<Domain.Models.DisputeSystemDetectedOcrIssues>(Oracle.DisputeSystemDetectedOcrIssues.Y));
        Assert.Equal(Domain.Models.DisputeSystemDetectedOcrIssues.N, _sut.Map<Domain.Models.DisputeSystemDetectedOcrIssues>(Oracle.DisputeSystemDetectedOcrIssues.N));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeSystemDetectedOcrIssues.UNKNOWN, _sut.Map<Oracle.DisputeSystemDetectedOcrIssues>(Domain.Models.DisputeSystemDetectedOcrIssues.UNKNOWN));
        Assert.Equal(Oracle.DisputeSystemDetectedOcrIssues.Y, _sut.Map<Oracle.DisputeSystemDetectedOcrIssues>(Domain.Models.DisputeSystemDetectedOcrIssues.Y));
        Assert.Equal(Oracle.DisputeSystemDetectedOcrIssues.N, _sut.Map<Oracle.DisputeSystemDetectedOcrIssues>(Domain.Models.DisputeSystemDetectedOcrIssues.N));
        #endregion DisputeSystemDetectedOcrIssues

        #region DisputeUpdateRequestStatus
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeUpdateRequestStatus.UNKNOWN, _sut.Map<Domain.Models.DisputeUpdateRequestStatus>(Oracle.DisputeUpdateRequestStatus.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeUpdateRequestStatus.ACCEPTED, _sut.Map<Domain.Models.DisputeUpdateRequestStatus>(Oracle.DisputeUpdateRequestStatus.ACCEPTED));
        Assert.Equal(Domain.Models.DisputeUpdateRequestStatus.PENDING, _sut.Map<Domain.Models.DisputeUpdateRequestStatus>(Oracle.DisputeUpdateRequestStatus.PENDING));
        Assert.Equal(Domain.Models.DisputeUpdateRequestStatus.REJECTED, _sut.Map<Domain.Models.DisputeUpdateRequestStatus>(Oracle.DisputeUpdateRequestStatus.REJECTED));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeUpdateRequestStatus.UNKNOWN, _sut.Map<Oracle.DisputeUpdateRequestStatus>(Domain.Models.DisputeUpdateRequestStatus.UNKNOWN));
        Assert.Equal(Oracle.DisputeUpdateRequestStatus.ACCEPTED, _sut.Map<Oracle.DisputeUpdateRequestStatus>(Domain.Models.DisputeUpdateRequestStatus.ACCEPTED));
        Assert.Equal(Oracle.DisputeUpdateRequestStatus.PENDING, _sut.Map<Oracle.DisputeUpdateRequestStatus>(Domain.Models.DisputeUpdateRequestStatus.PENDING));
        Assert.Equal(Oracle.DisputeUpdateRequestStatus.REJECTED, _sut.Map<Oracle.DisputeUpdateRequestStatus>(Domain.Models.DisputeUpdateRequestStatus.REJECTED));
        #endregion DisputeUpdateRequestStatus

        #region DisputeUpdateRequestStatus2
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeUpdateRequestStatus2.UNKNOWN, _sut.Map<Domain.Models.DisputeUpdateRequestStatus2>(Oracle.DisputeUpdateRequestStatus2.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeUpdateRequestStatus2.ACCEPTED, _sut.Map<Domain.Models.DisputeUpdateRequestStatus2>(Oracle.DisputeUpdateRequestStatus2.ACCEPTED));
        Assert.Equal(Domain.Models.DisputeUpdateRequestStatus2.PENDING, _sut.Map<Domain.Models.DisputeUpdateRequestStatus2>(Oracle.DisputeUpdateRequestStatus2.PENDING));
        Assert.Equal(Domain.Models.DisputeUpdateRequestStatus2.REJECTED, _sut.Map<Domain.Models.DisputeUpdateRequestStatus2>(Oracle.DisputeUpdateRequestStatus2.REJECTED));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeUpdateRequestStatus2.UNKNOWN, _sut.Map<Oracle.DisputeUpdateRequestStatus2>(Domain.Models.DisputeUpdateRequestStatus2.UNKNOWN));
        Assert.Equal(Oracle.DisputeUpdateRequestStatus2.ACCEPTED, _sut.Map<Oracle.DisputeUpdateRequestStatus2>(Domain.Models.DisputeUpdateRequestStatus2.ACCEPTED));
        Assert.Equal(Oracle.DisputeUpdateRequestStatus2.PENDING, _sut.Map<Oracle.DisputeUpdateRequestStatus2>(Domain.Models.DisputeUpdateRequestStatus2.PENDING));
        Assert.Equal(Oracle.DisputeUpdateRequestStatus2.REJECTED, _sut.Map<Oracle.DisputeUpdateRequestStatus2>(Domain.Models.DisputeUpdateRequestStatus2.REJECTED));
        #endregion DisputeUpdateRequestStatus2

        #region DisputeUpdateRequestUpdateType
        // Oracle => Domain
        Assert.Equal(Domain.Models.DisputeUpdateRequestUpdateType.UNKNOWN, _sut.Map<Domain.Models.DisputeUpdateRequestUpdateType>(Oracle.DisputeUpdateRequestUpdateType.UNKNOWN));
        Assert.Equal(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS, _sut.Map<Domain.Models.DisputeUpdateRequestUpdateType>(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS));
        Assert.Equal(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_PHONE, _sut.Map<Domain.Models.DisputeUpdateRequestUpdateType>(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_PHONE));
        Assert.Equal(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_NAME, _sut.Map<Domain.Models.DisputeUpdateRequestUpdateType>(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_NAME));
        Assert.Equal(Domain.Models.DisputeUpdateRequestUpdateType.COUNT, _sut.Map<Domain.Models.DisputeUpdateRequestUpdateType>(Oracle.DisputeUpdateRequestUpdateType.COUNT));
        Assert.Equal(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL, _sut.Map<Domain.Models.DisputeUpdateRequestUpdateType>(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL));
        Assert.Equal(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT, _sut.Map<Domain.Models.DisputeUpdateRequestUpdateType>(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT));
        Assert.Equal(Domain.Models.DisputeUpdateRequestUpdateType.COURT_OPTIONS, _sut.Map<Domain.Models.DisputeUpdateRequestUpdateType>(Oracle.DisputeUpdateRequestUpdateType.COURT_OPTIONS));
        // Domain => Oracle
        Assert.Equal(Oracle.DisputeUpdateRequestUpdateType.UNKNOWN, _sut.Map<Oracle.DisputeUpdateRequestUpdateType>(Domain.Models.DisputeUpdateRequestUpdateType.UNKNOWN));
        Assert.Equal(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS, _sut.Map<Oracle.DisputeUpdateRequestUpdateType>(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS));
        Assert.Equal(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_PHONE, _sut.Map<Oracle.DisputeUpdateRequestUpdateType>(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_PHONE));
        Assert.Equal(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_NAME, _sut.Map<Oracle.DisputeUpdateRequestUpdateType>(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_NAME));
        Assert.Equal(Oracle.DisputeUpdateRequestUpdateType.COUNT, _sut.Map<Oracle.DisputeUpdateRequestUpdateType>(Domain.Models.DisputeUpdateRequestUpdateType.COUNT));
        Assert.Equal(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL, _sut.Map<Oracle.DisputeUpdateRequestUpdateType>(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL));
        Assert.Equal(Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT, _sut.Map<Oracle.DisputeUpdateRequestUpdateType>(Domain.Models.DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT));
        Assert.Equal(Oracle.DisputeUpdateRequestUpdateType.COURT_OPTIONS, _sut.Map<Oracle.DisputeUpdateRequestUpdateType>(Domain.Models.DisputeUpdateRequestUpdateType.COURT_OPTIONS));
        #endregion DisputeUpdateRequestUpdateType

        #region DocumentType
        // Oracle => Domain
        Assert.Equal(Domain.Models.DocumentType.UNKNOWN, _sut.Map<Domain.Models.DocumentType>(Oracle.DocumentType.UNKNOWN));
        Assert.Equal(Domain.Models.DocumentType.NOTICE_OF_DISPUTE, _sut.Map<Domain.Models.DocumentType>(Oracle.DocumentType.NOTICE_OF_DISPUTE));
        Assert.Equal(Domain.Models.DocumentType.TICKET_IMAGE, _sut.Map<Domain.Models.DocumentType>(Oracle.DocumentType.TICKET_IMAGE));
        // Domain => Oracle
        Assert.Equal(Oracle.DocumentType.UNKNOWN, _sut.Map<Oracle.DocumentType>(Domain.Models.DocumentType.UNKNOWN));
        Assert.Equal(Oracle.DocumentType.NOTICE_OF_DISPUTE, _sut.Map<Oracle.DocumentType>(Domain.Models.DocumentType.NOTICE_OF_DISPUTE));
        Assert.Equal(Oracle.DocumentType.TICKET_IMAGE, _sut.Map<Oracle.DocumentType>(Domain.Models.DocumentType.TICKET_IMAGE));
        #endregion DocumentType

        #region EmailHistorySuccessfullySent
        // Oracle => Domain
        Assert.Equal(Domain.Models.EmailHistorySuccessfullySent.UNKNOWN, _sut.Map<Domain.Models.EmailHistorySuccessfullySent>(Oracle.EmailHistorySuccessfullySent.UNKNOWN));
        Assert.Equal(Domain.Models.EmailHistorySuccessfullySent.Y, _sut.Map<Domain.Models.EmailHistorySuccessfullySent>(Oracle.EmailHistorySuccessfullySent.Y));
        Assert.Equal(Domain.Models.EmailHistorySuccessfullySent.N, _sut.Map<Domain.Models.EmailHistorySuccessfullySent>(Oracle.EmailHistorySuccessfullySent.N));
        // Domain => Oracle
        Assert.Equal(Oracle.EmailHistorySuccessfullySent.UNKNOWN, _sut.Map<Oracle.EmailHistorySuccessfullySent>(Domain.Models.EmailHistorySuccessfullySent.UNKNOWN));
        Assert.Equal(Oracle.EmailHistorySuccessfullySent.Y, _sut.Map<Oracle.EmailHistorySuccessfullySent>(Domain.Models.EmailHistorySuccessfullySent.Y));
        Assert.Equal(Oracle.EmailHistorySuccessfullySent.N, _sut.Map<Oracle.EmailHistorySuccessfullySent>(Domain.Models.EmailHistorySuccessfullySent.N));
        #endregion EmailHistorySuccessfullySent

        #region ExcludeStatus
        // Oracle => Domain
        Assert.Equal(Domain.Models.ExcludeStatus.UNKNOWN, _sut.Map<Domain.Models.ExcludeStatus>(Oracle.ExcludeStatus.UNKNOWN));
        Assert.Equal(Domain.Models.ExcludeStatus.NEW, _sut.Map<Domain.Models.ExcludeStatus>(Oracle.ExcludeStatus.NEW));
        Assert.Equal(Domain.Models.ExcludeStatus.VALIDATED, _sut.Map<Domain.Models.ExcludeStatus>(Oracle.ExcludeStatus.VALIDATED));
        Assert.Equal(Domain.Models.ExcludeStatus.PROCESSING, _sut.Map<Domain.Models.ExcludeStatus>(Oracle.ExcludeStatus.PROCESSING));
        Assert.Equal(Domain.Models.ExcludeStatus.REJECTED, _sut.Map<Domain.Models.ExcludeStatus>(Oracle.ExcludeStatus.REJECTED));
        Assert.Equal(Domain.Models.ExcludeStatus.CANCELLED, _sut.Map<Domain.Models.ExcludeStatus>(Oracle.ExcludeStatus.CANCELLED));
        Assert.Equal(Domain.Models.ExcludeStatus.CONCLUDED, _sut.Map<Domain.Models.ExcludeStatus>(Oracle.ExcludeStatus.CONCLUDED));
        // Domain => Oracle
        Assert.Equal(Oracle.ExcludeStatus.UNKNOWN, _sut.Map<Oracle.ExcludeStatus>(Domain.Models.ExcludeStatus.UNKNOWN));
        Assert.Equal(Oracle.ExcludeStatus.NEW, _sut.Map<Oracle.ExcludeStatus>(Domain.Models.ExcludeStatus.NEW));
        Assert.Equal(Oracle.ExcludeStatus.VALIDATED, _sut.Map<Oracle.ExcludeStatus>(Domain.Models.ExcludeStatus.VALIDATED));
        Assert.Equal(Oracle.ExcludeStatus.PROCESSING, _sut.Map<Oracle.ExcludeStatus>(Domain.Models.ExcludeStatus.PROCESSING));
        Assert.Equal(Oracle.ExcludeStatus.REJECTED, _sut.Map<Oracle.ExcludeStatus>(Domain.Models.ExcludeStatus.REJECTED));
        Assert.Equal(Oracle.ExcludeStatus.CANCELLED, _sut.Map<Oracle.ExcludeStatus>(Domain.Models.ExcludeStatus.CANCELLED));
        Assert.Equal(Oracle.ExcludeStatus.CONCLUDED, _sut.Map<Oracle.ExcludeStatus>(Domain.Models.ExcludeStatus.CONCLUDED));
        #endregion ExcludeStatus

        #region ExcludeStatus2
        // Oracle => Domain
        Assert.Equal(Domain.Models.ExcludeStatus2.UNKNOWN, _sut.Map<Domain.Models.ExcludeStatus2>(Oracle.ExcludeStatus2.UNKNOWN));
        Assert.Equal(Domain.Models.ExcludeStatus2.NEW, _sut.Map<Domain.Models.ExcludeStatus2>(Oracle.ExcludeStatus2.NEW));
        Assert.Equal(Domain.Models.ExcludeStatus2.VALIDATED, _sut.Map<Domain.Models.ExcludeStatus2>(Oracle.ExcludeStatus2.VALIDATED));
        Assert.Equal(Domain.Models.ExcludeStatus2.PROCESSING, _sut.Map<Domain.Models.ExcludeStatus2>(Oracle.ExcludeStatus2.PROCESSING));
        Assert.Equal(Domain.Models.ExcludeStatus2.REJECTED, _sut.Map<Domain.Models.ExcludeStatus2>(Oracle.ExcludeStatus2.REJECTED));
        Assert.Equal(Domain.Models.ExcludeStatus2.CANCELLED, _sut.Map<Domain.Models.ExcludeStatus2>(Oracle.ExcludeStatus2.CANCELLED));
        Assert.Equal(Domain.Models.ExcludeStatus2.CONCLUDED, _sut.Map<Domain.Models.ExcludeStatus2>(Oracle.ExcludeStatus2.CONCLUDED));
        // Domain => Oracle
        Assert.Equal(Oracle.ExcludeStatus2.UNKNOWN, _sut.Map<Oracle.ExcludeStatus2>(Domain.Models.ExcludeStatus2.UNKNOWN));
        Assert.Equal(Oracle.ExcludeStatus2.NEW, _sut.Map<Oracle.ExcludeStatus2>(Domain.Models.ExcludeStatus2.NEW));
        Assert.Equal(Oracle.ExcludeStatus2.VALIDATED, _sut.Map<Oracle.ExcludeStatus2>(Domain.Models.ExcludeStatus2.VALIDATED));
        Assert.Equal(Oracle.ExcludeStatus2.PROCESSING, _sut.Map<Oracle.ExcludeStatus2>(Domain.Models.ExcludeStatus2.PROCESSING));
        Assert.Equal(Oracle.ExcludeStatus2.REJECTED, _sut.Map<Oracle.ExcludeStatus2>(Domain.Models.ExcludeStatus2.REJECTED));
        Assert.Equal(Oracle.ExcludeStatus2.CANCELLED, _sut.Map<Oracle.ExcludeStatus2>(Domain.Models.ExcludeStatus2.CANCELLED));
        Assert.Equal(Oracle.ExcludeStatus2.CONCLUDED, _sut.Map<Oracle.ExcludeStatus2>(Domain.Models.ExcludeStatus2.CONCLUDED));
        #endregion ExcludeStatus2

        #region FileHistoryAuditLogEntryType
        // Oracle => Domain
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.UNKNOWN, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.UNKNOWN));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.ARFL, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.ARFL));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CAIN, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CAIN));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CAWT, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CAWT));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CCAN, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CCAN));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CCON, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CCON));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CCWR, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CCWR));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CLEG, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CLEG));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CUEM, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CUEM));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CUEV, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CUEV));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CUIN, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CUIN));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CULG, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CULG));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CUPD, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CUPD));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CUWR, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CUWR));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.CUWT, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.CUWT));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.DURA, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.DURA));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.DURR, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.DURR));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMCA, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMCA));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMCF, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMCF));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMCR, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMCR));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMDC, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMDC));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMFD, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMFD));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMPR, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMPR));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMRJ, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMRJ));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMRV, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMRV));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMST, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMST));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMUP, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMUP));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.EMVF, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.EMVF));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.ESUR, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.ESUR));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.FDLD, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.FDLD));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.FDLS, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.FDLS));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.FUPD, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.FUPD));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.FUPS, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.FUPS));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.FRMK, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.FRMK));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.INIT, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.INIT));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.JASG, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.JASG));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.JCNF, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.JCNF));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.JDIV, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.JDIV));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.JPRG, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.JPRG));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.OCNT, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.OCNT));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.RCLD, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.RCLD));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.RECN, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.RECN));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.SADM, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.SADM));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.SCAN, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.SCAN));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.SPRC, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.SPRC));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.SREJ, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.SREJ));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.SUB, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.SUB));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.SUPL, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.SUPL));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.SVAL, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.SVAL));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.URSR, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.URSR));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.VREV, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.VREV));
        Assert.Equal(Domain.Models.FileHistoryAuditLogEntryType.VSUB, _sut.Map<Domain.Models.FileHistoryAuditLogEntryType>(Oracle.FileHistoryAuditLogEntryType.VSUB));
        // Domain => Oracle
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.UNKNOWN, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.UNKNOWN));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.ARFL, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.ARFL));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CAIN, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CAIN));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CAWT, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CAWT));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CCAN, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CCAN));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CCON, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CCON));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CCWR, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CCWR));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CLEG, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CLEG));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CUEM, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CUEM));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CUEV, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CUEV));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CUIN, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CUIN));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CULG, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CULG));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CUPD, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CUPD));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CUWR, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CUWR));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.CUWT, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.CUWT));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.DURA, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.DURA));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.DURR, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.DURR));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMCA, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMCA));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMCF, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMCF));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMCR, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMCR));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMDC, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMDC));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMFD, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMFD));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMPR, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMPR));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMRJ, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMRJ));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMRV, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMRV));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMST, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMST));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMUP, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMUP));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.EMVF, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.EMVF));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.ESUR, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.ESUR));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.FDLD, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.FDLD));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.FDLS, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.FDLS));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.FUPD, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.FUPD));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.FUPS, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.FUPS));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.FRMK, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.FRMK));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.INIT, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.INIT));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.JASG, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.JASG));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.JCNF, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.JCNF));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.JDIV, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.JDIV));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.JPRG, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.JPRG));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.OCNT, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.OCNT));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.RCLD, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.RCLD));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.RECN, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.RECN));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.SADM, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.SADM));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.SCAN, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.SCAN));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.SPRC, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.SPRC));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.SREJ, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.SREJ));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.SUB, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.SUB));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.SUPL, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.SUPL));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.SVAL, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.SVAL));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.URSR, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.URSR));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.VREV, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.VREV));
        Assert.Equal(Oracle.FileHistoryAuditLogEntryType.VSUB, _sut.Map<Oracle.FileHistoryAuditLogEntryType>(Domain.Models.FileHistoryAuditLogEntryType.VSUB));
        #endregion FileHistoryAuditLogEntryType

        #region JJDisputeAccidentYn
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeAccidentYn.UNKNOWN, _sut.Map<Domain.Models.JJDisputeAccidentYn>(Oracle.JJDisputeAccidentYn.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeAccidentYn.Y, _sut.Map<Domain.Models.JJDisputeAccidentYn>(Oracle.JJDisputeAccidentYn.Y));
        Assert.Equal(Domain.Models.JJDisputeAccidentYn.N, _sut.Map<Domain.Models.JJDisputeAccidentYn>(Oracle.JJDisputeAccidentYn.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeAccidentYn.UNKNOWN, _sut.Map<Oracle.JJDisputeAccidentYn>(Domain.Models.JJDisputeAccidentYn.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeAccidentYn.Y, _sut.Map<Oracle.JJDisputeAccidentYn>(Domain.Models.JJDisputeAccidentYn.Y));
        Assert.Equal(Oracle.JJDisputeAccidentYn.N, _sut.Map<Oracle.JJDisputeAccidentYn>(Domain.Models.JJDisputeAccidentYn.N));
        #endregion JJDisputeAccidentYn

        #region JJDisputeAppearInCourt
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeAppearInCourt.UNKNOWN, _sut.Map<Domain.Models.JJDisputeAppearInCourt>(Oracle.JJDisputeAppearInCourt.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeAppearInCourt.Y, _sut.Map<Domain.Models.JJDisputeAppearInCourt>(Oracle.JJDisputeAppearInCourt.Y));
        Assert.Equal(Domain.Models.JJDisputeAppearInCourt.N, _sut.Map<Domain.Models.JJDisputeAppearInCourt>(Oracle.JJDisputeAppearInCourt.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeAppearInCourt.UNKNOWN, _sut.Map<Oracle.JJDisputeAppearInCourt>(Domain.Models.JJDisputeAppearInCourt.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeAppearInCourt.Y, _sut.Map<Oracle.JJDisputeAppearInCourt>(Domain.Models.JJDisputeAppearInCourt.Y));
        Assert.Equal(Oracle.JJDisputeAppearInCourt.N, _sut.Map<Oracle.JJDisputeAppearInCourt>(Domain.Models.JJDisputeAppearInCourt.N));
        #endregion JJDisputeAppearInCourt

        #region JJDisputeContactType
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeContactType.UNKNOWN, _sut.Map<Domain.Models.JJDisputeContactType>(Oracle.JJDisputeContactType.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeContactType.INDIVIDUAL, _sut.Map<Domain.Models.JJDisputeContactType>(Oracle.JJDisputeContactType.INDIVIDUAL));
        Assert.Equal(Domain.Models.JJDisputeContactType.LAWYER, _sut.Map<Domain.Models.JJDisputeContactType>(Oracle.JJDisputeContactType.LAWYER));
        Assert.Equal(Domain.Models.JJDisputeContactType.OTHER, _sut.Map<Domain.Models.JJDisputeContactType>(Oracle.JJDisputeContactType.OTHER));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeContactType.UNKNOWN, _sut.Map<Oracle.JJDisputeContactType>(Domain.Models.JJDisputeContactType.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeContactType.INDIVIDUAL, _sut.Map<Oracle.JJDisputeContactType>(Domain.Models.JJDisputeContactType.INDIVIDUAL));
        Assert.Equal(Oracle.JJDisputeContactType.LAWYER, _sut.Map<Oracle.JJDisputeContactType>(Domain.Models.JJDisputeContactType.LAWYER));
        Assert.Equal(Oracle.JJDisputeContactType.OTHER, _sut.Map<Oracle.JJDisputeContactType>(Domain.Models.JJDisputeContactType.OTHER));
        #endregion JJDisputeContactType

        #region JJDisputeCourtAppearanceRoPAppCd
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPAppCd>(Oracle.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPAppCd.A, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPAppCd>(Oracle.JJDisputeCourtAppearanceRoPAppCd.A));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPAppCd.P, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPAppCd>(Oracle.JJDisputeCourtAppearanceRoPAppCd.P));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPAppCd.N, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPAppCd>(Oracle.JJDisputeCourtAppearanceRoPAppCd.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPAppCd>(Domain.Models.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPAppCd.A, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPAppCd>(Domain.Models.JJDisputeCourtAppearanceRoPAppCd.A));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPAppCd.P, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPAppCd>(Domain.Models.JJDisputeCourtAppearanceRoPAppCd.P));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPAppCd.N, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPAppCd>(Domain.Models.JJDisputeCourtAppearanceRoPAppCd.N));
        #endregion JJDisputeCourtAppearanceRoPAppCd

        #region JJDisputeCourtAppearanceRoPCrown
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPCrown.UNKNOWN, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPCrown>(Oracle.JJDisputeCourtAppearanceRoPCrown.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPCrown.P, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPCrown>(Oracle.JJDisputeCourtAppearanceRoPCrown.P));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPCrown.N, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPCrown>(Oracle.JJDisputeCourtAppearanceRoPCrown.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPCrown.UNKNOWN, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPCrown>(Domain.Models.JJDisputeCourtAppearanceRoPCrown.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPCrown.P, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPCrown>(Domain.Models.JJDisputeCourtAppearanceRoPCrown.P));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPCrown.N, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPCrown>(Domain.Models.JJDisputeCourtAppearanceRoPCrown.N));
        #endregion JJDisputeCourtAppearanceRoPCrown

        #region JJDisputeCourtAppearanceRoPDattCd
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPDattCd>(Oracle.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPDattCd.A, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPDattCd>(Oracle.JJDisputeCourtAppearanceRoPDattCd.A));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPDattCd.C, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPDattCd>(Oracle.JJDisputeCourtAppearanceRoPDattCd.C));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPDattCd.N, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPDattCd>(Oracle.JJDisputeCourtAppearanceRoPDattCd.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPDattCd>(Domain.Models.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPDattCd.A, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPDattCd>(Domain.Models.JJDisputeCourtAppearanceRoPDattCd.A));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPDattCd.C, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPDattCd>(Domain.Models.JJDisputeCourtAppearanceRoPDattCd.C));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPDattCd.N, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPDattCd>(Domain.Models.JJDisputeCourtAppearanceRoPDattCd.N));
        #endregion JJDisputeCourtAppearanceRoPDattCd

        #region JJDisputeCourtAppearanceRoPJjSeized
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPJjSeized>(Oracle.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPJjSeized.Y, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPJjSeized>(Oracle.JJDisputeCourtAppearanceRoPJjSeized.Y));
        Assert.Equal(Domain.Models.JJDisputeCourtAppearanceRoPJjSeized.N, _sut.Map<Domain.Models.JJDisputeCourtAppearanceRoPJjSeized>(Oracle.JJDisputeCourtAppearanceRoPJjSeized.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPJjSeized>(Domain.Models.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPJjSeized.Y, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPJjSeized>(Domain.Models.JJDisputeCourtAppearanceRoPJjSeized.Y));
        Assert.Equal(Oracle.JJDisputeCourtAppearanceRoPJjSeized.N, _sut.Map<Oracle.JJDisputeCourtAppearanceRoPJjSeized>(Domain.Models.JJDisputeCourtAppearanceRoPJjSeized.N));
        #endregion JJDisputeCourtAppearanceRoPJjSeized

        #region JJDisputedCountAppearInCourt
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountAppearInCourt.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountAppearInCourt>(Oracle.JJDisputedCountAppearInCourt.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountAppearInCourt.Y, _sut.Map<Domain.Models.JJDisputedCountAppearInCourt>(Oracle.JJDisputedCountAppearInCourt.Y));
        Assert.Equal(Domain.Models.JJDisputedCountAppearInCourt.N, _sut.Map<Domain.Models.JJDisputedCountAppearInCourt>(Oracle.JJDisputedCountAppearInCourt.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountAppearInCourt.UNKNOWN, _sut.Map<Oracle.JJDisputedCountAppearInCourt>(Domain.Models.JJDisputedCountAppearInCourt.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountAppearInCourt.Y, _sut.Map<Oracle.JJDisputedCountAppearInCourt>(Domain.Models.JJDisputedCountAppearInCourt.Y));
        Assert.Equal(Oracle.JJDisputedCountAppearInCourt.N, _sut.Map<Oracle.JJDisputedCountAppearInCourt>(Domain.Models.JJDisputedCountAppearInCourt.N));
        #endregion JJDisputedCountAppearInCourt

        #region JJDisputedCountIncludesSurcharge
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountIncludesSurcharge.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountIncludesSurcharge>(Oracle.JJDisputedCountIncludesSurcharge.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountIncludesSurcharge.Y, _sut.Map<Domain.Models.JJDisputedCountIncludesSurcharge>(Oracle.JJDisputedCountIncludesSurcharge.Y));
        Assert.Equal(Domain.Models.JJDisputedCountIncludesSurcharge.N, _sut.Map<Domain.Models.JJDisputedCountIncludesSurcharge>(Oracle.JJDisputedCountIncludesSurcharge.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountIncludesSurcharge.UNKNOWN, _sut.Map<Oracle.JJDisputedCountIncludesSurcharge>(Domain.Models.JJDisputedCountIncludesSurcharge.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountIncludesSurcharge.Y, _sut.Map<Oracle.JJDisputedCountIncludesSurcharge>(Domain.Models.JJDisputedCountIncludesSurcharge.Y));
        Assert.Equal(Oracle.JJDisputedCountIncludesSurcharge.N, _sut.Map<Oracle.JJDisputedCountIncludesSurcharge>(Domain.Models.JJDisputedCountIncludesSurcharge.N));
        #endregion JJDisputedCountIncludesSurcharge

        #region JJDisputedCountLatestPlea
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountLatestPlea.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountLatestPlea>(Oracle.JJDisputedCountLatestPlea.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountLatestPlea.G, _sut.Map<Domain.Models.JJDisputedCountLatestPlea>(Oracle.JJDisputedCountLatestPlea.G));
        Assert.Equal(Domain.Models.JJDisputedCountLatestPlea.N, _sut.Map<Domain.Models.JJDisputedCountLatestPlea>(Oracle.JJDisputedCountLatestPlea.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountLatestPlea.UNKNOWN, _sut.Map<Oracle.JJDisputedCountLatestPlea>(Domain.Models.JJDisputedCountLatestPlea.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountLatestPlea.G, _sut.Map<Oracle.JJDisputedCountLatestPlea>(Domain.Models.JJDisputedCountLatestPlea.G));
        Assert.Equal(Oracle.JJDisputedCountLatestPlea.N, _sut.Map<Oracle.JJDisputedCountLatestPlea>(Domain.Models.JJDisputedCountLatestPlea.N));
        #endregion JJDisputedCountLatestPlea

        #region JJDisputedCountPlea
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountPlea.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountPlea>(Oracle.JJDisputedCountPlea.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountPlea.G, _sut.Map<Domain.Models.JJDisputedCountPlea>(Oracle.JJDisputedCountPlea.G));
        Assert.Equal(Domain.Models.JJDisputedCountPlea.N, _sut.Map<Domain.Models.JJDisputedCountPlea>(Oracle.JJDisputedCountPlea.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountPlea.UNKNOWN, _sut.Map<Oracle.JJDisputedCountPlea>(Domain.Models.JJDisputedCountPlea.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountPlea.G, _sut.Map<Oracle.JJDisputedCountPlea>(Domain.Models.JJDisputedCountPlea.G));
        Assert.Equal(Oracle.JJDisputedCountPlea.N, _sut.Map<Oracle.JJDisputedCountPlea>(Domain.Models.JJDisputedCountPlea.N));
        #endregion JJDisputedCountPlea

        #region JJDisputedCountRequestReduction
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountRequestReduction.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountRequestReduction>(Oracle.JJDisputedCountRequestReduction.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountRequestReduction.Y, _sut.Map<Domain.Models.JJDisputedCountRequestReduction>(Oracle.JJDisputedCountRequestReduction.Y));
        Assert.Equal(Domain.Models.JJDisputedCountRequestReduction.N, _sut.Map<Domain.Models.JJDisputedCountRequestReduction>(Oracle.JJDisputedCountRequestReduction.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountRequestReduction.UNKNOWN, _sut.Map<Oracle.JJDisputedCountRequestReduction>(Domain.Models.JJDisputedCountRequestReduction.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountRequestReduction.Y, _sut.Map<Oracle.JJDisputedCountRequestReduction>(Domain.Models.JJDisputedCountRequestReduction.Y));
        Assert.Equal(Oracle.JJDisputedCountRequestReduction.N, _sut.Map<Oracle.JJDisputedCountRequestReduction>(Domain.Models.JJDisputedCountRequestReduction.N));
        #endregion JJDisputedCountRequestReduction

        #region JJDisputedCountRequestTimeToPay
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountRequestTimeToPay.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountRequestTimeToPay>(Oracle.JJDisputedCountRequestTimeToPay.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountRequestTimeToPay.Y, _sut.Map<Domain.Models.JJDisputedCountRequestTimeToPay>(Oracle.JJDisputedCountRequestTimeToPay.Y));
        Assert.Equal(Domain.Models.JJDisputedCountRequestTimeToPay.N, _sut.Map<Domain.Models.JJDisputedCountRequestTimeToPay>(Oracle.JJDisputedCountRequestTimeToPay.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountRequestTimeToPay.UNKNOWN, _sut.Map<Oracle.JJDisputedCountRequestTimeToPay>(Domain.Models.JJDisputedCountRequestTimeToPay.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountRequestTimeToPay.Y, _sut.Map<Oracle.JJDisputedCountRequestTimeToPay>(Domain.Models.JJDisputedCountRequestTimeToPay.Y));
        Assert.Equal(Oracle.JJDisputedCountRequestTimeToPay.N, _sut.Map<Oracle.JJDisputedCountRequestTimeToPay>(Domain.Models.JJDisputedCountRequestTimeToPay.N));
        #endregion JJDisputedCountRequestTimeToPay

        #region JJDisputedCountRoPAbatement
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountRoPAbatement.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountRoPAbatement>(Oracle.JJDisputedCountRoPAbatement.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountRoPAbatement.Y, _sut.Map<Domain.Models.JJDisputedCountRoPAbatement>(Oracle.JJDisputedCountRoPAbatement.Y));
        Assert.Equal(Domain.Models.JJDisputedCountRoPAbatement.N, _sut.Map<Domain.Models.JJDisputedCountRoPAbatement>(Oracle.JJDisputedCountRoPAbatement.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountRoPAbatement.UNKNOWN, _sut.Map<Oracle.JJDisputedCountRoPAbatement>(Domain.Models.JJDisputedCountRoPAbatement.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountRoPAbatement.Y, _sut.Map<Oracle.JJDisputedCountRoPAbatement>(Domain.Models.JJDisputedCountRoPAbatement.Y));
        Assert.Equal(Oracle.JJDisputedCountRoPAbatement.N, _sut.Map<Oracle.JJDisputedCountRoPAbatement>(Domain.Models.JJDisputedCountRoPAbatement.N));
        #endregion JJDisputedCountRoPAbatement

        #region JJDisputedCountRoPDismissed
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountRoPDismissed.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountRoPDismissed>(Oracle.JJDisputedCountRoPDismissed.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountRoPDismissed.Y, _sut.Map<Domain.Models.JJDisputedCountRoPDismissed>(Oracle.JJDisputedCountRoPDismissed.Y));
        Assert.Equal(Domain.Models.JJDisputedCountRoPDismissed.N, _sut.Map<Domain.Models.JJDisputedCountRoPDismissed>(Oracle.JJDisputedCountRoPDismissed.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountRoPDismissed.UNKNOWN, _sut.Map<Oracle.JJDisputedCountRoPDismissed>(Domain.Models.JJDisputedCountRoPDismissed.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountRoPDismissed.Y, _sut.Map<Oracle.JJDisputedCountRoPDismissed>(Domain.Models.JJDisputedCountRoPDismissed.Y));
        Assert.Equal(Oracle.JJDisputedCountRoPDismissed.N, _sut.Map<Oracle.JJDisputedCountRoPDismissed>(Domain.Models.JJDisputedCountRoPDismissed.N));
        #endregion JJDisputedCountRoPDismissed

        #region JJDisputedCountRoPFinding
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountRoPFinding.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountRoPFinding>(Oracle.JJDisputedCountRoPFinding.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountRoPFinding.GUILTY, _sut.Map<Domain.Models.JJDisputedCountRoPFinding>(Oracle.JJDisputedCountRoPFinding.GUILTY));
        Assert.Equal(Domain.Models.JJDisputedCountRoPFinding.NOT_GUILTY, _sut.Map<Domain.Models.JJDisputedCountRoPFinding>(Oracle.JJDisputedCountRoPFinding.NOT_GUILTY));
        Assert.Equal(Domain.Models.JJDisputedCountRoPFinding.CANCELLED, _sut.Map<Domain.Models.JJDisputedCountRoPFinding>(Oracle.JJDisputedCountRoPFinding.CANCELLED));
        Assert.Equal(Domain.Models.JJDisputedCountRoPFinding.PAID_PRIOR_TO_APPEARANCE, _sut.Map<Domain.Models.JJDisputedCountRoPFinding>(Oracle.JJDisputedCountRoPFinding.PAID_PRIOR_TO_APPEARANCE));
        Assert.Equal(Domain.Models.JJDisputedCountRoPFinding.GUILTY_LESSER, _sut.Map<Domain.Models.JJDisputedCountRoPFinding>(Oracle.JJDisputedCountRoPFinding.GUILTY_LESSER));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountRoPFinding.UNKNOWN, _sut.Map<Oracle.JJDisputedCountRoPFinding>(Domain.Models.JJDisputedCountRoPFinding.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountRoPFinding.GUILTY, _sut.Map<Oracle.JJDisputedCountRoPFinding>(Domain.Models.JJDisputedCountRoPFinding.GUILTY));
        Assert.Equal(Oracle.JJDisputedCountRoPFinding.NOT_GUILTY, _sut.Map<Oracle.JJDisputedCountRoPFinding>(Domain.Models.JJDisputedCountRoPFinding.NOT_GUILTY));
        Assert.Equal(Oracle.JJDisputedCountRoPFinding.CANCELLED, _sut.Map<Oracle.JJDisputedCountRoPFinding>(Domain.Models.JJDisputedCountRoPFinding.CANCELLED));
        Assert.Equal(Oracle.JJDisputedCountRoPFinding.PAID_PRIOR_TO_APPEARANCE, _sut.Map<Oracle.JJDisputedCountRoPFinding>(Domain.Models.JJDisputedCountRoPFinding.PAID_PRIOR_TO_APPEARANCE));
        Assert.Equal(Oracle.JJDisputedCountRoPFinding.GUILTY_LESSER, _sut.Map<Oracle.JJDisputedCountRoPFinding>(Domain.Models.JJDisputedCountRoPFinding.GUILTY_LESSER));
        #endregion JJDisputedCountRoPFinding

        #region JJDisputedCountRoPForWantOfProsecution
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountRoPForWantOfProsecution.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountRoPForWantOfProsecution>(Oracle.JJDisputedCountRoPForWantOfProsecution.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountRoPForWantOfProsecution.Y, _sut.Map<Domain.Models.JJDisputedCountRoPForWantOfProsecution>(Oracle.JJDisputedCountRoPForWantOfProsecution.Y));
        Assert.Equal(Domain.Models.JJDisputedCountRoPForWantOfProsecution.N, _sut.Map<Domain.Models.JJDisputedCountRoPForWantOfProsecution>(Oracle.JJDisputedCountRoPForWantOfProsecution.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountRoPForWantOfProsecution.UNKNOWN, _sut.Map<Oracle.JJDisputedCountRoPForWantOfProsecution>(Domain.Models.JJDisputedCountRoPForWantOfProsecution.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountRoPForWantOfProsecution.Y, _sut.Map<Oracle.JJDisputedCountRoPForWantOfProsecution>(Domain.Models.JJDisputedCountRoPForWantOfProsecution.Y));
        Assert.Equal(Oracle.JJDisputedCountRoPForWantOfProsecution.N, _sut.Map<Oracle.JJDisputedCountRoPForWantOfProsecution>(Domain.Models.JJDisputedCountRoPForWantOfProsecution.N));
        #endregion JJDisputedCountRoPForWantOfProsecution

        #region JJDisputedCountRoPJailIntermittent
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountRoPJailIntermittent.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountRoPJailIntermittent>(Oracle.JJDisputedCountRoPJailIntermittent.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountRoPJailIntermittent.Y, _sut.Map<Domain.Models.JJDisputedCountRoPJailIntermittent>(Oracle.JJDisputedCountRoPJailIntermittent.Y));
        Assert.Equal(Domain.Models.JJDisputedCountRoPJailIntermittent.N, _sut.Map<Domain.Models.JJDisputedCountRoPJailIntermittent>(Oracle.JJDisputedCountRoPJailIntermittent.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountRoPJailIntermittent.UNKNOWN, _sut.Map<Oracle.JJDisputedCountRoPJailIntermittent>(Domain.Models.JJDisputedCountRoPJailIntermittent.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountRoPJailIntermittent.Y, _sut.Map<Oracle.JJDisputedCountRoPJailIntermittent>(Domain.Models.JJDisputedCountRoPJailIntermittent.Y));
        Assert.Equal(Oracle.JJDisputedCountRoPJailIntermittent.N, _sut.Map<Oracle.JJDisputedCountRoPJailIntermittent>(Domain.Models.JJDisputedCountRoPJailIntermittent.N));
        #endregion JJDisputedCountRoPJailIntermittent

        #region JJDisputedCountRoPWithdrawn
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputedCountRoPWithdrawn.UNKNOWN, _sut.Map<Domain.Models.JJDisputedCountRoPWithdrawn>(Oracle.JJDisputedCountRoPWithdrawn.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputedCountRoPWithdrawn.Y, _sut.Map<Domain.Models.JJDisputedCountRoPWithdrawn>(Oracle.JJDisputedCountRoPWithdrawn.Y));
        Assert.Equal(Domain.Models.JJDisputedCountRoPWithdrawn.N, _sut.Map<Domain.Models.JJDisputedCountRoPWithdrawn>(Oracle.JJDisputedCountRoPWithdrawn.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputedCountRoPWithdrawn.UNKNOWN, _sut.Map<Oracle.JJDisputedCountRoPWithdrawn>(Domain.Models.JJDisputedCountRoPWithdrawn.UNKNOWN));
        Assert.Equal(Oracle.JJDisputedCountRoPWithdrawn.Y, _sut.Map<Oracle.JJDisputedCountRoPWithdrawn>(Domain.Models.JJDisputedCountRoPWithdrawn.Y));
        Assert.Equal(Oracle.JJDisputedCountRoPWithdrawn.N, _sut.Map<Oracle.JJDisputedCountRoPWithdrawn>(Domain.Models.JJDisputedCountRoPWithdrawn.N));
        #endregion JJDisputedCountRoPWithdrawn

        #region JJDisputeDisputantAttendanceType
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeDisputantAttendanceType.UNKNOWN, _sut.Map<Domain.Models.JJDisputeDisputantAttendanceType>(Oracle.JJDisputeDisputantAttendanceType.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeDisputantAttendanceType.WRITTEN_REASONS, _sut.Map<Domain.Models.JJDisputeDisputantAttendanceType>(Oracle.JJDisputeDisputantAttendanceType.WRITTEN_REASONS));
        Assert.Equal(Domain.Models.JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE, _sut.Map<Domain.Models.JJDisputeDisputantAttendanceType>(Oracle.JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE));
        Assert.Equal(Domain.Models.JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE, _sut.Map<Domain.Models.JJDisputeDisputantAttendanceType>(Oracle.JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE));
        Assert.Equal(Domain.Models.JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO, _sut.Map<Domain.Models.JJDisputeDisputantAttendanceType>(Oracle.JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO));
        Assert.Equal(Domain.Models.JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO, _sut.Map<Domain.Models.JJDisputeDisputantAttendanceType>(Oracle.JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO));
        Assert.Equal(Domain.Models.JJDisputeDisputantAttendanceType.IN_PERSON, _sut.Map<Domain.Models.JJDisputeDisputantAttendanceType>(Oracle.JJDisputeDisputantAttendanceType.IN_PERSON));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeDisputantAttendanceType.UNKNOWN, _sut.Map<Oracle.JJDisputeDisputantAttendanceType>(Domain.Models.JJDisputeDisputantAttendanceType.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeDisputantAttendanceType.WRITTEN_REASONS, _sut.Map<Oracle.JJDisputeDisputantAttendanceType>(Domain.Models.JJDisputeDisputantAttendanceType.WRITTEN_REASONS));
        Assert.Equal(Oracle.JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE, _sut.Map<Oracle.JJDisputeDisputantAttendanceType>(Domain.Models.JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE));
        Assert.Equal(Oracle.JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE, _sut.Map<Oracle.JJDisputeDisputantAttendanceType>(Domain.Models.JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE));
        Assert.Equal(Oracle.JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO, _sut.Map<Oracle.JJDisputeDisputantAttendanceType>(Domain.Models.JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO));
        Assert.Equal(Oracle.JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO, _sut.Map<Oracle.JJDisputeDisputantAttendanceType>(Domain.Models.JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO));
        Assert.Equal(Oracle.JJDisputeDisputantAttendanceType.IN_PERSON, _sut.Map<Oracle.JJDisputeDisputantAttendanceType>(Domain.Models.JJDisputeDisputantAttendanceType.IN_PERSON));
        #endregion JJDisputeDisputantAttendanceType

        #region JJDisputeElectronicTicketYn
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeElectronicTicketYn.UNKNOWN, _sut.Map<Domain.Models.JJDisputeElectronicTicketYn>(Oracle.JJDisputeElectronicTicketYn.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeElectronicTicketYn.Y, _sut.Map<Domain.Models.JJDisputeElectronicTicketYn>(Oracle.JJDisputeElectronicTicketYn.Y));
        Assert.Equal(Domain.Models.JJDisputeElectronicTicketYn.N, _sut.Map<Domain.Models.JJDisputeElectronicTicketYn>(Oracle.JJDisputeElectronicTicketYn.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeElectronicTicketYn.UNKNOWN, _sut.Map<Oracle.JJDisputeElectronicTicketYn>(Domain.Models.JJDisputeElectronicTicketYn.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeElectronicTicketYn.Y, _sut.Map<Oracle.JJDisputeElectronicTicketYn>(Domain.Models.JJDisputeElectronicTicketYn.Y));
        Assert.Equal(Oracle.JJDisputeElectronicTicketYn.N, _sut.Map<Oracle.JJDisputeElectronicTicketYn>(Domain.Models.JJDisputeElectronicTicketYn.N));
        #endregion JJDisputeElectronicTicketYn

        #region JJDisputeHearingType
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeHearingType.UNKNOWN, _sut.Map<Domain.Models.JJDisputeHearingType>(Oracle.JJDisputeHearingType.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeHearingType.COURT_APPEARANCE, _sut.Map<Domain.Models.JJDisputeHearingType>(Oracle.JJDisputeHearingType.COURT_APPEARANCE));
        Assert.Equal(Domain.Models.JJDisputeHearingType.WRITTEN_REASONS, _sut.Map<Domain.Models.JJDisputeHearingType>(Oracle.JJDisputeHearingType.WRITTEN_REASONS));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeHearingType.UNKNOWN, _sut.Map<Oracle.JJDisputeHearingType>(Domain.Models.JJDisputeHearingType.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeHearingType.COURT_APPEARANCE, _sut.Map<Oracle.JJDisputeHearingType>(Domain.Models.JJDisputeHearingType.COURT_APPEARANCE));
        Assert.Equal(Oracle.JJDisputeHearingType.WRITTEN_REASONS, _sut.Map<Oracle.JJDisputeHearingType>(Domain.Models.JJDisputeHearingType.WRITTEN_REASONS));
        #endregion JJDisputeHearingType

        #region JJDisputeMultipleOfficersYn
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeMultipleOfficersYn.UNKNOWN, _sut.Map<Domain.Models.JJDisputeMultipleOfficersYn>(Oracle.JJDisputeMultipleOfficersYn.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeMultipleOfficersYn.Y, _sut.Map<Domain.Models.JJDisputeMultipleOfficersYn>(Oracle.JJDisputeMultipleOfficersYn.Y));
        Assert.Equal(Domain.Models.JJDisputeMultipleOfficersYn.N, _sut.Map<Domain.Models.JJDisputeMultipleOfficersYn>(Oracle.JJDisputeMultipleOfficersYn.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeMultipleOfficersYn.UNKNOWN, _sut.Map<Oracle.JJDisputeMultipleOfficersYn>(Domain.Models.JJDisputeMultipleOfficersYn.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeMultipleOfficersYn.Y, _sut.Map<Oracle.JJDisputeMultipleOfficersYn>(Domain.Models.JJDisputeMultipleOfficersYn.Y));
        Assert.Equal(Oracle.JJDisputeMultipleOfficersYn.N, _sut.Map<Oracle.JJDisputeMultipleOfficersYn>(Domain.Models.JJDisputeMultipleOfficersYn.N));
        #endregion JJDisputeMultipleOfficersYn

        #region JJDisputeNoticeOfHearingYn
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeNoticeOfHearingYn.UNKNOWN, _sut.Map<Domain.Models.JJDisputeNoticeOfHearingYn>(Oracle.JJDisputeNoticeOfHearingYn.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeNoticeOfHearingYn.Y, _sut.Map<Domain.Models.JJDisputeNoticeOfHearingYn>(Oracle.JJDisputeNoticeOfHearingYn.Y));
        Assert.Equal(Domain.Models.JJDisputeNoticeOfHearingYn.N, _sut.Map<Domain.Models.JJDisputeNoticeOfHearingYn>(Oracle.JJDisputeNoticeOfHearingYn.N));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeNoticeOfHearingYn.UNKNOWN, _sut.Map<Oracle.JJDisputeNoticeOfHearingYn>(Domain.Models.JJDisputeNoticeOfHearingYn.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeNoticeOfHearingYn.Y, _sut.Map<Oracle.JJDisputeNoticeOfHearingYn>(Domain.Models.JJDisputeNoticeOfHearingYn.Y));
        Assert.Equal(Oracle.JJDisputeNoticeOfHearingYn.N, _sut.Map<Oracle.JJDisputeNoticeOfHearingYn>(Domain.Models.JJDisputeNoticeOfHearingYn.N));
        #endregion JJDisputeNoticeOfHearingYn

        #region JJDisputeSignatoryType
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeSignatoryType.U, _sut.Map<Domain.Models.JJDisputeSignatoryType>(Oracle.JJDisputeSignatoryType.U));
        Assert.Equal(Domain.Models.JJDisputeSignatoryType.D, _sut.Map<Domain.Models.JJDisputeSignatoryType>(Oracle.JJDisputeSignatoryType.D));
        Assert.Equal(Domain.Models.JJDisputeSignatoryType.A, _sut.Map<Domain.Models.JJDisputeSignatoryType>(Oracle.JJDisputeSignatoryType.A));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeSignatoryType.U, _sut.Map<Oracle.JJDisputeSignatoryType>(Domain.Models.JJDisputeSignatoryType.U));
        Assert.Equal(Oracle.JJDisputeSignatoryType.D, _sut.Map<Oracle.JJDisputeSignatoryType>(Domain.Models.JJDisputeSignatoryType.D));
        Assert.Equal(Oracle.JJDisputeSignatoryType.A, _sut.Map<Oracle.JJDisputeSignatoryType>(Domain.Models.JJDisputeSignatoryType.A));
        #endregion JJDisputeSignatoryType

        #region JJDisputeStatus
        // Oracle => Domain
        Assert.Equal(Domain.Models.JJDisputeStatus.UNKNOWN, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.UNKNOWN));
        Assert.Equal(Domain.Models.JJDisputeStatus.NEW, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.NEW));
        Assert.Equal(Domain.Models.JJDisputeStatus.IN_PROGRESS, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.IN_PROGRESS));
        Assert.Equal(Domain.Models.JJDisputeStatus.DATA_UPDATE, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.DATA_UPDATE));
        Assert.Equal(Domain.Models.JJDisputeStatus.CONFIRMED, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.CONFIRMED));
        Assert.Equal(Domain.Models.JJDisputeStatus.REQUIRE_COURT_HEARING, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.REQUIRE_COURT_HEARING));
        Assert.Equal(Domain.Models.JJDisputeStatus.REQUIRE_MORE_INFO, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.REQUIRE_MORE_INFO));
        Assert.Equal(Domain.Models.JJDisputeStatus.ACCEPTED, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.ACCEPTED));
        Assert.Equal(Domain.Models.JJDisputeStatus.REVIEW, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.REVIEW));
        Assert.Equal(Domain.Models.JJDisputeStatus.CONCLUDED, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.CONCLUDED));
        Assert.Equal(Domain.Models.JJDisputeStatus.CANCELLED, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.CANCELLED));
        Assert.Equal(Domain.Models.JJDisputeStatus.HEARING_SCHEDULED, _sut.Map<Domain.Models.JJDisputeStatus>(Oracle.JJDisputeStatus.HEARING_SCHEDULED));
        // Domain => Oracle
        Assert.Equal(Oracle.JJDisputeStatus.UNKNOWN, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.UNKNOWN));
        Assert.Equal(Oracle.JJDisputeStatus.NEW, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.NEW));
        Assert.Equal(Oracle.JJDisputeStatus.IN_PROGRESS, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.IN_PROGRESS));
        Assert.Equal(Oracle.JJDisputeStatus.DATA_UPDATE, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.DATA_UPDATE));
        Assert.Equal(Oracle.JJDisputeStatus.CONFIRMED, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.CONFIRMED));
        Assert.Equal(Oracle.JJDisputeStatus.REQUIRE_COURT_HEARING, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.REQUIRE_COURT_HEARING));
        Assert.Equal(Oracle.JJDisputeStatus.REQUIRE_MORE_INFO, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.REQUIRE_MORE_INFO));
        Assert.Equal(Oracle.JJDisputeStatus.ACCEPTED, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.ACCEPTED));
        Assert.Equal(Oracle.JJDisputeStatus.REVIEW, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.REVIEW));
        Assert.Equal(Oracle.JJDisputeStatus.CONCLUDED, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.CONCLUDED));
        Assert.Equal(Oracle.JJDisputeStatus.CANCELLED, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.CANCELLED));
        Assert.Equal(Oracle.JJDisputeStatus.HEARING_SCHEDULED, _sut.Map<Oracle.JJDisputeStatus>(Domain.Models.JJDisputeStatus.HEARING_SCHEDULED));
        #endregion JJDisputeStatus

        #region Status
        // Oracle => Domain
        Assert.Equal(Domain.Models.Status.UNKNOWN, _sut.Map<Domain.Models.Status>(Oracle.Status.UNKNOWN));
        Assert.Equal(Domain.Models.Status.ACCEPTED, _sut.Map<Domain.Models.Status>(Oracle.Status.ACCEPTED));
        Assert.Equal(Domain.Models.Status.PENDING, _sut.Map<Domain.Models.Status>(Oracle.Status.PENDING));
        Assert.Equal(Domain.Models.Status.REJECTED, _sut.Map<Domain.Models.Status>(Oracle.Status.REJECTED));
        // Domain => Oracle
        Assert.Equal(Oracle.Status.UNKNOWN, _sut.Map<Oracle.Status>(Domain.Models.Status.UNKNOWN));
        Assert.Equal(Oracle.Status.ACCEPTED, _sut.Map<Oracle.Status>(Domain.Models.Status.ACCEPTED));
        Assert.Equal(Oracle.Status.PENDING, _sut.Map<Oracle.Status>(Domain.Models.Status.PENDING));
        Assert.Equal(Oracle.Status.REJECTED, _sut.Map<Oracle.Status>(Domain.Models.Status.REJECTED));
        #endregion Status

        #region TicketImageDataJustinDocumentReportType
        // Oracle => Domain
        Assert.Equal(Domain.Models.TicketImageDataJustinDocumentReportType.UNKNOWN, _sut.Map<Domain.Models.TicketImageDataJustinDocumentReportType>(Oracle.TicketImageDataJustinDocumentReportType.UNKNOWN));
        Assert.Equal(Domain.Models.TicketImageDataJustinDocumentReportType.NOTICE_OF_DISPUTE, _sut.Map<Domain.Models.TicketImageDataJustinDocumentReportType>(Oracle.TicketImageDataJustinDocumentReportType.NOTICE_OF_DISPUTE));
        Assert.Equal(Domain.Models.TicketImageDataJustinDocumentReportType.TICKET_IMAGE, _sut.Map<Domain.Models.TicketImageDataJustinDocumentReportType>(Oracle.TicketImageDataJustinDocumentReportType.TICKET_IMAGE));
        // Domain => Oracle
        Assert.Equal(Oracle.TicketImageDataJustinDocumentReportType.UNKNOWN, _sut.Map<Oracle.TicketImageDataJustinDocumentReportType>(Domain.Models.TicketImageDataJustinDocumentReportType.UNKNOWN));
        Assert.Equal(Oracle.TicketImageDataJustinDocumentReportType.NOTICE_OF_DISPUTE, _sut.Map<Oracle.TicketImageDataJustinDocumentReportType>(Domain.Models.TicketImageDataJustinDocumentReportType.NOTICE_OF_DISPUTE));
        Assert.Equal(Oracle.TicketImageDataJustinDocumentReportType.TICKET_IMAGE, _sut.Map<Oracle.TicketImageDataJustinDocumentReportType>(Domain.Models.TicketImageDataJustinDocumentReportType.TICKET_IMAGE));
        #endregion TicketImageDataJustinDocumentReportType

        #region ViolationTicketCountIsAct
        // Oracle => Domain
        Assert.Equal(Domain.Models.ViolationTicketCountIsAct.UNKNOWN, _sut.Map<Domain.Models.ViolationTicketCountIsAct>(Oracle.ViolationTicketCountIsAct.UNKNOWN));
        Assert.Equal(Domain.Models.ViolationTicketCountIsAct.Y, _sut.Map<Domain.Models.ViolationTicketCountIsAct>(Oracle.ViolationTicketCountIsAct.Y));
        Assert.Equal(Domain.Models.ViolationTicketCountIsAct.N, _sut.Map<Domain.Models.ViolationTicketCountIsAct>(Oracle.ViolationTicketCountIsAct.N));
        // Domain => Oracle
        Assert.Equal(Oracle.ViolationTicketCountIsAct.UNKNOWN, _sut.Map<Oracle.ViolationTicketCountIsAct>(Domain.Models.ViolationTicketCountIsAct.UNKNOWN));
        Assert.Equal(Oracle.ViolationTicketCountIsAct.Y, _sut.Map<Oracle.ViolationTicketCountIsAct>(Domain.Models.ViolationTicketCountIsAct.Y));
        Assert.Equal(Oracle.ViolationTicketCountIsAct.N, _sut.Map<Oracle.ViolationTicketCountIsAct>(Domain.Models.ViolationTicketCountIsAct.N));
        #endregion ViolationTicketCountIsAct

        #region ViolationTicketCountIsRegulation
        // Oracle => Domain
        Assert.Equal(Domain.Models.ViolationTicketCountIsRegulation.UNKNOWN, _sut.Map<Domain.Models.ViolationTicketCountIsRegulation>(Oracle.ViolationTicketCountIsRegulation.UNKNOWN));
        Assert.Equal(Domain.Models.ViolationTicketCountIsRegulation.Y, _sut.Map<Domain.Models.ViolationTicketCountIsRegulation>(Oracle.ViolationTicketCountIsRegulation.Y));
        Assert.Equal(Domain.Models.ViolationTicketCountIsRegulation.N, _sut.Map<Domain.Models.ViolationTicketCountIsRegulation>(Oracle.ViolationTicketCountIsRegulation.N));
        // Domain => Oracle
        Assert.Equal(Oracle.ViolationTicketCountIsRegulation.UNKNOWN, _sut.Map<Oracle.ViolationTicketCountIsRegulation>(Domain.Models.ViolationTicketCountIsRegulation.UNKNOWN));
        Assert.Equal(Oracle.ViolationTicketCountIsRegulation.Y, _sut.Map<Oracle.ViolationTicketCountIsRegulation>(Domain.Models.ViolationTicketCountIsRegulation.Y));
        Assert.Equal(Oracle.ViolationTicketCountIsRegulation.N, _sut.Map<Oracle.ViolationTicketCountIsRegulation>(Domain.Models.ViolationTicketCountIsRegulation.N));
        #endregion ViolationTicketCountIsRegulation

        #region ViolationTicketIsChangeOfAddress
        // Oracle => Domain
        Assert.Equal(Domain.Models.ViolationTicketIsChangeOfAddress.UNKNOWN, _sut.Map<Domain.Models.ViolationTicketIsChangeOfAddress>(Oracle.ViolationTicketIsChangeOfAddress.UNKNOWN));
        Assert.Equal(Domain.Models.ViolationTicketIsChangeOfAddress.Y, _sut.Map<Domain.Models.ViolationTicketIsChangeOfAddress>(Oracle.ViolationTicketIsChangeOfAddress.Y));
        Assert.Equal(Domain.Models.ViolationTicketIsChangeOfAddress.N, _sut.Map<Domain.Models.ViolationTicketIsChangeOfAddress>(Oracle.ViolationTicketIsChangeOfAddress.N));
        // Domain => Oracle
        Assert.Equal(Oracle.ViolationTicketIsChangeOfAddress.UNKNOWN, _sut.Map<Oracle.ViolationTicketIsChangeOfAddress>(Domain.Models.ViolationTicketIsChangeOfAddress.UNKNOWN));
        Assert.Equal(Oracle.ViolationTicketIsChangeOfAddress.Y, _sut.Map<Oracle.ViolationTicketIsChangeOfAddress>(Domain.Models.ViolationTicketIsChangeOfAddress.Y));
        Assert.Equal(Oracle.ViolationTicketIsChangeOfAddress.N, _sut.Map<Oracle.ViolationTicketIsChangeOfAddress>(Domain.Models.ViolationTicketIsChangeOfAddress.N));
        #endregion ViolationTicketIsChangeOfAddress

        #region ViolationTicketIsDriver
        // Oracle => Domain
        Assert.Equal(Domain.Models.ViolationTicketIsDriver.UNKNOWN, _sut.Map<Domain.Models.ViolationTicketIsDriver>(Oracle.ViolationTicketIsDriver.UNKNOWN));
        Assert.Equal(Domain.Models.ViolationTicketIsDriver.Y, _sut.Map<Domain.Models.ViolationTicketIsDriver>(Oracle.ViolationTicketIsDriver.Y));
        Assert.Equal(Domain.Models.ViolationTicketIsDriver.N, _sut.Map<Domain.Models.ViolationTicketIsDriver>(Oracle.ViolationTicketIsDriver.N));
        // Domain => Oracle
        Assert.Equal(Oracle.ViolationTicketIsDriver.UNKNOWN, _sut.Map<Oracle.ViolationTicketIsDriver>(Domain.Models.ViolationTicketIsDriver.UNKNOWN));
        Assert.Equal(Oracle.ViolationTicketIsDriver.Y, _sut.Map<Oracle.ViolationTicketIsDriver>(Domain.Models.ViolationTicketIsDriver.Y));
        Assert.Equal(Oracle.ViolationTicketIsDriver.N, _sut.Map<Oracle.ViolationTicketIsDriver>(Domain.Models.ViolationTicketIsDriver.N));
        #endregion ViolationTicketIsDriver

        #region ViolationTicketIsOwner
        // Oracle => Domain
        Assert.Equal(Domain.Models.ViolationTicketIsOwner.UNKNOWN, _sut.Map<Domain.Models.ViolationTicketIsOwner>(Oracle.ViolationTicketIsOwner.UNKNOWN));
        Assert.Equal(Domain.Models.ViolationTicketIsOwner.Y, _sut.Map<Domain.Models.ViolationTicketIsOwner>(Oracle.ViolationTicketIsOwner.Y));
        Assert.Equal(Domain.Models.ViolationTicketIsOwner.N, _sut.Map<Domain.Models.ViolationTicketIsOwner>(Oracle.ViolationTicketIsOwner.N));
        // Domain => Oracle
        Assert.Equal(Oracle.ViolationTicketIsOwner.UNKNOWN, _sut.Map<Oracle.ViolationTicketIsOwner>(Domain.Models.ViolationTicketIsOwner.UNKNOWN));
        Assert.Equal(Oracle.ViolationTicketIsOwner.Y, _sut.Map<Oracle.ViolationTicketIsOwner>(Domain.Models.ViolationTicketIsOwner.Y));
        Assert.Equal(Oracle.ViolationTicketIsOwner.N, _sut.Map<Oracle.ViolationTicketIsOwner>(Domain.Models.ViolationTicketIsOwner.N));
        #endregion ViolationTicketIsOwner

        #region ViolationTicketIsYoungPerson
        // Oracle => Domain
        Assert.Equal(Domain.Models.ViolationTicketIsYoungPerson.UNKNOWN, _sut.Map<Domain.Models.ViolationTicketIsYoungPerson>(Oracle.ViolationTicketIsYoungPerson.UNKNOWN));
        Assert.Equal(Domain.Models.ViolationTicketIsYoungPerson.Y, _sut.Map<Domain.Models.ViolationTicketIsYoungPerson>(Oracle.ViolationTicketIsYoungPerson.Y));
        Assert.Equal(Domain.Models.ViolationTicketIsYoungPerson.N, _sut.Map<Domain.Models.ViolationTicketIsYoungPerson>(Oracle.ViolationTicketIsYoungPerson.N));
        // Domain => Oracle
        Assert.Equal(Oracle.ViolationTicketIsYoungPerson.UNKNOWN, _sut.Map<Oracle.ViolationTicketIsYoungPerson>(Domain.Models.ViolationTicketIsYoungPerson.UNKNOWN));
        Assert.Equal(Oracle.ViolationTicketIsYoungPerson.Y, _sut.Map<Oracle.ViolationTicketIsYoungPerson>(Domain.Models.ViolationTicketIsYoungPerson.Y));
        Assert.Equal(Oracle.ViolationTicketIsYoungPerson.N, _sut.Map<Oracle.ViolationTicketIsYoungPerson>(Domain.Models.ViolationTicketIsYoungPerson.N));
        #endregion ViolationTicketIsYoungPerson


    }
}

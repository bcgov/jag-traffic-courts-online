using Oracle = TrafficCourts.OracleDataApi.Client.V1;
using DomainModel = TrafficCourts.Domain.Models;

namespace TrafficCourts.OracleDataApi.Client.V1;

[System.CodeDom.Compiler.GeneratedCode("DomainModelMappingTestGenerator.generate_mapper", "")]
public class OracleDomainModelMappingProfile : AutoMapper.Profile
{
    public OracleDomainModelMappingProfile()
    {
        // enumerations
        CreateMap<Oracle.DisputeContactTypeCd, DomainModel.DisputeContactTypeCd>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeCountPleaCode, DomainModel.DisputeCountPleaCode>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeListItemJjDisputeStatus, DomainModel.DisputeListItemJjDisputeStatus>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeListItemStatus, DomainModel.DisputeListItemStatus>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeResultDisputeStatus, DomainModel.DisputeResultDisputeStatus>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeResultJjDisputeHearingType, DomainModel.DisputeResultJjDisputeHearingType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeResultJjDisputeStatus, DomainModel.DisputeResultJjDisputeStatus>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeSignatoryType, DomainModel.DisputeSignatoryType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeStatus, DomainModel.DisputeStatus>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeUpdateRequestStatus, DomainModel.DisputeUpdateRequestStatus>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeUpdateRequestStatus2, DomainModel.DisputeUpdateRequestStatus2>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeUpdateRequestUpdateType, DomainModel.DisputeUpdateRequestUpdateType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DocumentType, DomainModel.DocumentType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.ExcludeStatus, DomainModel.ExcludeStatus>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.ExcludeStatus2, DomainModel.ExcludeStatus2>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.FileHistoryAuditLogEntryType, DomainModel.FileHistoryAuditLogEntryType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeContactType, DomainModel.JJDisputeContactType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeCourtAppearanceRoPAppCd, DomainModel.JJDisputeCourtAppearanceRoPAppCd>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeCourtAppearanceRoPCrown, DomainModel.JJDisputeCourtAppearanceRoPCrown>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeCourtAppearanceRoPDattCd, DomainModel.JJDisputeCourtAppearanceRoPDattCd>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountLatestPlea, DomainModel.JJDisputedCountLatestPlea>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountPlea, DomainModel.JJDisputedCountPlea>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountRoPFinding, DomainModel.JJDisputedCountRoPFinding>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeDisputantAttendanceType, DomainModel.JJDisputeDisputantAttendanceType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeHearingType, DomainModel.JJDisputeHearingType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeSignatoryType, DomainModel.JJDisputeSignatoryType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeStatus, DomainModel.JJDisputeStatus>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.Status, DomainModel.Status>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.TicketImageDataJustinDocumentReportType, DomainModel.TicketImageDataJustinDocumentReportType>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeAppearanceLessThan14DaysYn, DomainModel.DisputeAppearanceLessThan14DaysYn>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeCountRequestCourtAppearance, DomainModel.DisputeCountRequestCourtAppearance>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeCountRequestReduction, DomainModel.DisputeCountRequestReduction>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeCountRequestTimeToPay, DomainModel.DisputeCountRequestTimeToPay>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeDisputantDetectedOcrIssues, DomainModel.DisputeDisputantDetectedOcrIssues>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeInterpreterRequired, DomainModel.DisputeInterpreterRequired>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeListItemDisputantDetectedOcrIssues, DomainModel.DisputeListItemDisputantDetectedOcrIssues>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeListItemRequestCourtAppearanceYn, DomainModel.DisputeListItemRequestCourtAppearanceYn>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeListItemSystemDetectedOcrIssues, DomainModel.DisputeListItemSystemDetectedOcrIssues>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeRepresentedByLawyer, DomainModel.DisputeRepresentedByLawyer>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeRequestCourtAppearanceYn, DomainModel.DisputeRequestCourtAppearanceYn>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.DisputeSystemDetectedOcrIssues, DomainModel.DisputeSystemDetectedOcrIssues>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.EmailHistorySuccessfullySent, DomainModel.EmailHistorySuccessfullySent>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeAccidentYn, DomainModel.JJDisputeAccidentYn>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeAppearInCourt, DomainModel.JJDisputeAppearInCourt>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeCourtAppearanceRoPJjSeized, DomainModel.JJDisputeCourtAppearanceRoPJjSeized>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountAppearInCourt, DomainModel.JJDisputedCountAppearInCourt>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountIncludesSurcharge, DomainModel.JJDisputedCountIncludesSurcharge>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountRequestReduction, DomainModel.JJDisputedCountRequestReduction>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountRequestTimeToPay, DomainModel.JJDisputedCountRequestTimeToPay>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountRoPAbatement, DomainModel.JJDisputedCountRoPAbatement>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountRoPDismissed, DomainModel.JJDisputedCountRoPDismissed>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountRoPForWantOfProsecution, DomainModel.JJDisputedCountRoPForWantOfProsecution>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountRoPJailIntermittent, DomainModel.JJDisputedCountRoPJailIntermittent>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputedCountRoPWithdrawn, DomainModel.JJDisputedCountRoPWithdrawn>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeElectronicTicketYn, DomainModel.JJDisputeElectronicTicketYn>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeMultipleOfficersYn, DomainModel.JJDisputeMultipleOfficersYn>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.JJDisputeNoticeOfHearingYn, DomainModel.JJDisputeNoticeOfHearingYn>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.ViolationTicketCountIsAct, DomainModel.ViolationTicketCountIsAct>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.ViolationTicketCountIsRegulation, DomainModel.ViolationTicketCountIsRegulation>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.ViolationTicketIsChangeOfAddress, DomainModel.ViolationTicketIsChangeOfAddress>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.ViolationTicketIsDriver, DomainModel.ViolationTicketIsDriver>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.ViolationTicketIsOwner, DomainModel.ViolationTicketIsOwner>().ConvertUsing<EnumTypeConverter>();
        CreateMap<Oracle.ViolationTicketIsYoungPerson, DomainModel.ViolationTicketIsYoungPerson>().ConvertUsing<EnumTypeConverter>();

        // classes
        CreateMap<Oracle.Dispute, DomainModel.Dispute>()
            .ForMember(dest => dest.FileData, opt => opt.Ignore())
            .ForMember(dest => dest.IcbcName, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<Oracle.DisputeCount, DomainModel.DisputeCount>().ReverseMap();
        CreateMap<Oracle.DisputeListItem, DomainModel.DisputeListItem>().ReverseMap();
        CreateMap<Oracle.DisputeResult, DomainModel.DisputeResult>().ReverseMap();
        CreateMap<Oracle.DisputeUpdateRequest, DomainModel.DisputeUpdateRequest>().ReverseMap();
        CreateMap<Oracle.EmailHistory, DomainModel.EmailHistory>().ReverseMap();
        CreateMap<Oracle.FileHistory, DomainModel.FileHistory>().ReverseMap();
        CreateMap<Oracle.JJDispute, DomainModel.JJDispute>()
            .ForMember(dest => dest.FileData, opt => opt.Ignore())
            .ForMember(dest => dest.LockId, opt => opt.Ignore())
            .ForMember(dest => dest.LockedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LockExpiresAtUtc, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<Oracle.JJDisputeCourtAppearanceRoP, DomainModel.JJDisputeCourtAppearanceRoP>().ReverseMap();
        CreateMap<Oracle.JJDisputedCount, DomainModel.JJDisputedCount>().ReverseMap();
        CreateMap<Oracle.JJDisputedCountRoP, DomainModel.JJDisputedCountRoP>().ReverseMap();
        CreateMap<Oracle.JJDisputeRemark, DomainModel.JJDisputeRemark>().ReverseMap();
        CreateMap<Oracle.TicketImageDataJustinDocument, DomainModel.TicketImageDataJustinDocument>().ReverseMap();
        CreateMap<Oracle.ViolationTicket, DomainModel.ViolationTicket>()
            .ForMember(dest => dest.ViolationTicketImage, opt => opt.Ignore())
            .ForMember(dest => dest.OcrViolationTicket, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<Oracle.ViolationTicketCount, DomainModel.ViolationTicketCount>().ReverseMap();

        // FileResponse does not have a default constructor
        CreateMap<Oracle.FileResponse, DomainModel.FileResponse>()
            .ConstructUsing(src => new DomainModel.FileResponse(src.StatusCode, src.Headers, src.Stream, null, null));
        CreateMap<DomainModel.FileResponse, Oracle.FileResponse>()
            .ConstructUsing(src => new Oracle.FileResponse(src.StatusCode, src.Headers, src.Stream, null, null));
    }
}

[System.CodeDom.Compiler.GeneratedCode("DomainModelMappingTestGenerator.generate_mapper", "")]
internal class EnumTypeConverter :
    // Oracle to Domain Model
    AutoMapper.ITypeConverter<Oracle.DisputeContactTypeCd, DomainModel.DisputeContactTypeCd>,
    AutoMapper.ITypeConverter<Oracle.DisputeCountPleaCode, DomainModel.DisputeCountPleaCode>,
    AutoMapper.ITypeConverter<Oracle.DisputeListItemJjDisputeStatus, DomainModel.DisputeListItemJjDisputeStatus>,
    AutoMapper.ITypeConverter<Oracle.DisputeListItemStatus, DomainModel.DisputeListItemStatus>,
    AutoMapper.ITypeConverter<Oracle.DisputeResultDisputeStatus, DomainModel.DisputeResultDisputeStatus>,
    AutoMapper.ITypeConverter<Oracle.DisputeResultJjDisputeHearingType, DomainModel.DisputeResultJjDisputeHearingType>,
    AutoMapper.ITypeConverter<Oracle.DisputeResultJjDisputeStatus, DomainModel.DisputeResultJjDisputeStatus>,
    AutoMapper.ITypeConverter<Oracle.DisputeSignatoryType, DomainModel.DisputeSignatoryType>,
    AutoMapper.ITypeConverter<Oracle.DisputeStatus, DomainModel.DisputeStatus>,
    AutoMapper.ITypeConverter<Oracle.DisputeUpdateRequestStatus, DomainModel.DisputeUpdateRequestStatus>,
    AutoMapper.ITypeConverter<Oracle.DisputeUpdateRequestStatus2, DomainModel.DisputeUpdateRequestStatus2>,
    AutoMapper.ITypeConverter<Oracle.DisputeUpdateRequestUpdateType, DomainModel.DisputeUpdateRequestUpdateType>,
    AutoMapper.ITypeConverter<Oracle.DocumentType, DomainModel.DocumentType>,
    AutoMapper.ITypeConverter<Oracle.ExcludeStatus, DomainModel.ExcludeStatus>,
    AutoMapper.ITypeConverter<Oracle.ExcludeStatus2, DomainModel.ExcludeStatus2>,
    AutoMapper.ITypeConverter<Oracle.FileHistoryAuditLogEntryType, DomainModel.FileHistoryAuditLogEntryType>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeContactType, DomainModel.JJDisputeContactType>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeCourtAppearanceRoPAppCd, DomainModel.JJDisputeCourtAppearanceRoPAppCd>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeCourtAppearanceRoPCrown, DomainModel.JJDisputeCourtAppearanceRoPCrown>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeCourtAppearanceRoPDattCd, DomainModel.JJDisputeCourtAppearanceRoPDattCd>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountLatestPlea, DomainModel.JJDisputedCountLatestPlea>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountPlea, DomainModel.JJDisputedCountPlea>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountRoPFinding, DomainModel.JJDisputedCountRoPFinding>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeDisputantAttendanceType, DomainModel.JJDisputeDisputantAttendanceType>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeHearingType, DomainModel.JJDisputeHearingType>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeSignatoryType, DomainModel.JJDisputeSignatoryType>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeStatus, DomainModel.JJDisputeStatus>,
    AutoMapper.ITypeConverter<Oracle.Status, DomainModel.Status>,
    AutoMapper.ITypeConverter<Oracle.TicketImageDataJustinDocumentReportType, DomainModel.TicketImageDataJustinDocumentReportType>,
    // Oracle to Domain Model (Unknown, Yes or No)
    AutoMapper.ITypeConverter<Oracle.DisputeAppearanceLessThan14DaysYn, DomainModel.DisputeAppearanceLessThan14DaysYn>,
    AutoMapper.ITypeConverter<Oracle.DisputeCountRequestCourtAppearance, DomainModel.DisputeCountRequestCourtAppearance>,
    AutoMapper.ITypeConverter<Oracle.DisputeCountRequestReduction, DomainModel.DisputeCountRequestReduction>,
    AutoMapper.ITypeConverter<Oracle.DisputeCountRequestTimeToPay, DomainModel.DisputeCountRequestTimeToPay>,
    AutoMapper.ITypeConverter<Oracle.DisputeDisputantDetectedOcrIssues, DomainModel.DisputeDisputantDetectedOcrIssues>,
    AutoMapper.ITypeConverter<Oracle.DisputeInterpreterRequired, DomainModel.DisputeInterpreterRequired>,
    AutoMapper.ITypeConverter<Oracle.DisputeListItemDisputantDetectedOcrIssues, DomainModel.DisputeListItemDisputantDetectedOcrIssues>,
    AutoMapper.ITypeConverter<Oracle.DisputeListItemRequestCourtAppearanceYn, DomainModel.DisputeListItemRequestCourtAppearanceYn>,
    AutoMapper.ITypeConverter<Oracle.DisputeListItemSystemDetectedOcrIssues, DomainModel.DisputeListItemSystemDetectedOcrIssues>,
    AutoMapper.ITypeConverter<Oracle.DisputeRepresentedByLawyer, DomainModel.DisputeRepresentedByLawyer>,
    AutoMapper.ITypeConverter<Oracle.DisputeRequestCourtAppearanceYn, DomainModel.DisputeRequestCourtAppearanceYn>,
    AutoMapper.ITypeConverter<Oracle.DisputeSystemDetectedOcrIssues, DomainModel.DisputeSystemDetectedOcrIssues>,
    AutoMapper.ITypeConverter<Oracle.EmailHistorySuccessfullySent, DomainModel.EmailHistorySuccessfullySent>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeAccidentYn, DomainModel.JJDisputeAccidentYn>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeAppearInCourt, DomainModel.JJDisputeAppearInCourt>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeCourtAppearanceRoPJjSeized, DomainModel.JJDisputeCourtAppearanceRoPJjSeized>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountAppearInCourt, DomainModel.JJDisputedCountAppearInCourt>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountIncludesSurcharge, DomainModel.JJDisputedCountIncludesSurcharge>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountRequestReduction, DomainModel.JJDisputedCountRequestReduction>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountRequestTimeToPay, DomainModel.JJDisputedCountRequestTimeToPay>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountRoPAbatement, DomainModel.JJDisputedCountRoPAbatement>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountRoPDismissed, DomainModel.JJDisputedCountRoPDismissed>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountRoPForWantOfProsecution, DomainModel.JJDisputedCountRoPForWantOfProsecution>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountRoPJailIntermittent, DomainModel.JJDisputedCountRoPJailIntermittent>,
    AutoMapper.ITypeConverter<Oracle.JJDisputedCountRoPWithdrawn, DomainModel.JJDisputedCountRoPWithdrawn>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeElectronicTicketYn, DomainModel.JJDisputeElectronicTicketYn>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeMultipleOfficersYn, DomainModel.JJDisputeMultipleOfficersYn>,
    AutoMapper.ITypeConverter<Oracle.JJDisputeNoticeOfHearingYn, DomainModel.JJDisputeNoticeOfHearingYn>,
    AutoMapper.ITypeConverter<Oracle.ViolationTicketCountIsAct, DomainModel.ViolationTicketCountIsAct>,
    AutoMapper.ITypeConverter<Oracle.ViolationTicketCountIsRegulation, DomainModel.ViolationTicketCountIsRegulation>,
    AutoMapper.ITypeConverter<Oracle.ViolationTicketIsChangeOfAddress, DomainModel.ViolationTicketIsChangeOfAddress>,
    AutoMapper.ITypeConverter<Oracle.ViolationTicketIsDriver, DomainModel.ViolationTicketIsDriver>,
    AutoMapper.ITypeConverter<Oracle.ViolationTicketIsOwner, DomainModel.ViolationTicketIsOwner>,
    AutoMapper.ITypeConverter<Oracle.ViolationTicketIsYoungPerson, DomainModel.ViolationTicketIsYoungPerson>,
    // Domain Model to Oracle
    AutoMapper.ITypeConverter<DomainModel.DisputeContactTypeCd, Oracle.DisputeContactTypeCd>,
    AutoMapper.ITypeConverter<DomainModel.DisputeCountPleaCode, Oracle.DisputeCountPleaCode>,
    AutoMapper.ITypeConverter<DomainModel.DisputeListItemJjDisputeStatus, Oracle.DisputeListItemJjDisputeStatus>,
    AutoMapper.ITypeConverter<DomainModel.DisputeListItemStatus, Oracle.DisputeListItemStatus>,
    AutoMapper.ITypeConverter<DomainModel.DisputeResultDisputeStatus, Oracle.DisputeResultDisputeStatus>,
    AutoMapper.ITypeConverter<DomainModel.DisputeResultJjDisputeHearingType, Oracle.DisputeResultJjDisputeHearingType>,
    AutoMapper.ITypeConverter<DomainModel.DisputeResultJjDisputeStatus, Oracle.DisputeResultJjDisputeStatus>,
    AutoMapper.ITypeConverter<DomainModel.DisputeSignatoryType, Oracle.DisputeSignatoryType>,
    AutoMapper.ITypeConverter<DomainModel.DisputeStatus, Oracle.DisputeStatus>,
    AutoMapper.ITypeConverter<DomainModel.DisputeUpdateRequestStatus, Oracle.DisputeUpdateRequestStatus>,
    AutoMapper.ITypeConverter<DomainModel.DisputeUpdateRequestStatus2, Oracle.DisputeUpdateRequestStatus2>,
    AutoMapper.ITypeConverter<DomainModel.DisputeUpdateRequestUpdateType, Oracle.DisputeUpdateRequestUpdateType>,
    AutoMapper.ITypeConverter<DomainModel.DocumentType, Oracle.DocumentType>,
    AutoMapper.ITypeConverter<DomainModel.ExcludeStatus, Oracle.ExcludeStatus>,
    AutoMapper.ITypeConverter<DomainModel.ExcludeStatus2, Oracle.ExcludeStatus2>,
    AutoMapper.ITypeConverter<DomainModel.FileHistoryAuditLogEntryType, Oracle.FileHistoryAuditLogEntryType>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeContactType, Oracle.JJDisputeContactType>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeCourtAppearanceRoPAppCd, Oracle.JJDisputeCourtAppearanceRoPAppCd>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeCourtAppearanceRoPCrown, Oracle.JJDisputeCourtAppearanceRoPCrown>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeCourtAppearanceRoPDattCd, Oracle.JJDisputeCourtAppearanceRoPDattCd>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountLatestPlea, Oracle.JJDisputedCountLatestPlea>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountPlea, Oracle.JJDisputedCountPlea>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountRoPFinding, Oracle.JJDisputedCountRoPFinding>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeDisputantAttendanceType, Oracle.JJDisputeDisputantAttendanceType>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeHearingType, Oracle.JJDisputeHearingType>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeSignatoryType, Oracle.JJDisputeSignatoryType>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeStatus, Oracle.JJDisputeStatus>,
    AutoMapper.ITypeConverter<DomainModel.Status, Oracle.Status>,
    AutoMapper.ITypeConverter<DomainModel.TicketImageDataJustinDocumentReportType, Oracle.TicketImageDataJustinDocumentReportType>,
    // Domain Model to Oracle (Unknown, Yes or No)
    AutoMapper.ITypeConverter<DomainModel.DisputeAppearanceLessThan14DaysYn, Oracle.DisputeAppearanceLessThan14DaysYn>,
    AutoMapper.ITypeConverter<DomainModel.DisputeCountRequestCourtAppearance, Oracle.DisputeCountRequestCourtAppearance>,
    AutoMapper.ITypeConverter<DomainModel.DisputeCountRequestReduction, Oracle.DisputeCountRequestReduction>,
    AutoMapper.ITypeConverter<DomainModel.DisputeCountRequestTimeToPay, Oracle.DisputeCountRequestTimeToPay>,
    AutoMapper.ITypeConverter<DomainModel.DisputeDisputantDetectedOcrIssues, Oracle.DisputeDisputantDetectedOcrIssues>,
    AutoMapper.ITypeConverter<DomainModel.DisputeInterpreterRequired, Oracle.DisputeInterpreterRequired>,
    AutoMapper.ITypeConverter<DomainModel.DisputeListItemDisputantDetectedOcrIssues, Oracle.DisputeListItemDisputantDetectedOcrIssues>,
    AutoMapper.ITypeConverter<DomainModel.DisputeListItemRequestCourtAppearanceYn, Oracle.DisputeListItemRequestCourtAppearanceYn>,
    AutoMapper.ITypeConverter<DomainModel.DisputeListItemSystemDetectedOcrIssues, Oracle.DisputeListItemSystemDetectedOcrIssues>,
    AutoMapper.ITypeConverter<DomainModel.DisputeRepresentedByLawyer, Oracle.DisputeRepresentedByLawyer>,
    AutoMapper.ITypeConverter<DomainModel.DisputeRequestCourtAppearanceYn, Oracle.DisputeRequestCourtAppearanceYn>,
    AutoMapper.ITypeConverter<DomainModel.DisputeSystemDetectedOcrIssues, Oracle.DisputeSystemDetectedOcrIssues>,
    AutoMapper.ITypeConverter<DomainModel.EmailHistorySuccessfullySent, Oracle.EmailHistorySuccessfullySent>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeAccidentYn, Oracle.JJDisputeAccidentYn>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeAppearInCourt, Oracle.JJDisputeAppearInCourt>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeCourtAppearanceRoPJjSeized, Oracle.JJDisputeCourtAppearanceRoPJjSeized>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountAppearInCourt, Oracle.JJDisputedCountAppearInCourt>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountIncludesSurcharge, Oracle.JJDisputedCountIncludesSurcharge>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountRequestReduction, Oracle.JJDisputedCountRequestReduction>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountRequestTimeToPay, Oracle.JJDisputedCountRequestTimeToPay>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountRoPAbatement, Oracle.JJDisputedCountRoPAbatement>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountRoPDismissed, Oracle.JJDisputedCountRoPDismissed>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountRoPForWantOfProsecution, Oracle.JJDisputedCountRoPForWantOfProsecution>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountRoPJailIntermittent, Oracle.JJDisputedCountRoPJailIntermittent>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputedCountRoPWithdrawn, Oracle.JJDisputedCountRoPWithdrawn>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeElectronicTicketYn, Oracle.JJDisputeElectronicTicketYn>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeMultipleOfficersYn, Oracle.JJDisputeMultipleOfficersYn>,
    AutoMapper.ITypeConverter<DomainModel.JJDisputeNoticeOfHearingYn, Oracle.JJDisputeNoticeOfHearingYn>,
    AutoMapper.ITypeConverter<DomainModel.ViolationTicketCountIsAct, Oracle.ViolationTicketCountIsAct>,
    AutoMapper.ITypeConverter<DomainModel.ViolationTicketCountIsRegulation, Oracle.ViolationTicketCountIsRegulation>,
    AutoMapper.ITypeConverter<DomainModel.ViolationTicketIsChangeOfAddress, Oracle.ViolationTicketIsChangeOfAddress>,
    AutoMapper.ITypeConverter<DomainModel.ViolationTicketIsDriver, Oracle.ViolationTicketIsDriver>,
    AutoMapper.ITypeConverter<DomainModel.ViolationTicketIsOwner, Oracle.ViolationTicketIsOwner>,
    AutoMapper.ITypeConverter<DomainModel.ViolationTicketIsYoungPerson, Oracle.ViolationTicketIsYoungPerson>
{
    public DomainModel.DisputeContactTypeCd Convert(Oracle.DisputeContactTypeCd source, DomainModel.DisputeContactTypeCd destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeContactTypeCd.UNKNOWN => DomainModel.DisputeContactTypeCd.UNKNOWN,
            Oracle.DisputeContactTypeCd.INDIVIDUAL => DomainModel.DisputeContactTypeCd.INDIVIDUAL,
            Oracle.DisputeContactTypeCd.LAWYER => DomainModel.DisputeContactTypeCd.LAWYER,
            Oracle.DisputeContactTypeCd.OTHER => DomainModel.DisputeContactTypeCd.OTHER,
            _ => DomainModel.DisputeContactTypeCd.UNKNOWN
        };
    }

    public Oracle.DisputeContactTypeCd Convert(DomainModel.DisputeContactTypeCd source, Oracle.DisputeContactTypeCd destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeContactTypeCd.UNKNOWN => Oracle.DisputeContactTypeCd.UNKNOWN,
            DomainModel.DisputeContactTypeCd.INDIVIDUAL => Oracle.DisputeContactTypeCd.INDIVIDUAL,
            DomainModel.DisputeContactTypeCd.LAWYER => Oracle.DisputeContactTypeCd.LAWYER,
            DomainModel.DisputeContactTypeCd.OTHER => Oracle.DisputeContactTypeCd.OTHER,
            _ => Oracle.DisputeContactTypeCd.UNKNOWN
        };
    }

    public DomainModel.DisputeCountPleaCode Convert(Oracle.DisputeCountPleaCode source, DomainModel.DisputeCountPleaCode destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeCountPleaCode.UNKNOWN => DomainModel.DisputeCountPleaCode.UNKNOWN,
            Oracle.DisputeCountPleaCode.G => DomainModel.DisputeCountPleaCode.G,
            Oracle.DisputeCountPleaCode.N => DomainModel.DisputeCountPleaCode.N,
            _ => DomainModel.DisputeCountPleaCode.UNKNOWN
        };
    }

    public Oracle.DisputeCountPleaCode Convert(DomainModel.DisputeCountPleaCode source, Oracle.DisputeCountPleaCode destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeCountPleaCode.UNKNOWN => Oracle.DisputeCountPleaCode.UNKNOWN,
            DomainModel.DisputeCountPleaCode.G => Oracle.DisputeCountPleaCode.G,
            DomainModel.DisputeCountPleaCode.N => Oracle.DisputeCountPleaCode.N,
            _ => Oracle.DisputeCountPleaCode.UNKNOWN
        };
    }

    public DomainModel.DisputeListItemJjDisputeStatus Convert(Oracle.DisputeListItemJjDisputeStatus source, DomainModel.DisputeListItemJjDisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeListItemJjDisputeStatus.UNKNOWN => DomainModel.DisputeListItemJjDisputeStatus.UNKNOWN,
            Oracle.DisputeListItemJjDisputeStatus.NEW => DomainModel.DisputeListItemJjDisputeStatus.NEW,
            Oracle.DisputeListItemJjDisputeStatus.IN_PROGRESS => DomainModel.DisputeListItemJjDisputeStatus.IN_PROGRESS,
            Oracle.DisputeListItemJjDisputeStatus.DATA_UPDATE => DomainModel.DisputeListItemJjDisputeStatus.DATA_UPDATE,
            Oracle.DisputeListItemJjDisputeStatus.CONFIRMED => DomainModel.DisputeListItemJjDisputeStatus.CONFIRMED,
            Oracle.DisputeListItemJjDisputeStatus.REQUIRE_COURT_HEARING => DomainModel.DisputeListItemJjDisputeStatus.REQUIRE_COURT_HEARING,
            Oracle.DisputeListItemJjDisputeStatus.REQUIRE_MORE_INFO => DomainModel.DisputeListItemJjDisputeStatus.REQUIRE_MORE_INFO,
            Oracle.DisputeListItemJjDisputeStatus.ACCEPTED => DomainModel.DisputeListItemJjDisputeStatus.ACCEPTED,
            Oracle.DisputeListItemJjDisputeStatus.REVIEW => DomainModel.DisputeListItemJjDisputeStatus.REVIEW,
            Oracle.DisputeListItemJjDisputeStatus.CONCLUDED => DomainModel.DisputeListItemJjDisputeStatus.CONCLUDED,
            Oracle.DisputeListItemJjDisputeStatus.CANCELLED => DomainModel.DisputeListItemJjDisputeStatus.CANCELLED,
            Oracle.DisputeListItemJjDisputeStatus.HEARING_SCHEDULED => DomainModel.DisputeListItemJjDisputeStatus.HEARING_SCHEDULED,
            _ => DomainModel.DisputeListItemJjDisputeStatus.UNKNOWN
        };
    }

    public Oracle.DisputeListItemJjDisputeStatus Convert(DomainModel.DisputeListItemJjDisputeStatus source, Oracle.DisputeListItemJjDisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeListItemJjDisputeStatus.UNKNOWN => Oracle.DisputeListItemJjDisputeStatus.UNKNOWN,
            DomainModel.DisputeListItemJjDisputeStatus.NEW => Oracle.DisputeListItemJjDisputeStatus.NEW,
            DomainModel.DisputeListItemJjDisputeStatus.IN_PROGRESS => Oracle.DisputeListItemJjDisputeStatus.IN_PROGRESS,
            DomainModel.DisputeListItemJjDisputeStatus.DATA_UPDATE => Oracle.DisputeListItemJjDisputeStatus.DATA_UPDATE,
            DomainModel.DisputeListItemJjDisputeStatus.CONFIRMED => Oracle.DisputeListItemJjDisputeStatus.CONFIRMED,
            DomainModel.DisputeListItemJjDisputeStatus.REQUIRE_COURT_HEARING => Oracle.DisputeListItemJjDisputeStatus.REQUIRE_COURT_HEARING,
            DomainModel.DisputeListItemJjDisputeStatus.REQUIRE_MORE_INFO => Oracle.DisputeListItemJjDisputeStatus.REQUIRE_MORE_INFO,
            DomainModel.DisputeListItemJjDisputeStatus.ACCEPTED => Oracle.DisputeListItemJjDisputeStatus.ACCEPTED,
            DomainModel.DisputeListItemJjDisputeStatus.REVIEW => Oracle.DisputeListItemJjDisputeStatus.REVIEW,
            DomainModel.DisputeListItemJjDisputeStatus.CONCLUDED => Oracle.DisputeListItemJjDisputeStatus.CONCLUDED,
            DomainModel.DisputeListItemJjDisputeStatus.CANCELLED => Oracle.DisputeListItemJjDisputeStatus.CANCELLED,
            DomainModel.DisputeListItemJjDisputeStatus.HEARING_SCHEDULED => Oracle.DisputeListItemJjDisputeStatus.HEARING_SCHEDULED,
            _ => Oracle.DisputeListItemJjDisputeStatus.UNKNOWN
        };
    }

    public DomainModel.DisputeListItemStatus Convert(Oracle.DisputeListItemStatus source, DomainModel.DisputeListItemStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeListItemStatus.UNKNOWN => DomainModel.DisputeListItemStatus.UNKNOWN,
            Oracle.DisputeListItemStatus.NEW => DomainModel.DisputeListItemStatus.NEW,
            Oracle.DisputeListItemStatus.VALIDATED => DomainModel.DisputeListItemStatus.VALIDATED,
            Oracle.DisputeListItemStatus.PROCESSING => DomainModel.DisputeListItemStatus.PROCESSING,
            Oracle.DisputeListItemStatus.REJECTED => DomainModel.DisputeListItemStatus.REJECTED,
            Oracle.DisputeListItemStatus.CANCELLED => DomainModel.DisputeListItemStatus.CANCELLED,
            Oracle.DisputeListItemStatus.CONCLUDED => DomainModel.DisputeListItemStatus.CONCLUDED,
            _ => DomainModel.DisputeListItemStatus.UNKNOWN
        };
    }

    public Oracle.DisputeListItemStatus Convert(DomainModel.DisputeListItemStatus source, Oracle.DisputeListItemStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeListItemStatus.UNKNOWN => Oracle.DisputeListItemStatus.UNKNOWN,
            DomainModel.DisputeListItemStatus.NEW => Oracle.DisputeListItemStatus.NEW,
            DomainModel.DisputeListItemStatus.VALIDATED => Oracle.DisputeListItemStatus.VALIDATED,
            DomainModel.DisputeListItemStatus.PROCESSING => Oracle.DisputeListItemStatus.PROCESSING,
            DomainModel.DisputeListItemStatus.REJECTED => Oracle.DisputeListItemStatus.REJECTED,
            DomainModel.DisputeListItemStatus.CANCELLED => Oracle.DisputeListItemStatus.CANCELLED,
            DomainModel.DisputeListItemStatus.CONCLUDED => Oracle.DisputeListItemStatus.CONCLUDED,
            _ => Oracle.DisputeListItemStatus.UNKNOWN
        };
    }

    public DomainModel.DisputeResultDisputeStatus Convert(Oracle.DisputeResultDisputeStatus source, DomainModel.DisputeResultDisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeResultDisputeStatus.UNKNOWN => DomainModel.DisputeResultDisputeStatus.UNKNOWN,
            Oracle.DisputeResultDisputeStatus.NEW => DomainModel.DisputeResultDisputeStatus.NEW,
            Oracle.DisputeResultDisputeStatus.VALIDATED => DomainModel.DisputeResultDisputeStatus.VALIDATED,
            Oracle.DisputeResultDisputeStatus.PROCESSING => DomainModel.DisputeResultDisputeStatus.PROCESSING,
            Oracle.DisputeResultDisputeStatus.REJECTED => DomainModel.DisputeResultDisputeStatus.REJECTED,
            Oracle.DisputeResultDisputeStatus.CANCELLED => DomainModel.DisputeResultDisputeStatus.CANCELLED,
            Oracle.DisputeResultDisputeStatus.CONCLUDED => DomainModel.DisputeResultDisputeStatus.CONCLUDED,
            _ => DomainModel.DisputeResultDisputeStatus.UNKNOWN
        };
    }

    public Oracle.DisputeResultDisputeStatus Convert(DomainModel.DisputeResultDisputeStatus source, Oracle.DisputeResultDisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeResultDisputeStatus.UNKNOWN => Oracle.DisputeResultDisputeStatus.UNKNOWN,
            DomainModel.DisputeResultDisputeStatus.NEW => Oracle.DisputeResultDisputeStatus.NEW,
            DomainModel.DisputeResultDisputeStatus.VALIDATED => Oracle.DisputeResultDisputeStatus.VALIDATED,
            DomainModel.DisputeResultDisputeStatus.PROCESSING => Oracle.DisputeResultDisputeStatus.PROCESSING,
            DomainModel.DisputeResultDisputeStatus.REJECTED => Oracle.DisputeResultDisputeStatus.REJECTED,
            DomainModel.DisputeResultDisputeStatus.CANCELLED => Oracle.DisputeResultDisputeStatus.CANCELLED,
            DomainModel.DisputeResultDisputeStatus.CONCLUDED => Oracle.DisputeResultDisputeStatus.CONCLUDED,
            _ => Oracle.DisputeResultDisputeStatus.UNKNOWN
        };
    }

    public DomainModel.DisputeResultJjDisputeHearingType Convert(Oracle.DisputeResultJjDisputeHearingType source, DomainModel.DisputeResultJjDisputeHearingType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeResultJjDisputeHearingType.UNKNOWN => DomainModel.DisputeResultJjDisputeHearingType.UNKNOWN,
            Oracle.DisputeResultJjDisputeHearingType.COURT_APPEARANCE => DomainModel.DisputeResultJjDisputeHearingType.COURT_APPEARANCE,
            Oracle.DisputeResultJjDisputeHearingType.WRITTEN_REASONS => DomainModel.DisputeResultJjDisputeHearingType.WRITTEN_REASONS,
            _ => DomainModel.DisputeResultJjDisputeHearingType.UNKNOWN
        };
    }

    public Oracle.DisputeResultJjDisputeHearingType Convert(DomainModel.DisputeResultJjDisputeHearingType source, Oracle.DisputeResultJjDisputeHearingType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeResultJjDisputeHearingType.UNKNOWN => Oracle.DisputeResultJjDisputeHearingType.UNKNOWN,
            DomainModel.DisputeResultJjDisputeHearingType.COURT_APPEARANCE => Oracle.DisputeResultJjDisputeHearingType.COURT_APPEARANCE,
            DomainModel.DisputeResultJjDisputeHearingType.WRITTEN_REASONS => Oracle.DisputeResultJjDisputeHearingType.WRITTEN_REASONS,
            _ => Oracle.DisputeResultJjDisputeHearingType.UNKNOWN
        };
    }

    public DomainModel.DisputeResultJjDisputeStatus Convert(Oracle.DisputeResultJjDisputeStatus source, DomainModel.DisputeResultJjDisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeResultJjDisputeStatus.UNKNOWN => DomainModel.DisputeResultJjDisputeStatus.UNKNOWN,
            Oracle.DisputeResultJjDisputeStatus.NEW => DomainModel.DisputeResultJjDisputeStatus.NEW,
            Oracle.DisputeResultJjDisputeStatus.IN_PROGRESS => DomainModel.DisputeResultJjDisputeStatus.IN_PROGRESS,
            Oracle.DisputeResultJjDisputeStatus.DATA_UPDATE => DomainModel.DisputeResultJjDisputeStatus.DATA_UPDATE,
            Oracle.DisputeResultJjDisputeStatus.CONFIRMED => DomainModel.DisputeResultJjDisputeStatus.CONFIRMED,
            Oracle.DisputeResultJjDisputeStatus.REQUIRE_COURT_HEARING => DomainModel.DisputeResultJjDisputeStatus.REQUIRE_COURT_HEARING,
            Oracle.DisputeResultJjDisputeStatus.REQUIRE_MORE_INFO => DomainModel.DisputeResultJjDisputeStatus.REQUIRE_MORE_INFO,
            Oracle.DisputeResultJjDisputeStatus.ACCEPTED => DomainModel.DisputeResultJjDisputeStatus.ACCEPTED,
            Oracle.DisputeResultJjDisputeStatus.REVIEW => DomainModel.DisputeResultJjDisputeStatus.REVIEW,
            Oracle.DisputeResultJjDisputeStatus.CONCLUDED => DomainModel.DisputeResultJjDisputeStatus.CONCLUDED,
            Oracle.DisputeResultJjDisputeStatus.CANCELLED => DomainModel.DisputeResultJjDisputeStatus.CANCELLED,
            Oracle.DisputeResultJjDisputeStatus.HEARING_SCHEDULED => DomainModel.DisputeResultJjDisputeStatus.HEARING_SCHEDULED,
            _ => DomainModel.DisputeResultJjDisputeStatus.UNKNOWN
        };
    }

    public Oracle.DisputeResultJjDisputeStatus Convert(DomainModel.DisputeResultJjDisputeStatus source, Oracle.DisputeResultJjDisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeResultJjDisputeStatus.UNKNOWN => Oracle.DisputeResultJjDisputeStatus.UNKNOWN,
            DomainModel.DisputeResultJjDisputeStatus.NEW => Oracle.DisputeResultJjDisputeStatus.NEW,
            DomainModel.DisputeResultJjDisputeStatus.IN_PROGRESS => Oracle.DisputeResultJjDisputeStatus.IN_PROGRESS,
            DomainModel.DisputeResultJjDisputeStatus.DATA_UPDATE => Oracle.DisputeResultJjDisputeStatus.DATA_UPDATE,
            DomainModel.DisputeResultJjDisputeStatus.CONFIRMED => Oracle.DisputeResultJjDisputeStatus.CONFIRMED,
            DomainModel.DisputeResultJjDisputeStatus.REQUIRE_COURT_HEARING => Oracle.DisputeResultJjDisputeStatus.REQUIRE_COURT_HEARING,
            DomainModel.DisputeResultJjDisputeStatus.REQUIRE_MORE_INFO => Oracle.DisputeResultJjDisputeStatus.REQUIRE_MORE_INFO,
            DomainModel.DisputeResultJjDisputeStatus.ACCEPTED => Oracle.DisputeResultJjDisputeStatus.ACCEPTED,
            DomainModel.DisputeResultJjDisputeStatus.REVIEW => Oracle.DisputeResultJjDisputeStatus.REVIEW,
            DomainModel.DisputeResultJjDisputeStatus.CONCLUDED => Oracle.DisputeResultJjDisputeStatus.CONCLUDED,
            DomainModel.DisputeResultJjDisputeStatus.CANCELLED => Oracle.DisputeResultJjDisputeStatus.CANCELLED,
            DomainModel.DisputeResultJjDisputeStatus.HEARING_SCHEDULED => Oracle.DisputeResultJjDisputeStatus.HEARING_SCHEDULED,
            _ => Oracle.DisputeResultJjDisputeStatus.UNKNOWN
        };
    }

    public DomainModel.DisputeSignatoryType Convert(Oracle.DisputeSignatoryType source, DomainModel.DisputeSignatoryType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeSignatoryType.U => DomainModel.DisputeSignatoryType.U,
            Oracle.DisputeSignatoryType.D => DomainModel.DisputeSignatoryType.D,
            Oracle.DisputeSignatoryType.A => DomainModel.DisputeSignatoryType.A,
            _ => DomainModel.DisputeSignatoryType.U
        };
    }

    public Oracle.DisputeSignatoryType Convert(DomainModel.DisputeSignatoryType source, Oracle.DisputeSignatoryType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeSignatoryType.U => Oracle.DisputeSignatoryType.U,
            DomainModel.DisputeSignatoryType.D => Oracle.DisputeSignatoryType.D,
            DomainModel.DisputeSignatoryType.A => Oracle.DisputeSignatoryType.A,
            _ => Oracle.DisputeSignatoryType.U
        };
    }

    public DomainModel.DisputeStatus Convert(Oracle.DisputeStatus source, DomainModel.DisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeStatus.UNKNOWN => DomainModel.DisputeStatus.UNKNOWN,
            Oracle.DisputeStatus.NEW => DomainModel.DisputeStatus.NEW,
            Oracle.DisputeStatus.VALIDATED => DomainModel.DisputeStatus.VALIDATED,
            Oracle.DisputeStatus.PROCESSING => DomainModel.DisputeStatus.PROCESSING,
            Oracle.DisputeStatus.REJECTED => DomainModel.DisputeStatus.REJECTED,
            Oracle.DisputeStatus.CANCELLED => DomainModel.DisputeStatus.CANCELLED,
            Oracle.DisputeStatus.CONCLUDED => DomainModel.DisputeStatus.CONCLUDED,
            _ => DomainModel.DisputeStatus.UNKNOWN
        };
    }

    public Oracle.DisputeStatus Convert(DomainModel.DisputeStatus source, Oracle.DisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeStatus.UNKNOWN => Oracle.DisputeStatus.UNKNOWN,
            DomainModel.DisputeStatus.NEW => Oracle.DisputeStatus.NEW,
            DomainModel.DisputeStatus.VALIDATED => Oracle.DisputeStatus.VALIDATED,
            DomainModel.DisputeStatus.PROCESSING => Oracle.DisputeStatus.PROCESSING,
            DomainModel.DisputeStatus.REJECTED => Oracle.DisputeStatus.REJECTED,
            DomainModel.DisputeStatus.CANCELLED => Oracle.DisputeStatus.CANCELLED,
            DomainModel.DisputeStatus.CONCLUDED => Oracle.DisputeStatus.CONCLUDED,
            _ => Oracle.DisputeStatus.UNKNOWN
        };
    }

    public DomainModel.DisputeUpdateRequestStatus Convert(Oracle.DisputeUpdateRequestStatus source, DomainModel.DisputeUpdateRequestStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeUpdateRequestStatus.UNKNOWN => DomainModel.DisputeUpdateRequestStatus.UNKNOWN,
            Oracle.DisputeUpdateRequestStatus.ACCEPTED => DomainModel.DisputeUpdateRequestStatus.ACCEPTED,
            Oracle.DisputeUpdateRequestStatus.PENDING => DomainModel.DisputeUpdateRequestStatus.PENDING,
            Oracle.DisputeUpdateRequestStatus.REJECTED => DomainModel.DisputeUpdateRequestStatus.REJECTED,
            _ => DomainModel.DisputeUpdateRequestStatus.UNKNOWN
        };
    }

    public Oracle.DisputeUpdateRequestStatus Convert(DomainModel.DisputeUpdateRequestStatus source, Oracle.DisputeUpdateRequestStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeUpdateRequestStatus.UNKNOWN => Oracle.DisputeUpdateRequestStatus.UNKNOWN,
            DomainModel.DisputeUpdateRequestStatus.ACCEPTED => Oracle.DisputeUpdateRequestStatus.ACCEPTED,
            DomainModel.DisputeUpdateRequestStatus.PENDING => Oracle.DisputeUpdateRequestStatus.PENDING,
            DomainModel.DisputeUpdateRequestStatus.REJECTED => Oracle.DisputeUpdateRequestStatus.REJECTED,
            _ => Oracle.DisputeUpdateRequestStatus.UNKNOWN
        };
    }

    public DomainModel.DisputeUpdateRequestStatus2 Convert(Oracle.DisputeUpdateRequestStatus2 source, DomainModel.DisputeUpdateRequestStatus2 destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeUpdateRequestStatus2.UNKNOWN => DomainModel.DisputeUpdateRequestStatus2.UNKNOWN,
            Oracle.DisputeUpdateRequestStatus2.ACCEPTED => DomainModel.DisputeUpdateRequestStatus2.ACCEPTED,
            Oracle.DisputeUpdateRequestStatus2.PENDING => DomainModel.DisputeUpdateRequestStatus2.PENDING,
            Oracle.DisputeUpdateRequestStatus2.REJECTED => DomainModel.DisputeUpdateRequestStatus2.REJECTED,
            _ => DomainModel.DisputeUpdateRequestStatus2.UNKNOWN
        };
    }

    public Oracle.DisputeUpdateRequestStatus2 Convert(DomainModel.DisputeUpdateRequestStatus2 source, Oracle.DisputeUpdateRequestStatus2 destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeUpdateRequestStatus2.UNKNOWN => Oracle.DisputeUpdateRequestStatus2.UNKNOWN,
            DomainModel.DisputeUpdateRequestStatus2.ACCEPTED => Oracle.DisputeUpdateRequestStatus2.ACCEPTED,
            DomainModel.DisputeUpdateRequestStatus2.PENDING => Oracle.DisputeUpdateRequestStatus2.PENDING,
            DomainModel.DisputeUpdateRequestStatus2.REJECTED => Oracle.DisputeUpdateRequestStatus2.REJECTED,
            _ => Oracle.DisputeUpdateRequestStatus2.UNKNOWN
        };
    }

    public DomainModel.DisputeUpdateRequestUpdateType Convert(Oracle.DisputeUpdateRequestUpdateType source, DomainModel.DisputeUpdateRequestUpdateType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeUpdateRequestUpdateType.UNKNOWN => DomainModel.DisputeUpdateRequestUpdateType.UNKNOWN,
            Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS => DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS,
            Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_PHONE => DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_PHONE,
            Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_NAME => DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_NAME,
            Oracle.DisputeUpdateRequestUpdateType.COUNT => DomainModel.DisputeUpdateRequestUpdateType.COUNT,
            Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL => DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL,
            Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT => DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT,
            Oracle.DisputeUpdateRequestUpdateType.COURT_OPTIONS => DomainModel.DisputeUpdateRequestUpdateType.COURT_OPTIONS,
            _ => DomainModel.DisputeUpdateRequestUpdateType.UNKNOWN
        };
    }

    public Oracle.DisputeUpdateRequestUpdateType Convert(DomainModel.DisputeUpdateRequestUpdateType source, Oracle.DisputeUpdateRequestUpdateType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeUpdateRequestUpdateType.UNKNOWN => Oracle.DisputeUpdateRequestUpdateType.UNKNOWN,
            DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS => Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS,
            DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_PHONE => Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_PHONE,
            DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_NAME => Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_NAME,
            DomainModel.DisputeUpdateRequestUpdateType.COUNT => Oracle.DisputeUpdateRequestUpdateType.COUNT,
            DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL => Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_EMAIL,
            DomainModel.DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT => Oracle.DisputeUpdateRequestUpdateType.DISPUTANT_DOCUMENT,
            DomainModel.DisputeUpdateRequestUpdateType.COURT_OPTIONS => Oracle.DisputeUpdateRequestUpdateType.COURT_OPTIONS,
            _ => Oracle.DisputeUpdateRequestUpdateType.UNKNOWN
        };
    }

    public DomainModel.DocumentType Convert(Oracle.DocumentType source, DomainModel.DocumentType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DocumentType.UNKNOWN => DomainModel.DocumentType.UNKNOWN,
            Oracle.DocumentType.NOTICE_OF_DISPUTE => DomainModel.DocumentType.NOTICE_OF_DISPUTE,
            Oracle.DocumentType.TICKET_IMAGE => DomainModel.DocumentType.TICKET_IMAGE,
            _ => DomainModel.DocumentType.UNKNOWN
        };
    }

    public Oracle.DocumentType Convert(DomainModel.DocumentType source, Oracle.DocumentType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DocumentType.UNKNOWN => Oracle.DocumentType.UNKNOWN,
            DomainModel.DocumentType.NOTICE_OF_DISPUTE => Oracle.DocumentType.NOTICE_OF_DISPUTE,
            DomainModel.DocumentType.TICKET_IMAGE => Oracle.DocumentType.TICKET_IMAGE,
            _ => Oracle.DocumentType.UNKNOWN
        };
    }

    public DomainModel.ExcludeStatus Convert(Oracle.ExcludeStatus source, DomainModel.ExcludeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.ExcludeStatus.UNKNOWN => DomainModel.ExcludeStatus.UNKNOWN,
            Oracle.ExcludeStatus.NEW => DomainModel.ExcludeStatus.NEW,
            Oracle.ExcludeStatus.VALIDATED => DomainModel.ExcludeStatus.VALIDATED,
            Oracle.ExcludeStatus.PROCESSING => DomainModel.ExcludeStatus.PROCESSING,
            Oracle.ExcludeStatus.REJECTED => DomainModel.ExcludeStatus.REJECTED,
            Oracle.ExcludeStatus.CANCELLED => DomainModel.ExcludeStatus.CANCELLED,
            Oracle.ExcludeStatus.CONCLUDED => DomainModel.ExcludeStatus.CONCLUDED,
            _ => DomainModel.ExcludeStatus.UNKNOWN
        };
    }

    public Oracle.ExcludeStatus Convert(DomainModel.ExcludeStatus source, Oracle.ExcludeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.ExcludeStatus.UNKNOWN => Oracle.ExcludeStatus.UNKNOWN,
            DomainModel.ExcludeStatus.NEW => Oracle.ExcludeStatus.NEW,
            DomainModel.ExcludeStatus.VALIDATED => Oracle.ExcludeStatus.VALIDATED,
            DomainModel.ExcludeStatus.PROCESSING => Oracle.ExcludeStatus.PROCESSING,
            DomainModel.ExcludeStatus.REJECTED => Oracle.ExcludeStatus.REJECTED,
            DomainModel.ExcludeStatus.CANCELLED => Oracle.ExcludeStatus.CANCELLED,
            DomainModel.ExcludeStatus.CONCLUDED => Oracle.ExcludeStatus.CONCLUDED,
            _ => Oracle.ExcludeStatus.UNKNOWN
        };
    }

    public DomainModel.ExcludeStatus2 Convert(Oracle.ExcludeStatus2 source, DomainModel.ExcludeStatus2 destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.ExcludeStatus2.UNKNOWN => DomainModel.ExcludeStatus2.UNKNOWN,
            Oracle.ExcludeStatus2.NEW => DomainModel.ExcludeStatus2.NEW,
            Oracle.ExcludeStatus2.VALIDATED => DomainModel.ExcludeStatus2.VALIDATED,
            Oracle.ExcludeStatus2.PROCESSING => DomainModel.ExcludeStatus2.PROCESSING,
            Oracle.ExcludeStatus2.REJECTED => DomainModel.ExcludeStatus2.REJECTED,
            Oracle.ExcludeStatus2.CANCELLED => DomainModel.ExcludeStatus2.CANCELLED,
            Oracle.ExcludeStatus2.CONCLUDED => DomainModel.ExcludeStatus2.CONCLUDED,
            _ => DomainModel.ExcludeStatus2.UNKNOWN
        };
    }

    public Oracle.ExcludeStatus2 Convert(DomainModel.ExcludeStatus2 source, Oracle.ExcludeStatus2 destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.ExcludeStatus2.UNKNOWN => Oracle.ExcludeStatus2.UNKNOWN,
            DomainModel.ExcludeStatus2.NEW => Oracle.ExcludeStatus2.NEW,
            DomainModel.ExcludeStatus2.VALIDATED => Oracle.ExcludeStatus2.VALIDATED,
            DomainModel.ExcludeStatus2.PROCESSING => Oracle.ExcludeStatus2.PROCESSING,
            DomainModel.ExcludeStatus2.REJECTED => Oracle.ExcludeStatus2.REJECTED,
            DomainModel.ExcludeStatus2.CANCELLED => Oracle.ExcludeStatus2.CANCELLED,
            DomainModel.ExcludeStatus2.CONCLUDED => Oracle.ExcludeStatus2.CONCLUDED,
            _ => Oracle.ExcludeStatus2.UNKNOWN
        };
    }

    public DomainModel.FileHistoryAuditLogEntryType Convert(Oracle.FileHistoryAuditLogEntryType source, DomainModel.FileHistoryAuditLogEntryType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.FileHistoryAuditLogEntryType.UNKNOWN => DomainModel.FileHistoryAuditLogEntryType.UNKNOWN,
            Oracle.FileHistoryAuditLogEntryType.ARFL => DomainModel.FileHistoryAuditLogEntryType.ARFL,
            Oracle.FileHistoryAuditLogEntryType.CAIN => DomainModel.FileHistoryAuditLogEntryType.CAIN,
            Oracle.FileHistoryAuditLogEntryType.CAWT => DomainModel.FileHistoryAuditLogEntryType.CAWT,
            Oracle.FileHistoryAuditLogEntryType.CCAN => DomainModel.FileHistoryAuditLogEntryType.CCAN,
            Oracle.FileHistoryAuditLogEntryType.CCON => DomainModel.FileHistoryAuditLogEntryType.CCON,
            Oracle.FileHistoryAuditLogEntryType.CCWR => DomainModel.FileHistoryAuditLogEntryType.CCWR,
            Oracle.FileHistoryAuditLogEntryType.CLEG => DomainModel.FileHistoryAuditLogEntryType.CLEG,
            Oracle.FileHistoryAuditLogEntryType.CUEM => DomainModel.FileHistoryAuditLogEntryType.CUEM,
            Oracle.FileHistoryAuditLogEntryType.CUEV => DomainModel.FileHistoryAuditLogEntryType.CUEV,
            Oracle.FileHistoryAuditLogEntryType.CUIN => DomainModel.FileHistoryAuditLogEntryType.CUIN,
            Oracle.FileHistoryAuditLogEntryType.CULG => DomainModel.FileHistoryAuditLogEntryType.CULG,
            Oracle.FileHistoryAuditLogEntryType.CUPD => DomainModel.FileHistoryAuditLogEntryType.CUPD,
            Oracle.FileHistoryAuditLogEntryType.CUWR => DomainModel.FileHistoryAuditLogEntryType.CUWR,
            Oracle.FileHistoryAuditLogEntryType.CUWT => DomainModel.FileHistoryAuditLogEntryType.CUWT,
            Oracle.FileHistoryAuditLogEntryType.DURA => DomainModel.FileHistoryAuditLogEntryType.DURA,
            Oracle.FileHistoryAuditLogEntryType.DURR => DomainModel.FileHistoryAuditLogEntryType.DURR,
            Oracle.FileHistoryAuditLogEntryType.EMCA => DomainModel.FileHistoryAuditLogEntryType.EMCA,
            Oracle.FileHistoryAuditLogEntryType.EMCF => DomainModel.FileHistoryAuditLogEntryType.EMCF,
            Oracle.FileHistoryAuditLogEntryType.EMCR => DomainModel.FileHistoryAuditLogEntryType.EMCR,
            Oracle.FileHistoryAuditLogEntryType.EMDC => DomainModel.FileHistoryAuditLogEntryType.EMDC,
            Oracle.FileHistoryAuditLogEntryType.EMFD => DomainModel.FileHistoryAuditLogEntryType.EMFD,
            Oracle.FileHistoryAuditLogEntryType.EMPR => DomainModel.FileHistoryAuditLogEntryType.EMPR,
            Oracle.FileHistoryAuditLogEntryType.EMRJ => DomainModel.FileHistoryAuditLogEntryType.EMRJ,
            Oracle.FileHistoryAuditLogEntryType.EMRV => DomainModel.FileHistoryAuditLogEntryType.EMRV,
            Oracle.FileHistoryAuditLogEntryType.EMST => DomainModel.FileHistoryAuditLogEntryType.EMST,
            Oracle.FileHistoryAuditLogEntryType.EMUP => DomainModel.FileHistoryAuditLogEntryType.EMUP,
            Oracle.FileHistoryAuditLogEntryType.EMVF => DomainModel.FileHistoryAuditLogEntryType.EMVF,
            Oracle.FileHistoryAuditLogEntryType.ESUR => DomainModel.FileHistoryAuditLogEntryType.ESUR,
            Oracle.FileHistoryAuditLogEntryType.FDLD => DomainModel.FileHistoryAuditLogEntryType.FDLD,
            Oracle.FileHistoryAuditLogEntryType.FDLS => DomainModel.FileHistoryAuditLogEntryType.FDLS,
            Oracle.FileHistoryAuditLogEntryType.FUPD => DomainModel.FileHistoryAuditLogEntryType.FUPD,
            Oracle.FileHistoryAuditLogEntryType.FUPS => DomainModel.FileHistoryAuditLogEntryType.FUPS,
            Oracle.FileHistoryAuditLogEntryType.FRMK => DomainModel.FileHistoryAuditLogEntryType.FRMK,
            Oracle.FileHistoryAuditLogEntryType.INIT => DomainModel.FileHistoryAuditLogEntryType.INIT,
            Oracle.FileHistoryAuditLogEntryType.JASG => DomainModel.FileHistoryAuditLogEntryType.JASG,
            Oracle.FileHistoryAuditLogEntryType.JCNF => DomainModel.FileHistoryAuditLogEntryType.JCNF,
            Oracle.FileHistoryAuditLogEntryType.JDIV => DomainModel.FileHistoryAuditLogEntryType.JDIV,
            Oracle.FileHistoryAuditLogEntryType.JPRG => DomainModel.FileHistoryAuditLogEntryType.JPRG,
            Oracle.FileHistoryAuditLogEntryType.OCNT => DomainModel.FileHistoryAuditLogEntryType.OCNT,
            Oracle.FileHistoryAuditLogEntryType.RCLD => DomainModel.FileHistoryAuditLogEntryType.RCLD,
            Oracle.FileHistoryAuditLogEntryType.RECN => DomainModel.FileHistoryAuditLogEntryType.RECN,
            Oracle.FileHistoryAuditLogEntryType.SADM => DomainModel.FileHistoryAuditLogEntryType.SADM,
            Oracle.FileHistoryAuditLogEntryType.SCAN => DomainModel.FileHistoryAuditLogEntryType.SCAN,
            Oracle.FileHistoryAuditLogEntryType.SPRC => DomainModel.FileHistoryAuditLogEntryType.SPRC,
            Oracle.FileHistoryAuditLogEntryType.SREJ => DomainModel.FileHistoryAuditLogEntryType.SREJ,
            Oracle.FileHistoryAuditLogEntryType.SUB => DomainModel.FileHistoryAuditLogEntryType.SUB,
            Oracle.FileHistoryAuditLogEntryType.SUPL => DomainModel.FileHistoryAuditLogEntryType.SUPL,
            Oracle.FileHistoryAuditLogEntryType.SVAL => DomainModel.FileHistoryAuditLogEntryType.SVAL,
            Oracle.FileHistoryAuditLogEntryType.URSR => DomainModel.FileHistoryAuditLogEntryType.URSR,
            Oracle.FileHistoryAuditLogEntryType.VREV => DomainModel.FileHistoryAuditLogEntryType.VREV,
            Oracle.FileHistoryAuditLogEntryType.VSUB => DomainModel.FileHistoryAuditLogEntryType.VSUB,
            _ => DomainModel.FileHistoryAuditLogEntryType.UNKNOWN
        };
    }

    public Oracle.FileHistoryAuditLogEntryType Convert(DomainModel.FileHistoryAuditLogEntryType source, Oracle.FileHistoryAuditLogEntryType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.FileHistoryAuditLogEntryType.UNKNOWN => Oracle.FileHistoryAuditLogEntryType.UNKNOWN,
            DomainModel.FileHistoryAuditLogEntryType.ARFL => Oracle.FileHistoryAuditLogEntryType.ARFL,
            DomainModel.FileHistoryAuditLogEntryType.CAIN => Oracle.FileHistoryAuditLogEntryType.CAIN,
            DomainModel.FileHistoryAuditLogEntryType.CAWT => Oracle.FileHistoryAuditLogEntryType.CAWT,
            DomainModel.FileHistoryAuditLogEntryType.CCAN => Oracle.FileHistoryAuditLogEntryType.CCAN,
            DomainModel.FileHistoryAuditLogEntryType.CCON => Oracle.FileHistoryAuditLogEntryType.CCON,
            DomainModel.FileHistoryAuditLogEntryType.CCWR => Oracle.FileHistoryAuditLogEntryType.CCWR,
            DomainModel.FileHistoryAuditLogEntryType.CLEG => Oracle.FileHistoryAuditLogEntryType.CLEG,
            DomainModel.FileHistoryAuditLogEntryType.CUEM => Oracle.FileHistoryAuditLogEntryType.CUEM,
            DomainModel.FileHistoryAuditLogEntryType.CUEV => Oracle.FileHistoryAuditLogEntryType.CUEV,
            DomainModel.FileHistoryAuditLogEntryType.CUIN => Oracle.FileHistoryAuditLogEntryType.CUIN,
            DomainModel.FileHistoryAuditLogEntryType.CULG => Oracle.FileHistoryAuditLogEntryType.CULG,
            DomainModel.FileHistoryAuditLogEntryType.CUPD => Oracle.FileHistoryAuditLogEntryType.CUPD,
            DomainModel.FileHistoryAuditLogEntryType.CUWR => Oracle.FileHistoryAuditLogEntryType.CUWR,
            DomainModel.FileHistoryAuditLogEntryType.CUWT => Oracle.FileHistoryAuditLogEntryType.CUWT,
            DomainModel.FileHistoryAuditLogEntryType.DURA => Oracle.FileHistoryAuditLogEntryType.DURA,
            DomainModel.FileHistoryAuditLogEntryType.DURR => Oracle.FileHistoryAuditLogEntryType.DURR,
            DomainModel.FileHistoryAuditLogEntryType.EMCA => Oracle.FileHistoryAuditLogEntryType.EMCA,
            DomainModel.FileHistoryAuditLogEntryType.EMCF => Oracle.FileHistoryAuditLogEntryType.EMCF,
            DomainModel.FileHistoryAuditLogEntryType.EMCR => Oracle.FileHistoryAuditLogEntryType.EMCR,
            DomainModel.FileHistoryAuditLogEntryType.EMDC => Oracle.FileHistoryAuditLogEntryType.EMDC,
            DomainModel.FileHistoryAuditLogEntryType.EMFD => Oracle.FileHistoryAuditLogEntryType.EMFD,
            DomainModel.FileHistoryAuditLogEntryType.EMPR => Oracle.FileHistoryAuditLogEntryType.EMPR,
            DomainModel.FileHistoryAuditLogEntryType.EMRJ => Oracle.FileHistoryAuditLogEntryType.EMRJ,
            DomainModel.FileHistoryAuditLogEntryType.EMRV => Oracle.FileHistoryAuditLogEntryType.EMRV,
            DomainModel.FileHistoryAuditLogEntryType.EMST => Oracle.FileHistoryAuditLogEntryType.EMST,
            DomainModel.FileHistoryAuditLogEntryType.EMUP => Oracle.FileHistoryAuditLogEntryType.EMUP,
            DomainModel.FileHistoryAuditLogEntryType.EMVF => Oracle.FileHistoryAuditLogEntryType.EMVF,
            DomainModel.FileHistoryAuditLogEntryType.ESUR => Oracle.FileHistoryAuditLogEntryType.ESUR,
            DomainModel.FileHistoryAuditLogEntryType.FDLD => Oracle.FileHistoryAuditLogEntryType.FDLD,
            DomainModel.FileHistoryAuditLogEntryType.FDLS => Oracle.FileHistoryAuditLogEntryType.FDLS,
            DomainModel.FileHistoryAuditLogEntryType.FUPD => Oracle.FileHistoryAuditLogEntryType.FUPD,
            DomainModel.FileHistoryAuditLogEntryType.FUPS => Oracle.FileHistoryAuditLogEntryType.FUPS,
            DomainModel.FileHistoryAuditLogEntryType.FRMK => Oracle.FileHistoryAuditLogEntryType.FRMK,
            DomainModel.FileHistoryAuditLogEntryType.INIT => Oracle.FileHistoryAuditLogEntryType.INIT,
            DomainModel.FileHistoryAuditLogEntryType.JASG => Oracle.FileHistoryAuditLogEntryType.JASG,
            DomainModel.FileHistoryAuditLogEntryType.JCNF => Oracle.FileHistoryAuditLogEntryType.JCNF,
            DomainModel.FileHistoryAuditLogEntryType.JDIV => Oracle.FileHistoryAuditLogEntryType.JDIV,
            DomainModel.FileHistoryAuditLogEntryType.JPRG => Oracle.FileHistoryAuditLogEntryType.JPRG,
            DomainModel.FileHistoryAuditLogEntryType.OCNT => Oracle.FileHistoryAuditLogEntryType.OCNT,
            DomainModel.FileHistoryAuditLogEntryType.RCLD => Oracle.FileHistoryAuditLogEntryType.RCLD,
            DomainModel.FileHistoryAuditLogEntryType.RECN => Oracle.FileHistoryAuditLogEntryType.RECN,
            DomainModel.FileHistoryAuditLogEntryType.SADM => Oracle.FileHistoryAuditLogEntryType.SADM,
            DomainModel.FileHistoryAuditLogEntryType.SCAN => Oracle.FileHistoryAuditLogEntryType.SCAN,
            DomainModel.FileHistoryAuditLogEntryType.SPRC => Oracle.FileHistoryAuditLogEntryType.SPRC,
            DomainModel.FileHistoryAuditLogEntryType.SREJ => Oracle.FileHistoryAuditLogEntryType.SREJ,
            DomainModel.FileHistoryAuditLogEntryType.SUB => Oracle.FileHistoryAuditLogEntryType.SUB,
            DomainModel.FileHistoryAuditLogEntryType.SUPL => Oracle.FileHistoryAuditLogEntryType.SUPL,
            DomainModel.FileHistoryAuditLogEntryType.SVAL => Oracle.FileHistoryAuditLogEntryType.SVAL,
            DomainModel.FileHistoryAuditLogEntryType.URSR => Oracle.FileHistoryAuditLogEntryType.URSR,
            DomainModel.FileHistoryAuditLogEntryType.VREV => Oracle.FileHistoryAuditLogEntryType.VREV,
            DomainModel.FileHistoryAuditLogEntryType.VSUB => Oracle.FileHistoryAuditLogEntryType.VSUB,
            _ => Oracle.FileHistoryAuditLogEntryType.UNKNOWN
        };
    }

    public DomainModel.JJDisputeContactType Convert(Oracle.JJDisputeContactType source, DomainModel.JJDisputeContactType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeContactType.UNKNOWN => DomainModel.JJDisputeContactType.UNKNOWN,
            Oracle.JJDisputeContactType.INDIVIDUAL => DomainModel.JJDisputeContactType.INDIVIDUAL,
            Oracle.JJDisputeContactType.LAWYER => DomainModel.JJDisputeContactType.LAWYER,
            Oracle.JJDisputeContactType.OTHER => DomainModel.JJDisputeContactType.OTHER,
            _ => DomainModel.JJDisputeContactType.UNKNOWN
        };
    }

    public Oracle.JJDisputeContactType Convert(DomainModel.JJDisputeContactType source, Oracle.JJDisputeContactType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeContactType.UNKNOWN => Oracle.JJDisputeContactType.UNKNOWN,
            DomainModel.JJDisputeContactType.INDIVIDUAL => Oracle.JJDisputeContactType.INDIVIDUAL,
            DomainModel.JJDisputeContactType.LAWYER => Oracle.JJDisputeContactType.LAWYER,
            DomainModel.JJDisputeContactType.OTHER => Oracle.JJDisputeContactType.OTHER,
            _ => Oracle.JJDisputeContactType.UNKNOWN
        };
    }

    public DomainModel.JJDisputeCourtAppearanceRoPAppCd Convert(Oracle.JJDisputeCourtAppearanceRoPAppCd source, DomainModel.JJDisputeCourtAppearanceRoPAppCd destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN => DomainModel.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN,
            Oracle.JJDisputeCourtAppearanceRoPAppCd.A => DomainModel.JJDisputeCourtAppearanceRoPAppCd.A,
            Oracle.JJDisputeCourtAppearanceRoPAppCd.P => DomainModel.JJDisputeCourtAppearanceRoPAppCd.P,
            Oracle.JJDisputeCourtAppearanceRoPAppCd.N => DomainModel.JJDisputeCourtAppearanceRoPAppCd.N,
            _ => DomainModel.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN
        };
    }

    public Oracle.JJDisputeCourtAppearanceRoPAppCd Convert(DomainModel.JJDisputeCourtAppearanceRoPAppCd source, Oracle.JJDisputeCourtAppearanceRoPAppCd destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN => Oracle.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN,
            DomainModel.JJDisputeCourtAppearanceRoPAppCd.A => Oracle.JJDisputeCourtAppearanceRoPAppCd.A,
            DomainModel.JJDisputeCourtAppearanceRoPAppCd.P => Oracle.JJDisputeCourtAppearanceRoPAppCd.P,
            DomainModel.JJDisputeCourtAppearanceRoPAppCd.N => Oracle.JJDisputeCourtAppearanceRoPAppCd.N,
            _ => Oracle.JJDisputeCourtAppearanceRoPAppCd.UNKNOWN
        };
    }

    public DomainModel.JJDisputeCourtAppearanceRoPCrown Convert(Oracle.JJDisputeCourtAppearanceRoPCrown source, DomainModel.JJDisputeCourtAppearanceRoPCrown destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeCourtAppearanceRoPCrown.UNKNOWN => DomainModel.JJDisputeCourtAppearanceRoPCrown.UNKNOWN,
            Oracle.JJDisputeCourtAppearanceRoPCrown.P => DomainModel.JJDisputeCourtAppearanceRoPCrown.P,
            Oracle.JJDisputeCourtAppearanceRoPCrown.N => DomainModel.JJDisputeCourtAppearanceRoPCrown.N,
            _ => DomainModel.JJDisputeCourtAppearanceRoPCrown.UNKNOWN
        };
    }

    public Oracle.JJDisputeCourtAppearanceRoPCrown Convert(DomainModel.JJDisputeCourtAppearanceRoPCrown source, Oracle.JJDisputeCourtAppearanceRoPCrown destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeCourtAppearanceRoPCrown.UNKNOWN => Oracle.JJDisputeCourtAppearanceRoPCrown.UNKNOWN,
            DomainModel.JJDisputeCourtAppearanceRoPCrown.P => Oracle.JJDisputeCourtAppearanceRoPCrown.P,
            DomainModel.JJDisputeCourtAppearanceRoPCrown.N => Oracle.JJDisputeCourtAppearanceRoPCrown.N,
            _ => Oracle.JJDisputeCourtAppearanceRoPCrown.UNKNOWN
        };
    }

    public DomainModel.JJDisputeCourtAppearanceRoPDattCd Convert(Oracle.JJDisputeCourtAppearanceRoPDattCd source, DomainModel.JJDisputeCourtAppearanceRoPDattCd destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN => DomainModel.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN,
            Oracle.JJDisputeCourtAppearanceRoPDattCd.A => DomainModel.JJDisputeCourtAppearanceRoPDattCd.A,
            Oracle.JJDisputeCourtAppearanceRoPDattCd.C => DomainModel.JJDisputeCourtAppearanceRoPDattCd.C,
            Oracle.JJDisputeCourtAppearanceRoPDattCd.N => DomainModel.JJDisputeCourtAppearanceRoPDattCd.N,
            _ => DomainModel.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN
        };
    }

    public Oracle.JJDisputeCourtAppearanceRoPDattCd Convert(DomainModel.JJDisputeCourtAppearanceRoPDattCd source, Oracle.JJDisputeCourtAppearanceRoPDattCd destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN => Oracle.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN,
            DomainModel.JJDisputeCourtAppearanceRoPDattCd.A => Oracle.JJDisputeCourtAppearanceRoPDattCd.A,
            DomainModel.JJDisputeCourtAppearanceRoPDattCd.C => Oracle.JJDisputeCourtAppearanceRoPDattCd.C,
            DomainModel.JJDisputeCourtAppearanceRoPDattCd.N => Oracle.JJDisputeCourtAppearanceRoPDattCd.N,
            _ => Oracle.JJDisputeCourtAppearanceRoPDattCd.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountLatestPlea Convert(Oracle.JJDisputedCountLatestPlea source, DomainModel.JJDisputedCountLatestPlea destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountLatestPlea.UNKNOWN => DomainModel.JJDisputedCountLatestPlea.UNKNOWN,
            Oracle.JJDisputedCountLatestPlea.G => DomainModel.JJDisputedCountLatestPlea.G,
            Oracle.JJDisputedCountLatestPlea.N => DomainModel.JJDisputedCountLatestPlea.N,
            _ => DomainModel.JJDisputedCountLatestPlea.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountLatestPlea Convert(DomainModel.JJDisputedCountLatestPlea source, Oracle.JJDisputedCountLatestPlea destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountLatestPlea.UNKNOWN => Oracle.JJDisputedCountLatestPlea.UNKNOWN,
            DomainModel.JJDisputedCountLatestPlea.G => Oracle.JJDisputedCountLatestPlea.G,
            DomainModel.JJDisputedCountLatestPlea.N => Oracle.JJDisputedCountLatestPlea.N,
            _ => Oracle.JJDisputedCountLatestPlea.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountPlea Convert(Oracle.JJDisputedCountPlea source, DomainModel.JJDisputedCountPlea destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountPlea.UNKNOWN => DomainModel.JJDisputedCountPlea.UNKNOWN,
            Oracle.JJDisputedCountPlea.G => DomainModel.JJDisputedCountPlea.G,
            Oracle.JJDisputedCountPlea.N => DomainModel.JJDisputedCountPlea.N,
            _ => DomainModel.JJDisputedCountPlea.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountPlea Convert(DomainModel.JJDisputedCountPlea source, Oracle.JJDisputedCountPlea destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountPlea.UNKNOWN => Oracle.JJDisputedCountPlea.UNKNOWN,
            DomainModel.JJDisputedCountPlea.G => Oracle.JJDisputedCountPlea.G,
            DomainModel.JJDisputedCountPlea.N => Oracle.JJDisputedCountPlea.N,
            _ => Oracle.JJDisputedCountPlea.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountRoPFinding Convert(Oracle.JJDisputedCountRoPFinding source, DomainModel.JJDisputedCountRoPFinding destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountRoPFinding.UNKNOWN => DomainModel.JJDisputedCountRoPFinding.UNKNOWN,
            Oracle.JJDisputedCountRoPFinding.GUILTY => DomainModel.JJDisputedCountRoPFinding.GUILTY,
            Oracle.JJDisputedCountRoPFinding.NOT_GUILTY => DomainModel.JJDisputedCountRoPFinding.NOT_GUILTY,
            Oracle.JJDisputedCountRoPFinding.CANCELLED => DomainModel.JJDisputedCountRoPFinding.CANCELLED,
            Oracle.JJDisputedCountRoPFinding.PAID_PRIOR_TO_APPEARANCE => DomainModel.JJDisputedCountRoPFinding.PAID_PRIOR_TO_APPEARANCE,
            Oracle.JJDisputedCountRoPFinding.GUILTY_LESSER => DomainModel.JJDisputedCountRoPFinding.GUILTY_LESSER,
            _ => DomainModel.JJDisputedCountRoPFinding.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountRoPFinding Convert(DomainModel.JJDisputedCountRoPFinding source, Oracle.JJDisputedCountRoPFinding destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountRoPFinding.UNKNOWN => Oracle.JJDisputedCountRoPFinding.UNKNOWN,
            DomainModel.JJDisputedCountRoPFinding.GUILTY => Oracle.JJDisputedCountRoPFinding.GUILTY,
            DomainModel.JJDisputedCountRoPFinding.NOT_GUILTY => Oracle.JJDisputedCountRoPFinding.NOT_GUILTY,
            DomainModel.JJDisputedCountRoPFinding.CANCELLED => Oracle.JJDisputedCountRoPFinding.CANCELLED,
            DomainModel.JJDisputedCountRoPFinding.PAID_PRIOR_TO_APPEARANCE => Oracle.JJDisputedCountRoPFinding.PAID_PRIOR_TO_APPEARANCE,
            DomainModel.JJDisputedCountRoPFinding.GUILTY_LESSER => Oracle.JJDisputedCountRoPFinding.GUILTY_LESSER,
            _ => Oracle.JJDisputedCountRoPFinding.UNKNOWN
        };
    }

    public DomainModel.JJDisputeDisputantAttendanceType Convert(Oracle.JJDisputeDisputantAttendanceType source, DomainModel.JJDisputeDisputantAttendanceType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeDisputantAttendanceType.UNKNOWN => DomainModel.JJDisputeDisputantAttendanceType.UNKNOWN,
            Oracle.JJDisputeDisputantAttendanceType.WRITTEN_REASONS => DomainModel.JJDisputeDisputantAttendanceType.WRITTEN_REASONS,
            Oracle.JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE => DomainModel.JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE,
            Oracle.JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE => DomainModel.JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE,
            Oracle.JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO => DomainModel.JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO,
            Oracle.JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO => DomainModel.JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO,
            Oracle.JJDisputeDisputantAttendanceType.IN_PERSON => DomainModel.JJDisputeDisputantAttendanceType.IN_PERSON,
            _ => DomainModel.JJDisputeDisputantAttendanceType.UNKNOWN
        };
    }

    public Oracle.JJDisputeDisputantAttendanceType Convert(DomainModel.JJDisputeDisputantAttendanceType source, Oracle.JJDisputeDisputantAttendanceType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeDisputantAttendanceType.UNKNOWN => Oracle.JJDisputeDisputantAttendanceType.UNKNOWN,
            DomainModel.JJDisputeDisputantAttendanceType.WRITTEN_REASONS => Oracle.JJDisputeDisputantAttendanceType.WRITTEN_REASONS,
            DomainModel.JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE => Oracle.JJDisputeDisputantAttendanceType.VIDEO_CONFERENCE,
            DomainModel.JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE => Oracle.JJDisputeDisputantAttendanceType.TELEPHONE_CONFERENCE,
            DomainModel.JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO => Oracle.JJDisputeDisputantAttendanceType.MSTEAMS_AUDIO,
            DomainModel.JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO => Oracle.JJDisputeDisputantAttendanceType.MSTEAMS_VIDEO,
            DomainModel.JJDisputeDisputantAttendanceType.IN_PERSON => Oracle.JJDisputeDisputantAttendanceType.IN_PERSON,
            _ => Oracle.JJDisputeDisputantAttendanceType.UNKNOWN
        };
    }

    public DomainModel.JJDisputeHearingType Convert(Oracle.JJDisputeHearingType source, DomainModel.JJDisputeHearingType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeHearingType.UNKNOWN => DomainModel.JJDisputeHearingType.UNKNOWN,
            Oracle.JJDisputeHearingType.COURT_APPEARANCE => DomainModel.JJDisputeHearingType.COURT_APPEARANCE,
            Oracle.JJDisputeHearingType.WRITTEN_REASONS => DomainModel.JJDisputeHearingType.WRITTEN_REASONS,
            _ => DomainModel.JJDisputeHearingType.UNKNOWN
        };
    }

    public Oracle.JJDisputeHearingType Convert(DomainModel.JJDisputeHearingType source, Oracle.JJDisputeHearingType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeHearingType.UNKNOWN => Oracle.JJDisputeHearingType.UNKNOWN,
            DomainModel.JJDisputeHearingType.COURT_APPEARANCE => Oracle.JJDisputeHearingType.COURT_APPEARANCE,
            DomainModel.JJDisputeHearingType.WRITTEN_REASONS => Oracle.JJDisputeHearingType.WRITTEN_REASONS,
            _ => Oracle.JJDisputeHearingType.UNKNOWN
        };
    }

    public DomainModel.JJDisputeSignatoryType Convert(Oracle.JJDisputeSignatoryType source, DomainModel.JJDisputeSignatoryType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeSignatoryType.U => DomainModel.JJDisputeSignatoryType.U,
            Oracle.JJDisputeSignatoryType.D => DomainModel.JJDisputeSignatoryType.D,
            Oracle.JJDisputeSignatoryType.A => DomainModel.JJDisputeSignatoryType.A,
            _ => DomainModel.JJDisputeSignatoryType.U
        };
    }

    public Oracle.JJDisputeSignatoryType Convert(DomainModel.JJDisputeSignatoryType source, Oracle.JJDisputeSignatoryType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeSignatoryType.U => Oracle.JJDisputeSignatoryType.U,
            DomainModel.JJDisputeSignatoryType.D => Oracle.JJDisputeSignatoryType.D,
            DomainModel.JJDisputeSignatoryType.A => Oracle.JJDisputeSignatoryType.A,
            _ => Oracle.JJDisputeSignatoryType.U
        };
    }

    public DomainModel.JJDisputeStatus Convert(Oracle.JJDisputeStatus source, DomainModel.JJDisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeStatus.UNKNOWN => DomainModel.JJDisputeStatus.UNKNOWN,
            Oracle.JJDisputeStatus.NEW => DomainModel.JJDisputeStatus.NEW,
            Oracle.JJDisputeStatus.IN_PROGRESS => DomainModel.JJDisputeStatus.IN_PROGRESS,
            Oracle.JJDisputeStatus.DATA_UPDATE => DomainModel.JJDisputeStatus.DATA_UPDATE,
            Oracle.JJDisputeStatus.CONFIRMED => DomainModel.JJDisputeStatus.CONFIRMED,
            Oracle.JJDisputeStatus.REQUIRE_COURT_HEARING => DomainModel.JJDisputeStatus.REQUIRE_COURT_HEARING,
            Oracle.JJDisputeStatus.REQUIRE_MORE_INFO => DomainModel.JJDisputeStatus.REQUIRE_MORE_INFO,
            Oracle.JJDisputeStatus.ACCEPTED => DomainModel.JJDisputeStatus.ACCEPTED,
            Oracle.JJDisputeStatus.REVIEW => DomainModel.JJDisputeStatus.REVIEW,
            Oracle.JJDisputeStatus.CONCLUDED => DomainModel.JJDisputeStatus.CONCLUDED,
            Oracle.JJDisputeStatus.CANCELLED => DomainModel.JJDisputeStatus.CANCELLED,
            Oracle.JJDisputeStatus.HEARING_SCHEDULED => DomainModel.JJDisputeStatus.HEARING_SCHEDULED,
            _ => DomainModel.JJDisputeStatus.UNKNOWN
        };
    }

    public Oracle.JJDisputeStatus Convert(DomainModel.JJDisputeStatus source, Oracle.JJDisputeStatus destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeStatus.UNKNOWN => Oracle.JJDisputeStatus.UNKNOWN,
            DomainModel.JJDisputeStatus.NEW => Oracle.JJDisputeStatus.NEW,
            DomainModel.JJDisputeStatus.IN_PROGRESS => Oracle.JJDisputeStatus.IN_PROGRESS,
            DomainModel.JJDisputeStatus.DATA_UPDATE => Oracle.JJDisputeStatus.DATA_UPDATE,
            DomainModel.JJDisputeStatus.CONFIRMED => Oracle.JJDisputeStatus.CONFIRMED,
            DomainModel.JJDisputeStatus.REQUIRE_COURT_HEARING => Oracle.JJDisputeStatus.REQUIRE_COURT_HEARING,
            DomainModel.JJDisputeStatus.REQUIRE_MORE_INFO => Oracle.JJDisputeStatus.REQUIRE_MORE_INFO,
            DomainModel.JJDisputeStatus.ACCEPTED => Oracle.JJDisputeStatus.ACCEPTED,
            DomainModel.JJDisputeStatus.REVIEW => Oracle.JJDisputeStatus.REVIEW,
            DomainModel.JJDisputeStatus.CONCLUDED => Oracle.JJDisputeStatus.CONCLUDED,
            DomainModel.JJDisputeStatus.CANCELLED => Oracle.JJDisputeStatus.CANCELLED,
            DomainModel.JJDisputeStatus.HEARING_SCHEDULED => Oracle.JJDisputeStatus.HEARING_SCHEDULED,
            _ => Oracle.JJDisputeStatus.UNKNOWN
        };
    }

    public DomainModel.Status Convert(Oracle.Status source, DomainModel.Status destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.Status.UNKNOWN => DomainModel.Status.UNKNOWN,
            Oracle.Status.ACCEPTED => DomainModel.Status.ACCEPTED,
            Oracle.Status.PENDING => DomainModel.Status.PENDING,
            Oracle.Status.REJECTED => DomainModel.Status.REJECTED,
            _ => DomainModel.Status.UNKNOWN
        };
    }

    public Oracle.Status Convert(DomainModel.Status source, Oracle.Status destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.Status.UNKNOWN => Oracle.Status.UNKNOWN,
            DomainModel.Status.ACCEPTED => Oracle.Status.ACCEPTED,
            DomainModel.Status.PENDING => Oracle.Status.PENDING,
            DomainModel.Status.REJECTED => Oracle.Status.REJECTED,
            _ => Oracle.Status.UNKNOWN
        };
    }

    public DomainModel.TicketImageDataJustinDocumentReportType Convert(Oracle.TicketImageDataJustinDocumentReportType source, DomainModel.TicketImageDataJustinDocumentReportType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.TicketImageDataJustinDocumentReportType.UNKNOWN => DomainModel.TicketImageDataJustinDocumentReportType.UNKNOWN,
            Oracle.TicketImageDataJustinDocumentReportType.NOTICE_OF_DISPUTE => DomainModel.TicketImageDataJustinDocumentReportType.NOTICE_OF_DISPUTE,
            Oracle.TicketImageDataJustinDocumentReportType.TICKET_IMAGE => DomainModel.TicketImageDataJustinDocumentReportType.TICKET_IMAGE,
            _ => DomainModel.TicketImageDataJustinDocumentReportType.UNKNOWN
        };
    }

    public Oracle.TicketImageDataJustinDocumentReportType Convert(DomainModel.TicketImageDataJustinDocumentReportType source, Oracle.TicketImageDataJustinDocumentReportType destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.TicketImageDataJustinDocumentReportType.UNKNOWN => Oracle.TicketImageDataJustinDocumentReportType.UNKNOWN,
            DomainModel.TicketImageDataJustinDocumentReportType.NOTICE_OF_DISPUTE => Oracle.TicketImageDataJustinDocumentReportType.NOTICE_OF_DISPUTE,
            DomainModel.TicketImageDataJustinDocumentReportType.TICKET_IMAGE => Oracle.TicketImageDataJustinDocumentReportType.TICKET_IMAGE,
            _ => Oracle.TicketImageDataJustinDocumentReportType.UNKNOWN
        };
    }

    public DomainModel.DisputeAppearanceLessThan14DaysYn Convert(Oracle.DisputeAppearanceLessThan14DaysYn source, DomainModel.DisputeAppearanceLessThan14DaysYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeAppearanceLessThan14DaysYn.UNKNOWN => DomainModel.DisputeAppearanceLessThan14DaysYn.UNKNOWN,
            Oracle.DisputeAppearanceLessThan14DaysYn.Y => DomainModel.DisputeAppearanceLessThan14DaysYn.Y,
            Oracle.DisputeAppearanceLessThan14DaysYn.N => DomainModel.DisputeAppearanceLessThan14DaysYn.N,
            _ => DomainModel.DisputeAppearanceLessThan14DaysYn.UNKNOWN
        };
    }

    public Oracle.DisputeAppearanceLessThan14DaysYn Convert(DomainModel.DisputeAppearanceLessThan14DaysYn source, Oracle.DisputeAppearanceLessThan14DaysYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeAppearanceLessThan14DaysYn.UNKNOWN => Oracle.DisputeAppearanceLessThan14DaysYn.UNKNOWN,
            DomainModel.DisputeAppearanceLessThan14DaysYn.Y => Oracle.DisputeAppearanceLessThan14DaysYn.Y,
            DomainModel.DisputeAppearanceLessThan14DaysYn.N => Oracle.DisputeAppearanceLessThan14DaysYn.N,
            _ => Oracle.DisputeAppearanceLessThan14DaysYn.UNKNOWN
        };
    }

    public DomainModel.DisputeCountRequestCourtAppearance Convert(Oracle.DisputeCountRequestCourtAppearance source, DomainModel.DisputeCountRequestCourtAppearance destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeCountRequestCourtAppearance.UNKNOWN => DomainModel.DisputeCountRequestCourtAppearance.UNKNOWN,
            Oracle.DisputeCountRequestCourtAppearance.Y => DomainModel.DisputeCountRequestCourtAppearance.Y,
            Oracle.DisputeCountRequestCourtAppearance.N => DomainModel.DisputeCountRequestCourtAppearance.N,
            _ => DomainModel.DisputeCountRequestCourtAppearance.UNKNOWN
        };
    }

    public Oracle.DisputeCountRequestCourtAppearance Convert(DomainModel.DisputeCountRequestCourtAppearance source, Oracle.DisputeCountRequestCourtAppearance destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeCountRequestCourtAppearance.UNKNOWN => Oracle.DisputeCountRequestCourtAppearance.UNKNOWN,
            DomainModel.DisputeCountRequestCourtAppearance.Y => Oracle.DisputeCountRequestCourtAppearance.Y,
            DomainModel.DisputeCountRequestCourtAppearance.N => Oracle.DisputeCountRequestCourtAppearance.N,
            _ => Oracle.DisputeCountRequestCourtAppearance.UNKNOWN
        };
    }

    public DomainModel.DisputeCountRequestReduction Convert(Oracle.DisputeCountRequestReduction source, DomainModel.DisputeCountRequestReduction destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeCountRequestReduction.UNKNOWN => DomainModel.DisputeCountRequestReduction.UNKNOWN,
            Oracle.DisputeCountRequestReduction.Y => DomainModel.DisputeCountRequestReduction.Y,
            Oracle.DisputeCountRequestReduction.N => DomainModel.DisputeCountRequestReduction.N,
            _ => DomainModel.DisputeCountRequestReduction.UNKNOWN
        };
    }

    public Oracle.DisputeCountRequestReduction Convert(DomainModel.DisputeCountRequestReduction source, Oracle.DisputeCountRequestReduction destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeCountRequestReduction.UNKNOWN => Oracle.DisputeCountRequestReduction.UNKNOWN,
            DomainModel.DisputeCountRequestReduction.Y => Oracle.DisputeCountRequestReduction.Y,
            DomainModel.DisputeCountRequestReduction.N => Oracle.DisputeCountRequestReduction.N,
            _ => Oracle.DisputeCountRequestReduction.UNKNOWN
        };
    }

    public DomainModel.DisputeCountRequestTimeToPay Convert(Oracle.DisputeCountRequestTimeToPay source, DomainModel.DisputeCountRequestTimeToPay destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeCountRequestTimeToPay.UNKNOWN => DomainModel.DisputeCountRequestTimeToPay.UNKNOWN,
            Oracle.DisputeCountRequestTimeToPay.Y => DomainModel.DisputeCountRequestTimeToPay.Y,
            Oracle.DisputeCountRequestTimeToPay.N => DomainModel.DisputeCountRequestTimeToPay.N,
            _ => DomainModel.DisputeCountRequestTimeToPay.UNKNOWN
        };
    }

    public Oracle.DisputeCountRequestTimeToPay Convert(DomainModel.DisputeCountRequestTimeToPay source, Oracle.DisputeCountRequestTimeToPay destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeCountRequestTimeToPay.UNKNOWN => Oracle.DisputeCountRequestTimeToPay.UNKNOWN,
            DomainModel.DisputeCountRequestTimeToPay.Y => Oracle.DisputeCountRequestTimeToPay.Y,
            DomainModel.DisputeCountRequestTimeToPay.N => Oracle.DisputeCountRequestTimeToPay.N,
            _ => Oracle.DisputeCountRequestTimeToPay.UNKNOWN
        };
    }

    public DomainModel.DisputeDisputantDetectedOcrIssues Convert(Oracle.DisputeDisputantDetectedOcrIssues source, DomainModel.DisputeDisputantDetectedOcrIssues destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeDisputantDetectedOcrIssues.UNKNOWN => DomainModel.DisputeDisputantDetectedOcrIssues.UNKNOWN,
            Oracle.DisputeDisputantDetectedOcrIssues.Y => DomainModel.DisputeDisputantDetectedOcrIssues.Y,
            Oracle.DisputeDisputantDetectedOcrIssues.N => DomainModel.DisputeDisputantDetectedOcrIssues.N,
            _ => DomainModel.DisputeDisputantDetectedOcrIssues.UNKNOWN
        };
    }

    public Oracle.DisputeDisputantDetectedOcrIssues Convert(DomainModel.DisputeDisputantDetectedOcrIssues source, Oracle.DisputeDisputantDetectedOcrIssues destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeDisputantDetectedOcrIssues.UNKNOWN => Oracle.DisputeDisputantDetectedOcrIssues.UNKNOWN,
            DomainModel.DisputeDisputantDetectedOcrIssues.Y => Oracle.DisputeDisputantDetectedOcrIssues.Y,
            DomainModel.DisputeDisputantDetectedOcrIssues.N => Oracle.DisputeDisputantDetectedOcrIssues.N,
            _ => Oracle.DisputeDisputantDetectedOcrIssues.UNKNOWN
        };
    }

    public DomainModel.DisputeInterpreterRequired Convert(Oracle.DisputeInterpreterRequired source, DomainModel.DisputeInterpreterRequired destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeInterpreterRequired.UNKNOWN => DomainModel.DisputeInterpreterRequired.UNKNOWN,
            Oracle.DisputeInterpreterRequired.Y => DomainModel.DisputeInterpreterRequired.Y,
            Oracle.DisputeInterpreterRequired.N => DomainModel.DisputeInterpreterRequired.N,
            _ => DomainModel.DisputeInterpreterRequired.UNKNOWN
        };
    }

    public Oracle.DisputeInterpreterRequired Convert(DomainModel.DisputeInterpreterRequired source, Oracle.DisputeInterpreterRequired destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeInterpreterRequired.UNKNOWN => Oracle.DisputeInterpreterRequired.UNKNOWN,
            DomainModel.DisputeInterpreterRequired.Y => Oracle.DisputeInterpreterRequired.Y,
            DomainModel.DisputeInterpreterRequired.N => Oracle.DisputeInterpreterRequired.N,
            _ => Oracle.DisputeInterpreterRequired.UNKNOWN
        };
    }

    public DomainModel.DisputeListItemDisputantDetectedOcrIssues Convert(Oracle.DisputeListItemDisputantDetectedOcrIssues source, DomainModel.DisputeListItemDisputantDetectedOcrIssues destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN => DomainModel.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN,
            Oracle.DisputeListItemDisputantDetectedOcrIssues.Y => DomainModel.DisputeListItemDisputantDetectedOcrIssues.Y,
            Oracle.DisputeListItemDisputantDetectedOcrIssues.N => DomainModel.DisputeListItemDisputantDetectedOcrIssues.N,
            _ => DomainModel.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN
        };
    }

    public Oracle.DisputeListItemDisputantDetectedOcrIssues Convert(DomainModel.DisputeListItemDisputantDetectedOcrIssues source, Oracle.DisputeListItemDisputantDetectedOcrIssues destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN => Oracle.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN,
            DomainModel.DisputeListItemDisputantDetectedOcrIssues.Y => Oracle.DisputeListItemDisputantDetectedOcrIssues.Y,
            DomainModel.DisputeListItemDisputantDetectedOcrIssues.N => Oracle.DisputeListItemDisputantDetectedOcrIssues.N,
            _ => Oracle.DisputeListItemDisputantDetectedOcrIssues.UNKNOWN
        };
    }

    public DomainModel.DisputeListItemRequestCourtAppearanceYn Convert(Oracle.DisputeListItemRequestCourtAppearanceYn source, DomainModel.DisputeListItemRequestCourtAppearanceYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeListItemRequestCourtAppearanceYn.UNKNOWN => DomainModel.DisputeListItemRequestCourtAppearanceYn.UNKNOWN,
            Oracle.DisputeListItemRequestCourtAppearanceYn.Y => DomainModel.DisputeListItemRequestCourtAppearanceYn.Y,
            Oracle.DisputeListItemRequestCourtAppearanceYn.N => DomainModel.DisputeListItemRequestCourtAppearanceYn.N,
            _ => DomainModel.DisputeListItemRequestCourtAppearanceYn.UNKNOWN
        };
    }

    public Oracle.DisputeListItemRequestCourtAppearanceYn Convert(DomainModel.DisputeListItemRequestCourtAppearanceYn source, Oracle.DisputeListItemRequestCourtAppearanceYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeListItemRequestCourtAppearanceYn.UNKNOWN => Oracle.DisputeListItemRequestCourtAppearanceYn.UNKNOWN,
            DomainModel.DisputeListItemRequestCourtAppearanceYn.Y => Oracle.DisputeListItemRequestCourtAppearanceYn.Y,
            DomainModel.DisputeListItemRequestCourtAppearanceYn.N => Oracle.DisputeListItemRequestCourtAppearanceYn.N,
            _ => Oracle.DisputeListItemRequestCourtAppearanceYn.UNKNOWN
        };
    }

    public DomainModel.DisputeListItemSystemDetectedOcrIssues Convert(Oracle.DisputeListItemSystemDetectedOcrIssues source, DomainModel.DisputeListItemSystemDetectedOcrIssues destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeListItemSystemDetectedOcrIssues.UNKNOWN => DomainModel.DisputeListItemSystemDetectedOcrIssues.UNKNOWN,
            Oracle.DisputeListItemSystemDetectedOcrIssues.Y => DomainModel.DisputeListItemSystemDetectedOcrIssues.Y,
            Oracle.DisputeListItemSystemDetectedOcrIssues.N => DomainModel.DisputeListItemSystemDetectedOcrIssues.N,
            _ => DomainModel.DisputeListItemSystemDetectedOcrIssues.UNKNOWN
        };
    }

    public Oracle.DisputeListItemSystemDetectedOcrIssues Convert(DomainModel.DisputeListItemSystemDetectedOcrIssues source, Oracle.DisputeListItemSystemDetectedOcrIssues destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeListItemSystemDetectedOcrIssues.UNKNOWN => Oracle.DisputeListItemSystemDetectedOcrIssues.UNKNOWN,
            DomainModel.DisputeListItemSystemDetectedOcrIssues.Y => Oracle.DisputeListItemSystemDetectedOcrIssues.Y,
            DomainModel.DisputeListItemSystemDetectedOcrIssues.N => Oracle.DisputeListItemSystemDetectedOcrIssues.N,
            _ => Oracle.DisputeListItemSystemDetectedOcrIssues.UNKNOWN
        };
    }

    public DomainModel.DisputeRepresentedByLawyer Convert(Oracle.DisputeRepresentedByLawyer source, DomainModel.DisputeRepresentedByLawyer destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeRepresentedByLawyer.UNKNOWN => DomainModel.DisputeRepresentedByLawyer.UNKNOWN,
            Oracle.DisputeRepresentedByLawyer.Y => DomainModel.DisputeRepresentedByLawyer.Y,
            Oracle.DisputeRepresentedByLawyer.N => DomainModel.DisputeRepresentedByLawyer.N,
            _ => DomainModel.DisputeRepresentedByLawyer.UNKNOWN
        };
    }

    public Oracle.DisputeRepresentedByLawyer Convert(DomainModel.DisputeRepresentedByLawyer source, Oracle.DisputeRepresentedByLawyer destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeRepresentedByLawyer.UNKNOWN => Oracle.DisputeRepresentedByLawyer.UNKNOWN,
            DomainModel.DisputeRepresentedByLawyer.Y => Oracle.DisputeRepresentedByLawyer.Y,
            DomainModel.DisputeRepresentedByLawyer.N => Oracle.DisputeRepresentedByLawyer.N,
            _ => Oracle.DisputeRepresentedByLawyer.UNKNOWN
        };
    }

    public DomainModel.DisputeRequestCourtAppearanceYn Convert(Oracle.DisputeRequestCourtAppearanceYn source, DomainModel.DisputeRequestCourtAppearanceYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeRequestCourtAppearanceYn.UNKNOWN => DomainModel.DisputeRequestCourtAppearanceYn.UNKNOWN,
            Oracle.DisputeRequestCourtAppearanceYn.Y => DomainModel.DisputeRequestCourtAppearanceYn.Y,
            Oracle.DisputeRequestCourtAppearanceYn.N => DomainModel.DisputeRequestCourtAppearanceYn.N,
            _ => DomainModel.DisputeRequestCourtAppearanceYn.UNKNOWN
        };
    }

    public Oracle.DisputeRequestCourtAppearanceYn Convert(DomainModel.DisputeRequestCourtAppearanceYn source, Oracle.DisputeRequestCourtAppearanceYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeRequestCourtAppearanceYn.UNKNOWN => Oracle.DisputeRequestCourtAppearanceYn.UNKNOWN,
            DomainModel.DisputeRequestCourtAppearanceYn.Y => Oracle.DisputeRequestCourtAppearanceYn.Y,
            DomainModel.DisputeRequestCourtAppearanceYn.N => Oracle.DisputeRequestCourtAppearanceYn.N,
            _ => Oracle.DisputeRequestCourtAppearanceYn.UNKNOWN
        };
    }

    public DomainModel.DisputeSystemDetectedOcrIssues Convert(Oracle.DisputeSystemDetectedOcrIssues source, DomainModel.DisputeSystemDetectedOcrIssues destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.DisputeSystemDetectedOcrIssues.UNKNOWN => DomainModel.DisputeSystemDetectedOcrIssues.UNKNOWN,
            Oracle.DisputeSystemDetectedOcrIssues.Y => DomainModel.DisputeSystemDetectedOcrIssues.Y,
            Oracle.DisputeSystemDetectedOcrIssues.N => DomainModel.DisputeSystemDetectedOcrIssues.N,
            _ => DomainModel.DisputeSystemDetectedOcrIssues.UNKNOWN
        };
    }

    public Oracle.DisputeSystemDetectedOcrIssues Convert(DomainModel.DisputeSystemDetectedOcrIssues source, Oracle.DisputeSystemDetectedOcrIssues destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.DisputeSystemDetectedOcrIssues.UNKNOWN => Oracle.DisputeSystemDetectedOcrIssues.UNKNOWN,
            DomainModel.DisputeSystemDetectedOcrIssues.Y => Oracle.DisputeSystemDetectedOcrIssues.Y,
            DomainModel.DisputeSystemDetectedOcrIssues.N => Oracle.DisputeSystemDetectedOcrIssues.N,
            _ => Oracle.DisputeSystemDetectedOcrIssues.UNKNOWN
        };
    }

    public DomainModel.EmailHistorySuccessfullySent Convert(Oracle.EmailHistorySuccessfullySent source, DomainModel.EmailHistorySuccessfullySent destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.EmailHistorySuccessfullySent.UNKNOWN => DomainModel.EmailHistorySuccessfullySent.UNKNOWN,
            Oracle.EmailHistorySuccessfullySent.Y => DomainModel.EmailHistorySuccessfullySent.Y,
            Oracle.EmailHistorySuccessfullySent.N => DomainModel.EmailHistorySuccessfullySent.N,
            _ => DomainModel.EmailHistorySuccessfullySent.UNKNOWN
        };
    }

    public Oracle.EmailHistorySuccessfullySent Convert(DomainModel.EmailHistorySuccessfullySent source, Oracle.EmailHistorySuccessfullySent destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.EmailHistorySuccessfullySent.UNKNOWN => Oracle.EmailHistorySuccessfullySent.UNKNOWN,
            DomainModel.EmailHistorySuccessfullySent.Y => Oracle.EmailHistorySuccessfullySent.Y,
            DomainModel.EmailHistorySuccessfullySent.N => Oracle.EmailHistorySuccessfullySent.N,
            _ => Oracle.EmailHistorySuccessfullySent.UNKNOWN
        };
    }

    public DomainModel.JJDisputeAccidentYn Convert(Oracle.JJDisputeAccidentYn source, DomainModel.JJDisputeAccidentYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeAccidentYn.UNKNOWN => DomainModel.JJDisputeAccidentYn.UNKNOWN,
            Oracle.JJDisputeAccidentYn.Y => DomainModel.JJDisputeAccidentYn.Y,
            Oracle.JJDisputeAccidentYn.N => DomainModel.JJDisputeAccidentYn.N,
            _ => DomainModel.JJDisputeAccidentYn.UNKNOWN
        };
    }

    public Oracle.JJDisputeAccidentYn Convert(DomainModel.JJDisputeAccidentYn source, Oracle.JJDisputeAccidentYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeAccidentYn.UNKNOWN => Oracle.JJDisputeAccidentYn.UNKNOWN,
            DomainModel.JJDisputeAccidentYn.Y => Oracle.JJDisputeAccidentYn.Y,
            DomainModel.JJDisputeAccidentYn.N => Oracle.JJDisputeAccidentYn.N,
            _ => Oracle.JJDisputeAccidentYn.UNKNOWN
        };
    }

    public DomainModel.JJDisputeAppearInCourt Convert(Oracle.JJDisputeAppearInCourt source, DomainModel.JJDisputeAppearInCourt destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeAppearInCourt.UNKNOWN => DomainModel.JJDisputeAppearInCourt.UNKNOWN,
            Oracle.JJDisputeAppearInCourt.Y => DomainModel.JJDisputeAppearInCourt.Y,
            Oracle.JJDisputeAppearInCourt.N => DomainModel.JJDisputeAppearInCourt.N,
            _ => DomainModel.JJDisputeAppearInCourt.UNKNOWN
        };
    }

    public Oracle.JJDisputeAppearInCourt Convert(DomainModel.JJDisputeAppearInCourt source, Oracle.JJDisputeAppearInCourt destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeAppearInCourt.UNKNOWN => Oracle.JJDisputeAppearInCourt.UNKNOWN,
            DomainModel.JJDisputeAppearInCourt.Y => Oracle.JJDisputeAppearInCourt.Y,
            DomainModel.JJDisputeAppearInCourt.N => Oracle.JJDisputeAppearInCourt.N,
            _ => Oracle.JJDisputeAppearInCourt.UNKNOWN
        };
    }

    public DomainModel.JJDisputeCourtAppearanceRoPJjSeized Convert(Oracle.JJDisputeCourtAppearanceRoPJjSeized source, DomainModel.JJDisputeCourtAppearanceRoPJjSeized destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN => DomainModel.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN,
            Oracle.JJDisputeCourtAppearanceRoPJjSeized.Y => DomainModel.JJDisputeCourtAppearanceRoPJjSeized.Y,
            Oracle.JJDisputeCourtAppearanceRoPJjSeized.N => DomainModel.JJDisputeCourtAppearanceRoPJjSeized.N,
            _ => DomainModel.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN
        };
    }

    public Oracle.JJDisputeCourtAppearanceRoPJjSeized Convert(DomainModel.JJDisputeCourtAppearanceRoPJjSeized source, Oracle.JJDisputeCourtAppearanceRoPJjSeized destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN => Oracle.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN,
            DomainModel.JJDisputeCourtAppearanceRoPJjSeized.Y => Oracle.JJDisputeCourtAppearanceRoPJjSeized.Y,
            DomainModel.JJDisputeCourtAppearanceRoPJjSeized.N => Oracle.JJDisputeCourtAppearanceRoPJjSeized.N,
            _ => Oracle.JJDisputeCourtAppearanceRoPJjSeized.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountAppearInCourt Convert(Oracle.JJDisputedCountAppearInCourt source, DomainModel.JJDisputedCountAppearInCourt destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountAppearInCourt.UNKNOWN => DomainModel.JJDisputedCountAppearInCourt.UNKNOWN,
            Oracle.JJDisputedCountAppearInCourt.Y => DomainModel.JJDisputedCountAppearInCourt.Y,
            Oracle.JJDisputedCountAppearInCourt.N => DomainModel.JJDisputedCountAppearInCourt.N,
            _ => DomainModel.JJDisputedCountAppearInCourt.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountAppearInCourt Convert(DomainModel.JJDisputedCountAppearInCourt source, Oracle.JJDisputedCountAppearInCourt destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountAppearInCourt.UNKNOWN => Oracle.JJDisputedCountAppearInCourt.UNKNOWN,
            DomainModel.JJDisputedCountAppearInCourt.Y => Oracle.JJDisputedCountAppearInCourt.Y,
            DomainModel.JJDisputedCountAppearInCourt.N => Oracle.JJDisputedCountAppearInCourt.N,
            _ => Oracle.JJDisputedCountAppearInCourt.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountIncludesSurcharge Convert(Oracle.JJDisputedCountIncludesSurcharge source, DomainModel.JJDisputedCountIncludesSurcharge destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountIncludesSurcharge.UNKNOWN => DomainModel.JJDisputedCountIncludesSurcharge.UNKNOWN,
            Oracle.JJDisputedCountIncludesSurcharge.Y => DomainModel.JJDisputedCountIncludesSurcharge.Y,
            Oracle.JJDisputedCountIncludesSurcharge.N => DomainModel.JJDisputedCountIncludesSurcharge.N,
            _ => DomainModel.JJDisputedCountIncludesSurcharge.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountIncludesSurcharge Convert(DomainModel.JJDisputedCountIncludesSurcharge source, Oracle.JJDisputedCountIncludesSurcharge destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountIncludesSurcharge.UNKNOWN => Oracle.JJDisputedCountIncludesSurcharge.UNKNOWN,
            DomainModel.JJDisputedCountIncludesSurcharge.Y => Oracle.JJDisputedCountIncludesSurcharge.Y,
            DomainModel.JJDisputedCountIncludesSurcharge.N => Oracle.JJDisputedCountIncludesSurcharge.N,
            _ => Oracle.JJDisputedCountIncludesSurcharge.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountRequestReduction Convert(Oracle.JJDisputedCountRequestReduction source, DomainModel.JJDisputedCountRequestReduction destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountRequestReduction.UNKNOWN => DomainModel.JJDisputedCountRequestReduction.UNKNOWN,
            Oracle.JJDisputedCountRequestReduction.Y => DomainModel.JJDisputedCountRequestReduction.Y,
            Oracle.JJDisputedCountRequestReduction.N => DomainModel.JJDisputedCountRequestReduction.N,
            _ => DomainModel.JJDisputedCountRequestReduction.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountRequestReduction Convert(DomainModel.JJDisputedCountRequestReduction source, Oracle.JJDisputedCountRequestReduction destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountRequestReduction.UNKNOWN => Oracle.JJDisputedCountRequestReduction.UNKNOWN,
            DomainModel.JJDisputedCountRequestReduction.Y => Oracle.JJDisputedCountRequestReduction.Y,
            DomainModel.JJDisputedCountRequestReduction.N => Oracle.JJDisputedCountRequestReduction.N,
            _ => Oracle.JJDisputedCountRequestReduction.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountRequestTimeToPay Convert(Oracle.JJDisputedCountRequestTimeToPay source, DomainModel.JJDisputedCountRequestTimeToPay destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountRequestTimeToPay.UNKNOWN => DomainModel.JJDisputedCountRequestTimeToPay.UNKNOWN,
            Oracle.JJDisputedCountRequestTimeToPay.Y => DomainModel.JJDisputedCountRequestTimeToPay.Y,
            Oracle.JJDisputedCountRequestTimeToPay.N => DomainModel.JJDisputedCountRequestTimeToPay.N,
            _ => DomainModel.JJDisputedCountRequestTimeToPay.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountRequestTimeToPay Convert(DomainModel.JJDisputedCountRequestTimeToPay source, Oracle.JJDisputedCountRequestTimeToPay destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountRequestTimeToPay.UNKNOWN => Oracle.JJDisputedCountRequestTimeToPay.UNKNOWN,
            DomainModel.JJDisputedCountRequestTimeToPay.Y => Oracle.JJDisputedCountRequestTimeToPay.Y,
            DomainModel.JJDisputedCountRequestTimeToPay.N => Oracle.JJDisputedCountRequestTimeToPay.N,
            _ => Oracle.JJDisputedCountRequestTimeToPay.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountRoPAbatement Convert(Oracle.JJDisputedCountRoPAbatement source, DomainModel.JJDisputedCountRoPAbatement destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountRoPAbatement.UNKNOWN => DomainModel.JJDisputedCountRoPAbatement.UNKNOWN,
            Oracle.JJDisputedCountRoPAbatement.Y => DomainModel.JJDisputedCountRoPAbatement.Y,
            Oracle.JJDisputedCountRoPAbatement.N => DomainModel.JJDisputedCountRoPAbatement.N,
            _ => DomainModel.JJDisputedCountRoPAbatement.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountRoPAbatement Convert(DomainModel.JJDisputedCountRoPAbatement source, Oracle.JJDisputedCountRoPAbatement destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountRoPAbatement.UNKNOWN => Oracle.JJDisputedCountRoPAbatement.UNKNOWN,
            DomainModel.JJDisputedCountRoPAbatement.Y => Oracle.JJDisputedCountRoPAbatement.Y,
            DomainModel.JJDisputedCountRoPAbatement.N => Oracle.JJDisputedCountRoPAbatement.N,
            _ => Oracle.JJDisputedCountRoPAbatement.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountRoPDismissed Convert(Oracle.JJDisputedCountRoPDismissed source, DomainModel.JJDisputedCountRoPDismissed destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountRoPDismissed.UNKNOWN => DomainModel.JJDisputedCountRoPDismissed.UNKNOWN,
            Oracle.JJDisputedCountRoPDismissed.Y => DomainModel.JJDisputedCountRoPDismissed.Y,
            Oracle.JJDisputedCountRoPDismissed.N => DomainModel.JJDisputedCountRoPDismissed.N,
            _ => DomainModel.JJDisputedCountRoPDismissed.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountRoPDismissed Convert(DomainModel.JJDisputedCountRoPDismissed source, Oracle.JJDisputedCountRoPDismissed destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountRoPDismissed.UNKNOWN => Oracle.JJDisputedCountRoPDismissed.UNKNOWN,
            DomainModel.JJDisputedCountRoPDismissed.Y => Oracle.JJDisputedCountRoPDismissed.Y,
            DomainModel.JJDisputedCountRoPDismissed.N => Oracle.JJDisputedCountRoPDismissed.N,
            _ => Oracle.JJDisputedCountRoPDismissed.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountRoPForWantOfProsecution Convert(Oracle.JJDisputedCountRoPForWantOfProsecution source, DomainModel.JJDisputedCountRoPForWantOfProsecution destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountRoPForWantOfProsecution.UNKNOWN => DomainModel.JJDisputedCountRoPForWantOfProsecution.UNKNOWN,
            Oracle.JJDisputedCountRoPForWantOfProsecution.Y => DomainModel.JJDisputedCountRoPForWantOfProsecution.Y,
            Oracle.JJDisputedCountRoPForWantOfProsecution.N => DomainModel.JJDisputedCountRoPForWantOfProsecution.N,
            _ => DomainModel.JJDisputedCountRoPForWantOfProsecution.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountRoPForWantOfProsecution Convert(DomainModel.JJDisputedCountRoPForWantOfProsecution source, Oracle.JJDisputedCountRoPForWantOfProsecution destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountRoPForWantOfProsecution.UNKNOWN => Oracle.JJDisputedCountRoPForWantOfProsecution.UNKNOWN,
            DomainModel.JJDisputedCountRoPForWantOfProsecution.Y => Oracle.JJDisputedCountRoPForWantOfProsecution.Y,
            DomainModel.JJDisputedCountRoPForWantOfProsecution.N => Oracle.JJDisputedCountRoPForWantOfProsecution.N,
            _ => Oracle.JJDisputedCountRoPForWantOfProsecution.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountRoPJailIntermittent Convert(Oracle.JJDisputedCountRoPJailIntermittent source, DomainModel.JJDisputedCountRoPJailIntermittent destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountRoPJailIntermittent.UNKNOWN => DomainModel.JJDisputedCountRoPJailIntermittent.UNKNOWN,
            Oracle.JJDisputedCountRoPJailIntermittent.Y => DomainModel.JJDisputedCountRoPJailIntermittent.Y,
            Oracle.JJDisputedCountRoPJailIntermittent.N => DomainModel.JJDisputedCountRoPJailIntermittent.N,
            _ => DomainModel.JJDisputedCountRoPJailIntermittent.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountRoPJailIntermittent Convert(DomainModel.JJDisputedCountRoPJailIntermittent source, Oracle.JJDisputedCountRoPJailIntermittent destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountRoPJailIntermittent.UNKNOWN => Oracle.JJDisputedCountRoPJailIntermittent.UNKNOWN,
            DomainModel.JJDisputedCountRoPJailIntermittent.Y => Oracle.JJDisputedCountRoPJailIntermittent.Y,
            DomainModel.JJDisputedCountRoPJailIntermittent.N => Oracle.JJDisputedCountRoPJailIntermittent.N,
            _ => Oracle.JJDisputedCountRoPJailIntermittent.UNKNOWN
        };
    }

    public DomainModel.JJDisputedCountRoPWithdrawn Convert(Oracle.JJDisputedCountRoPWithdrawn source, DomainModel.JJDisputedCountRoPWithdrawn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputedCountRoPWithdrawn.UNKNOWN => DomainModel.JJDisputedCountRoPWithdrawn.UNKNOWN,
            Oracle.JJDisputedCountRoPWithdrawn.Y => DomainModel.JJDisputedCountRoPWithdrawn.Y,
            Oracle.JJDisputedCountRoPWithdrawn.N => DomainModel.JJDisputedCountRoPWithdrawn.N,
            _ => DomainModel.JJDisputedCountRoPWithdrawn.UNKNOWN
        };
    }

    public Oracle.JJDisputedCountRoPWithdrawn Convert(DomainModel.JJDisputedCountRoPWithdrawn source, Oracle.JJDisputedCountRoPWithdrawn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputedCountRoPWithdrawn.UNKNOWN => Oracle.JJDisputedCountRoPWithdrawn.UNKNOWN,
            DomainModel.JJDisputedCountRoPWithdrawn.Y => Oracle.JJDisputedCountRoPWithdrawn.Y,
            DomainModel.JJDisputedCountRoPWithdrawn.N => Oracle.JJDisputedCountRoPWithdrawn.N,
            _ => Oracle.JJDisputedCountRoPWithdrawn.UNKNOWN
        };
    }

    public DomainModel.JJDisputeElectronicTicketYn Convert(Oracle.JJDisputeElectronicTicketYn source, DomainModel.JJDisputeElectronicTicketYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeElectronicTicketYn.UNKNOWN => DomainModel.JJDisputeElectronicTicketYn.UNKNOWN,
            Oracle.JJDisputeElectronicTicketYn.Y => DomainModel.JJDisputeElectronicTicketYn.Y,
            Oracle.JJDisputeElectronicTicketYn.N => DomainModel.JJDisputeElectronicTicketYn.N,
            _ => DomainModel.JJDisputeElectronicTicketYn.UNKNOWN
        };
    }

    public Oracle.JJDisputeElectronicTicketYn Convert(DomainModel.JJDisputeElectronicTicketYn source, Oracle.JJDisputeElectronicTicketYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeElectronicTicketYn.UNKNOWN => Oracle.JJDisputeElectronicTicketYn.UNKNOWN,
            DomainModel.JJDisputeElectronicTicketYn.Y => Oracle.JJDisputeElectronicTicketYn.Y,
            DomainModel.JJDisputeElectronicTicketYn.N => Oracle.JJDisputeElectronicTicketYn.N,
            _ => Oracle.JJDisputeElectronicTicketYn.UNKNOWN
        };
    }

    public DomainModel.JJDisputeMultipleOfficersYn Convert(Oracle.JJDisputeMultipleOfficersYn source, DomainModel.JJDisputeMultipleOfficersYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeMultipleOfficersYn.UNKNOWN => DomainModel.JJDisputeMultipleOfficersYn.UNKNOWN,
            Oracle.JJDisputeMultipleOfficersYn.Y => DomainModel.JJDisputeMultipleOfficersYn.Y,
            Oracle.JJDisputeMultipleOfficersYn.N => DomainModel.JJDisputeMultipleOfficersYn.N,
            _ => DomainModel.JJDisputeMultipleOfficersYn.UNKNOWN
        };
    }

    public Oracle.JJDisputeMultipleOfficersYn Convert(DomainModel.JJDisputeMultipleOfficersYn source, Oracle.JJDisputeMultipleOfficersYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeMultipleOfficersYn.UNKNOWN => Oracle.JJDisputeMultipleOfficersYn.UNKNOWN,
            DomainModel.JJDisputeMultipleOfficersYn.Y => Oracle.JJDisputeMultipleOfficersYn.Y,
            DomainModel.JJDisputeMultipleOfficersYn.N => Oracle.JJDisputeMultipleOfficersYn.N,
            _ => Oracle.JJDisputeMultipleOfficersYn.UNKNOWN
        };
    }

    public DomainModel.JJDisputeNoticeOfHearingYn Convert(Oracle.JJDisputeNoticeOfHearingYn source, DomainModel.JJDisputeNoticeOfHearingYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.JJDisputeNoticeOfHearingYn.UNKNOWN => DomainModel.JJDisputeNoticeOfHearingYn.UNKNOWN,
            Oracle.JJDisputeNoticeOfHearingYn.Y => DomainModel.JJDisputeNoticeOfHearingYn.Y,
            Oracle.JJDisputeNoticeOfHearingYn.N => DomainModel.JJDisputeNoticeOfHearingYn.N,
            _ => DomainModel.JJDisputeNoticeOfHearingYn.UNKNOWN
        };
    }

    public Oracle.JJDisputeNoticeOfHearingYn Convert(DomainModel.JJDisputeNoticeOfHearingYn source, Oracle.JJDisputeNoticeOfHearingYn destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.JJDisputeNoticeOfHearingYn.UNKNOWN => Oracle.JJDisputeNoticeOfHearingYn.UNKNOWN,
            DomainModel.JJDisputeNoticeOfHearingYn.Y => Oracle.JJDisputeNoticeOfHearingYn.Y,
            DomainModel.JJDisputeNoticeOfHearingYn.N => Oracle.JJDisputeNoticeOfHearingYn.N,
            _ => Oracle.JJDisputeNoticeOfHearingYn.UNKNOWN
        };
    }

    public DomainModel.ViolationTicketCountIsAct Convert(Oracle.ViolationTicketCountIsAct source, DomainModel.ViolationTicketCountIsAct destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.ViolationTicketCountIsAct.UNKNOWN => DomainModel.ViolationTicketCountIsAct.UNKNOWN,
            Oracle.ViolationTicketCountIsAct.Y => DomainModel.ViolationTicketCountIsAct.Y,
            Oracle.ViolationTicketCountIsAct.N => DomainModel.ViolationTicketCountIsAct.N,
            _ => DomainModel.ViolationTicketCountIsAct.UNKNOWN
        };
    }

    public Oracle.ViolationTicketCountIsAct Convert(DomainModel.ViolationTicketCountIsAct source, Oracle.ViolationTicketCountIsAct destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.ViolationTicketCountIsAct.UNKNOWN => Oracle.ViolationTicketCountIsAct.UNKNOWN,
            DomainModel.ViolationTicketCountIsAct.Y => Oracle.ViolationTicketCountIsAct.Y,
            DomainModel.ViolationTicketCountIsAct.N => Oracle.ViolationTicketCountIsAct.N,
            _ => Oracle.ViolationTicketCountIsAct.UNKNOWN
        };
    }

    public DomainModel.ViolationTicketCountIsRegulation Convert(Oracle.ViolationTicketCountIsRegulation source, DomainModel.ViolationTicketCountIsRegulation destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.ViolationTicketCountIsRegulation.UNKNOWN => DomainModel.ViolationTicketCountIsRegulation.UNKNOWN,
            Oracle.ViolationTicketCountIsRegulation.Y => DomainModel.ViolationTicketCountIsRegulation.Y,
            Oracle.ViolationTicketCountIsRegulation.N => DomainModel.ViolationTicketCountIsRegulation.N,
            _ => DomainModel.ViolationTicketCountIsRegulation.UNKNOWN
        };
    }

    public Oracle.ViolationTicketCountIsRegulation Convert(DomainModel.ViolationTicketCountIsRegulation source, Oracle.ViolationTicketCountIsRegulation destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.ViolationTicketCountIsRegulation.UNKNOWN => Oracle.ViolationTicketCountIsRegulation.UNKNOWN,
            DomainModel.ViolationTicketCountIsRegulation.Y => Oracle.ViolationTicketCountIsRegulation.Y,
            DomainModel.ViolationTicketCountIsRegulation.N => Oracle.ViolationTicketCountIsRegulation.N,
            _ => Oracle.ViolationTicketCountIsRegulation.UNKNOWN
        };
    }

    public DomainModel.ViolationTicketIsChangeOfAddress Convert(Oracle.ViolationTicketIsChangeOfAddress source, DomainModel.ViolationTicketIsChangeOfAddress destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.ViolationTicketIsChangeOfAddress.UNKNOWN => DomainModel.ViolationTicketIsChangeOfAddress.UNKNOWN,
            Oracle.ViolationTicketIsChangeOfAddress.Y => DomainModel.ViolationTicketIsChangeOfAddress.Y,
            Oracle.ViolationTicketIsChangeOfAddress.N => DomainModel.ViolationTicketIsChangeOfAddress.N,
            _ => DomainModel.ViolationTicketIsChangeOfAddress.UNKNOWN
        };
    }

    public Oracle.ViolationTicketIsChangeOfAddress Convert(DomainModel.ViolationTicketIsChangeOfAddress source, Oracle.ViolationTicketIsChangeOfAddress destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.ViolationTicketIsChangeOfAddress.UNKNOWN => Oracle.ViolationTicketIsChangeOfAddress.UNKNOWN,
            DomainModel.ViolationTicketIsChangeOfAddress.Y => Oracle.ViolationTicketIsChangeOfAddress.Y,
            DomainModel.ViolationTicketIsChangeOfAddress.N => Oracle.ViolationTicketIsChangeOfAddress.N,
            _ => Oracle.ViolationTicketIsChangeOfAddress.UNKNOWN
        };
    }

    public DomainModel.ViolationTicketIsDriver Convert(Oracle.ViolationTicketIsDriver source, DomainModel.ViolationTicketIsDriver destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.ViolationTicketIsDriver.UNKNOWN => DomainModel.ViolationTicketIsDriver.UNKNOWN,
            Oracle.ViolationTicketIsDriver.Y => DomainModel.ViolationTicketIsDriver.Y,
            Oracle.ViolationTicketIsDriver.N => DomainModel.ViolationTicketIsDriver.N,
            _ => DomainModel.ViolationTicketIsDriver.UNKNOWN
        };
    }

    public Oracle.ViolationTicketIsDriver Convert(DomainModel.ViolationTicketIsDriver source, Oracle.ViolationTicketIsDriver destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.ViolationTicketIsDriver.UNKNOWN => Oracle.ViolationTicketIsDriver.UNKNOWN,
            DomainModel.ViolationTicketIsDriver.Y => Oracle.ViolationTicketIsDriver.Y,
            DomainModel.ViolationTicketIsDriver.N => Oracle.ViolationTicketIsDriver.N,
            _ => Oracle.ViolationTicketIsDriver.UNKNOWN
        };
    }

    public DomainModel.ViolationTicketIsOwner Convert(Oracle.ViolationTicketIsOwner source, DomainModel.ViolationTicketIsOwner destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.ViolationTicketIsOwner.UNKNOWN => DomainModel.ViolationTicketIsOwner.UNKNOWN,
            Oracle.ViolationTicketIsOwner.Y => DomainModel.ViolationTicketIsOwner.Y,
            Oracle.ViolationTicketIsOwner.N => DomainModel.ViolationTicketIsOwner.N,
            _ => DomainModel.ViolationTicketIsOwner.UNKNOWN
        };
    }

    public Oracle.ViolationTicketIsOwner Convert(DomainModel.ViolationTicketIsOwner source, Oracle.ViolationTicketIsOwner destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.ViolationTicketIsOwner.UNKNOWN => Oracle.ViolationTicketIsOwner.UNKNOWN,
            DomainModel.ViolationTicketIsOwner.Y => Oracle.ViolationTicketIsOwner.Y,
            DomainModel.ViolationTicketIsOwner.N => Oracle.ViolationTicketIsOwner.N,
            _ => Oracle.ViolationTicketIsOwner.UNKNOWN
        };
    }

    public DomainModel.ViolationTicketIsYoungPerson Convert(Oracle.ViolationTicketIsYoungPerson source, DomainModel.ViolationTicketIsYoungPerson destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            Oracle.ViolationTicketIsYoungPerson.UNKNOWN => DomainModel.ViolationTicketIsYoungPerson.UNKNOWN,
            Oracle.ViolationTicketIsYoungPerson.Y => DomainModel.ViolationTicketIsYoungPerson.Y,
            Oracle.ViolationTicketIsYoungPerson.N => DomainModel.ViolationTicketIsYoungPerson.N,
            _ => DomainModel.ViolationTicketIsYoungPerson.UNKNOWN
        };
    }

    public Oracle.ViolationTicketIsYoungPerson Convert(DomainModel.ViolationTicketIsYoungPerson source, Oracle.ViolationTicketIsYoungPerson destination, AutoMapper.ResolutionContext context)
    {
        return source switch
        {
            DomainModel.ViolationTicketIsYoungPerson.UNKNOWN => Oracle.ViolationTicketIsYoungPerson.UNKNOWN,
            DomainModel.ViolationTicketIsYoungPerson.Y => Oracle.ViolationTicketIsYoungPerson.Y,
            DomainModel.ViolationTicketIsYoungPerson.N => Oracle.ViolationTicketIsYoungPerson.N,
            _ => Oracle.ViolationTicketIsYoungPerson.UNKNOWN
        };
    }

}

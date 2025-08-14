using System.Text.Json.Serialization;

namespace TrafficCourts.OrdsDataService.Tco;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles

public class OrdsDisputeCaseFileSummary
{
    public long dispute_id { get; set; }
    public DateTime? submitted_dt { get; set; }
    public DateTime? jj_decision_dt { get; set; }
    public string signed_by { get; set; }
    public string hearing_type_cd { get; set; }
    public string ticket_number_txt { get; set; }
    public DateTime? violation_dt { get; set; }
    public int unique_violation_dt_count { get; set; }
    public decimal? to_be_heard_at_agen_id { get; set; }
    public string to_be_heard_at_agen_nm { get; set; }
    public string prof_surname_nm { get; set; }
    public string prof_given_1_nm { get; set; }
    public string prof_given_2_nm { get; set; }
    public string prof_given_3_nm { get; set; }
    public string prof_org_nm { get; set; }
    public string time_to_pay_reason_txt { get; set; }
    public string fine_reduction_reason_txt { get; set; }
    public string dispute_status_type_cd { get; set; }
    public string dispute_status_type_dsc { get; set; }
    public decimal? detachment_agen_id { get; set; }
    public string detachment_agency_nm { get; set; }
    public string accident_yn { get; set; }
    public string notice_of_hearing_yn { get; set; }
    public string multiple_officers_yn { get; set; }
    public string electronic_ticket_yn { get; set; }
    public string jj_assigned_to { get; set; }
    public string vtc_assigned_to { get; set; }
    public DateTime? vtc_assigned_dtm { get; set; }
    public decimal? appr_ctrm_agen_id { get; set; }
    public string appr_ctrm_agen_nm { get; set; }
    public string appr_ctrm_room_cd { get; set; }
    public DateTime? appr_tm { get; set; }
    public int? appr_estimated_duration_hh { get; set; }
    public int? appr_estimated_duration_mi { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore IDE1006 // Naming Styles

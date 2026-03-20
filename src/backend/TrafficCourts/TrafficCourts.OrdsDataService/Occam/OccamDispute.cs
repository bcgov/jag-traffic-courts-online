namespace TrafficCourts.OrdsDataService.Occam;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable IDE1006 // Naming Styles


public class OccamDispute
{
    public int dispute_id { get; set; }
    public DateTime submitted_dt { get; set; }
    public string disputant_surname_nm { get; set; }
    public string disputant_given_1_nm { get; set; }
    public string disputant_given_2_nm { get; set; }
    public string disputant_given_3_nm { get; set; }
    public string email_address_txt { get; set; }
    public string email_verified_yn { get; set; }
    public string dispute_status_type_cd { get; set; }
    public string request_court_appearance_yn { get; set; }
    public string disputant_detect_ocr_issues_yn { get; set; }
    public string system_detect_ocr_issues_yn { get; set; }
    public string interpreter_required_yn { get; set; }
    public string user_assigned_to { get; set; }
    public DateTime? filing_dt { get; set; }
    public float? court_agen_id { get; set; }
    public string court_agen_nm { get; set; }
    public int? violation_ticket_upload_id { get; set; }
    public string ticket_number_txt { get; set; }
    public DateTime? violation_dt { get; set; }
    public string jj_assigned_to { get; set; }
    public string most_recent_decision_made_by { get; set; }
    public DateTime? jj_decision_dt { get; set; }
    public string ent_user_id { get; set; }
}


#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore IDE1006 // Naming Styles

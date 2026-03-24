namespace TrafficCourts.Domain.Models;

/// <summary>
/// A temporary extension of the JJDisputeCourtAppearanceRoP model.
/// </summary>
public partial class JJDisputeCourtAppearanceRoP
{
    public JJDisputeCourtAppearanceAmendments? Amendments { get; set; }
}

public class JJDisputeCourtAppearanceAmendments
{
    public long? AppearanceAmendmentId { get; set; }
    
    /// <summary>
    /// Max 30 characters.
    /// </summary>
    public string DisputantSurnameNm { get; set; }
    
    /// <summary>
    /// Max 100 characters.
    /// </summary>
    public string DisputantGivenNamesNm { get; set; }
    
    public DateTime? ViolationDate { get; set; }
    
    /// <summary>
    /// Max 500 characters.
    /// </summary>
    public string OtherNotes { get; set; }
    
    /// <summary>
    /// Max 500 characters.
    /// </summary>
    public string Count1ActSectDescTxt { get; set; }
    
    /// <summary>
    /// Max 500 characters.
    /// </summary>
    public string Count1OtherTxt { get; set; }
    
    /// <summary>
    /// Max 500 characters.
    /// </summary>
    public string Count2ActSectDescTxt { get; set; }
    
    /// <summary>
    /// Max 500 characters.
    /// </summary>
    public string Count2OtherTxt { get; set; }
    
    /// <summary>
    /// Max 500 characters.
    /// </summary>
    public string Count3ActSectDescTxt { get; set; }
    
    /// <summary>
    /// Max 500 characters.
    /// </summary>
    public string Count3OtherTxt { get; set; }
    
    public string CreatedBy { get; set; }

    public DateTime CreatedTs { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime? ModifiedTs { get; set; }
}

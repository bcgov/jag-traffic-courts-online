namespace TrafficCourts.Common.Models;

public record Statute(string StatId, string ActCd, string StatSectionTxt, string StatSubSectionTxt,
    string StatParagraphTxt, string StatSubParagraphTxt, string StatCode, string StatShortDescriptionTxt, string StatDescriptionTxt);

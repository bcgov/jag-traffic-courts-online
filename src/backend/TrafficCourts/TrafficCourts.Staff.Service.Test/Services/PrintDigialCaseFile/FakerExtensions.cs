using Bogus;

namespace TrafficCourts.Staff.Service.Test.Services.PrintDigialCaseFile;

public static class FakerExtensions
{
    public static decimal FineAmount(this Faker faker)
    {
        return faker.PickRandom(new decimal[] { 81m, 109m, 121m, 138m, 167m, 196m, 230m, 253m, 276m, 368m, 483m });
    }
    public static string OffenseDescription(this Faker faker)
    {
        // https://icbc.com/driver-licensing/tickets/Pages/fines-points-offences.aspx
        return faker.PickRandom(new string[]
        {
            "Drive over fire hose",
            "Fail to keep right on divided highway",
            "Follow too closely",
            "Pass on right",
            "Fail to stop for police",
            "No driver's licence/wrong class",
            "Speed in school zone",
            "​​Excessive speed",
            "Speed in  playground zone",
            "Drive on sidewalk",
            "Improper display of plate",
            "Unnecessary noise",
            "Fail to display \"L\" sign in violation of driver's licence condition",
            "Fail to display \"N\" sign in violation of driver's licence condition",
            "Fail to slow down or move over near stopped official vehicle"
        });
    }

    public static string YesOrNo(this Faker faker, bool word = false)
    {
        if (word)
        {
            return faker.PickRandom(new string[] { "Yes", "No" });
        }
        else
        {
            return faker.PickRandom(new string[] { "Y", "N" });
        }
    }

    public static string AttendanceType(this Faker faker)
    {
        return faker.PickRandom(new string[] { "In Person", "Zoom" });
    }

    public static string HearingReason(this Faker faker)
    {
        return faker.PickRandom(new string[] { "HR", "DFW"});
    }

    public static string HistoricalHearingReason(this Faker faker)
    {
        return faker.PickRandom(new string[] { "HR", "DFW", "IJB" });
    }

    public static string Room(this Faker faker)
    {
        return faker.Random.Int(100, 400).ToString("D3");
    }
    public static string PresentOrAbsent(this Faker faker)
    {
        return faker.PickRandom(new string[] { "P", "A" });
    }

    public static string PresentAbsentOrNotRequired(this Faker faker)
    {
        return faker.PickRandom(new string[] { "P", "A", "N" });
    }

    public static string DriversLicenceNumber(this Faker faker)
    {
        return faker.Random.Number(1000000, 9999999).ToString("D7");
    }

    public static string TicketNumber(this Faker faker)
    {
        return faker.Random.String2(2).ToUpper() + faker.Random.Number(0, 99999999).ToString("D8");
    }

    public static int WitnessCount(this Faker faker, int max)
    {
        return faker.Random.Number(0, max);
    }

    public static string InterpreterLanguage(this Faker faker)
    {
        return faker.PickRandom(Languages);
    }

    public static string AgencyId(this Faker faker)
    {
        return faker.PickRandom(AgencyIds);
    }

    public static string AppearanceReasonCode(this Faker faker)
    {
        return faker.PickRandom(AppearanceReasonCodes);
    }

    public static string LegalCounselFirmName(this Faker faker)
    {
        switch (faker.Random.Int(0, 3))
        {
            case 0:
                return $"{faker.Name.LastName()} Law, PLLC";
            case 1:
                return $"{faker.Name.LastName()} & {faker.Name.LastName()} Law, PLLC";
            case 2:
                var name = faker.Name.LastName();
                return $"{name} & {name} Law, PLLC";
            case 3:
                return $"{faker.Name.LastName()}, {faker.Name.LastName()} and {faker.Name.LastName()} Law";
            default:
                return string.Empty;
        }

    }

    private static string[] Languages = new[]
    {
        "Cantonese",
        "Chinese",
        "English",
        "Farsi",
        "Finnish",
        "French",
        "German",
        "Greek",
        "Inuit",
        "Italian",
        "Japanese",
        "Korean",
        "Lepeno",
        "Metis",
        "Polish",
        "Punjabi",
        "Spanish",
        "Swedish",
        "Ukrainian",
        "Unknown",
        "Vietnamese",
        "Sign Language",
        "Arabic",
        "Burmese",
        "Hungarian",
        "Iranian",
        "Mandarin",
        "Persian",
        "Phillipino",
        "Portugese",
        "Romanian",
        "Russian",
        "Serbian",
        "Somalian",
        "Turkish",
        "Tagalog",
        "Indonesian",
        "Amharic",
        "Albanian",
        "Bengali",
        "Cambodian",
        "Croatian",
        "Czech",
        "Hindi",
        "Iiocano",
        "Irish",
        "Kurdish",
        "Khmer",
        "Kashubian",
        "Laotian",
        "Norwegian",
        "Pidgin English",
        "Mandingo",
        "Sinhalese",
        "Slovenian",
        "Slovak",
        "Swahili",
        "Tamil",
        "Thai",
        "Tigrinya",
        "Urdu",
        "Visayan",
        "Yugoslavian",
        "Gujarati",
        "Armenian",
        "Hebrew",
        "Taiwanese",
        "Nubian",
        "Bulgarian",
        "Fijian",
        "Carrier",
        "Chilcotin",
    };

    private static string[] AgencyIds = new[]
    {
        "123.0001",
        "124.0001",
        "8805.0001",
        "8807.0001",
        "8813.0001",
        "8823.0001",
        "8824.0001",
        "8834.0001",
        "8837.0001",
        "8839.0001",
        "8841.0001",
        "8844.0001",
        "9062.0001",
        "9064.0001",
        "9066.0001",
        "9067.0001",
        "9068.0001",
        "9070.0001",
        "9071.0001",
        "9073.0001",
        "9074.0001",
        "9393.0001",
        "10231.0001",
        "10232.0001",
        "10233.0001",
        "10234.0001",
        "10235.0001",
        "10236.0001",
        "10237.0001",
        "10239.0001",
        "10240.0001",
        "10241.0001",
        "10242.0001",
        "10243.0001",
        "22.0001",
        "23.0001",
        "27.0001",
        "29.0001",
        "77.0001",
        "78.0001",
        "79.0001",
        "80.0001",
        "81.0001",
        "82.0001",
        "84.0001",
        "85.0001",
        "87.0001",
        "88.0001",
        "89.0001",
        "91.0001",
        "92.0001",
        "93.0001",
        "94.0001",
        "95.0001",
        "96.0001",
        "104.0001",
        "105.0001",
        "106.0001",
        "107.0001",
        "108.0001",
        "109.0001",
        "110.0001",
        "111.0001",
        "112.0001",
        "113.0001",
        "114.0001",
        "115.0001",
        "116.0001",
        "118.0001",
        "119.0001",
        "121.0001",
        "122.0001",
        "10245.0001",
        "10246.0001",
        "10247.0001",
        "10248.0001",
        "10249.0001",
        "10251.0001",
        "10252.0001",
        "10253.0001",
        "10255.0001",
        "10256.0001",
        "10257.0001",
        "10258.0001",
        "10264.0001",
        "10266.0001",
        "10267.0001",
        "10268.0001",
        "10912.0026",
        "16164.0026",
        "19187.0734",
        "19247.0734",
        "19307.0734",
        "10244.0001",
        "83.0001",
        "18817.0045",
        "18886.0045",
        "19057.0002",
        "19058.0002",
        "19059.0002",
        "19060.0002",
        "19061.0002",
        "19062.0002",
        "19063.0002",
        "19064.0002",
        "19065.0002",
        "28.0001",
        "90.0001",
        "117.0001",
        "120.0001",
        "9072.0001",
        "9075.0001",
        "10230.0001",
        "10254.0001",
        "10265.0001",
        "8842.0001",
        "9144.0001",
        "10238.0001",
        "10250.0001",
        "19066.0002",
        "19227.0734",
        "19228.0734",
        "19614.0734",
        "19678.0734",
        "19679.0734",
        "19622.0734",
        "19625.0734",
        "19626.0734",
        "19627.0734",
        "19635.0734",
    };

    public static string[] AppearanceReasonCodes =
    {
        "ASC", // Aboriginal Sentencing Circle
        "ELE", // Accused will elect mode of trial
        "ACT", // Additional Continuation Date
        "AC", // Administrative Court 
        "ACS", // App after alleg. Breach of Community Supervision
        "ACM", // App after alleg. Breach of Cond. Supervision - Murder
        "ACP", // App after alleg. Breach of Cond. Supervision - Presumptive
        "ADC", // App after alleg. Breach of Def. Cust. & Supervision
        "AIR", // App after alleg. Breach of Int. Rehab. Supervision
        "APP", // Application
        "APW", // Application For Warrant
        "ACC", // Application for Continued Custody
        "AMR", // Application for Mandatory Review
        "AOR", // Application for Optional Review
        "ANC", // Application for a non-custodial Review
        "ARB", // Application to Revoke Bail
        "AVB", // Application to Vary Bail
        "AVS", // Application to Vary Sentence
        "ARH", // Arraignment Hearing
        "BCA", // BCA - BC Court of Appeal Hearing
        "BR", // Bail Review
        "CH", // Chambers Application
        "CSH", // Conditional Sentence Hearing
        "CNF", // Conference
        "CRO", // Conferencing Report Ordered
        "CSR", // Conferencing Report and Pre-sentence Report Ord.
        "CTD", // Confirm Trial Date
        "CNT", // Continuation of a trial or hearing
        "CNA", // Continuation of hearing on an application
        "DOH", // Dangerous Offender Hearing
        "DEA", // Decision on Application
        "DEC", // Decision to be rendered by court
        "DRH", // Detention Review Hearing
        "DFW", // Dispute Fine Amount in Writing
        "EMR", // Electronic Monitoring Report
        "FA", // First Appearance
        "FAW", // First Appearance after Warrant/Police Release
        "FOH", // Focus Hearing
        "DSC", // For Disclosure
        "DSP", // For Disposition
        "HR", // For Hearing
        "PAR", // For Particulars
        "FT", // For Trial
        "FTJ", // For Trial By Jury
        "AHR", // For an Arraignment Hearing
        "CWI", // For compliance with instructions
        "DBJ", // For direction by a Judge
        "HOA", // Hearing of Appeal
        "IMM", // Immigration Hearing
        "IGP", // Intention to enter a guilty plea
        "JMT", // Judgment
        "JIR", // Judicial Interim Release
        "JRH", // Judicial Referral Hearing
        "JR", // Judicial Review
        "JSL", // Jury Selection
        "JSC", // Jury Selection Challenge
        "JST", // Jury Selection and Trial
        "VDT", // Jury Trial Verdict
        "LTH", // Long Term Offender Hearing
        "OBC", // Obtained Counsel
        "CD", // Old ACTS Appearance Reason: Data Conversion
        "IBD", // Old ACTS Appearance Reason: Data Conversion
        "IBJ", // Old ACTS Appearance Reason: Data Conversion
        "VA", // Old ACTS Appearance Reason: Data Conversion
        "IR", // Old ACTS Appearance Reason: Data Conversion
        "SA", // Old ACTS Appearance Reason: Data Conversion
        "SC", // Old ACTS Appearance Reason: Data Conversion
        "BWH", // Old ACTS Appearance Reason: Data Conversion
        "IBP", // Old ACTS Appearance Reason: Data Conversion
        "PBW", // Pick up on Bench Warrant
        "PLE", // Plea
        "PSB", // Pre-Sentence report plus Psychiatric or Psychological report
        "PDR", // Pre-disposition Report
        "PSR", // Pre-sentence Report
        "PTC", // Pre-trial Conference
        "ARG", // Pre-trial argument
        "PI", // Preliminary Inquiry
        "PRH", // Progress Hearing
        "PGR", // Progress Report
        "PSY", // Psychiatric exam and report/assessment order
        "REE", // Re-election
        "REV", // Review of Sentence/Dispostion
        "SH", // Scheduling Hearing
        "SNT", // Sentence Hearing
        "ST", // Simplified Trail
        "SCA", // Summary Conviction Appeal Hearing
        "TSR", // Technical Suitability Report
        "TLM", // Time Line Alternative Measures
        "TLD", // Time Line Discussion
        "TLX", // Time Line Extension
        "TLH", // Time Line Hearing
        "FXD", // To Fix a Date
        "TGC", // To Get Counsel
        "TPA", // To Prove Age
        "CLC", // To consult counsel
        "TH", // Transfer Hearing
        "TCH", // Trial Confirmation Hearing
        "TDC", // Trial Date Cancelled
        "UNK", // Unknown
        "VD", // Voir Dire
    };
}



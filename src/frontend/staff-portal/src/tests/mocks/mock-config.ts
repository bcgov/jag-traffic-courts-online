import { Configuration } from '@config/config.model';

export class MockConfig {
  public static get(): Configuration {
    /* eslint-disable */
    // Export of /lookup response:
    return {
      statuses: [
        {
          code: 0,
          name: 'New',
        },
        {
          code: 1,
          name: 'Under Review',
        },
        {
          code: 2,
          name: 'In Progress',
        },
        {
          code: 3,
          name: 'Completed',
        },
        {
          code: 4,
          name: 'Rejected',
        },
      ],
      countries: [
        {
          ctryId: 1,
          ctryLongNm: 'Canada',
        },
        {
          ctryId: 1,
          ctryLongNm: 'United States',
        },
      ],
      provincesAndStates: [
      {
          ctryId: 1,
          provSeqNo: 1,
          provNm: "British Columbia",
          provAbbreviationCd: "BC"
      },
      {
          ctryId: 1,
          provSeqNo: 2,
          provNm: "Manitoba",
          provAbbreviationCd: "MB"
      },
      {
          ctryId: 1,
          provSeqNo: 3,
          provNm: "Quebec",
          provAbbreviationCd: "PQ"
      },
      {
          ctryId: 1,
          provSeqNo: 4,
          provNm: "New Brunswick",
          provAbbreviationCd: "NB"
      },
      {
          ctryId: 1,
          provSeqNo: 4,
          provNm: "Newfoundland",
          provAbbreviationCd: "NL"
      },
      {
          ctryId: 1,
          provSeqNo: 6,
          provNm: "Nova Scotia",
          provAbbreviationCd: "NS"
      },
      {
          ctryId: 1,
          provSeqNo: 7,
          provNm: "Prince Edward Island",
          provAbbreviationCd: "PE"
      },
      {
          ctryId: 1,
          provSeqNo: 8,
          provNm: "Northwest Territories",
          provAbbreviationCd: "NT"
      },
      {
          ctryId: 1,
          provSeqNo: 9,
          provNm: "Yukon",
          provAbbreviationCd: "YT"
      },
      {
          ctryId: 1,
          provSeqNo: 10,
          provNm: "Alberta",
          provAbbreviationCd: "AB"
      },
      {
          ctryId: 1,
          provSeqNo: 11,
          provNm: "Saskatchewan",
          provAbbreviationCd: "SK"
      },
      {
          ctryId: 1,
          provSeqNo: 12,
          provNm: "Ontario",
          provAbbreviationCd: "ON"
      },
      {
          ctryId: 1,
          provSeqNo: 13,
          provNm: "Nunavut",
          provAbbreviationCd: "NU"
      },
      {
        ctryId: 2,
        provSeqNo: 1,
        provNm: "Washington",
        provAbbreviationCd: "WA"
    },
    {
        ctryId: 2,
        provSeqNo: 2,
        provNm: "Oregon",
        provAbbreviationCd: "OR"
    },
    {
        ctryId: 2,
        provSeqNo: 3,
        provNm: "California",
        provAbbreviationCd: "CA"
    },
    {
        ctryId: 2,
        provSeqNo: 4,
        provNm: "Arizona",
        provAbbreviationCd: "AZ"
    },
    {
        ctryId: 2,
        provSeqNo: 5,
        provNm: "Nevada",
        provAbbreviationCd: "NV"
    },
    {
        ctryId: 2,
        provSeqNo: 6,
        provNm: "Texas",
        provAbbreviationCd: "TX"
    },
    {
        ctryId: 2,
        provSeqNo: 7,
        provNm: "Louisiana",
        provAbbreviationCd: "LA"
    },
    {
        ctryId: 2,
        provSeqNo: 8,
        provNm: "Mississippi",
        provAbbreviationCd: "MS"
    },
    {
        ctryId: 2,
        provSeqNo: 9,
        provNm: "Alabama",
        provAbbreviationCd: "AL"
    },
    {
        ctryId: 2,
        provSeqNo: 10,
        provNm: "Georgia",
        provAbbreviationCd: "GA"
    },
    {
        ctryId: 2,
        provSeqNo: 11,
        provNm: "North Dakota",
        provAbbreviationCd: "ND"
    },
    {
        ctryId: 2,
        provSeqNo: 12,
        provNm: "South Dakota",
        provAbbreviationCd: "SD"
    },
    {
        ctryId: 2,
        provSeqNo: 13,
        provNm: "South Carolina",
        provAbbreviationCd: "SC"
    },
    {
        ctryId: 2,
        provSeqNo: 14,
        provNm: "North Carolina",
        provAbbreviationCd: "NC"
    },
    {
        ctryId: 2,
        provSeqNo: 15,
        provNm: "Michigan",
        provAbbreviationCd: "MI"
    },
    {
        ctryId: 2,
        provSeqNo: 16,
        provNm: "Minnesota",
        provAbbreviationCd: "MN"
    },
    {
        ctryId: 2,
        provSeqNo: 17,
        provNm: "Kentucky",
        provAbbreviationCd: "KY"
    },
    {
        ctryId: 2,
        provSeqNo: 18,
        provNm: "Tennessee",
        provAbbreviationCd: "TN"
    },
    {
        ctryId: 2,
        provSeqNo: 19,
        provNm: "Ohio",
        provAbbreviationCd: "OH"
    },
    {
        ctryId: 2,
        provSeqNo: 20,
        provNm: "Illinois",
        provAbbreviationCd: "IL"
    },
    {
        ctryId: 2,
        provSeqNo: 21,
        provNm: "New York",
        provAbbreviationCd: "NY"
    },
    {
        ctryId: 2,
        provSeqNo: 22,
        provNm: "Colorado",
        provAbbreviationCd: "CO"
    },
    {
        ctryId: 2,
        provSeqNo: 23,
        provNm: "Florida",
        provAbbreviationCd: "FL"
    },
    {
        ctryId: 2,
        provSeqNo: 24,
        provNm: "Iowa",
        provAbbreviationCd: "IA"
    },
    {
        ctryId: 2,
        provSeqNo: 25,
        provNm: "Indiana",
        provAbbreviationCd: "IN"
    },
    {
        ctryId: 2,
        provSeqNo: 26,
        provNm: "Kansas",
        provAbbreviationCd: "KS"
    },
    {
        ctryId: 2,
        provSeqNo: 27,
        provNm: "Arkansas",
        provAbbreviationCd: "AR"
    },
    {
        ctryId: 2,
        provSeqNo: 28,
        provNm: "Alaska",
        provAbbreviationCd: "AK"
    },
    {
        ctryId: 2,
        provSeqNo: 29,
        provNm: "Montana",
        provAbbreviationCd: "MT"
    },
    {
        ctryId: 2,
        provSeqNo: 30,
        provNm: "Connecticut",
        provAbbreviationCd: "CT"
    },
    {
        ctryId: 2,
        provSeqNo: 31,
        provNm: "Delaware",
        provAbbreviationCd: "DE"
    },
    {
        ctryId: 2,
        provSeqNo: 32,
        provNm: "District of Columbia",
        provAbbreviationCd: "DC"
    },
    {
        ctryId: 2,
        provSeqNo: 33,
        provNm: "Hawaii",
        provAbbreviationCd: "HI"
    },
    {
        ctryId: 2,
        provSeqNo: 34,
        provNm: "Idaho",
        provAbbreviationCd: "ID"
    },
    {
        ctryId: 2,
        provSeqNo: 35,
        provNm: "Maine",
        provAbbreviationCd: "ME"
    },
    {
        ctryId: 2,
        provSeqNo: 36,
        provNm: "Maryland",
        provAbbreviationCd: "MD"
    },
    {
        ctryId: 2,
        provSeqNo: 37,
        provNm: "Massachusetts",
        provAbbreviationCd: "MA"
    },
    {
        ctryId: 2,
        provSeqNo: 38,
        provNm: "Missouri",
        provAbbreviationCd: "MO"
    },
    {
        ctryId: 2,
        provSeqNo: 39,
        provNm: "Nebraska",
        provAbbreviationCd: "NE"
    },
    {
        ctryId: 2,
        provSeqNo: 40,
        provNm: "New Hampshire",
        provAbbreviationCd: "NH"
    },
    {
        ctryId: 2,
        provSeqNo: 41,
        provNm: "New Jersey",
        provAbbreviationCd: "NJ"
    },
    {
        ctryId: 2,
        provSeqNo: 42,
        provNm: "New Mexico",
        provAbbreviationCd: "NM"
    },
    {
        ctryId: 2,
        provSeqNo: 43,
        provNm: "Oklahoma",
        provAbbreviationCd: "OK"
    },
    {
        ctryId: 2,
        provSeqNo: 44,
        provNm: "Pennsylvania",
        provAbbreviationCd: "PA"
    },
    {
        ctryId: 2,
        provSeqNo: 45,
        provNm: "Rhode Island",
        provAbbreviationCd: "RI"
    },
    {
        ctryId: 2,
        provSeqNo: 46,
        provNm: "Utah",
        provAbbreviationCd: "UT"
    },
    {
        ctryId: 2,
        provSeqNo: 47,
        provNm: "Vermont",
        provAbbreviationCd: "VT"
    },
    {
        ctryId: 2,
        provSeqNo: 48,
        provNm: "Virginia",
        provAbbreviationCd: "VA"
    },
    {
        ctryId: 2,
        provSeqNo: 49,
        provNm: "West Virginia",
        provAbbreviationCd: "WV"
    },
    {
        ctryId: 2,
        provSeqNo: 50,
        provNm: "Wisconsin",
        provAbbreviationCd: "WI"
    },
    {
        ctryId: 2,
        provSeqNo: 51,
        provNm: "Wyoming",
        provAbbreviationCd: "WY"
    }
      ]
    };
    /* eslint-enable */
  }
}

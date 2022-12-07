import { Configuration } from '@config/config.model';

export class MockConfig {
  public static get(): Configuration {
    /* eslint-disable */
    // Export of /lookup response:
    return {
      countries: [
        {
          ctryId: 1,
          ctryLongNm: 'Canada',
        },
        {
          ctryId: 2,
          ctryLongNm: 'United States',
        },
      ],
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
      provincesAndStates: [
        {
          provId: 1,
          ctryId: 1,
          provSeqNo: 1,
          provNm: "British Columbia",
          provAbbreviationCd: "BC"
        },
        {
          provId: 2,
          ctryId: 1,
          provSeqNo: 2,
          provNm: "Manitoba",
          provAbbreviationCd: "MB"
        },
        {
          provId: 3,
          ctryId: 1,
          provSeqNo: 3,
          provNm: "Quebec",
          provAbbreviationCd: "PQ"
        },
        {
          provId: 4,
          ctryId: 1,
          provSeqNo: 4,
          provNm: "New Brunswick",
          provAbbreviationCd: "NB"
        },
        {
          provId: 5,
          ctryId: 1,
          provSeqNo: 5,
          provNm: "Newfoundland",
          provAbbreviationCd: "NL"
        },
        {
          provId: 6,
          ctryId: 1,
          provSeqNo: 6,
          provNm: "Nova Scotia",
          provAbbreviationCd: "NS"
        },
        {
          provId: 7,
          ctryId: 1,
          provSeqNo: 7,
          provNm: "Prince Edward Island",
          provAbbreviationCd: "PE"
        },
        {
          provId: 8,
          ctryId: 1,
          provSeqNo: 8,
          provNm: "Northwest Territories",
          provAbbreviationCd: "NT"
        },
        {
          provId: 9,
          ctryId: 1,
          provSeqNo: 9,
          provNm: "Yukon",
          provAbbreviationCd: "YT"
        },
        {
          provId: 10,
          ctryId: 1,
          provSeqNo: 10,
          provNm: "Alberta",
          provAbbreviationCd: "AB"
        },
        {
          provId: 11,
          ctryId: 1,
          provSeqNo: 11,
          provNm: "Saskatchewan",
          provAbbreviationCd: "SK"
        },
        {
          provId: 12,
          ctryId: 1,
          provSeqNo: 12,
          provNm: "Ontario",
          provAbbreviationCd: "ON"
        },
        {
          provId: 13,
          ctryId: 1,
          provSeqNo: 13,
          provNm: "Nunavut",
          provAbbreviationCd: "NU"
        }
      ],
    };
    /* eslint-enable */
  }
}

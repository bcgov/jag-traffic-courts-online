import { Configuration } from '@config/config.model';

export class MockConfig {
  public static get(): Configuration {
    /* eslint-disable */
    // Export of /lookup response:
    return {
      courtLocations: [
        { code: '10264.0007', name: '100 Mile House Law Courts', jjTeam: 'D'},
        { code: '9393.0007', name: 'Abbotsford Provincial Court', jjTeam: 'C'},
        { code: '104.0007', name: 'Alexis Creek Provincial Court', jjTeam: 'D'},
        { code: '105.0007', name: 'Anahim Lake Provincial Court', jjTeam: 'D'},
        { code: '50764.0009', name: 'Anvil Centre', jjTeam: 'D'},
        { code: '106.0007', name: 'Atlin Provincial Court', jjTeam: 'D'},
        { code: '10244.0007', name: 'Bella Bella Provincial Court', jjTeam: 'C'},
        { code: '10245.0007', name: 'Bella Coola Provincial Court', jjTeam: 'C'},
        { code: '107.0007', name: 'Burns Lake Provincial Court', jjTeam: 'D'},
        { code: '9067.0007', name: 'Campbell River Law Courts', jjTeam: 'D'},
        { code: '23.0007', name: 'Castlegar Provincial Court', jjTeam: 'D'},
        { code: '109.0007', name: 'Chetwynd Provincial Court', jjTeam: 'D'},
        { code: '51064.0009', name: 'Chilliwack Cultural Centre', jjTeam: 'C'},
        { code: '8824.0007', name: 'Chilliwack Law Courts', jjTeam: 'C'},
        { code: '28.0007', name: 'Clearwater Provincial Court', jjTeam: 'D'},
        { code: '51514.0009', name: 'Coast Kamloops Hotel & Conference Centre', jjTeam: 'D'},
        { code: '9068.0007', name: 'Courtenay Law Courts', jjTeam: 'D'},
        { code: '51316.0009', name: 'Cowichan Community Centre', jjTeam: 'D'},
        { code: '29.0007', name: 'Cranbrook Law Courts', jjTeam: 'D'},
        { code: '77.0007', name: 'Creston Law Courts', jjTeam: 'D'},
        { code: '110.0007', name: 'Dawson Creek Law Courts', jjTeam: 'D'},
        { code: '111.0007', name: 'Dease Lake Provincial Court', jjTeam: 'D'},
        { code: '33414.0007', name: 'Downtown Community Court', jjTeam: 'D'},
        { code: '10231.0007', name: 'Duncan Law Courts', jjTeam: 'D'},
        { code: '52214.0009', name: 'Erwin Stege Community Centre', jjTeam: 'D'},
        { code: '50814.0009', name: 'Evergreen Hall', jjTeam: 'D'},
        { code: '78.0007', name: 'Fernie Law Courts', jjTeam: 'D'},
        { code: '112.0007', name: 'Fort Nelson Law Courts', jjTeam: 'D'},
        { code: '113.0007', name: 'Fort St James Provincial Court', jjTeam: 'D'},
        { code: '114.0007', name: 'Fort St John Law Courts', jjTeam: 'D'},
        { code: '115.0007', name: 'Fraser Lake Provincial Court', jjTeam: 'D'},
        { code: '10232.0007', name: 'Ganges Provincial Court', jjTeam: 'D'},
        { code: '10233.0007', name: 'Gold River Provincial Court', jjTeam: 'D'},
        { code: '79.0007', name: 'Golden Law Courts', jjTeam: 'D'},
        { code: '108.0007', name: 'Good Hope Lake Provincial Court', jjTeam: 'D'},
        { code: '80.0007', name: 'Grand Forks Law Courts', jjTeam: 'D'},
        { code: '124.0007', name: 'Hazelton Provincial Court', jjTeam: 'D'},
        { code: '116.0007', name: 'Houston Provincial Court', jjTeam: 'D'},
        { code: '117.0007', name: 'Hudson\'s Hope Provincial Court', jjTeam: 'D'},
        { code: '51464.0009', name: 'Inn at the Quay', jjTeam: 'D'},
        { code: '81.0007', name: 'Invermere Law Courts', jjTeam: 'D'},
        { code: '20263.0007', name: 'Justice Centre (Judicial)', jjTeam: 'D'},
        { code: '82.0007', name: 'Kamloops Law Courts', jjTeam: 'D'},
        { code: '51164.0009', name: 'Kamloops Thompson River University', jjTeam: 'D'},
        { code: '83.0007', name: 'Kelowna Law Courts', jjTeam: 'D'},
        { code: '118.0007', name: 'Kitimat Law Courts', jjTeam: 'D'},
        { code: '51714.0009', name: 'Kiwanis Performing Arts Centre', jjTeam: 'D'},
        { code: '16988.0007', name: 'Klemtu Provincial Court', jjTeam: 'D'},
        { code: '10257.0007', name: 'Kwadacha Provincial Court', jjTeam: 'D'},
        { code: '52114.0009', name: 'Lillooet Elks Hall', jjTeam: 'D'},
        { code: '85.0007', name: 'Lillooet Law Courts', jjTeam: 'D'},
        { code: '119.0007', name: 'Lower Post Provincial Court', jjTeam: 'D'},
        { code: '120.0007', name: 'Mackenzie Provincial Court', jjTeam: 'D'},
        { code: '121.0007', name: 'Masset Provincial Court', jjTeam: 'D'},
        { code: '122.0007', name: 'McBride Provincial Court', jjTeam: 'D'},
        { code: '51264.0009', name: 'Merritt Civic Centre', jjTeam: 'D'},
        { code: '87.0007', name: 'Merritt Law Courts', jjTeam: 'D'},
        { code: '51214.0009', name: 'Nakusp & District Sports Complex', jjTeam: 'D'},
        { code: '88.0007', name: 'Nakusp Provincial Court', jjTeam: 'D'},
        { code: '8805.0007', name: 'Nanaimo Law Courts', jjTeam: 'D'},
        { code: '50864.0009', name: 'Nelson Capitol Theatre', jjTeam: 'D'},
        { code: '89.0007', name: 'Nelson Law Courts', jjTeam: 'D'},
        { code: '123.0007', name: 'New Aiyansh Provincial Court', jjTeam: 'D'},
        { code: '8839.0007', name: 'New Westminster Law Courts', jjTeam: 'C'},
        { code: '9064.0007', name: 'North Vancouver Provincial Court', jjTeam: 'B'},
        { code: '29464.0007', name: 'Office of the Chief Judge', jjTeam: 'D'},
        { code: '10246.0007', name: 'Pemberton Provincial Court', jjTeam: 'B'},
        { code: '91.0007', name: 'Penticton Law Courts', jjTeam: 'D'},
        { code: '52465.0009', name: 'Penticton Seniors Centre', jjTeam: 'D'},
        { code: '10235.0007', name: 'Port Alberni Law Courts', jjTeam: 'D'},
        { code: '8834.0007', name: 'Port Coquitlam Provincial Court', jjTeam: 'C'},
        { code: '10236.0007', name: 'Port Hardy Law Courts', jjTeam: 'D'},
        { code: '10237.0007', name: 'Powell River Law Courts', jjTeam: 'D'},
        { code: '8844.0007', name: 'Prince George Law Courts', jjTeam: 'D'},
        { code: '9075.0007', name: 'Prince Rupert Law Courts', jjTeam: 'D'},
        { code: '92.0007', name: 'Princeton Law Courts', jjTeam: 'D'},
        { code: '10266.0007', name: 'Queen Charlotte Provincial Court', jjTeam: 'D'},
        { code: '9074.0007', name: 'Quesnel Law Courts', jjTeam: 'D'},
        { code: '93.0007', name: 'Revelstoke Law Courts', jjTeam: 'D'},
        { code: '9062.0007', name: 'Richmond Provincial Court', jjTeam: 'B'},
        { code: '10250.0007', name: 'Robson Square Provincial Court', jjTeam: 'A'},
        { code: '94.0007', name: 'Rossland Law Courts', jjTeam: 'D'},
        { code: '95.0007', name: 'Salmon Arm Law Courts', jjTeam: 'D'},
        { code: '10248.0007', name: 'Sechelt Provincial Court', jjTeam: 'B'},
        { code: '10238.0007', name: 'Sidney Provincial Court', jjTeam: 'D'},
        { code: '9073.0007', name: 'Smithers Law Courts', jjTeam: 'D'},
        { code: '10256.0007', name: 'Sparwood Provincial Court', jjTeam: 'D'},
        { code: '10267.0007', name: 'Stewart Provincial Court', jjTeam: 'D'},
        { code: '8841.0007', name: 'Surrey Provincial Court', jjTeam: 'B'},
        { code: '10239.0007', name: 'Tahsis Provincial Court', jjTeam: 'D'},
        { code: '9072.0007', name: 'Terrace Law Courts', jjTeam: 'D'},
        { code: '51364.0009', name: 'Terrace Sportsplex', jjTeam: 'D'},
        { code: '10240.0007', name: 'Tofino Provincial Court', jjTeam: 'D'},
        { code: '10258.0007', name: 'Tsay Keh Dene Provincial Court', jjTeam: 'D'},
        { code: '10268.0007', name: 'Tumbler Ridge Provincial Court', jjTeam: 'D'},
        { code: '10241.0007', name: 'Ucluelet Provincial Court', jjTeam: 'D'},
        { code: '9144.0007', name: 'Valemount Provincial Court', jjTeam: 'D'},
        { code: '8816.0007', name: 'Vancouver Law Courts', jjTeam: 'A'},
        { code: '8813.0007', name: 'Vancouver Provincial Court', jjTeam: 'A'},
        { code: '9071.0007', name: 'Vanderhoof Law Courts', jjTeam: 'D'},
        { code: '96.0007', name: 'Vernon Law Courts', jjTeam: 'D'},
        { code: '8807.0007', name: 'Victoria Law Courts', jjTeam: 'D'},
        { code: '31964.0007', name: 'Violation Ticket Centre', jjTeam: 'D'},
        { code: '10243.0007', name: 'Western Communities Provincial Court', jjTeam: 'D'},
        { code: '51365.0009', name: 'Williams Lake Elks Hall', jjTeam: 'D'},
        { code: '9070.0007', name: 'Williams Lake Law Courts', jjTeam: 'D'},
        { code: '52615.0009', name: 'Williams Lake MacKinnon Hall', jjTeam: 'D'}
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

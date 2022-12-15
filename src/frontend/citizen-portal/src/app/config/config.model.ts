export interface Configuration {
  countries: CountryCodeValue[];
  provincesAndStates: ProvinceCodeValue[];
  statuses: Config<number>[];
}

export class Config<T> {
  code: T;
  name: string;

  constructor(code: T, name: string) {
    this.code = code;
    this.name = name;
  }
}

export class CountryCodeValue {
  ctryId?: number;
  ctryLongNm?: string;

  constructor(ctryId: number, ctryLongNm: string) {
    this.ctryId = ctryId;
    this.ctryLongNm = ctryLongNm;
  }
}

export class ProvinceCodeValue {
  ctryId?: number;
  provSeqNo?: number;
  provNm?: string;
  provAbbreviationCd?: string;
  provId?: number;

  constructor (ctryId?: number, provSeqNo?: number, provNm?: string, provAbbreviationCd?: string, provId?: number) {
    this.ctryId = ctryId;
    this.provSeqNo = provSeqNo;
    this.provNm = provNm;
    this.provAbbreviationCd = provAbbreviationCd;
    this.provId = provId;
  }
}

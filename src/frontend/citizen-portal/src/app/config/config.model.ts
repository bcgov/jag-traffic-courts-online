import { CountryCodeValue, ProvinceCodeValue } from "./config.service";

export interface Configuration {
  policeLocations: Config<string>[];
  countries: CountryCodeValue[];
  provincesAndStates: ProvinceCodeValue[];
  statuses: Config<number>[];
  statutes: Config<number>[];
}

export class Config<T> {
  code: T;
  name: string;

  constructor(code: T, name: string) {
    this.code = code;
    this.name = name;
  }
}

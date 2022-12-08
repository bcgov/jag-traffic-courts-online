import { CountryCodeValue, ProvinceCodeValue } from "./config.service";

export interface Configuration {
  courtLocations: CourthouseConfig[];
  countries: CountryCodeValue[];
  statuses: Config<number>[];
  provincesAndStates: ProvinceCodeValue[];
}

export class Config<T> {
  code: T;
  name: string;

  constructor(code: T, name: string) {
    this.code = code;
    this.name = name;
  }
}

export interface CourthouseConfig extends Config<string> {
  jjTeam: string;
}

export interface Configuration {
  courtLocations: CourthouseConfig[];
  policeLocations: Config<string>[];
  countries: Config<string>[];
  provinces: ProvinceConfig[];
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

export interface ProvinceConfig extends Config<string> {
  countryCode: string;
}

export interface CourthouseConfig extends Config<string> {
  jjTeam: string;
}

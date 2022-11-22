export interface Configuration {
  courtLocations: Config<string>[];
  policeLocations: Config<string>[];
  countries: Config<string>[];
  provinces: ProvinceConfig[];
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

export interface ProvinceConfig extends Config<string> {
  countryCode: string;
}

import { Injectable, Inject } from '@angular/core';

import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { Configuration, Config, ProvinceConfig } from '@config/config.model';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { UtilsService, SortWeight } from '@core/services/utils.service';

export interface IConfigService extends Configuration {
  load(): Observable<Configuration>;
}

@Injectable({
  providedIn: 'root',
})
export class ConfigService implements IConfigService {
  protected configuration: Configuration;

  constructor(
    protected apiResource: ApiResource,
    protected utilsService: UtilsService
  ) {}

  public get provinces(): ProvinceConfig[] {
    return [...this.configuration.provinces].sort(this.sortConfigByName());
  }

  public get courtLocations(): Config<string>[] {
    return [...this.configuration.courtLocations].sort(this.sortConfigByName());
  }

  public get countries(): Config<string>[] {
    return [...this.configuration.countries].sort(this.sortConfigByName());
  }

  public get statuses(): Config<number>[] {
    return [...this.configuration.statuses].sort(this.sortConfigByName());
  }

  public get statutes(): Config<number>[] {
    return [...this.configuration.statutes].sort(this.sortConfigByName());
  }

  /**
   * @description
   * Load the runtime configuration.
   */
  public load(): Observable<Configuration> {
    if (!this.configuration) {
      return this.getConfiguration().pipe(
        map((config: Configuration) => (this.configuration = config))
      );
    }

    return of({ ...this.configuration });
  }

  /**
   * @description
   * Get the configuration for bootstrapping the application.
   */
  private getConfiguration(): Observable<Configuration> {
    return this.apiResource
      .get<Configuration>('lookups')
      .pipe(map((response: ApiHttpResponse<Configuration>) => response.result));
  }

  /**
   * @description
   * Sort the configuration by name.
   */
  private sortConfigByName(): (
    a: Config<number | string>,
    b: Config<number | string>
  ) => SortWeight {
    return (a: Config<number | string>, b: Config<number | string>) =>
      this.utilsService.sortByKey<Config<number | string>>(a, b, 'name');
  }
}

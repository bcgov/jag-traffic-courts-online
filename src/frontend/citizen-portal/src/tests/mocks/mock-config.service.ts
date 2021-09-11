import { Injectable } from '@angular/core';
import { Configuration } from '@config/config.model';
import { ConfigService, IConfigService } from '@config/config.service';
import { UtilsService } from '@core/services/utils.service';
import { LookupAPIService } from 'app/api';
import { AppConfigService } from 'app/services/app-config.service';
import { Observable } from 'rxjs';
import { MockConfig } from './mock-config';

@Injectable({
  providedIn: 'root',
})
export class MockConfigService extends ConfigService implements IConfigService {
  constructor(
    protected lookupAPIService: LookupAPIService,
    protected utilsService: UtilsService,
    protected appConfigService: AppConfigService
  ) {
    super(utilsService, appConfigService, lookupAPIService);

    // Load the runtime configuration
    this.load().subscribe();
  }

  public load(): Observable<Configuration> {
    return new Observable<Configuration>((subscriber) => {
      const configuration = MockConfig.get();

      subscriber.next((this.configuration = configuration));
      subscriber.complete();
    });
  }
}

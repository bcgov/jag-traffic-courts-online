import { Injectable } from '@angular/core';
import { ConfigService } from '@config/config.service';
import { UtilsService } from '@core/services/utils.service';
import { LookupService } from 'app/api';
import { AppConfigService } from 'app/services/app-config.service';

@Injectable({
  providedIn: 'root',
})
export class MockConfigService extends ConfigService {
  constructor(
    protected lookupAPIService: LookupService,
    protected utilsService: UtilsService,
    protected appConfigService: AppConfigService
  ) {
    super(utilsService, appConfigService);
  }
}

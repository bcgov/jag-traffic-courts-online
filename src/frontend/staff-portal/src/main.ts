import { enableProdMode, provideZoneChangeDetection } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppConfigService } from 'app/services/app-config.service';

import { AppModule } from './app/app.module';

fetch('/assets/app.config.json')
  .then((response) => response.json())
  .then((config) => {
    console.log('Is production?', config.production);
    if (config.production) {
      enableProdMode();
    }

    platformBrowserDynamic([{ provide: AppConfigService, useValue: config }])
      .bootstrapModule(AppModule, { applicationProviders: [provideZoneChangeDetection()], })
      .catch((err) => console.error(err));
  });

import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppConfig } from 'app/services/app-config.service';

import { AppModule } from './app/app.module';

fetch('/assets/app.config.json')
  .then((response) => response.json())
  .then((appConfig) => {
    console.log('Is production?', appConfig.production);
    if (appConfig.production) {
      enableProdMode();
    }

    platformBrowserDynamic([{ provide: AppConfig, useValue: appConfig }])
      .bootstrapModule(AppModule)
      .catch((err) => console.error(err));
  });

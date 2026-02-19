import { enableProdMode, provideZoneChangeDetection } from '@angular/core';
import { platformBrowser } from '@angular/platform-browser';
import { AppConfigService } from 'app/services/app-config.service';

import { createAppModule } from './app/app.module';

Promise.all([
  fetch('/assets/app.config.json').then((response) => response.json()),
  fetch('/assets/config/keycloak.config.json').then((response) =>
    response.json(),
  ),
]).then(([appConfig, keycloakConfig]) => {
  console.log('Is production?', appConfig.production);
  if (appConfig.production) {
    enableProdMode();
  }

  platformBrowser([
    { provide: AppConfigService, useValue: appConfig },
  ])
    .bootstrapModule(createAppModule(keycloakConfig.config), {
      applicationProviders: [provideZoneChangeDetection()],
    })
    .catch((err) => console.error(err));
});

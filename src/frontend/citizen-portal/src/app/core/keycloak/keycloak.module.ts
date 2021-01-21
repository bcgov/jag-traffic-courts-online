import { NgModule, APP_INITIALIZER, Injector } from '@angular/core';
import { Router } from '@angular/router';

import {
  KeycloakAngularModule,
  KeycloakService,
  KeycloakOptions,
} from 'keycloak-angular';

import { environment } from '@env/environment';
import { ToastService } from '@core/services/toast.service';
import { MockKeycloakService } from 'tests/mocks/mock-keycloak.service';

function initializer(
  keycloak: KeycloakService,
  injector: Injector
): () => Promise<void> {
  const routeToDefault = () => injector.get(Router).navigateByUrl('/home');

  return async (): Promise<void> => {
    const authenticated = await keycloak.init(
      environment.keycloakConfig as KeycloakOptions
    );

    keycloak.getKeycloakInstance().onTokenExpired = () => {
      keycloak.updateToken().catch(() => {
        injector
          .get(ToastService)
          .openErrorToast(
            'Your session has expired, you will need to re-authenticate'
          );
        routeToDefault();
      });
    };

    if (authenticated) {
      // Force refresh to begin expiry timer.
      keycloak.updateToken(-1);
    }
  };
}

@NgModule({
  imports: [KeycloakAngularModule],
  providers: [
    {
      provide: KeycloakService,
      useClass: environment.useKeycloak ? KeycloakService : MockKeycloakService,
    },
    {
      provide: APP_INITIALIZER,
      useFactory: initializer,
      multi: true,
      deps: [KeycloakService, Injector],
    },
  ],
})
export class KeycloakModule {}

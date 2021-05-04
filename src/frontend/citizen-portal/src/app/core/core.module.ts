import {
  APP_INITIALIZER,
  ErrorHandler,
  Injector,
  NgModule,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { KeycloakModule } from './keycloak/keycloak.module';
import { ErrorHandlerService } from './services/error-handler.service';
import { ErrorHandlerInterceptor } from './interceptors/error-handler.interceptor';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ConfigService } from '@config/config.service';

export function initConfig(config: ConfigService) {
  return async () => await config.load().toPromise();
}

@NgModule({
  declarations: [],
  imports: [CommonModule, KeycloakModule],
  providers: [
    {
      provide: ErrorHandler,
      useClass: ErrorHandlerService,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorHandlerInterceptor,
      multi: true,
    },
    {
      provide: APP_INITIALIZER,
      useFactory: initConfig,
      deps: [ConfigService],
      multi: true,
    },
  ],
})
export class CoreModule {}

import { ErrorHandler, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { KeycloakModule } from './keycloak/keycloak.module';
import { ErrorHandlerService } from './services/error-handler.service';
import { ErrorHandlerInterceptor } from './interceptors/error-handler.interceptor';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

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
  ],
})
export class CoreModule {}

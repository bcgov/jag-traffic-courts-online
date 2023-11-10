import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, ErrorHandler, NgModule } from '@angular/core';
import { ConfigService } from '@config/config.service';
import { ErrorHandlerInterceptor } from './interceptors/error-handler.interceptor';
import { ErrorHandlerService } from './services/error-handler.service';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { LoadingStore } from './store';
import { LoadingInterceptor } from './interceptors/loading.interceptor';

export function initConfig(config: ConfigService) {
  return () => {
    return config.load();
  };
}

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    StoreModule.forFeature(LoadingStore.StoreName, LoadingStore.Reducer),
    EffectsModule.forFeature([LoadingStore.Effects])
  ],
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
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true
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

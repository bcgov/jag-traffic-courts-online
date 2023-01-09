import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpInterceptor, HttpResponse, HttpEvent } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AuthConfigService } from '../services/auth-config.service';
import { Store } from '@ngrx/store';
import { AuthStore } from '../store';
import { AppConfigService } from 'app/services/app-config.service';

@Injectable({ providedIn: "root" })
export class AuthHttpInterceptor implements HttpInterceptor {
  private token: string;

  constructor(
    private authConfigService: AuthConfigService,
    private appConfigService: AppConfigService,
    private store: Store
  ) {
    this.store.select(AuthStore.Selectors.AccessToken).subscribe(token => {
      if (token) {
        this.token = token;
      }
    })
  }

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    if (request.url === this.authConfigService.config?.authority + "/.well-known/openid-configuration") {
      return this.handleWellKnownRequest(request.method);
    }
    if (this.token && request.url.startsWith(this.appConfigService.apiBaseUrl)) {
      request = request.clone({
        headers: request.headers.set("Authorization", "Bearer " + this.token)
      })
    }
    return next.handle(request)
  }

  private handleWellKnownRequest(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    switch (requestMethod) {
      case "GET":
        return of(new HttpResponse<unknown>({
          status: 200,
          body: this.authConfigService.authWellKnownDocument
        }))
    }
  }
}
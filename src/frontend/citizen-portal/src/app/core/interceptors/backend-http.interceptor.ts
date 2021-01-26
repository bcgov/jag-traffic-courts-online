import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse
} from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '@env/environment';
import { RouteUtils } from '@core/utils/route-utils.class';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { LoggerService } from '@core/services/logger.service';

@Injectable()
export class BackendHttpInterceptor implements HttpInterceptor {

  constructor(private mockDisputeService: MockDisputeService, private logger: LoggerService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.logger.info('useMockServices', environment.useMockServices);
    this.logger.info('request.method', request.method);

    if (environment.useMockServices) {
      if (request.method === "GET") {
        const currentRoutePath = RouteUtils.currentRoutePath(request.url);
        this.logger.info('currentRoutePath', currentRoutePath);

        if (currentRoutePath === "ticket") {
          const result = this.mockDisputeService.ticket;
          return of(new HttpResponse({ status: 200, body: { result } }));
        }
      }
    }

    return next.handle(request);
  }
}

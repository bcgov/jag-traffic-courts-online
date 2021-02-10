import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '@env/environment';
import { RouteUtils } from '@core/utils/route-utils.class';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { LoggerService } from '@core/services/logger.service';

@Injectable()
export class BackendHttpInterceptor implements HttpInterceptor {
  constructor(
    private mockDisputeService: MockDisputeService,
    private logger: LoggerService
  ) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    if (environment.useMockServices) {
      const ticket = this.mockDisputeService.ticket;
      const currentRoutePath = RouteUtils.currentRoutePath(request.url);

      this.logger.info(
        'BackendHttpInterceptor',
        request.method,
        currentRoutePath
      );

      if (currentRoutePath !== 'ticket')
      {
        throw new HttpErrorResponse({
        error: 'Mock Bad Request',
        status: 400,
        });
      }

      switch (request.method) {
        case 'GET':
        case 'PUT':
        case 'POST':
          return of(
            new HttpResponse({ status: 200, body: { result: ticket } })
          );
        break;
        default:
          throw new HttpErrorResponse({
            error: 'Mock Bad Request',
            status: 400,
          });
      }
    }
    return next.handle(request);
  }
}

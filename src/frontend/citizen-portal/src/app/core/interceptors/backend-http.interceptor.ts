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
import { MockConfig } from 'tests/mocks/mock-config';

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
      const currentRoutePath = RouteUtils.currentRoutePath(request.url);

      if (
        currentRoutePath !== 'ticket' &&
        currentRoutePath !== 'tickets' &&
        currentRoutePath !== 'lookups'
      ) {
        throw new HttpErrorResponse({
          error: 'Mock Bad Request',
          status: 400,
        });
      }

      // Handle 'ticket' requests
      if (currentRoutePath === 'ticket') {
        const ticket = this.mockDisputeService.ticket;

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

        // Handle 'tickets' requests
      } else if (currentRoutePath === 'tickets') {
        const tickets = this.mockDisputeService.tickets;

        switch (request.method) {
          case 'GET':
          case 'PUT':
          case 'POST':
            return of(
              new HttpResponse({ status: 200, body: { result: tickets } })
            );
            break;
          default:
            throw new HttpErrorResponse({
              error: 'Mock Bad Request',
              status: 400,
            });
        }

        // Handle 'lookups' requests
      } else if (currentRoutePath === 'lookups') {
        switch (request.method) {
          case 'GET':
            return of(
              new HttpResponse({
                status: 200,
                body: { result: MockConfig.get() },
              })
            );
            break;
          default:
            throw new HttpErrorResponse({
              error: 'Mock Bad Request',
              status: 400,
            });
        }
      }
    }
    return next.handle(request);
  }
}

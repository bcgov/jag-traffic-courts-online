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
        currentRoutePath !== 'tickets' &&
        currentRoutePath !== 'ticket' &&
        currentRoutePath !== 'alltickets' &&
        currentRoutePath !== 'dispute' &&
        currentRoutePath !== 'alldisputes' &&
        currentRoutePath !== 'lookups'
      ) {
        throw new HttpErrorResponse({
          error: 'Mock Bad Request',
          status: 400,
        });
      }

      // Handle 'ticket' requests
      if (currentRoutePath === 'ticket') {
        return this.handleTicketRequests(request.method);

        // Handle 'tickets' requests
      } else if (currentRoutePath === 'alltickets') {
        return this.handleTicketsRequests(request.method);

        // Handle 'dispute' requests
      } else if (currentRoutePath === 'dispute') {
        return this.handleDisputeRequests(request.method);

        // Handle 'disputes' requests
      } else if (currentRoutePath === 'alldisputes') {
        return this.handleDisputesRequests(request.method);

        // Handle 'lookups' requests
      } else if (currentRoutePath === 'lookups') {
        return this.handleLookupsRequests(request.method);
      }
    }
    return next.handle(request);
  }

  private handleTicketRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    const ticket = this.mockDisputeService.ticket;

    switch (requestMethod) {
      case 'GET':
      case 'PUT':
      case 'POST':
        return of(new HttpResponse({ status: 200, body: { result: ticket } }));
        break;
      default:
        throw new HttpErrorResponse({
          error: 'Mock Bad Request',
          status: 400,
        });
    }
  }

  private handleTicketsRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    const tickets = this.mockDisputeService.tickets;

    switch (requestMethod) {
      case 'GET':
        return of(new HttpResponse({ status: 200, body: { result: tickets } }));
        break;
      default:
        throw new HttpErrorResponse({
          error: 'Mock Bad Request',
          status: 400,
        });
    }
  }

  private handleDisputeRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    const dispute = this.mockDisputeService.dispute;
    const rsiTicket = this.mockDisputeService.rsiTicket;

    switch (requestMethod) {
      case 'GET':
        return of(
          new HttpResponse({
            status: 200,
            body: { result: dispute },
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

  private handleDisputesRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    const disputes = this.mockDisputeService.disputes;

    switch (requestMethod) {
      case 'GET':
        return of(
          new HttpResponse({ status: 200, body: { result: disputes } })
        );
        break;
      default:
        throw new HttpErrorResponse({
          error: 'Mock Bad Request',
          status: 400,
        });
    }
  }

  private handleLookupsRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    switch (requestMethod) {
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

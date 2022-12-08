import {
  HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { RouteUtils } from '@core/utils/route-utils.class';
import { AppConfigService } from 'app/services/app-config.service';
import { Observable, of } from 'rxjs';
import { MockConfig } from 'tests/mocks/mock-config';
import { MockNoticeOfDisputeService } from 'tests/mocks/mock-notice-of-dispute.service';

@Injectable()
export class BackendHttpInterceptor implements HttpInterceptor {
  constructor(
    private mockNoticeOfDisputeService: MockNoticeOfDisputeService,
    private appConfigService: AppConfigService,
    private logger: LoggerService
  ) { }

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    const currentRoutePath = RouteUtils.currentRoutePath(
      request.url
    ).toLowerCase();

    // handle translations
    if (currentRoutePath.includes('json')) {
      return next.handle(request);
    } 

    if (this.appConfigService.useMockServices) {
      if (
        !currentRoutePath.includes('ticket') &&
        !currentRoutePath.includes('dispute') &&
        !currentRoutePath.includes('pay') &&
        !currentRoutePath.includes('lookup') &&
        !currentRoutePath.includes('find') &&
        !currentRoutePath.includes('retrieve')
      ) {
        throw new HttpErrorResponse({
          error: 'Mock Bad Request',
          status: 400,
        });
      }

      // Handle 'ticket' requests
      if (currentRoutePath.includes('ticket')) {
        return this.handleTicketsRequests(request.method);
      } else if (currentRoutePath.includes('pay')) {
        return this.handlePayRequests(request.method);

        // Handle 'dispute' requests
      } else if (currentRoutePath.includes('dispute')) {
        return this.handleDisputesRequests(request.method);
      } 
    }
    return next.handle(request);
  }

  private handleTicketsRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    const ticket = this.mockNoticeOfDisputeService.ticket;

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

  private handlePayRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    switch (requestMethod) {
      case 'GET':
      case 'PUT':
      case 'POST':
        return of(
          new HttpResponse({
            status: 200,
            body: {
              result: {
                violationTicketNumber: 'EZ02000460',
                violationTime: '09:54',
                counts: '1,2,3',
                callbackUrl:
                  'http://localhost:5000/id=7af161b6-bd97-4ee9-b271-3d14b9d198bf',
                redirectUrl: null,
              },
            },
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
    switch (requestMethod) {
      case 'POST':
        return of(new HttpResponse({ status: 200, body: {} }));
        break;
      case 'GET':
      case 'PUT':
      default:
        throw new HttpErrorResponse({
          error: 'Mock Bad Request',
          status: 400,
        });
    }
  }
}

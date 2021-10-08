import {
  HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { RouteUtils } from '@core/utils/route-utils.class';
import { AppConfigService } from 'app/services/app-config.service';
import { Observable, of } from 'rxjs';
import { MockConfig } from 'tests/mocks/mock-config';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

@Injectable()
export class BackendHttpInterceptor implements HttpInterceptor {
  constructor(
    private mockDisputeService: MockDisputeService,
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

      // TODO: remove later
      // for now, lookups always use mock
    } else if (currentRoutePath === 'lookup') {
      return this.handleLookupsRequests(request.method);
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

        // Handle 'addressAutocomplete' requests
      } else if (currentRoutePath.includes('find')) {
        return this.handleAddressAutocompleteFindRequests(request.method);

        // Handle 'addressAutocomplete' requests
      } else if (currentRoutePath.includes('retrieve')) {
        return this.handleAddressAutocompleteRetrieveRequests(request.method);

        // Handle 'lookup' requests
      } else if (currentRoutePath === 'lookup') {
        return this.handleLookupsRequests(request.method);
      }
    }
    return next.handle(request);
  }

  private handleTicketsRequests(
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

  private handleAddressAutocompleteFindRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    switch (requestMethod) {
      case 'GET':
        return of(
          new HttpResponse({
            status: 200,
            body: this.mockDisputeService.addressAutocompleteFindResponse,
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

  private handleAddressAutocompleteRetrieveRequests(
    requestMethod: string
  ): Observable<HttpEvent<unknown>> {
    switch (requestMethod) {
      case 'GET':
        return of(
          new HttpResponse({
            status: 200,
            body: this.mockDisputeService.addressAutocompleteRetrieveResponse,
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

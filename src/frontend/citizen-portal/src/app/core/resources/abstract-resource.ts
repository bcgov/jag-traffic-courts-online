import {
  HttpErrorResponse,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';

import { Observable, throwError, pipe, of, ObservableInput } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiHttpErrorResponse } from '@core/models/api-http-error-response.model';
import { LoggerService } from '@core/services/logger.service';

// Type for NoContent responses from the API
export type NoContent = Observable<void>;
export const NoContentResponse = pipe(map(() => void 0));

export abstract class AbstractResource {
  constructor(protected logger: LoggerService) {}

  public abstract get(
    path: string,
    params: HttpParams,
    options: { [key: string]: any }
  ): Observable<ApiHttpResponse<any> | ApiHttpErrorResponse>;

  public abstract post(
    path: string,
    body: { [key: string]: any },
    params: HttpParams,
    options: { [key: string]: any }
  ): Observable<ApiHttpResponse<any> | ApiHttpErrorResponse>;

  public abstract put(
    path: string,
    body: { [key: string]: any },
    params: HttpParams,
    options: { [key: string]: any }
  ): Observable<ApiHttpResponse<any> | ApiHttpErrorResponse>;

  public abstract delete(
    path: string,
    params: HttpParams,
    options: { [key: string]: any }
  ): Observable<ApiHttpResponse<any> | ApiHttpErrorResponse>;

  /**
   * @description
   * Handle the HTTP response.
   */
  protected handleResponse<T>() {
    return pipe(
      map(this.handleSuccess<T>()),
      catchError(
        (error: any): Observable<any> => {
          return this.handleError(error);
        }
      )
    );
  }

  /**
   * @description
   * Handle NoContent HTTP response.
   */
  protected handleNoContent() {
    return (
      _: HttpResponse<ApiHttpResponse<NoContent>>
    ): Observable<NoContent> => of(void 0);
  }

  /**
   * @description
   * Handle a successful HTTP response.
   */
  protected handleSuccess<T>(): (
    response: HttpResponse<ApiHttpResponse<T>>
  ) => ApiHttpResponse<T> {
    return ({ body }: HttpResponse<ApiHttpResponse<T>>): ApiHttpResponse<T> => {
      // this.logger.info(`RESPONSE: ${status}`, body);
      return body;
    };
  }

  /**
   * @description
   * Handle a erroneous HTTP response.
   */
  protected handleError({
    error,
    status,
  }: HttpErrorResponse): Observable<ApiHttpErrorResponse> {
    this.logger.error('An error occurred:', error);

    return throwError(new ApiHttpErrorResponse(error, status));
  }
}

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '@env/environment';

import { AbstractResource } from '@core/resources/abstract-resource';
import { LoggerService } from '@core/services/logger.service';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiHttpErrorResponse } from '@core/models/api-http-error-response.model';

@Injectable({
  providedIn: 'root',
})
export class ApiResource extends AbstractResource {
  constructor(protected logger: LoggerService, private http: HttpClient) {
    super(logger);
  }

  public get<T>(
    path: string,
    params: HttpParams = new HttpParams(),
    options: { [key: string]: any } = {}
  ): Observable<ApiHttpResponse<T> | ApiHttpErrorResponse> {
    return this.http
      .get(`${environment.apiUrl}/${path}`, {
        params,
        observe: 'response',
        ...options,
      })
      .pipe(this.handleResponse<T>());
  }

  public post<T>(
    path: string,
    body: any = {},
    params: HttpParams = new HttpParams(),
    options: { [key: string]: any } = {}
  ): Observable<ApiHttpResponse<T> | ApiHttpErrorResponse> {
    return this.http
      .post(`${environment.apiUrl}/${path}`, body, {
        params,
        observe: 'response',
        ...options,
      })
      .pipe(this.handleResponse<T>());
  }

  public put<T>(
    path: string,
    body: any = {},
    params: HttpParams = new HttpParams(),
    options: { [key: string]: any } = {}
  ): Observable<ApiHttpResponse<T> | ApiHttpErrorResponse> {
    return this.http
      .put(`${environment.apiUrl}/${path}`, body, {
        params,
        observe: 'response',
        ...options,
      })
      .pipe(this.handleResponse<T>());
  }

  public patch<T>(
    path: string,
    body: any = {},
    params: HttpParams = new HttpParams(),
    options: { [key: string]: any } = {}
  ): Observable<ApiHttpResponse<T> | ApiHttpErrorResponse> {
    return this.http
      .patch(`${environment.apiUrl}/${path}`, body, {
        params,
        observe: 'response',
        ...options,
      })
      .pipe(this.handleResponse<T>());
  }

  public delete<T>(
    path: string,
    params: HttpParams = new HttpParams(),
    options: { [key: string]: any } = {}
  ): Observable<ApiHttpResponse<T> | ApiHttpErrorResponse> {
    return this.http
      .delete(`${environment.apiUrl}/${path}`, {
        params,
        observe: 'response',
        ...options,
      })
      .pipe(this.handleResponse<T>());
  }
}

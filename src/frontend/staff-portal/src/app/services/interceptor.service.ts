import { Injectable } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http';
import { HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHandler } from '@angular/common/http';
import { HttpEvent } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class InterceptorService implements HttpInterceptor {
  constructor() { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    // response headers to add
    // 'Access-Control-Allow-Origin': '*',
    // 'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE,OPTIONS',
    // 'Access-Control-Allow-Headers': ' Origin, Content-Type, Authorization, Content-Length, X-Requested-With',

    const corsReq = req.clone({ headers: req.headers.set('withCredentials', 'true') });
    return next.handle(corsReq);
  }
}
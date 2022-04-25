import { Injectable } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http';
import { HttpRequest, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHandler } from '@angular/common/http';
import { HttpEvent } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class InterceptorService implements HttpInterceptor {
  constructor() { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    // const corsReq = req.clone({
    //   withCredentials: true
    // });

    // add header for OIDC Content-Security-Policy: script-src 'self' 'unsafe-inline';style-src 'self' 'unsafe-inline';img-src 'self' data:;font-src 'self';frame-ancestors 'self' https://localhost:44318;block-all-mixed-content
    const OIDCReq = req.clone({
      headers: req.headers.set(
        'Content-Security-Policy', "script-src 'self' 'unsafe-inline';style-src 'self' 'unsafe-inline';img-src 'self' data:;font-src 'self';frame-ancestors 'self' https://dev.oidc.gov.bc.ca/auth/realms/ezb8kej4;block-all-mixed-content"),
    });

    // add header to pass preflight check
    const preFlightReq = OIDCReq.clone({
      headers: OIDCReq.headers.append('Access-Control-Allow-Origin', 'http://localhost:4200')
    });

    return next.handle(preFlightReq);
  }
}
import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class TimezoneInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    const clonedRequest = req.clone({
      setHeaders: {
        'X-Timezone': timeZone
      }
    });
    return next.handle(clonedRequest);
  }
}

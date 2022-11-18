import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AuthService } from "app/services/auth.service";
import { Observable } from "rxjs";

@Injectable() export class AppHttpInterceptor implements HttpInterceptor {
  constructor(public authService: AuthService) {

  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    //Add the JUSTIN part id to the request header

    if (this.authService.userProfile?.attributes['partid'] && this.authService.userProfile?.attributes['partid'].length > 0 ) {
      const token: string = this.authService?.userProfile?.attributes['partid'][0];
      if (token) {
        req = req.clone({ headers: req.headers.set('partid', token) });
      }
    }

    return next.handle(req);   //invoke the next handler
  }
}

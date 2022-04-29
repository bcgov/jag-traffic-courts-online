import { Injectable } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http';
import { HttpRequest, HttpHeaders, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OidcSecurityService } from 'angular-auth-oidc-client';
@Injectable({
  providedIn: 'root'
})
export class InterceptorService implements HttpInterceptor {
  accessToken: string = "";
  constructor(public oidcSecurityService: OidcSecurityService
    ) {   }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    const corsReq = req.clone({
      withCredentials: true
    });

    // const token = this.oidcSecurityService.getAccessToken().subscribe((token) => {
      let token = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJqT1YtdTZ1OEZzYmtMYldwOGoyU1ZLNUQ3Uko1MjN0bHJabVVaTnpTMXVFIn0.eyJleHAiOjE2NTEyOTQwNTYsImlhdCI6MTY1MTI1ODA1NywiYXV0aF90aW1lIjoxNjUxMjU4MDU2LCJqdGkiOiJiNzYyZDg4NC1mMzNjLTQ0OGUtYWNkZC00MjU1NmQwZGU3NGEiLCJpc3MiOiJodHRwczovL2Rldi5vaWRjLmdvdi5iYy5jYS9hdXRoL3JlYWxtcy9lemI4a2VqNCIsImF1ZCI6WyJ0Y28tc3RhZmYtcG9ydGFsIiwiYWNjb3VudCJdLCJzdWIiOiI4OTg0NzU3ZC1lZGE3LTQ1Y2QtYTEyMy1jZDhkMzI2YTA1OTEiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJ0Y28tc3RhZmYtcG9ydGFsIiwibm9uY2UiOiI0MmZkNmU1YjBhYWZkZjBmMjY3MzlkZDYxMjE0OWMwOTdiY2tWNkN6ZSIsInNlc3Npb25fc3RhdGUiOiI3ZmE0ZTYwZS0xYWEzLTRlNGYtYmMxNC0xMzNkNTY1OWIxOTkiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbIioiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJ0Y28tc3RhZmYtcG9ydGFsIjp7InJvbGVzIjpbInZ0Yy11c2VyIl19LCJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im9wZW5pZCB2dGMtdXNlci1zY29wZSBlbWFpbCBwcm9maWxlIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJuYW1lIjoiTG9ycmFpbmUgRGFtZSIsInByZWZlcnJlZF91c2VybmFtZSI6ImxkYW1lQGlkaXIiLCJnaXZlbl9uYW1lIjoiTG9ycmFpbmUiLCJmYW1pbHlfbmFtZSI6IkRhbWUiLCJlbWFpbCI6ImxvcnJhaW5lLmRhbWVAZ292LmJjLmNhIn0.CrEwKFleQHpBnB1zWDUUKn_rVjqamSGoekw485QuXPlIsvqs8LX45pr7pjXlS8LagbVqVLgY4TdfzHYWNaHMkUKdJEVATHMUrwz-JtAOQ_XXf-wC3Ub0ct_UT8M4R6Lp6FOpv4y-rhc1cAjY2aLVZ9NGUfOeD5DFeuE0pH9Xmuo093WyW4QZ1f8_tCblZwYWy1ECKjZwqMYA0-D2Vj4oWHFYhRXYdVayPYZZxTBFb80OiD_eLFBmYIGDvFE4jAPKiZvqJSIyVojpu-BbWZxUIo1dRNQlNlO3M2UWzCNctRgETR_WgII0SOc9NvPjtDhCLo1elzq7BJBUus5GFuT0fw";
      const authReq = corsReq.clone({headers: corsReq.headers.set("Authorization", "Bearer " + token)});
      return next.handle(authReq);
    // });
  }
}
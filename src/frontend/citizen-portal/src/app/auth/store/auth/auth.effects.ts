import { Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { switchMap } from 'rxjs/operators';
import { Actions } from '.';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Injectable()
export class AuthEffects {
  constructor(
    private actions$: StoreActions,
    private router: Router,
    private oidcSecurityService: OidcSecurityService,
  ) { }

  Authorize$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Authorize),
    switchMap(() => {
      localStorage.setItem("url", this.router.url);
      this.oidcSecurityService.authorize();
      return of(Actions.Authorizing());
    }))
  );

  Authorized$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Authorized),
    switchMap((action) => {
      if (action.payload.isAuthenticated) {
        let url = localStorage.getItem("url");
        if (url) {
          this.router.navigateByUrl(url);
          localStorage.removeItem("url");
        }
        return of(Actions.Redirect());
      }
      return of();
    }))
  );
}
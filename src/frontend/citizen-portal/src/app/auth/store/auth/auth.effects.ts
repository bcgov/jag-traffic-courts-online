import { Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { switchMap } from 'rxjs/operators';
import { Actions } from '.';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { UserService } from '../../../services/user.service';

@Injectable()
export class AuthEffects {

  constructor(
    private actions$: StoreActions,
    private router: Router,
    private oidcSecurityService: OidcSecurityService,
    private userService: UserService
  ) { }

  Authorize$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Authorize),
    switchMap(action => {
      localStorage.setItem("url", action.redirectUrl);
      this.oidcSecurityService.authorize();
      return of(Actions.Authorizing());
    }))
  );

  Authorized$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Authorized),
    switchMap((action) => {
      if (action.payload.isAuthenticated) {
        // cache identity information server side
        this.userService.getUser().subscribe((response: any) => {});

        // redirect
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

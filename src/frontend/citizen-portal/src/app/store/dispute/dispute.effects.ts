import { Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, switchMap } from 'rxjs/operators';
import { Actions } from '.';
import { DisputeService } from 'app/services/dispute.service';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { AppRoutes } from 'app/app.routes';

@Injectable()
export class DisputeEffects {
  constructor(
    private actions$: StoreActions,
    private router: Router,
    private disputeService: DisputeService,
  ) { }

  getDispute$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Search),
    switchMap(action => {
      let params = action.params;
      if (!params || !params?.ticketNumber || !params?.time) {
        return of(Actions.SearchFailed());
      }
      return this.disputeService.searchDispute(params)
        .pipe(
          map(result => {
            if (result.identifier) {
              let path = AppRoutes.disputePath("");
              let findPagePath = AppRoutes.disputePath(AppRoutes.FIND_DISPUTE);
              if (this.router.url.indexOf(path) !== 0 || this.router.url.indexOf(findPagePath) === 0) {
                this.router.navigate([AppRoutes.disputePath(AppRoutes.UPDATE_DISPUTE)], {
                  queryParams: params,
                });
              }
              return Actions.SearchSuccess({ payload: { result, params } });
            } else {
              this.disputeService.openDisputeNotFoundDialog();
              this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND_DISPUTE)]);
              return Actions.SearchFailed();
            }
          }),
          catchError(err => {
            this.disputeService.openDisputeNotFoundDialog();
            this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND_DISPUTE)]);
            return of(Actions.SearchFailed());
          })
        )
    }))
  );
}
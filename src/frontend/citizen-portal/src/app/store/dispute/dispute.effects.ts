import { Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, switchMap } from 'rxjs/operators';
import { Actions } from '.';
import { DisputeService } from 'app/services/dispute.service';
import { MatDialog } from '@angular/material/dialog';
import { DisputeNotFoundDialogComponent } from '@shared/dialogs/dispute-not-found-dialog/dispute-not-found-dialog.component';
import { of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AppRoutes } from 'app/app.routes';
import { QueryParamsForSearch } from '@shared/models/query-params-for-search.model';

@Injectable()
export class DisputeEffects {
  constructor(
    private actions$: StoreActions,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private router: Router,
    private disputeService: DisputeService,
  ) { }

  getDispute$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Search),
    switchMap(action => {
      let params = action.params;
      if (!params) {
        let queryParams: QueryParamsForSearch = <QueryParamsForSearch>this.route.snapshot.queryParams;
        if (!queryParams.ticketNumber || !queryParams.time) {
          this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND_DISPUTE)]);
          return of(Actions.SearchFailed());
        } else {
          params = queryParams;
        }
      }
      return this.disputeService.searchDispute(params)
        .pipe(
          map(dispute => {
            if (dispute.dispute_id) {
              this.router.navigate([AppRoutes.disputePath(AppRoutes.UPDATE_DISPUTE)], {
                queryParams: params,
              });
              return Actions.SearchSuccess({ dispute, params });
            } else {
              this.openDisputeNotFoundDialog();
              this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND_DISPUTE)]);
              return Actions.SearchFailed();
            }
          }),
          catchError(err => {
            this.openDisputeNotFoundDialog();
            this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND_DISPUTE)]);
            return of(Actions.SearchFailed());
          })
        )
    }))
  );

  private openDisputeNotFoundDialog() {
    return this.dialog.open(DisputeNotFoundDialogComponent, { width: '600px' });
  }
}
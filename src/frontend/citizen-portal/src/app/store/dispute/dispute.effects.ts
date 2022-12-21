import { Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, switchMap, withLatestFrom } from 'rxjs/operators';
import { Actions } from '.';
import { DisputeService } from 'app/services/dispute.service';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { AppRoutes } from 'app/app.routes';
import { Store } from '@ngrx/store';
import { DisputeStore } from '..';
import { DisputeFormMode } from '@shared/enums/dispute-form-mode';

@Injectable()
export class DisputeEffects {
  constructor(
    private actions$: StoreActions,
    private router: Router,
    private disputeService: DisputeService,
    private store: Store
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
                this.disputeService.goToUpdateDisputeLanding(params);
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

  updateContact$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.UpdateContact),
    withLatestFrom(this.store.select(DisputeStore.Selectors.Params)),
    mergeMap(([action, params]) => {
      return this.disputeService.updateDisputeContact(action.uuid, action.payload)
        .pipe(
          map(result => {
            if (action.payload.email_address) {
              this.router.navigate([AppRoutes.EMAILVERIFICATIONREQUIRED], {
                queryParams: {
                  email: action.payload.email_address,
                  token: action.uuid
                },
              });
            }
            else {
              this.router.navigate([AppRoutes.ticketPath(AppRoutes.UPDATE_DISPUTE)], {
                queryParams: {
                  mode: DisputeFormMode.UPDATEDISPUTANT
                },
              });
            }
            return Actions.UpdateContactSuccess();
          }),
          catchError(err => {
            return of(Actions.UpdateContactFailed());
          })
        )
    }))
  );
}
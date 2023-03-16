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
import { DocumentService } from 'app/api';
import { HttpResponse } from '@angular/common/http';

@Injectable()
export class DisputeEffects {
  constructor(
    private actions$: StoreActions,
    private router: Router,
    private disputeService: DisputeService,
    private documentService: DocumentService,
    private store: Store
  ) { }

  searchDispute$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Search),
    switchMap(action => {
      let params = action.params;
      if (!params || !params?.ticketNumber || !params?.time) {
        return of(Actions.SearchFailed());
      }
      return this.disputeService.searchDispute(params)
        .pipe(
          map(result => {
            if (result.token) {
              let path = AppRoutes.disputePath("");
              let findPagePath = AppRoutes.disputePath(AppRoutes.FIND_DISPUTE);
              if (this.router.url.indexOf(path) !== 0 || this.router.url.indexOf(findPagePath) === 0) {
                this.disputeService.goToUpdateDisputeLanding(params);
              }
              return Actions.SearchSuccess({ result });
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

  getDispute$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Get),
    withLatestFrom(this.store.select(DisputeStore.Selectors.Result), this.store.select(DisputeStore.Selectors.Params)),
    switchMap(([action, searchResult, params]) => {
      if (!searchResult || !searchResult?.token) {
        return of(Actions.GetFailed());
      }
      return this.disputeService.getDispute(searchResult.token)
        .pipe(
          map(noticeOfDispute => {
            if (noticeOfDispute.ticket_number) {
              return Actions.GetSuccess({ noticeOfDispute });
            } else {
              this.disputeService.openDisputantNotMatchDialog();
              this.disputeService.goToUpdateDisputeLanding(params);
              return Actions.GetFailed();
            }
          }),
          catchError(err => {
            this.disputeService.openDisputantNotMatchDialog();
            this.disputeService.goToUpdateDisputeLanding(params);
            return of(Actions.GetFailed());
          })
        )
    }))
  );

  updateContact$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.UpdateContact),
    withLatestFrom(this.store.select(DisputeStore.Selectors.Result)),
    mergeMap(([action, searchResult]) => {
      return this.disputeService.updateDisputeContact(searchResult.token, action.payload)
        .pipe(
          map(result => {
            if (action.payload.email_address) {
              this.router.navigate([AppRoutes.EMAILVERIFICATIONREQUIRED], {
                queryParams: {
                  email: action.payload.email_address,
                  token: searchResult.token
                },
              });
            }
            else {
              this.router.navigate([AppRoutes.ticketPath(AppRoutes.UPDATE_DISPUTE_LANDING)], {
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

  update$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Update),
    withLatestFrom(this.store.select(DisputeStore.Selectors.Result)),
    mergeMap(([action, searchResult]) => {
      return this.disputeService.updateDispute(searchResult.token, action.payload)
        .pipe(
          map(result => {
            if (action.payload.email_address) {
              this.router.navigate([AppRoutes.EMAILVERIFICATIONREQUIRED], {
                queryParams: {
                  email: action.payload.email_address,
                  token: searchResult.token
                },
              });
            }
            else {
              this.router.navigate([AppRoutes.ticketPath(AppRoutes.UPDATE_DISPUTE_LANDING)], {
                queryParams: {
                  mode: DisputeFormMode.UPDATEDISPUTANT
                },
              });
            }
            return Actions.UpdateSuccess();
          }),
          catchError(err => {
            return of(Actions.UpdateFailed());
          })
        )
    }))
  );

  getDocument$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.GetDocument),
    mergeMap(action => {
      return this.documentService.apiDocumentGetGet(action.fileId)
        .pipe(
          map((result: HttpResponse<Blob>) => {
            var url = URL.createObjectURL(result.body);
            window.open(url);
            return Actions.GetDocumentSuccess();
          }),
          catchError(err => {
            return of(Actions.GetDocumentFailed());
          })
        )
    }))
  );
}
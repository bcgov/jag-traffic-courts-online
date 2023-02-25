import { Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, mergeMap, switchMap } from 'rxjs/operators';
import { AppState } from '../';
import { Actions } from './';
import { JJDisputeService } from 'app/services/jj-dispute.service';

@Injectable()
export class JJDisputeEffects {
  constructor(
    private actions$: StoreActions,
    private store: Store<AppState>,
    private jjDisputeService: JJDisputeService
  ) { }

  getJJDisputes$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Get),
    switchMap(() => this.jjDisputeService.getJJDisputes()
      .pipe(
        map(data => {
          return Actions.GetSuccess({ data })
        }),
      )))
  );

  assignJJDisputes$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Assign),
    mergeMap(action => this.jjDisputeService.apiJjAssignPut(action.disputeIds, action.username)
      .pipe(
        map(() => {
          this.store.dispatch(Actions.Get());
          return Actions.AssignSuccess();
        }),
      )))
  );
}

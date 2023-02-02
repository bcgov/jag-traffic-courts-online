import { Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { switchMap, withLatestFrom } from 'rxjs/operators';
import { Actions } from '.';
import { of } from 'rxjs';
import { Store } from '@ngrx/store';
import { LoadingStore } from '..';

@Injectable()
export class LoadingEffects {
  constructor(
    private actions$: StoreActions,
    private store: Store,
  ) { }

  removeLoadingitem$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Remove),
    withLatestFrom(this.store.select(LoadingStore.Selectors.NumberOfLoadingItems)),
    switchMap(([action, numberOfLoadingItems]) => {
      if (numberOfLoadingItems === 0) {
        return of(Actions.LoadingDone());
      }
      return of();
    }))
  );
}
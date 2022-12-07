import { Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, exhaustMap, withLatestFrom, mergeMap } from 'rxjs/operators';
import { AppState } from '../';
import { Actions } from './';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { Observable } from 'rxjs';

@Injectable()
export class JJDisputeEffects {
  constructor(
    private actions$: StoreActions,
    private store: Store<AppState>,
    private jjDisputeService: JJDisputeService
  ) { }

  getJJDisputes$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Get),
    exhaustMap(() => this.jjDisputeService.getJJDisputes()
      .pipe(
        map(data => {
          return Actions.GetSuccess({ data })
        })
      ))
  ));

  // assignJJDisputes$ = createEffect(() => this.actions$.pipe(
  //   ofType(Actions.Assign),
  //   withLatestFrom(this.store.select(state => state.jjDispute.data)),
  //   mergeMap(([action, storedData]) =>
  //     this.jjDisputeService.apiJjAssignPut(action.ticketNumbers, action.username)
  //       .pipe(
  //         map(() => {
  //           let updatedData = storedData.filter(i => action.ticketNumbers.indexOf(i.ticketNumber) > -1)
  //             .map(i => { 
  //               let jjAssignedToName = this.jjDisputeService.getJJAssignedToName(i);
  //               return { ...i, jjAssignedTo: action.username, jjAssignedToName } 
  //             });
  //           return Actions.AssignSuccess({ updatedData });
  //         })
  //       ))
  // ));

  assignJJDisputes$ = createEffect(() => this.actions$.pipe(
    ofType(Actions.Assign),
    withLatestFrom(this.store.select(state => state.jjDispute.data)),
    mergeMap(([action, storedData]) =>
      new Observable((subscriber) => {
        subscriber.next(1)
      }).pipe(
        map(() => {
          let updatedData = storedData.filter(i => action.ticketNumbers.indexOf(i.ticketNumber) > -1)
            .map(i => {
              let jjAssignedToName = this.jjDisputeService.getJJAssignedToName(i);
              return { ...i, jjAssignedTo: action.username, jjAssignedToName }
            });
          return Actions.AssignSuccess({ updatedData });
        })
      ))
  ));
}
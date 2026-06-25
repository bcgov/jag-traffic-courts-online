import { inject, Injectable } from '@angular/core';
import { Actions as StoreActions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { catchError, filter, map, switchMap } from 'rxjs/operators';
import { Actions } from '.';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { of } from 'rxjs';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';
import { HearingType } from '@shared/consts/HearingType.model';
import { ReturnedDecisionSelectors } from './returned-decision.selectors';
import { Store } from '@ngrx/store';

@Injectable()
export class ReturnedDecisionEffects {
  private actions$ = inject(StoreActions);
  private jjDisputeService = inject(JJDisputeService);
  private store = inject(Store);

  get$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(Actions.Get),
      concatLatestFrom(() => [
        this.store.select(ReturnedDecisionSelectors.PageNumber),
        this.store.select(ReturnedDecisionSelectors.SortBy),
      ]),
      switchMap(([{ assignedTo }, pageNumber, sortBy]) =>
        this.jjDisputeService
          .getTCODisputes({
            appearances: true,
            multipleOfficersYn: true,
            jjAssignedTo: assignedTo,
            disputeStatusCodes: [DisputeStatus.Review].join(','),
            hearingTypeCd: HearingType.CourtAppearance,
            sortBy: sortBy,
            pageNumber: pageNumber,
            pageSize: 25,
            fetchPendingAdjournments: true,
          })
          .pipe(
            map((data) => Actions.GetSuccess({ data, assignedTo })),
            catchError((error) => of(Actions.GetFailure({ error }))),
          ),
      ),
    );
  });

  // if the requested page number exceeds the total number of pages, re-query for the last page of results
  getSuccess$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(Actions.GetSuccess),
      filter(
        (props) =>
          (props.data?.pageNumber ?? 0) > (props.data?.totalPages ?? 0) &&
          (props.data?.totalPages ?? 0) > 0,
      ),
      concatLatestFrom(() =>
        this.store.select(ReturnedDecisionSelectors.SortBy),
      ),
      map(([props, sortBy]) =>
        Actions.Get({
          assignedTo: props.assignedTo,
          pageNumber: props.data.totalPages,
          sortBy: sortBy,
        }),
      ),
    );
  });
}

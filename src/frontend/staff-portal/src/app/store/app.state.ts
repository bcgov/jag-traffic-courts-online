import { ActionReducerMap } from '@ngrx/store';
import { ReturnedDecisionStore } from '.';
import { JJDisputeStore } from '.';

export interface AppState {
  [JJDisputeStore.StoreName]: JJDisputeStore.State;
  [ReturnedDecisionStore.StoreName]: ReturnedDecisionStore.State;
}

export const reducers: ActionReducerMap<AppState> = {
  [JJDisputeStore.StoreName]: JJDisputeStore.Reducers,
  [ReturnedDecisionStore.StoreName]: ReturnedDecisionStore.Reducers,
};

import { ActionReducerMap, createSelector } from "@ngrx/store";
import { DisputeStore } from ".";

export interface AppState {
  dispute: DisputeStore.State
}

export const reducers: ActionReducerMap<AppState> = {
  dispute: DisputeStore.Reducers
}

export const disputeStateSelector = (state: AppState) => state.dispute;

export const disputeStateDataSelector = createSelector(
  disputeStateSelector,
  (state: DisputeStore.State) => state.data
);

export const disputeLoadingSelector = createSelector(
  disputeStateSelector,
  (state: DisputeStore.State) => state.loading
);

export const disputeSelector = createSelector(
  disputeStateSelector,
  (state: DisputeStore.State) => state.data?.dispute
);
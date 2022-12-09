import { ActionReducerMap, createSelector } from "@ngrx/store";
import { DisputeStore } from ".";

export interface AppState {
  dispute: DisputeStore.State
}

export const reducers: ActionReducerMap<AppState> = {
  dispute: DisputeStore.Reducers
}

export const disputeSelectorState = (state: AppState) => state.dispute;

export const disputeStateDataSelector = createSelector(
  disputeSelectorState,
  (state: DisputeStore.State) => state.data
);

export const disputeLoadingSelector = createSelector(
  disputeSelectorState,
  (state: DisputeStore.State) => state.loading
);

export const disputeSelector = createSelector(
  disputeSelectorState,
  (state: DisputeStore.State) => state.data?.dispute
);
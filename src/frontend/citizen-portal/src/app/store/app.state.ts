import { ActionReducerMap, createSelector } from "@ngrx/store";
import { DisputeStore } from ".";

export interface AppState {
  dispute: DisputeStore.State
}

export const reducers: ActionReducerMap<AppState> = {
  dispute: DisputeStore.Reducers
}

export const selectDisputeState = (state: AppState) => state.dispute;

export const selectDisputeStateData = createSelector(
  selectDisputeState,
  (state: DisputeStore.State) => state.data
);

export const selectLoadingDispute = createSelector(
  selectDisputeState,
  (state: DisputeStore.State) => state.loading
);

export const selectDispute = createSelector(
  selectDisputeState,
  (state: DisputeStore.State) => state.data?.dispute
);
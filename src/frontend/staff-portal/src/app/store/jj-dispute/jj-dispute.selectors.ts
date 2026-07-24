import { createSelector } from "@ngrx/store";
import { AppState, JJDisputeStore } from "..";
import { RequestStatus } from "../common-state";

const state = (state: AppState) => state[JJDisputeStore.StoreName];

const jjDisputes = createSelector(
  state,
  (state: JJDisputeStore.State) => state.data
);

const selectedJJDispute = createSelector(
  state,
  (state: JJDisputeStore.State) => state.selectedItem
);

const isLoading = createSelector(
  state,
  (state: JJDisputeStore.State) => state.status === RequestStatus.Loading
);

export const JJDisputeSelectors = {
  State: state,
  JJDisputes: jjDisputes,
  SelectedJJDispute: selectedJJDispute,
  IsLoading: isLoading,
}

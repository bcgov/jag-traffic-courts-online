import { createSelector } from "@ngrx/store";
import { AppState, DisputeStore } from "..";
 
const state = (state: AppState) => state.dispute;
const result = createSelector(
  state,
  (state: DisputeStore.State) => state.result
);
const params = createSelector(
  state,
  (state: DisputeStore.State) => state.params
);
const noticeOfDispute = createSelector(
  state,
  (state: DisputeStore.State) => state.noticeOfDispute
);

export const DisputeSelectors = {
  State: state,
  Result: result,
  Params: params,
  NoticeOfDispute: noticeOfDispute
}
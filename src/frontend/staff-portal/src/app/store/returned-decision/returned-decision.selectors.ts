import { createSelector } from "@ngrx/store";
import { AppState, ReturnedDecisionStore } from "..";
import { RequestStatus } from "../common-state";

const state = (state: AppState) => state[ReturnedDecisionStore.StoreName];

const selectPagedCollection = createSelector(
  state,
  (state: ReturnedDecisionStore.State) => state.data
);

const selectDecisions = createSelector(
  state,
  (state: ReturnedDecisionStore.State) => state.data?.items ?? []
);

const selectIsLoading = createSelector(
  state,
  (state: ReturnedDecisionStore.State) => state.status === RequestStatus.Loading
);

const selectPageNumber = createSelector(
  state,
  (state: ReturnedDecisionStore.State) => state.pageNumber
);

const selectSortBy = createSelector(
  state,
  (state: ReturnedDecisionStore.State) => state.sortBy
);

export const ReturnedDecisionSelectors = {
  State: state,
  PagedCollection: selectPagedCollection,
  Decisions: selectDecisions,
  IsLoading: selectIsLoading,
  PageNumber: selectPageNumber,
  SortBy: selectSortBy,
}

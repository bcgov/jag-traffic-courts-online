import { createSelector } from "@ngrx/store";
import { AppCoreState, LoadingStore } from "..";

const state = (state: AppCoreState) => state[LoadingStore.StoreName];

const isLoading = createSelector(
  state,
  (state: LoadingStore.State) => state.isLoading
);

const loadingItems = createSelector(
  state,
  (state: LoadingStore.State) => state.loadingItems
);

const numberOfLoadingItems = createSelector(
  state,
  (state: LoadingStore.State) => state.numberOfLoadingItems
);

export const LoadingSelectors = {
  State: state,
  IsLoading: isLoading,
  LoadingItems: loadingItems,
  NumberOfLoadingItems: numberOfLoadingItems,
}

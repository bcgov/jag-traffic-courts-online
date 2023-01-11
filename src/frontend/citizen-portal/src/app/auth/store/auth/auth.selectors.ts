import { createSelector } from "@ngrx/store";
import { AppCoreState, AuthStore } from "..";

const state = (state: AppCoreState) => state[AuthStore.StoreName];

const isLoading = createSelector(
  state,
  (state: AuthStore.State) => state.isLoading
);

const isAuthenticated = createSelector(
  state,
  (state: AuthStore.State) => state.isAuthenticated
);

const accessToken = createSelector(
  state,
  (state: AuthStore.State) => state.accessToken
);

export const AuthSelectors = {
  State: state,
  IsLoading: isLoading,
  IsAuthenticated: isAuthenticated,
  AccessToken: accessToken,
}

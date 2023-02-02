export { AuthActionTypes as ActionTypes } from "./auth.actions-types";
export { AuthState as State, initialState as InitialState } from "./auth.state";
export * as Actions from "./auth.actions";
export { AuthReducer as Reducer } from "./auth.reducers";
export { AuthEffects as Effects } from "./auth.effects";
export { AuthSelectors as Selectors } from "./auth.selectors";

export const StoreName = "auth";
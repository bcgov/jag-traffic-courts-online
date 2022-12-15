export { LoadingActionTypes as ActionTypes } from "./laoding.actions-types";
export { LoadingState as State, initialState as InitialState } from "./loading.state";
export * as Actions from "./loading.actions";
export { LoadingReducer as Reducer } from "./loading.reducers";
export { LoadingEffects as Effects } from "./loading.effects";
export { LoadingSelectors as Selectors } from "./loading.selectors";

export const StoreName = "loading";
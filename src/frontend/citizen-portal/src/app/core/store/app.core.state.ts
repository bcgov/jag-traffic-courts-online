import { ActionReducerMap } from "@ngrx/store";
import { LoadingStore } from ".";
import { AppState } from "app/store";

export interface AppCoreState extends AppState {
  [LoadingStore.StoreName] : LoadingStore.State
}

export const reducers: ActionReducerMap<AppCoreState> = {
  [LoadingStore.StoreName]: LoadingStore.Reducer
}
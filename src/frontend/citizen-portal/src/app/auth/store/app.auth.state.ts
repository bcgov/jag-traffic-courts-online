import { ActionReducerMap } from "@ngrx/store";
import { AuthStore } from ".";
import { AppState } from "app/store";

export interface AppCoreState extends AppState {
  [AuthStore.StoreName] : AuthStore.State
}

export const reducers: ActionReducerMap<AppCoreState> = {
  [AuthStore.StoreName]: AuthStore.Reducer
}
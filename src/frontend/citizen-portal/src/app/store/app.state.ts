import { ActionReducerMap } from "@ngrx/store";
import { DisputeStore } from ".";

export interface AppState {
    dispute: DisputeStore.State
}

export const reducers: ActionReducerMap<AppState> = {
    dispute: DisputeStore.Reducers
}
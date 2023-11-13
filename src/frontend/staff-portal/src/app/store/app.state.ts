import { ActionReducerMap } from "@ngrx/store";
import { JJDisputeStore } from ".";

export interface AppState {
    [JJDisputeStore.StoreName]: JJDisputeStore.State
}

export const reducers: ActionReducerMap<AppState> = {
    [JJDisputeStore.StoreName]: JJDisputeStore.Reducers,
}
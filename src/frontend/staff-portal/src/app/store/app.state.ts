import { ActionReducerMap } from "@ngrx/store";
import { JJDisputeStore } from ".";

export interface AppState {
    jjDispute: JJDisputeStore.State
}

export const reducers: ActionReducerMap<AppState> = {
    jjDispute: JJDisputeStore.Reducers
}
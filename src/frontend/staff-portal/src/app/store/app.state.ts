import { ActionReducerMap } from "@ngrx/store";
import * as JJDisputeStore from "./jj-dispute";

export interface AppState {
    jjDispute: JJDisputeStore.State
}

export const reducers: ActionReducerMap<AppState> = {
    jjDispute: JJDisputeStore.Reducers
}
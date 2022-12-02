import { JJDispute } from "app/services/jj-dispute.service";
import { CustomState } from "../custom-state";

export interface JJDisputeState extends CustomState {
    data: JJDispute[],
    dataByIDIR: JJDispute[],
    selectedItem: JJDispute
}

export const initialState: JJDisputeState = {
    loading: false,
    data: [],
    dataByIDIR: [],
    selectedItem: null
}
import { RequestState, RequestStatus } from "../common-state";
import { PagedDisputeCaseFileSummaryCollection } from "app/api";

export interface ReturnedDecisionState extends RequestState<PagedDisputeCaseFileSummaryCollection> {
  pageNumber: number | undefined;
  sortBy: string;
}

export const initialState: ReturnedDecisionState = {
    status: RequestStatus.Idle,
    data: undefined,
    pageNumber: undefined,
    sortBy: 'appearanceTs'
}

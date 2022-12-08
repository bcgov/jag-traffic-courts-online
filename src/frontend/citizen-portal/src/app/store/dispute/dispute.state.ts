import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { SearchDisputeResult } from "app/api";
import { CustomState } from "../custom-state";

export interface DisputeState extends CustomState {
  data?: DisputeStateData
}

export interface DisputeStateData {
  dispute?: SearchDisputeResult,
  params?: QueryParamsForSearch
}

export const initialState: DisputeState = {
  loading: false,
  data: null,
}
import { NoticeOfDispute } from "@shared/models/dispute-form.model";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { SearchDisputeResult } from "app/api";
import { CustomState } from "../custom-state";

export interface DisputeState extends CustomState {
  result?: SearchDisputeResult,
  noticeOfDispute?: NoticeOfDispute,
  params?: QueryParamsForSearch,
}

export const initialState: DisputeState = {
  result: null,
  noticeOfDispute: null,
  params: null,
  loading: false
}
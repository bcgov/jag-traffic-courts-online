import { createAction, props } from "@ngrx/store";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { ActionTypes } from ".";
import { DisputeStore } from "..";

export const Search = createAction(
  ActionTypes.SEARCH,
  props<{ params?: QueryParamsForSearch }>()
);

export const SearchSuccess = createAction(
  ActionTypes.SEARCH_SUCCESS,
  props<DisputeStore.StateData>()
);

export const SearchFailed = createAction(
  ActionTypes.SEARCH_FAILED
);
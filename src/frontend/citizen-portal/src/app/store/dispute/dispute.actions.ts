import { createAction, props } from "@ngrx/store";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { DisputantContactInformation } from "app/api";
import { ActionTypes } from ".";
import { DisputeStore } from "..";

export const Search = createAction(
  ActionTypes.SEARCH,
  props<{ params?: QueryParamsForSearch }>()
);

export const SearchSuccess = createAction(
  ActionTypes.SEARCH_SUCCESS,
  props<{ payload?: DisputeStore.State }>()
);

export const SearchFailed = createAction(
  ActionTypes.SEARCH_FAILED
);

export const UpdateContact = createAction(
  ActionTypes.UPDATE_CONTACT,
  props<{ guid: string, payload: DisputantContactInformation }>()
);

export const UpdateContactSuccess = createAction(
  ActionTypes.UPDATE_CONTACT_SUCCESS,
);

export const UpdateContactFailed = createAction(
  ActionTypes.UPDATE_CONTACT_FAILED
);
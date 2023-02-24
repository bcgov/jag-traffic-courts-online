import { createAction, props } from "@ngrx/store";
import { NoticeOfDispute } from "@shared/models/dispute-form.model";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { DisputantContactInformation, FileMetadata, SearchDisputeResult } from "app/api";
import { ActionTypes } from ".";

export const Search = createAction(
  ActionTypes.SEARCH,
  props<{ params?: QueryParamsForSearch }>()
);

export const SearchSuccess = createAction(
  ActionTypes.SEARCH_SUCCESS,
  props<{ result?: SearchDisputeResult }>()
);

export const SearchFailed = createAction(
  ActionTypes.SEARCH_FAILED
);

export const UpdateContact = createAction(
  ActionTypes.UPDATE_CONTACT,
  props<{ payload: DisputantContactInformation }>()
);

export const UpdateContactSuccess = createAction(
  ActionTypes.UPDATE_CONTACT_SUCCESS,
);

export const UpdateContactFailed = createAction(
  ActionTypes.UPDATE_CONTACT_FAILED
);

export const Get = createAction(
  ActionTypes.GET
);

export const GetSuccess = createAction(
  ActionTypes.GET_SUCCESS,
  props<{ noticeOfDispute?: NoticeOfDispute }>()
);

export const GetFailed = createAction(
  ActionTypes.GET_FAILED
);

export const Update = createAction(
  ActionTypes.UPDATE,
  props<{ payload: NoticeOfDispute }>()
);

export const UpdateSuccess = createAction(
  ActionTypes.UPDATE_SUCCESS,
);

export const UpdateFailed = createAction(
  ActionTypes.UPDATE_FAILED
);

export const GetDocument = createAction(
  ActionTypes.GET_DOCUMENT,
  props<{ fileId: string }>()
);

export const GetDocumentSuccess = createAction(
  ActionTypes.GET_DOCUMENT_SUCCESS,
);

export const GetDocumentFailed = createAction(
  ActionTypes.GET_DOCUMENT_FAILED
);

export const AddDocument = createAction(
  ActionTypes.ADD_DOCUMENT,
  props<{ file: File }>()
);

export const AddDocumentSuccess = createAction(
  ActionTypes.ADD_DOCUMENT_SUCCESS,
  props<{ file: FileMetadata }>()
);

export const AddDocumentFailed = createAction(
  ActionTypes.ADD_DOCUMENT_FAILED
);

export const RemoveDocument = createAction(
  ActionTypes.REMOVE_DOCUMENT,
  props<{ file: FileMetadata }>()
);

export const RemoveDocumentSuccess = createAction(
  ActionTypes.REMOVE_DOCUMENT_SUCCESS,
  props<{ fileId: string }>()
);

export const RemoveDocumentFailed = createAction(
  ActionTypes.REMOVE_DOCUMENT_FAILED
);
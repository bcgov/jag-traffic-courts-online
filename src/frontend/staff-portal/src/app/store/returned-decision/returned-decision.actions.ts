import { createAction, props } from '@ngrx/store';
import { PagedDisputeCaseFileSummaryCollection } from 'app/api';
import { ActionTypes } from '.';

export const Get = createAction(
  ActionTypes.GET,
  props<{
    assignedTo: string | undefined;
    sortBy?: string;
    pageNumber?: number;
  }>(),
);

export const GetSuccess = createAction(
  ActionTypes.GET_SUCCESS,
  props<{
    data: PagedDisputeCaseFileSummaryCollection;
    assignedTo: string | undefined;
  }>(),
);

export const GetFailure = createAction(
  ActionTypes.GET_FAILURE,
  props<{ error: unknown }>(),
);

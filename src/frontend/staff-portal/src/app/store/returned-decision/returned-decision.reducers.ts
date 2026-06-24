import { Action, createReducer, on } from '@ngrx/store';
import { Actions } from '.';
import { initialState, ReturnedDecisionState } from './returned-decision.state';
import { RequestStatus } from '../common-state';

export function ReturnedDecisionReducer(
  state: ReturnedDecisionState = initialState,
  action: Action,
): ReturnedDecisionState {
  return jjDisputeReducer(state, action);
}

const jjDisputeReducer = createReducer(
  initialState,
  on(
    Actions.Get,
    (state, props): ReturnedDecisionState => ({
      ...state,
      status: RequestStatus.Loading,
      pageNumber: props.pageNumber ?? state.pageNumber,
      sortBy: props.sortBy ?? state.sortBy,
    }),
  ),
  on(
    Actions.GetSuccess,
    (state, props): ReturnedDecisionState => ({
      ...state,
      status: RequestStatus.Success,
      data: props.data,
      pageNumber:
        props.data.totalPages && props.data.pageNumber
          ? props.data.pageNumber
          : 1,
    }),
  ),
  on(
    Actions.GetFailure,
    (state, _props): ReturnedDecisionState => ({
      ...state,
      status: RequestStatus.Error,
    }),
  ),
);

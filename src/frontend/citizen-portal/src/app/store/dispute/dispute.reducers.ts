import { Action, createReducer, on } from '@ngrx/store';
import { Actions } from '.';
import { initialState, DisputeState } from './dispute.state';

export function DisputeReducer(state: DisputeState = initialState, action: Action): DisputeState {
  return disputeReducer(state, action)
}

const disputeReducer = createReducer(initialState,
  on(Actions.Search, (state, input) => ({ ...state, result: null, params: input.params, loading: true })),
  on(Actions.SearchSuccess, (state, response) => ({ ...state, ...response.payload, loading: false })),
  on(Actions.SearchFailed, state => ({ ...state, params: null, loading: false })),
)

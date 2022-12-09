import { Action, createReducer, on } from '@ngrx/store';
import { Actions } from '.';
import { initialState, DisputeState } from './dispute.state';

export function DisputeReducer(state: DisputeState = initialState, action: Action): DisputeState {
  return disputeReducer(state, action)
}

const disputeReducer = createReducer(initialState,
  on(Actions.Search, (state, input) => ({ ...state, loading: true, data: { dispute: null, params: input.params } })),
  on(Actions.SearchSuccess, (state, result) => ({ ...state, loading: false, data: result })),
  on(Actions.SearchFailed, state => ({ ...state, loading: false })),
)

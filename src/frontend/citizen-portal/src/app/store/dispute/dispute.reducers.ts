import { Action, createReducer, on } from '@ngrx/store';
import { Actions } from '.';
import { initialState, DisputeState } from './dispute.state';

export function DisputeReducer(state: DisputeState = initialState, action: Action): DisputeState {
  return disputeReducer(state, action)
}

const disputeReducer = createReducer(initialState,
  on(Actions.Search, (state, input) => ({ ...state, result: null, params: input.params, loading: true })),
  on(Actions.SearchSuccess, (state, output) => ({ ...state, ...output, loading: false })),
  on(Actions.SearchFailed, state => ({ ...state, params: null, loading: false })),
  
  on(Actions.UpdateContact, (state) => ({ ...state, loading: true })),
  on(Actions.UpdateContactSuccess, (state) => ({ ...state, loading: false })),
  on(Actions.UpdateContactFailed, state => ({ ...state, loading: false })),

  on(Actions.Get, (state) => ({ ...state, noticeOfDispute: null, loading: true })),
  on(Actions.GetSuccess, (state, output) => ({ ...state, ...output, loading: false })),
  on(Actions.GetFailed, state => ({ ...state, loading: false })),
    
  on(Actions.Update, (state) => ({ ...state, loading: true })),
  on(Actions.UpdateSuccess, (state) => ({ ...state, loading: false })),
  on(Actions.UpdateFailed, state => ({ ...state, loading: false })),
)

import { Action, createReducer, on } from '@ngrx/store';
import { Actions } from './';
import { initialState, JJDisputeState } from './jj-dispute.state';
import { RequestStatus } from '../common-state';

export function JJDisputeReducer(state: JJDisputeState = initialState, action: Action): JJDisputeState {
  return jjDisputeReducer(state, action)
}

const jjDisputeReducer = createReducer(initialState,
  on(Actions.Get, state => ({ ...state, status: RequestStatus.Loading, data: [] })), // temporary solution for refresh "data: []"
  on(Actions.GetSuccess, (state, result) => ({ ...state, status: RequestStatus.Success, data: result.data })),

  on(Actions.Assign, state => ({ ...state, status: RequestStatus.Loading })),
  on(Actions.AssignSuccess, state => state), // Fire get event in effect
)

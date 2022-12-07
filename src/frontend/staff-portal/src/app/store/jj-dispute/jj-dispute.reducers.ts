import { Action, createReducer, on } from '@ngrx/store';
import { Actions } from './';
import { initialState, JJDisputeState } from './jj-dispute.state';

export function JJDisputeReducer(state: JJDisputeState = initialState, action: Action): JJDisputeState {
  return jjDisputeReducer(state, action)
}

const jjDisputeReducer = createReducer(initialState,
  on(Actions.Get, state => ({ ...state, loading: true })),
  on(Actions.GetSuccess, (state, result) => ({ ...state, loading: false, data: result.data })),

  on(Actions.Assign, state => ({ ...state, loading: true })),
  on(Actions.AssignSuccess, (state, result) => {
    let updatedTicketNumbers = result.updatedData.map(i => i.ticketNumber)
    let data = state.data.filter(i => updatedTicketNumbers.indexOf(i.ticketNumber) < 0).concat(result.updatedData);
    return { ...state, loading: false, data };
  }),
)

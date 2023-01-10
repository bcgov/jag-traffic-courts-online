import { Action, createReducer, on } from '@ngrx/store';
import { Actions } from '.';
import { initialState, AuthState } from './auth.state';

export function AuthReducer(state: AuthState = initialState, action: Action): AuthState {
  return authReducer(state, action)
}

const authReducer = createReducer(initialState,
  on(Actions.Authorize, state => ({ ...state, isLoading: true })),
  on(Actions.Authorizing, state => ({ ...state, isLoading: true })),
  on(Actions.Authorized, (state, response) => ({ ...state, ...response.payload, isLoading: false })),
)

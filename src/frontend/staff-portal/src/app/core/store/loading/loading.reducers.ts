import { Action, createReducer, on } from '@ngrx/store';
import { Actions } from '.';
import { initialState, LoadingState } from './loading.state';

export function LoadingReducer(state: LoadingState = initialState, action: Action): LoadingState {
  return loadingReducer(state, action)
}

const loadingReducer = createReducer(initialState,
  on(Actions.Add, (state) => ({ ...state, isLoading: true, numberOfLoadingItems: state.numberOfLoadingItems + 1 })),
  on(Actions.Remove, (state) => ({ ...state, numberOfLoadingItems: state.numberOfLoadingItems - 1 })),
  on(Actions.LoadingDone, state => ({ ...state, isLoading: false })),
)

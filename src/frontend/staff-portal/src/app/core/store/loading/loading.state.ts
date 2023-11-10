import { Subscription } from "rxjs";

export interface LoadingState {
  isLoading: boolean;
  numberOfLoadingItems: number;
  loadingItems: Subscription[];
}

export const initialState: LoadingState = {
  isLoading: false,
  numberOfLoadingItems: 0,
  loadingItems: [],
}
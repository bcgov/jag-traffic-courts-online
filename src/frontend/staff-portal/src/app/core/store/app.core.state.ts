import { LoadingStore } from ".";
import { AppState } from "app/store";

export interface AppCoreState extends AppState {
  [LoadingStore.StoreName] : LoadingStore.State
}
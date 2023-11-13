import { createAction, props } from "@ngrx/store";
import { ActionTypes } from ".";
import { Subscription } from "rxjs";

export const Add = createAction(
  ActionTypes.ADD,
);

export const Remove = createAction(
  ActionTypes.REMOVE,
);

export const LoadingDone = createAction(
  ActionTypes.LOADING_DONE
);
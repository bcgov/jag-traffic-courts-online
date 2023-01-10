import { createAction, props } from "@ngrx/store";
import { ActionTypes } from ".";
import { AuthStore } from "..";

export const Authorize = createAction(
  ActionTypes.AUTHORIZE
);

export const Authorizing = createAction(
  ActionTypes.AUTHORIZING
);

export const Authorized = createAction(
  ActionTypes.AUTHORIZED,
  props<{ payload?: AuthStore.State }>()
);

export const Redirect = createAction(
  ActionTypes.REDIRECT
);

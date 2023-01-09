import { createAction, props } from "@ngrx/store";
import { ActionTypes } from ".";
import { AuthStore } from "..";

export const Authorize = createAction(
  ActionTypes.AUTHORIZE
);

export const Authorizing = createAction(
  ActionTypes.AUTHORIZING
);

export const CheckAuthorize = createAction(
  ActionTypes.CHECK_AUTHORIZE
);

export const CheckAuthorizeFinished = createAction(
  ActionTypes.CHECK_AUTHORIZE_FINISHED
);

export const Authorized = createAction(
  ActionTypes.AUTHORIZED,
  props<{ payload?: AuthStore.State }>()
);
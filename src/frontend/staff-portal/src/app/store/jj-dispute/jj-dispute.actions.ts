import { createAction, props } from "@ngrx/store";
import { JJDispute } from "app/api";
import { ActionTypes } from "./";

export const Get = createAction(
  ActionTypes.GET,
);

export const GetSuccess = createAction(
  ActionTypes.GET,
  props<{ data: JJDispute[] }>()
);

export const Assign = createAction(
  ActionTypes.ASSIGN,
  props<{ ticketNumbers: string[], username: string }>()
);

export const AssignSuccess = createAction(
  ActionTypes.ASSIGN_SUCCESS
);
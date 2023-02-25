import { createAction, props } from "@ngrx/store";
import { JJDispute } from "app/api";
import { ActionTypes } from "./";

export const Get = createAction(
  ActionTypes.GET,
);

export const GetSuccess = createAction(
  ActionTypes.GET_SUCCESS,
  props<{ data: JJDispute[] }>()
);

export const Assign = createAction(
  ActionTypes.ASSIGN,
  props<{ disputeIds: number[], username: string }>()
);

export const AssignSuccess = createAction(
  ActionTypes.ASSIGN_SUCCESS
);

import { DisputeUpdateRequestStatus2 } from "app/api";

export interface DisputantUpdateRequestExpanded {
  requestedTs: string;
  fieldTitle: string;
  oldValue: string;
  newValue: string;
  status: DisputeUpdateRequestStatus2;
  statusTs: string;
}
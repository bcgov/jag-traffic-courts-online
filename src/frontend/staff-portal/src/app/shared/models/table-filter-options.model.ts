import { DisputeStatus } from "app/api";

export class TableFilter {
  dateSubmittedFrom?: string;
  dateSubmittedTo?: string;
  decisionDateFrom?: string;
  decisionDateTo?: string;
  ticketNumber?: string;
  disputantSurname?: string;
  occamDisputantName?: string;
  team?: string;
  courthouseLocation?: string;
  status?: DisputeStatus | '';
}
export type TableFilterKeys = keyof TableFilter;
export type TableFilterConfigs = {
  [key in TableFilterKeys]?: boolean;
}
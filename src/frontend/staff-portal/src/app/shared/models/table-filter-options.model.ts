import { Agency, DisputeStatus } from "app/api";


  /**
     * This is an intermediate layer of "options" to allow us to create special cases like "New & Validated", but still only send a final list of DisputeStatuses we want to the API
     */
export class TableFilterStatus {
  label: string;
  mapping: DisputeStatus[];
}

export const TableFilterStatusOptions = [
    {
        Label: 'UNKNOWN',
        Mapping: [DisputeStatus.Unknown]
    },
    {
        Label: 'NEW',
        Mapping: [DisputeStatus.New]
    },
    {
        Label: 'VALIDATED',
        Mapping: [DisputeStatus.Validated]
    },
     {
        Label: 'NEW AND VALIDATED',
        Mapping: [DisputeStatus.New, DisputeStatus.Validated]
    },
    {
        Label: 'PROCESSING',
        Mapping: [DisputeStatus.Processing]
    },
    {
        Label: 'REJECTED',
        Mapping: [DisputeStatus.Rejected]
    },
    {
        Label: 'CANCELLED',
        Mapping: [DisputeStatus.Cancelled]
    },
    {
        Label: 'CONCLUDED',
        Mapping: [DisputeStatus.Concluded]
    },
];

export class TableFilter {
  dateSubmittedFrom?: string;
  dateSubmittedTo?: string;
  decisionDateFrom?: string;
  decisionDateTo?: string;
  ticketNumber?: string;
  disputantSurname?: string;
  surname?: string;
  team?: string;
  courthouseLocation?: Agency[];
  status?: TableFilterStatus;
}
export type TableFilterKeys = keyof TableFilter;
export type TableFilterConfigs = {
  [key in TableFilterKeys]?: boolean;
}
import { Agency, DisputeStatus } from "app/api";


  /**
     * This is an intermediate layer of "options" to allow us to create special cases like "New & Validated", but still only send a final list of DisputeStatuses we want to the API
     */
export class TableFilterStatus {
  label: string;
  mapping: DisputeStatus[];
}

export const TableFilterStatusOptions = [{
        label: 'NEW',
        mapping: [DisputeStatus.New]
    },{
        label: 'VALIDATED',
        mapping: [DisputeStatus.Validated]
    },{
        label: 'NEW & VALIDATED',
        mapping: [DisputeStatus.New, DisputeStatus.Validated]
    },{
        label: 'PROCESSING',
        mapping: [DisputeStatus.Processing]
    },{
        label: 'REJECTED',
        mapping: [DisputeStatus.Rejected]
    },{
        label: 'CANCELLED',
        mapping: [DisputeStatus.Cancelled]
    },{
        label: 'CONCLUDED',
        mapping: [DisputeStatus.Concluded]
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
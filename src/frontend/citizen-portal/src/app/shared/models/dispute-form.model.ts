import { AbstractControl, FormGroup, FormControlState, FormControlOptions } from "@angular/forms";
import { NoticeOfDispute as NoticeOfDisputeBase, DisputeCount as DisputeCountBase, DisputantContactInformation as DisputantContactInformationBase, ViolationTicketCount } from "app/api";

export interface DisputantContactInformation extends DisputantContactInformationBase {
  disputant_given_names?: string;
  contact_given_names?: string;
  address?: string;
}
export type DisputantContactInformationKeys = keyof DisputantContactInformation;
export type DisputantContactInformationFormConfigs = {
  [key in DisputantContactInformationKeys]?: {
    value: DisputantContactInformation[key] | null | FormControlState<DisputantContactInformation[key] | null>,
    options?: FormControlOptions
  } | DisputantContactInformation[key] | null
}
export type DisputantContactInformationFormControls = {
  [key in DisputantContactInformationKeys]?: AbstractControl;
}
export interface DisputantContactInformationFormGroup extends FormGroup {
  value: DisputantContactInformation;
  controls: DisputantContactInformationFormControls;
}

// Notice of dispute, including additional and legal representative
export interface NoticeOfDispute extends NoticeOfDisputeBase {
  disputant_given_names?: string;
  contact_given_names?: string;
  address?: string;
  lawyer_full_name?: string;
  __witness_present?: boolean;
}
export type NoticeOfDisputeKeys = keyof NoticeOfDispute;
export type NoticeOfDisputeFormConfigs = {
  [key in NoticeOfDisputeKeys]?: {
    value: NoticeOfDispute[key] | null | FormControlState<NoticeOfDispute[key] | null>,
    options?: FormControlOptions
  } | NoticeOfDispute[key] | null
}
export type NoticeOfDisputeFormControls = {
  [key in NoticeOfDisputeKeys]?: AbstractControl;
}
export interface NoticeOfDisputeFormGroup extends FormGroup {
  value: NoticeOfDispute;
  controls: NoticeOfDisputeFormControls;
}

// Dispute count
export interface DisputeCount extends DisputeCountBase {
  __skip?: boolean;
  __apply_to_remaining_counts?: boolean;
}
export type DisputeCountKeys = keyof DisputeCount;
export type DisputeCountFormConfigs = {
  [key in DisputeCountKeys]?: {
    value: DisputeCount[key] | null | FormControlState<DisputeCount[key] | null>,
    options?: FormControlOptions
  } | DisputeCount[key] | null
}
export type DisputeCountFormControls = {
  [key in DisputeCountKeys]?: AbstractControl;
}
export interface DisputeCountFormGroup extends FormGroup {
  value: DisputeCount;
  controls: DisputeCountFormControls;
}
export interface Count {
  ticket_count?: ViolationTicketCount;
  form?: DisputeCountFormGroup;
}

// Dispute counts Actions
export interface DisputeCountActions extends DisputeCountBase { // just used as key template
  guilty?: any;
  not_guilty?: any;
}
export type DisputeCountActionsKeys = keyof DisputeCountActions;
export type CountsActions = {
  [key in DisputeCountActionsKeys]?: string;
}

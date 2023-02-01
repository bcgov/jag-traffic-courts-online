import { DatePipe } from "@angular/common";
import { Injectable } from "@angular/core";
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { FormGroupValidators } from "@core/validators/form-group.validators";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { DisputeFormMode } from "@shared/enums/dispute-form-mode";
import { DisputeCount, DisputesService, NoticeOfDispute as NoticeOfDisputeBase, DisputeCountPleaCode, DisputeCountRequestCourtAppearance, DisputeRepresentedByLawyer, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, DisputeStatus, DisputeContactType } from "app/api";
import { AppRoutes } from "app/app.routes";
import { BehaviorSubject, Observable } from "rxjs";
import { ViolationTicketService } from "./violation-ticket.service";

@Injectable({
  providedIn: "root",
})
export class NoticeOfDisputeService {
  private _noticeOfDispute: BehaviorSubject<NoticeOfDispute> = new BehaviorSubject<NoticeOfDispute>(null);
  private _countFormDefaultValue: any;
  private _additionFormDefaultValue: any;

  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;
  RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  PleaCode = DisputeCountPleaCode;
  ContactType = DisputeContactType;

  ticketFormFields: NoticeOfDisputeFormControls = { // need to reset before using, all default value should be set in the component itself
    disputant_surname: new FormControl<string | null>(null, [Validators.required]),
    disputant_given_names: new FormControl<string | null>(null, [Validators.required]),
    contact_given_names: new FormControl<string | null>(null, [Validators.required]),
    contact_surname: new FormControl<string | null>(null, [Validators.required]),
    contact_law_firm_name: new FormControl<string | null>(null),
    contact_type: new FormControl<string>(this.ContactType.Individual,[Validators.required]),
    address: new FormControl<string | null>(null, [Validators.required, Validators.maxLength(300)]),
    address_city: new FormControl<string | null>(null, [Validators.required]),
    address_province: new FormControl<string | null>(null, [Validators.required, Validators.maxLength(30)]),
    address_province_country_id: new FormControl<number | null>(null),
    address_province_seq_no: new FormControl<number | null>(null),
    address_country_id: new FormControl<number | null>(null, [Validators.required]),
    postal_code: new FormControl<string | null>(null, [Validators.required]),
    home_phone_number: new FormControl<string | null>(null, [FormControlValidators.phone]),
    email_address: new FormControl<string | null>(null, [Validators.required, Validators.email]),
    drivers_licence_number: new FormControl<string | null>(null, [Validators.required, Validators.minLength(7), Validators.maxLength(9)]),
    drivers_licence_province: new FormControl<string | null>(null, [Validators.required]),
    drivers_licence_country_id: new FormControl<number | null>(null),
    drivers_licence_province_seq_no: new FormControl<number | null>(null),
  }

  countFormFields = {
    plea_cd: [null],
    request_time_to_pay: this.RequestTimeToPay.N,
    request_reduction: this.RequestReduction.N,
    __skip: [false],
    __apply_to_remaining_counts: [false],
  }

  additionFormFields = {
    represented_by_lawyer: [this.RepresentedByLawyer.N],
    request_court_appearance: [null, [Validators.required]],
    interpreter_language_cd: [null],
    witness_no: [0],
    fine_reduction_reason: [null, []],
    time_to_pay_reason: [null, []],

    __witness_present: [false],
    __interpreter_required: [false],
  }

  additionFormValidators = [
    FormGroupValidators.requiredIfTrue("__interpreter_required", "interpreter_language_cd"),
    FormGroupValidators.requiredIfTrue("__witness_present", "witness_no"),
  ]

  legalRepresentationFields = {
    law_firm_name: [null, [Validators.required]],
    lawyer_full_name: [null, [Validators.required]],
    lawyer_email: [null, [Validators.required, Validators.email]],
    lawyer_phone_number: [null, [Validators.required]],
    lawyer_address: [null, [Validators.required]],
  }

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private disputesService: DisputesService,
    private violationTicketService: ViolationTicketService,
    private datePipe: DatePipe,
    private fb: FormBuilder,
  ) {
    this._countFormDefaultValue = this.fb.group(this.countFormFields).value;
    this._additionFormDefaultValue = this.fb.group(this.additionFormFields).value;
  }

  get noticeOfDispute$(): Observable<NoticeOfDispute> {
    return this._noticeOfDispute.asObservable();
  }

  get noticeOfDispute(): NoticeOfDispute {
    return this._noticeOfDispute.value;
  }

  get countFormDefaultValue(): any {
    return this._countFormDefaultValue;
  }

  get additionFormDefaultValue(): any {
    return this._additionFormDefaultValue;
  }

  createNoticeOfDispute(input: NoticeOfDispute): void {
    input.issued_date = this.datePipe.transform(input.issued_date, "yyyy-MM-ddTHH:mm:ss");
    input = this.splitDisputantGivenNames(input);  // break disputant names into first, second, third
    input = this.splitContactGivenNames(input);  // break disputant names into first, second, third
    input = this.splitLawyerNames(input); // break lawyer names into first, second, surname
    input = this.splitAddressLines(input); // break address into line 1,2,3 by comma

    const data: DialogOptions = {
      titleKey: "Submit request",
      messageKey:
        "When your request is submitted for adjudication, it can no longer be updated. Are you ready to submit your request?",
      actionTextKey: "Submit request",
      cancelTextKey: "Cancel",
      icon: null,
    };
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: boolean) => {
        if (action) {
          input.dispute_counts = input.dispute_counts.filter(i => i.plea_cd);
          return this.disputesService.apiDisputesCreatePost(input).subscribe(res => {
            this._noticeOfDispute.next(input);
            if (input.email_address) {
              this.router.navigate([AppRoutes.EMAILVERIFICATIONREQUIRED], {
                queryParams: {
                  email: input.email_address,
                  token: res.noticeOfDisputeId,
                  mode: DisputeFormMode.CREATE
                },
              });
            }
            else {
              this.router.navigate([AppRoutes.ticketPath(AppRoutes.SUBMIT_SUCCESS)], {
                queryParams: {
                  ticketNumber: input.ticket_number,
                  time: this.datePipe.transform(input.issued_date, "HH:mm"),
                  mode: DisputeFormMode.CREATE
                },
              });
            }
          })
        }
      });
  }

  resendVerificationEmail(noticeOfDisputeId: string): Observable<any> {
    return this.disputesService.apiDisputesEmailGuidHashResendPut(noticeOfDisputeId);
  }

  verifyEmail(token: string): Observable<any> {
    return this.disputesService.apiDisputesEmailVerifyPut(token);
  }

  splitAddressLines(noticeOfDispute: NoticeOfDispute): NoticeOfDispute {
    // split up where commas occur and stuff in address lines 1,2,3
    // Canada post guidelines state that each address line should be no more than 40 chars, we are chopping each line at 100
    if (noticeOfDispute.address) {
      let addressLines = noticeOfDispute.address.split(",");
      if (addressLines.length > 0) noticeOfDispute.address_line1 = addressLines[0].length > 100 ? addressLines[0].substring(0, 100) : addressLines[0];
      if (addressLines.length > 1) noticeOfDispute.address_line2 = addressLines[1].length > 100 ? addressLines[1].substring(0, 100) : addressLines[1];
      if (addressLines.length > 2) noticeOfDispute.address_line3 = addressLines[2].length > 100 ? addressLines[2].substring(0, 100) : addressLines[2];
    }

    return noticeOfDispute;
  }

  splitDisputantGivenNames(noticeOfDispute: NoticeOfDispute): NoticeOfDispute {
    // split up where spaces occur and stuff in given names 1,2,3
    if (noticeOfDispute.disputant_given_names) {
      let givenNames = noticeOfDispute.disputant_given_names.split(" ");
      if (givenNames.length > 0) noticeOfDispute.disputant_given_name1 = givenNames[0];
      if (givenNames.length > 1) noticeOfDispute.disputant_given_name2 = givenNames[1];
      if (givenNames.length > 2) noticeOfDispute.disputant_given_name3 = givenNames[2];
    }

    return noticeOfDispute;
  }

  splitContactGivenNames(noticeOfDispute: NoticeOfDispute): NoticeOfDispute {
    // split up where spaces occur and stuff in given names 1,2,3
    if (noticeOfDispute.contact_given_names) {
      let contactGivenNames = noticeOfDispute.contact_given_names.split(" ");
      if (contactGivenNames.length > 0) noticeOfDispute.contact_given_name1 = contactGivenNames[0];
      if (contactGivenNames.length > 1) noticeOfDispute.contact_given_name2 = contactGivenNames[1];
      if (contactGivenNames.length > 2) noticeOfDispute.contact_given_name3 = contactGivenNames[2];
    }

    return noticeOfDispute;
  }

  splitLawyerNames(noticeOfDispute: NoticeOfDispute): NoticeOfDispute {
    // split up where spaces occur and stuff in given names 1,2,3
    if (noticeOfDispute.lawyer_full_name) {
      let lawyerNames = noticeOfDispute.lawyer_full_name.split(" ");
      if (lawyerNames.length > 0) noticeOfDispute.lawyer_surname = lawyerNames[lawyerNames.length - 1]; // last one
      if (lawyerNames.length > 1) noticeOfDispute.lawyer_given_name1 = lawyerNames[0];
      if (lawyerNames.length > 2) noticeOfDispute.lawyer_given_name2 = lawyerNames[1];
      if (lawyerNames.length > 3) noticeOfDispute.lawyer_given_name3 = lawyerNames[2];
    }

    return noticeOfDispute;
  }

  getCountsActions(counts: DisputeCount[]): any {
    let countsActions: any = {};
    let toCountStr = (arr: DisputeCount[]) => arr.map(i => "Count " + i.count_no).join(", ");
    countsActions.guilty = toCountStr(counts.filter(i => i.plea_cd === this.PleaCode.G));
    countsActions.not_guilty = toCountStr(counts.filter(i => i.plea_cd === this.PleaCode.N));
    countsActions.request_reduction = toCountStr(counts.filter(i => i.request_reduction === this.RequestReduction.Y));
    countsActions.request_time_to_pay = toCountStr(counts.filter(i => i.request_time_to_pay === this.RequestTimeToPay.Y));
    return countsActions;
  }

  getNoticeOfDispute(formValue): NoticeOfDispute {
    // form contains all sub forms
    // get the ticket from storage to make sure the user can't change the ticket info
    return <NoticeOfDispute>{ ...this.violationTicketService.ticket, ...formValue };
  }
}

export interface NoticeOfDispute extends NoticeOfDisputeBase {
  disputant_given_names?: string;
  contact_given_names?: string;
  lawyer_full_name?: string;
  address?: string;
}

export type NoticeOfDisputeKeys = keyof NoticeOfDispute;
export type NoticeOfDisputeFormControls = {
  [key in NoticeOfDisputeKeys]?: AbstractControl;
}
export interface NoticeOfDisputeFormGroup extends FormGroup {
  value: NoticeOfDispute;
  controls: NoticeOfDisputeFormControls;
}

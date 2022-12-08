import { DatePipe } from "@angular/common";
import { Injectable } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { FormGroupValidators } from "@core/validators/form-group.validators";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { DisputeStatusDialogComponent } from "@shared/dialogs/dispute-status-dialog/dispute-status-dialog.component";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { DisputeCount, DisputesService, NoticeOfDispute as NoticeOfDisputeBase, DisputeCountPleaCode, DisputeCountRequestCourtAppearance, DisputeRepresentedByLawyer, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, SearchDisputeResult, DisputeStatus } from "app/api";
import { AppRoutes } from "app/app.routes";
import { DisputeStore } from "app/store";
import { BehaviorSubject, map, Observable, of } from "rxjs";
import { ViolationTicketService } from "./violation-ticket.service";

@Injectable({
  providedIn: "root",
})
export class DisputeService {
  private _noticeOfDispute: BehaviorSubject<NoticeOfDispute> = new BehaviorSubject<NoticeOfDispute>(null);
  private _countFormDefaultValue: any;
  private _additionFormDefaultValue: any;

  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;
  RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  PleaCode = DisputeCountPleaCode;
  Status = DisputeStatus;

  statusOfUpdatable = [
    this.Status.New,
    this.Status.Processing,
  ]

  ticketFormFields = {
    disputant_surname: [null, [Validators.required]],
    disputant_given_names: [null, [Validators.required]],
    address: [null, [Validators.required, Validators.maxLength(300)]],
    address_city: [null, [Validators.required]],
    address_province: [null, [Validators.required, Validators.maxLength(30)]],
    country: ["Canada", [Validators.required]],
    postal_code: [null, [Validators.required]],
    home_phone_number: [null, [FormControlValidators.phone]],
    work_phone_number: [null, [FormControlValidators.phone]], // not using now
    email_address: [null, [Validators.required, Validators.email]],
    disputant_birthdate: [null, [Validators.required]],
    drivers_licence_number: [null, [Validators.required, Validators.minLength(7), Validators.maxLength(9)]],
    drivers_licence_province: [null, [Validators.required]],
    dispute_counts: [],
  }

  countFormFields = {
    plea_cd: [null],
    request_time_to_pay: this.RequestTimeToPay.N,
    request_reduction: this.RequestReduction.N,
    request_court_appearance: [null, [Validators.required]],
    __skip: [false],
    __apply_to_remaining_counts: [false],
  }

  additionFormFields = {
    represented_by_lawyer: [this.RepresentedByLawyer.N],
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
    input.disputant_birthdate = this.datePipe.transform(input.disputant_birthdate, "yyyy-MM-dd");
    input.issued_date = this.datePipe.transform(input.issued_date, "yyyy-MM-ddTHH:mm:ss");
    input = this.splitGivenNames(input);  // break disputant names into first, second, third
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
              this.router.navigate([AppRoutes.disputePath(AppRoutes.EMAILVERIFICATIONREQUIRED)], {
                queryParams: {
                  email: input.email_address,
                  token: res.noticeOfDisputeId
                },
              });
            }
            else {
              this.router.navigate([AppRoutes.disputePath(AppRoutes.SUBMIT_SUCCESS)], {
                queryParams: {
                  ticketNumber: input.ticket_number,
                  time: this.datePipe.transform(input.issued_date, "HH:mm"),
                },
              });
            }
          })
        }
      });
  }

  resendVerificationEmail(noticeOfDisputeId: string): Observable<any> {
    return this.disputesService.apiDisputesEmailUuidHashResendPut(noticeOfDisputeId);
  }

  verifyEmail(token: string): Observable<any> {
    return this.disputesService.apiDisputesEmailVerifyPut(token);
  }

  searchDispute(params: QueryParamsForSearch): Observable<SearchDisputeResult> {
    return this.disputesService.apiDisputesSearchGet(params.ticketNumber, params.time)
      .pipe(
        map((response: SearchDisputeResult) => {
          return response;
        })
      );
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

  splitGivenNames(noticeOfDispute: NoticeOfDispute): NoticeOfDispute {
    // split up where spaces occur and stuff in given names 1,2,3
    if (noticeOfDispute.disputant_given_names) {
      let givenNames = noticeOfDispute.disputant_given_names.split(" ");
      if (givenNames.length > 0) noticeOfDispute.disputant_given_name1 = givenNames[0];
      if (givenNames.length > 1) noticeOfDispute.disputant_given_name2 = givenNames[1];
      if (givenNames.length > 2) noticeOfDispute.disputant_given_name3 = givenNames[2];
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
    countsActions.not_request_court_appearance = toCountStr(counts.filter(i => i.request_court_appearance === this.RequestCourtAppearance.N));
    countsActions.guilty = toCountStr(counts.filter(i => i.plea_cd === this.PleaCode.G));
    countsActions.not_guilty = toCountStr(counts.filter(i => i.plea_cd === this.PleaCode.N));
    countsActions.request_reduction = toCountStr(counts.filter(i => i.request_reduction === this.RequestReduction.Y));
    countsActions.request_time_to_pay = toCountStr(counts.filter(i => i.request_time_to_pay === this.RequestTimeToPay.Y));
    countsActions.request_court_appearance = toCountStr(counts.filter(i => i.request_court_appearance === this.RequestCourtAppearance.Y));
    return countsActions;
  }

  getNoticeOfDispute(formValue): NoticeOfDispute {
    // form contains all sub forms
    // get the ticket from storage to make sure the user can't change the ticket info
    return <NoticeOfDispute>{ ...this.violationTicketService.ticket, ...formValue };
  }

  showDisputeStatus(data: DisputeStore.StateData): void {
    this.dialog.open(DisputeStatusDialogComponent, {
      width: "60vw",
      data: data
    })
  }
}

export interface NoticeOfDispute extends NoticeOfDisputeBase {
  disputant_given_names?: string;
  lawyer_full_name?: string;
  address?: string;
}

import { DatePipe } from "@angular/common";
import { Placeholder } from "@angular/compiler/src/i18n/i18n_ast";
import { Injectable } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { FormGroupValidators } from "@core/validators/form-group.validators";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { DisputeCount, DisputesService, NoticeOfDispute, DisputeCountPleaCode, DisputeCountRequestCourtAppearance, DisputeRepresentedByLawyer, DisputeCountRequestTimeToPay, DisputeCountRequestReduction } from "app/api";
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
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public RequestTimeToPay = DisputeCountRequestTimeToPay;
  public RequestReduction = DisputeCountRequestReduction;
  public RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  public PleaCode = DisputeCountPleaCode;

  public ticketFormFields = {
    disputant_surname: [null, [Validators.required]],
    disputant_given_names: [null, [Validators.required]],
    address: [null, [Validators.required]],
    address_city: [null, [Validators.required]],
    address_province: [null, [Validators.required, Validators.maxLength(30)]],
    country: ["Canada", [Validators.required]],
    postal_code: [null, [Validators.required]],
    home_phone_number: [null, [Validators.required, FormControlValidators.phone]],
    work_phone_number: [null, [FormControlValidators.phone]], // not using now
    email_address: [null, [Validators.required, Validators.email]],
    disputant_birthdate: [null, [Validators.required]],
    drivers_licence_number: [null, [Validators.required, Validators.minLength(7), Validators.maxLength(9)]],
    drivers_licence_province: [null, [Validators.required]],
    dispute_counts: [],
  }

  public countFormFields = {
    plea_cd: [null],
    request_time_to_pay: this.RequestTimeToPay.N,
    request_reduction: this.RequestReduction.N,
    request_court_appearance: [null, [Validators.required]],
    __skip: [false],
    __apply_to_remaining_counts: [false],
  }

  public additionFormFields = {
    represented_by_lawyer: [this.RepresentedByLawyer.N],
    interpreter_language: [null],
    witness_no: [0],
    fine_reduction_reason: [null, []],
    time_to_pay_reason: [null, []],

    __witness_present: [false],
    __interpreter_required: [false],
  }

  public additionFormValidators = [
    FormGroupValidators.requiredIfTrue("__interpreter_required", "interpreter_language"),
    FormGroupValidators.requiredIfTrue("__witness_present", "witness_no"),
  ]

  public legalRepresentationFields = {
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

  public get noticeOfDispute$(): Observable<NoticeOfDispute> {
    return this._noticeOfDispute.asObservable();
  }

  public get noticeOfDispute(): NoticeOfDispute {
    return this._noticeOfDispute.value;
  }

  public get countFormDefaultValue(): any {
    return this._countFormDefaultValue;
  }

  public get additionFormDefaultValue(): any {
    return this._additionFormDefaultValue;
  }

  public createNoticeOfDispute(input: NoticeOfDisputeExtended): void {
    input.disputant_birthdate = this.datePipe.transform(input.disputant_birthdate, "yyyy-MM-dd");
    input.issued_date = this.datePipe.transform(input.issued_date, "yyyy-MM-ddTHH:mm:ss");
    input = this.splitGivenNames(input);  // break disputant names into first, second, third
    input = this.splitLawyerNames(input); // break lawyer names into first, second, surname

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
          console.log("about to submit", input);
          return this.disputesService.apiDisputesCreatePost(input).subscribe(res => {
            this._noticeOfDispute.next(input);
            this.router.navigate([AppRoutes.disputePath(AppRoutes.SUBMIT_SUCCESS)], {
              queryParams: {
                ticketNumber: input.ticket_number,
                time: this.datePipe.transform(input.issued_date, "HH:mm"),
              },
            });
          })
        }
      });
  }

  public splitGivenNames(noticeOfDisputeExtended: NoticeOfDisputeExtended):NoticeOfDisputeExtended {
    let noticeOfDispute = noticeOfDisputeExtended;

    // split up where spaces occur and stuff in given names 1,2,3
    if (noticeOfDisputeExtended.disputant_given_names) {
      let givenNames = noticeOfDisputeExtended.disputant_given_names.split(" ");
      if (givenNames.length > 0)noticeOfDispute.disputant_given_name1 = givenNames[0];
      if (givenNames.length > 1) noticeOfDispute.disputant_given_name2 = givenNames[1];
      if (givenNames.length > 2) noticeOfDispute.disputant_given_name3 = givenNames[2];
    }

    return noticeOfDispute;
  }

  public splitLawyerNames(noticeOfDisputeExtended: NoticeOfDisputeExtended):NoticeOfDisputeExtended {
    let noticeOfDispute = noticeOfDisputeExtended;

    // split up where spaces occur and stuff in given names 1,2,3
    if (noticeOfDisputeExtended.lawyer_full_name) {
      let lawyerNames = noticeOfDisputeExtended.lawyer_full_name.split(" ");
      if (lawyerNames.length > 0)noticeOfDispute.lawyer_surname = lawyerNames[lawyerNames.length - 1]; // last one
      if (lawyerNames.length > 1) noticeOfDispute.lawyer_given_name1 = lawyerNames[0];
      if (lawyerNames.length > 2) noticeOfDispute.lawyer_given_name2 = lawyerNames[1];
    }

    return noticeOfDispute;
  }

  public getCountsActions(counts: DisputeCount[]): any {
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

  public getNoticeOfDispute(formValue): NoticeOfDispute {
    // form contains all sub forms
    // get the ticket from storage to make sure the user can't change the ticket info
    return <NoticeOfDispute>{ ...this.violationTicketService.ticket, ...formValue };
  }
}
export interface NoticeOfDisputeExtended extends NoticeOfDispute {
  disputant_given_names?: string;
  lawyer_full_name?: string;
}

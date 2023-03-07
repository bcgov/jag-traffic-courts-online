import { DatePipe } from "@angular/common";
import { Injectable } from "@angular/core";
import { FormBuilder, FormControl, ValidatorFn, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { FormGroupValidators } from "@core/validators/form-group.validators";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { DisputeFormMode } from "@shared/enums/dispute-form-mode";
import { CountsActions, DisputeCount, DisputeCountFormControls, DisputeCountFormGroup, NoticeOfDispute, NoticeOfDisputeFormControls, NoticeOfDisputeFormGroup, NoticeOfDisputeFormConfigs, DisputeCountFormConfigs } from "@shared/models/dispute-form.model";
import { DisputeRequestCourtAppearanceYn, DisputeContactTypeCd, DisputesService, DisputeCountPleaCode, DisputeRepresentedByLawyer, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, ViolationTicket, ViolationTicketCount, DisputeInterpreterRequired } from "app/api";
import { AppRoutes } from "app/app.routes";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class NoticeOfDisputeService {
  private _noticeOfDispute: BehaviorSubject<NoticeOfDispute> = new BehaviorSubject<NoticeOfDispute>(null);

  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  PleaCode = DisputeCountPleaCode;
  InterpreterRequired = DisputeInterpreterRequired;
  ContactType = DisputeContactTypeCd;

  noticeOfDisputeFormConfigs: NoticeOfDisputeFormConfigs = {
    disputant_surname: { value: null, options: { validators: [Validators.required] } },
    disputant_given_names: { value: null, options: { validators: [Validators.required] } },
    contact_given_names: null,
    contact_surname: null,
    contact_law_firm_name: null,
    contact_type: { value: this.ContactType.Individual, options: { validators: [Validators.required] } },
    address: { value: null, options: { validators: [Validators.required, Validators.maxLength(300)] } },
    address_city: { value: null, options: { validators: [Validators.required] } },
    address_province: { value: null, options: { validators: [Validators.required, Validators.maxLength(30)] } },
    address_province_country_id: null,
    address_country_id: { value: null, options: { validators: [Validators.required] } },
    address_province_seq_no: { value: null, options: { validators: [Validators.required] } },
    postal_code: { value: null, options: { validators: [Validators.required] } },
    home_phone_number: { value: null, options: { validators: [FormControlValidators.phone] } },
    email_address: { value: null, options: { validators: [Validators.required, Validators.email] } },
    drivers_licence_number: { value: null, options: { validators: [Validators.required, Validators.minLength(7), Validators.maxLength(9)] } },
    drivers_licence_province: { value: null, options: { validators: [Validators.required] } },
    drivers_licence_country_id: null,
    drivers_licence_province_seq_no: null
  }
  
  countFormConfigs: DisputeCountFormConfigs = {
    count_no: null,
    plea_cd: null,
    request_time_to_pay: this.RequestTimeToPay.N,
    request_reduction: this.RequestReduction.N,
    __skip: false,
    __apply_to_remaining_counts: false,
  }
  
  additionFormConfigs: NoticeOfDisputeFormConfigs = {
    represented_by_lawyer: this.RepresentedByLawyer.N,
    interpreter_language_cd: null,
    witness_no: 0,
    fine_reduction_reason: null,
    time_to_pay_reason: null,

    __witness_present: false,
    interpreter_required: this.InterpreterRequired.N,
  }

  additionFormValidators: ValidatorFn[] = [
    FormGroupValidators.requiredIfTrue("__witness_present", "witness_no"),
  ]

  legalRepresentationConfigs: NoticeOfDisputeFormConfigs = {
    law_firm_name: { value: null, options: { validators: [Validators.required] } },
    lawyer_full_name: { value: null, options: { validators: [Validators.required] } },
    lawyer_email: { value: null, options: { validators: [Validators.required] } },
    lawyer_phone_number: { value: null, options: { validators: [Validators.required] } },
    lawyer_address: { value: null, options: { validators: [Validators.required] } }
  }

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private disputesService: DisputesService,
    private datePipe: DatePipe,
    private fb: FormBuilder,
  ) {
  }

  getNoticeOfDisputeForm(
    noticeOfDispute: ViolationTicket | NoticeOfDispute = {},
    configs: NoticeOfDisputeFormConfigs = this.noticeOfDisputeFormConfigs
  ): NoticeOfDisputeFormGroup {
    var controls: NoticeOfDisputeFormControls = {};
    Object.keys(configs).forEach(key => {
      let config = configs[key];
      let value = config?.value === undefined ? config : config?.value;
      controls[key] = new FormControl(value, config?.options);
    })
    let form = this.fb.group(controls);

    noticeOfDispute = noticeOfDispute ? noticeOfDispute : {};
    Object.keys(noticeOfDispute).forEach(key => {
      noticeOfDispute[key] && form.controls[key]?.patchValue(noticeOfDispute[key]);
    });
    noticeOfDispute.drivers_licence_number && form.controls.drivers_licence_number?.setValue(noticeOfDispute.drivers_licence_number.toString());
    return form;
  }

  getCountForm(ticket_count?: ViolationTicketCount, dispute_count?: DisputeCount, isUpdate?: boolean): DisputeCountFormGroup {
    let controls: DisputeCountFormControls = {};
    let configs = this.countFormConfigs; // create all fields
    Object.keys(configs).forEach(key => {
      let config = configs[key];
      let value = config?.value === undefined ? config : config?.value;
      controls[key] = new FormControl(value, config?.options);
    })
    let form = this.fb.group(controls);
    form.patchValue(ticket_count); // set count_no
    if (isUpdate && !dispute_count) {
      form.controls.__skip.setValue(true);
    } else if (dispute_count) {
      form.patchValue(dispute_count); // set others
    }
    return form;
  }

  getAdditionalForm(additionalInfo: ViolationTicket | NoticeOfDispute = {}): NoticeOfDisputeFormGroup {
    let form = this.getNoticeOfDisputeForm(additionalInfo, this.additionFormConfigs);
    form.addValidators(this.additionFormValidators);
    return form;
  }

  getLegalRepresentationForm(legalRepresentation: ViolationTicket | NoticeOfDispute = {}): NoticeOfDisputeFormGroup {
    return this.getNoticeOfDisputeForm(legalRepresentation, this.legalRepresentationConfigs);
  }

  get noticeOfDispute$(): Observable<NoticeOfDispute> {
    return this._noticeOfDispute.asObservable();
  }

  get noticeOfDispute(): NoticeOfDispute {
    return this._noticeOfDispute.value;
  }

  get countFormDefaultValue(): any {
    return this.countFormConfigs;
  }

  createNoticeOfDispute(input: NoticeOfDispute): void {
    input.issued_date = this.datePipe.transform(input.issued_date, "yyyy-MM-ddTHH:mm:ss");
    input = this.splitDisputantGivenNames(input);  // break disputant names into first, second, third
    input = this.splitContactGivenNames(input);  // break disputant names into first, second, third
    input = this.splitLawyerNames(input); // break lawyer names into first, second, surname
    input = this.splitAddressLines(input); // break address into line 1,2,3 by comma

    input.dispute_counts.forEach(count => { // TODO: remove this once request_court_appearance removed from dispute count schema, API
      count.request_court_appearance = input.request_court_appearance;
    });

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

  getCountsActions(counts: DisputeCount[]): CountsActions {
    let countsActions: CountsActions = {};
    let toCountStr = (arr: DisputeCount[]) => arr.map(i => "Count " + i.count_no).join(", ");
    countsActions.guilty = toCountStr(counts.filter(i => i.plea_cd === this.PleaCode.G));
    countsActions.not_guilty = toCountStr(counts.filter(i => i.plea_cd === this.PleaCode.N));
    countsActions.request_reduction = toCountStr(counts.filter(i => i.request_reduction === this.RequestReduction.Y));
    countsActions.request_time_to_pay = toCountStr(counts.filter(i => i.request_time_to_pay === this.RequestTimeToPay.Y));
    return countsActions;
  }

  getNoticeOfDispute(ticket: ViolationTicket | NoticeOfDispute, formValue: NoticeOfDispute): NoticeOfDispute {
    // form contains all sub forms
    // get the ticket from storage to make sure the user can't change the ticket info
    return <NoticeOfDispute>{ ...ticket, ...formValue };
  }
}

// Export in service, much clear when accessing
// Overriding models in app/api
export * from "../shared/models/dispute-form.model";

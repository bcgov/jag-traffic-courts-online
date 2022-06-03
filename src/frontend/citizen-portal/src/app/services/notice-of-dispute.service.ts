import { DatePipe } from "@angular/common";
import { Injectable } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { FormGroupValidators } from "@core/validators/form-group.validators";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { DisputedCount, DisputesService, NoticeOfDispute, Plea } from "app/api";
import { AppRoutes } from "app/app.routes";
import { BehaviorSubject } from "rxjs";
import { ViolationTicketService } from "./violation-ticket.service";

@Injectable({
  providedIn: "root",
})
export class NoticeOfDisputeService {
  private _noticeOfDispute: BehaviorSubject<NoticeOfDispute> = new BehaviorSubject<NoticeOfDispute>(null);
  private _countFormDefaultValue: any;
  private _additionFormDefaultValue: any;

  public ticketFormFields = {
    surname: [null, [Validators.required]],
    given_names: [null, [Validators.required]],
    address: [null, [Validators.required]],
    city: [null, [Validators.required]],
    province: [null, [Validators.required, Validators.maxLength(30)]],
    country: [{value: "Canada"}, [Validators.required]],
    postal_code: [null, [Validators.required]],
    home_phone_number: [null, [Validators.required, FormControlValidators.phone]],
    work_phone_number: [null, [FormControlValidators.phone]], // not using now
    email_address: [null, [Validators.required, Validators.email]],
    birthdate: [null, [Validators.required]],
    drivers_licence_number: [null, [Validators.required]],
    drivers_licence_province: [null, [Validators.required]],
    disputed_counts: [],
  }

  public countFormFields = {
    plea: [null],
    request_time_to_pay: [false],
    request_reduction: [false],
    appear_in_court: [null, [Validators.required]],
    __skip: [false],
    __apply_to_remaining_counts: [false],
  }

  public additionFormFields = {
    represented_by_lawyer: [false],
    interpreter_language: [null],
    number_of_witness: [0],
    fine_reduction_reason: [null, []],
    time_to_pay_reason: [null, []],

    __witness_present: [false],
    __interpreter_required: [false],
  }

  public additionFormValidators = [
    FormGroupValidators.requiredIfTrue("__interpreter_required", "interpreter_language"),
    FormGroupValidators.requiredIfTrue("__witness_present", "number_of_witness"),
  ]

  public legalRepresentationFields = {
    law_firm_name: [null, [Validators.required]],
    lawyer_full_name: [null, [Validators.required]],
    lawyer_email: [null, [Validators.required, Validators.email]],
    lawyer_phone: [null, [Validators.required]],
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

  public get noticeOfDispute$(): BehaviorSubject<NoticeOfDispute> {
    return this._noticeOfDispute;
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

  public createNoticeOfDispute(input: NoticeOfDispute): void {
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
          input.disputed_counts = input.disputed_counts.filter(i => i.plea);
          return this.disputesService.apiDisputesCreatePost(input).subscribe(res => {
            this.noticeOfDispute$.next(input);
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

  public getCountsActions(counts: DisputedCount[]): any {
    let countsActions: any = {};
    let fields = Object.keys(this.countFormFields);
    fields.forEach(field => {
      if (counts && counts.length > 0) {
        countsActions[field] = counts.filter(i => i[field]).map(i => "Count " + i.count).join(", ");
      } else {
        countsActions[field] = [];
      }
    });
    countsActions.not_appear_in_court = counts.filter(i => i.appear_in_court === false).map(i => "Count " + i.count).join(", ");
    return countsActions;
  }

  public getNoticeOfDispute(formValue): NoticeOfDispute {
    // form contains all sub forms
    // get the ticket from storage to make sure the user can't change the ticket info
    return <NoticeOfDispute>{ ...this.violationTicketService.ticket, ...formValue };
  }
}

import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatStepper } from "@angular/material/stepper";
import { MatCheckboxChange } from "@angular/material/checkbox";
import { MatDialog } from "@angular/material/dialog";
import { TranslateService } from "@ngx-translate/core";
import { ToastService } from "@core/services/toast.service";
import { UtilsService } from "@core/services/utils.service";
import { FormUtilsService } from "@core/services/form-utils.service";
import { ConfigService } from "@config/config.service";
import { TicketTypes } from "@shared/enums/ticket-type.enum";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { FormErrorStateMatcher } from "@shared/directives/form-error-state-matcher.directive";
import { ViolationTicket, DisputeCountPleaCode, DisputeRequestCourtAppearanceYn, DisputeRepresentedByLawyer, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, Language } from "app/api";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { NoticeOfDisputeService, NoticeOfDispute, NoticeOfDisputeFormGroup } from "app/services/notice-of-dispute.service";
import { LookupsService } from "app/services/lookups.service";
import { DisputeFormMode } from "@shared/enums/dispute-form-mode";
import { Observable } from "rxjs";

@Component({
  selector: "app-dispute-stepper",
  templateUrl: "./dispute-stepper.component.html",
  styleUrls: ["./dispute-stepper.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DisputeStepperComponent implements OnInit, AfterViewInit {
  @ViewChild(MatStepper) private stepper: MatStepper;

  mode: DisputeFormMode = DisputeFormMode.CREATE;
  previousButtonIcon = "keyboard_arrow_left";
  defaultLanguage: string;
  ticketTypes = TicketTypes;
  Plea = DisputeCountPleaCode;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;

  form: NoticeOfDisputeFormGroup;
  countForms: FormArray;
  additionalForm: FormGroup;
  legalRepresentationForm: FormGroup;
  ticket: ViolationTicket;
  ticket$: Observable<ViolationTicket>;
  noticeOfDispute: NoticeOfDispute;
  ticketType: string;
  matcher = new FormErrorStateMatcher();

  // Additional
  countsActions: any;
  customWitnessOption = false;
  minWitnesses = 1;
  maxWitnesses = 99;
  countIndex: number = 1;
  additionalIndex: number = 2;

  // Summary
  declared = false;

  // Consume from the service
  languages: Language[] = [];
  private ticketFormFields = this.noticeOfDisputeService.ticketFormFields;
  private countFormFields = this.noticeOfDisputeService.countFormFields;
  private countFormDefaultValue = this.noticeOfDisputeService.countFormDefaultValue;
  private additionFormFields = this.noticeOfDisputeService.additionFormFields;
  private additionFormValidators = this.noticeOfDisputeService.additionFormValidators;
  private legalRepresentationFields = this.noticeOfDisputeService.legalRepresentationFields;

  constructor(
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    private violationTicketService: ViolationTicketService,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private utilsService: UtilsService,
    private formUtilsService: FormUtilsService,
    private translateService: TranslateService,
    private toastService: ToastService,
    private config: ConfigService,
    private lookups: LookupsService
  ) {
    // config or static
    this.defaultLanguage = this.translateService.getDefaultLang();

    this.lookups.languages$.subscribe(languages => {
      this.languages = languages;
    })
  }

  ngOnInit(): void {
    this.ticket = this.violationTicketService.ticket;
    if (!this.ticket) {
      this.violationTicketService.goToFind();
      return;
    }
    this.ticket$ = this.violationTicketService.ticket$;
    this.ticketType = this.violationTicketService.ticketType;

    // build inner object array before the form
    let countArray = [];
    this.ticket.counts.forEach(count => {
      countArray.push(this.formBuilder.group({ ...count, ...this.countFormFields }));
    })
    this.countForms = this.formBuilder.array(countArray);
    this.countForms.controls.forEach(countform => {countform.patchValue({ ...this.countFormDefaultValue, __skip: false })});

    // additional form fields
    this.additionalForm = this.formBuilder.group({
      ...this.additionFormFields,
    }, {
      validators: [...this.additionFormValidators]
    });

    // build form
    this.form = this.formBuilder.group({
      ...this.ticketFormFields,
    });
    this.form.reset();

    // take info from ticket, convert dl number to string
    Object.keys(this.ticket).forEach(key => {
      this.ticket[key] && this.form.get(key)?.patchValue(this.ticket[key]);
    });
    this.ticket.drivers_licence_number && this.form.controls.drivers_licence_number.setValue(this.ticket.drivers_licence_number.toString());

    this.legalRepresentationForm = this.formBuilder.group(this.legalRepresentationFields);
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.stepper?.selectionChange.subscribe(event => { // stepper change subscription
        if (event.previouslySelectedIndex < event.selectedIndex) {
          this.onStepSave();
        }
        this.scrollToSectionHook(); // correct angular faulty vertical scrolling
      })
    }, 0)
  }

  // make sure to scroll to top of mat-step
  // angular doesnt do this right on its own :) https://github.com/angular/components/issues/8881
  private scrollToSectionHook() {
    const stepId = this.stepper._getStepLabelId(this.stepper.selectedIndex);
    const stepElement = document.getElementById(stepId);
    if (stepElement) {
      setTimeout(() => {
        stepElement.scrollIntoView({ block: "start", inline: "nearest", behavior: "smooth" });
      }, 250);
    }
  }

  private getCountFormInitValue(count) {
    return { ...this.countFormDefaultValue, ...count };
  }

  private getCountsActions() {
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.countForms.value);
  }

  onStepCancel(): void {
    this.violationTicketService.goToInitiateResolution();
  }

  onStepSave(): void {
    let isValid = this.formUtilsService.checkValidity(this.form);

    if (this.stepper.selectedIndex === this.countIndex) {
      this.countForms.controls.forEach(countForm => {
        if (countForm.value.request_time_to_pay === this.RequestTimeToPay.Y || countForm.value.request_reduction === this.RequestReduction.Y) {
          countForm.patchValue({ plea_cd: this.Plea.G });
        }
        if (countForm.value.__skip) {
          countForm.patchValue({ ...this.getCountFormInitValue(this.ticket.counts[countForm.value.count_no]), __skip: true });
        }
      });
      this.setAdditionalRequired();

    } else if (!isValid) {
      this.utilsService.scrollToErrorSection();
      this.toastService.openErrorToast(this.config.dispute_validation_error);
      return;
    }

    this.getCountsActions();

    this.noticeOfDispute = this.noticeOfDisputeService.getNoticeOfDispute({
      ...this.form.value,
      ...this.additionalForm.value,
      ...this.countForms.value,
      ...this.legalRepresentationForm.value,
      address_country_id: this.form.get("address_country_id").value, // disabled field is not available in this.form.value
      dispute_counts: this.countForms.value
    });
  }

  private setAdditionalRequired() {
    this.getCountsActions();
    this.additionalForm.controls.fine_reduction_reason.clearValidators();
    this.additionalForm.controls.time_to_pay_reason.clearValidators();
    if (this.countsActions.request_reduction.length > 0) {
      this.additionalForm.controls.fine_reduction_reason.addValidators([Validators.required])
    }
    if (this.countsActions.request_time_to_pay.length > 0) {
      this.additionalForm.controls.time_to_pay_reason.addValidators([Validators.required]);
    }
  }

  isValid(): boolean {
    if (this.stepper?.selectedIndex >= this.countIndex) {
      let allCountsValid: boolean = true;
      this.countForms?.controls.forEach(countForm => {
        if (countForm) {
          let valid = countForm.valid || countForm.value.__skip;
          if (this.additionalForm.value.request_court_appearance === this.RequestCourtAppearance.Y) {
            valid = valid && (countForm.value.plea_cd === this.Plea.G || countForm.value.plea_cd === this.Plea.N);
          } else if (this.additionalForm.value.request_court_appearance === this.RequestCourtAppearance.N) {
            valid = valid && ((countForm.value.request_time_to_pay === this.RequestTimeToPay.Y) || (countForm.value.request_reduction === this.RequestReduction.Y));
          }
          allCountsValid = allCountsValid && valid && !this.isAllCountsSkipped;
        }
      });
      if (!allCountsValid) return false;
      else return this.form.valid;
    } else return this.form.valid;
  }

  onChangeRepresentedByLawyer(event: MatCheckboxChange) {
    this.additionalForm.markAsUntouched();
  }

  onChangeWitnessPresent(event: MatCheckboxChange) {
    if (event.checked) {
      this.additionalForm.controls.witness_no.setValidators([Validators.min(this.minWitnesses), Validators.max(this.maxWitnesses), Validators.required]);
    } else {
      this.additionalForm.controls.witness_no.clearValidators();
      this.additionalForm.controls.witness_no.updateValueAndValidity();
    }
  }

  getToolTipDEata(data) {
    if (data) {
      let msg = "";
      this.lookups.languages.forEach(res => {
        if (res.code === data.value) {
          msg = res.description
        }
      })
      return msg;
    } else {
      return "please select a language";
    }
  }

  private get isAllCountsSkipped() {
    if (this.countForms.value.filter(i => i.__skip).length === this.countForms.length) {
      return true;
    }
    else {
      return false;
    }
  }

  onSkipChecked() {
    if (this.isAllCountsSkipped) {
      const data: DialogOptions = {
        titleKey: "Warning",
        actionType: "warn",
        messageKey: `You have selected "Skip this count, no action required" for all counts on your ticket. No request will be created. Please review your selection(s).`,
        actionTextKey: "Close",
        cancelHide: true
      };
      this.dialog.open(ConfirmDialogComponent, { data });
    }
  }

  /**
   * @description
   * Submit the dispute
   */
  public submitDispute(): void {
    this.noticeOfDisputeService.createNoticeOfDispute(this.noticeOfDispute);
  }
}

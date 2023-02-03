import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy, Input, Output, EventEmitter } from "@angular/core";
import { FormArray, FormBuilder, Validators } from "@angular/forms";
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
import { ViolationTicket, DisputeCountPleaCode, DisputeRepresentedByLawyer, DisputeCountRequestCourtAppearance, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, Language, ViolationTicketCount } from "app/api";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { NoticeOfDisputeService, NoticeOfDispute, NoticeOfDisputeFormGroup, CountsActions, DisputeCountFormGroup, DisputeCount, Count } from "app/services/notice-of-dispute.service";
import { LookupsService } from "app/services/lookups.service";
import { DisputeFormMode } from "@shared/enums/dispute-form-mode";

@Component({
  selector: "app-dispute-stepper",
  templateUrl: "./dispute-stepper.component.html",
  styleUrls: ["./dispute-stepper.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DisputeStepperComponent implements OnInit, AfterViewInit {
  @Input() ticket: ViolationTicket | NoticeOfDispute;
  @Input() ticketCounts: ViolationTicketCount[] = [];
  @Input() disputeCounts: DisputeCount[] = [];
  @Input() ticketType: string;
  @Input() mode: DisputeFormMode;
  @Output() saveDispute: EventEmitter<NoticeOfDispute> = new EventEmitter();

  @ViewChild(MatStepper) private stepper: MatStepper;

  previousButtonIcon = "keyboard_arrow_left";
  defaultLanguage: string;
  ticketTypes = TicketTypes;
  Plea = DisputeCountPleaCode;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;

  form: NoticeOfDisputeFormGroup;
  counts: Count[];
  additionalForm: NoticeOfDisputeFormGroup;
  legalRepresentationForm: NoticeOfDisputeFormGroup;
  noticeOfDispute: NoticeOfDispute;
  matcher = new FormErrorStateMatcher();

  // Count
  countIndexes: number[];

  // Additional
  countsActions: CountsActions;
  customWitnessOption = false;
  minWitnesses = 1;
  maxWitnesses = 99;
  additionalIndex: number;

  // Summary
  declared = false;

  // Consume from the service
  languages: Language[] = [];
  private countFormDefaultValue = this.noticeOfDisputeService.countFormDefaultValue;

  constructor(
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
    // build form
    this.form = this.noticeOfDisputeService.getNoticeOfDisputeForm(this.ticket);

    this.counts = this.ticketCounts.map((ticketCount, inx) => {
      return { ticket_count: ticketCount, form: this.noticeOfDisputeService.getCountForm(this.disputeCounts[inx]) };
    })
    this.legalRepresentationForm = this.noticeOfDisputeService.getLegalRepresentationForm(this.ticket);

    this.countIndexes = this.ticketCounts.map(i => i.count_no);
    let lastCountInx = this.countIndexes[this.countIndexes.length - 1]
    this.additionalIndex = lastCountInx + 1;
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

  private getCountFormInitValue(count): DisputeCount {
    return { ...this.countFormDefaultValue, ...count };
  }

  onAttendHearingChange(countForm: DisputeCountFormGroup, event): void {
    countForm.patchValue({ ...this.countFormDefaultValue, request_court_appearance: event.value, __skip: false });
  }

  onStepCancel(): void {
    this.violationTicketService.goToInitiateResolution();
  }

  onStepSave(): void {
    let isAdditional = this.stepper.selectedIndex === this.additionalIndex;
    let isValid = this.formUtilsService.checkValidity(this.form);

    if (this.countIndexes?.indexOf(this.stepper.selectedIndex) > -1) {
      let countInx = this.stepper.selectedIndex - 1;
      let countForm = this.counts[countInx].form;
      if (countForm.value.request_time_to_pay === this.RequestTimeToPay.Y || countForm.value.request_reduction === this.RequestReduction.Y) {
        countForm.patchValue({ plea_cd: this.Plea.G });
      }
      if (countForm.value.__skip) {
        countForm.patchValue({ ...this.getCountFormInitValue(this.ticketCounts[countInx]), __skip: true });
      }
      if (countForm.value.__apply_to_remaining_counts && countInx + 1 < this.counts.length) {
        let value = this.counts[countInx].form.value;
        for (let i = countInx; i < this.counts.length; i++) {
          this.counts[i].form.patchValue({ ...value, ...this.ticketCounts[i], __apply_to_remaining_counts: false })
        }
      }
      this.setAdditional();
    } else if (!isValid) {
      this.utilsService.scrollToErrorSection();
      this.toastService.openErrorToast(this.config.dispute_validation_error);
      return;
    }

    if (isAdditional) {
      this.noticeOfDispute = this.noticeOfDisputeService.getNoticeOfDispute(this.ticket, {
        ...this.form.value,
        ...this.additionalForm.value,
        ...this.legalRepresentationForm.value,
        dispute_counts: this.counts.map(i => i.form.value)
      });
    } else {
      this.noticeOfDispute = null;
    }
  }

  private setAdditional() {
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.counts.map(i => i.form.value));
    this.additionalForm = this.noticeOfDisputeService.getAdditionalForm(this.ticket);

    if (this.countsActions.request_reduction.length > 0 && !this.additionalForm.controls.fine_reduction_reason.hasValidator(Validators.required)) {
      this.additionalForm.controls.fine_reduction_reason.addValidators(Validators.required);
    }
    if (this.countsActions.request_time_to_pay.length > 0 && !this.additionalForm.controls.time_to_pay_reason.hasValidator(Validators.required)) {
      this.additionalForm.controls.time_to_pay_reason.addValidators(Validators.required);
    }
  }

  isValid(countForm?: DisputeCountFormGroup): boolean {
    if (countForm) {
      let valid = countForm.valid;
      if (countForm.value.request_court_appearance === this.RequestCourtAppearance.Y) {
        valid = valid && (countForm.value.plea_cd === this.Plea.G || countForm.value.plea_cd === this.Plea.N);
      } else if (countForm.value.request_court_appearance === this.RequestCourtAppearance.N) {
        valid = valid && ((countForm.value.request_time_to_pay === this.RequestTimeToPay.Y) || (countForm.value.request_reduction === this.RequestReduction.Y));
      }
      return (valid || countForm.value.__skip) && !this.isAllCountsSkipped;
    }
    return this.form.valid;
  }

  isAdditionalFormValid(): boolean {
    var result = true;
    this.counts?.forEach((count: Count) => {
      if (!this.isValid(count?.form)) {
        result = false;
      }
    });
    if (this.additionalForm?.value.represented_by_lawyer === this.RepresentedByLawyer.Y && !this.legalRepresentationForm?.valid) {
      result = false;
    }
    return result && this.additionalForm?.valid;
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
    if (this.counts?.filter(i => i.form?.value.__skip).length === this.counts.length) {
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

  submitDispute() {
    this.saveDispute.emit(this.noticeOfDispute);
  }
}

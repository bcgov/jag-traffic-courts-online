import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy, Input, Output, EventEmitter, ElementRef } from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
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
import { ViolationTicket, DisputeCountPleaCode, DisputeRepresentedByLawyer, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, Language, ViolationTicketCount, DisputeRequestCourtAppearanceYn, DisputeInterpreterRequired } from "app/api";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { NoticeOfDisputeService, NoticeOfDispute, NoticeOfDisputeFormGroup, CountsActions, DisputeCount, Count, DisputeCountFormGroup } from "app/services/notice-of-dispute.service";
import { LookupsService } from "app/services/lookups.service";
import { DisputeFormMode } from "@shared/enums/dispute-form-mode";
import { Observable } from "rxjs";
import { DisputeStore } from "app/store";
import { Store } from "@ngrx/store";
import { FileMetadata } from "app/services/dispute.service";
import { AppConfigService } from "app/services/app-config.service";

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
  @Input() fileData$: Observable<FileMetadata[]>;
  @Input() ticketType: string;
  @Input() mode: DisputeFormMode;
  @Output() saveDispute: EventEmitter<NoticeOfDispute> = new EventEmitter();

  @ViewChild(MatStepper) private stepper: MatStepper;
  @ViewChild("fileInput") private fileInput: ElementRef;

  previousButtonIcon = "keyboard_arrow_left";
  defaultLanguage: string;
  adjournmentFormLink: string;
  disputeFormMode = DisputeFormMode;
  ticketTypes = TicketTypes;
  Plea = DisputeCountPleaCode;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;
  InterpreterRequired = DisputeInterpreterRequired;

  form: NoticeOfDisputeFormGroup;
  requestCourtAppearanceFormControl: FormControl<DisputeRequestCourtAppearanceYn> = new FormControl(null, [Validators.required]);
  counts: Count[];
  additionalForm: NoticeOfDisputeFormGroup;
  legalRepresentationForm: NoticeOfDisputeFormGroup;
  noticeOfDispute: NoticeOfDispute;
  matcher = new FormErrorStateMatcher();

  // TODO: use ViewChild to detect instead of hardcode
  countStepIndex: number = 1;

  // Additional
  countsActions: CountsActions;
  customWitnessOption = false;
  minWitnesses = 1;
  maxWitnesses = 99;

  // Summary
  declared = false;

  // Upload
  adjournmentFileType = { key: "Adjournment", value: "Application for Adjournment" };
  fileTypes = [
    this.adjournmentFileType,
    { key: "Other", value: "Other" },
  ]
  fileTypeToUpload: string = this.adjournmentFileType.key;

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
    private store: Store,
    private lookups: LookupsService,
    private appConfigService: AppConfigService,
  ) {
    // config or static
    this.defaultLanguage = this.translateService.getDefaultLang();
    this.adjournmentFormLink = this.appConfigService.adjournmentFormLink;

    this.lookups.languages$.subscribe(languages => {
      this.languages = languages;
    })
  }

  ngOnInit(): void {
    // build form
    this.form = this.noticeOfDisputeService.getNoticeOfDisputeForm(this.ticket);
    this.requestCourtAppearanceFormControl.setValue((<NoticeOfDispute>this.ticket)?.request_court_appearance);

    this.counts = this.ticketCounts.map(ticketCount => {
      var dispute_count = this.disputeCounts.filter(i => i.count_no === ticketCount.count_no).shift();
      return {
        ticket_count: ticketCount,
        form: this.noticeOfDisputeService.getCountForm(ticketCount, dispute_count, this.mode !== DisputeFormMode.CREATE)
      };
    });
    this.legalRepresentationForm = this.noticeOfDisputeService.getLegalRepresentationForm(this.ticket);
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

  onStepCancel(): void {
    this.violationTicketService.goToInitiateResolution();
  }

  onStepSave(): void {
    let isValid = this.formUtilsService.checkValidity(this.form);

    if (this.stepper.selectedIndex === this.countStepIndex) {
      this.counts.forEach(count => {
        let countForm = count.form;
        if (countForm.value.request_time_to_pay === this.RequestTimeToPay.Y || countForm.value.request_reduction === this.RequestReduction.Y) {
          countForm.controls.plea_cd.patchValue(this.Plea.G);
        }
        if (countForm.value.__skip) {
          // TODO: move to onSkipChange
          countForm.patchValue({ ...this.getCountFormInitValue(count.ticket_count), __skip: true, plea_cd: this.Plea.G });
        }
      });
      this.setAdditional();
    } else if (!isValid) {
      this.utilsService.scrollToErrorSection();
      this.toastService.openErrorToast(this.config.dispute_validation_error);
      return;
    }

    if (this.additionalForm?.value.represented_by_lawyer === this.RepresentedByLawyer.N) {
      this.legalRepresentationForm.reset();
    }

    var fileData = [];
    this.fileData$?.subscribe(i => { fileData = i; })
    this.noticeOfDispute = this.noticeOfDisputeService.getNoticeOfDispute(this.ticket, {
      ...this.form.value,
      ...this.additionalForm?.value,
      ...this.legalRepresentationForm?.value,
      request_court_appearance: this.requestCourtAppearanceFormControl.value,
      dispute_counts: this.counts.map(i => i.form.value),
      file_data: fileData
    });
  }

  private setAdditional() {
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.counts.map(i => i.form.value));
    this.additionalForm = this.noticeOfDisputeService.getAdditionalForm(this.ticket);

    if (this.requestCourtAppearanceFormControl.value === this.RequestCourtAppearance.N && this.countsActions.request_reduction.length > 0) {
      this.additionalForm.controls.fine_reduction_reason.addValidators([Validators.required]);
    } else this.additionalForm.controls.fine_reduction_reason.removeValidators([Validators.required]);
    if (this.requestCourtAppearanceFormControl.value === this.RequestCourtAppearance.N && this.countsActions.request_time_to_pay.length > 0) {
      this.additionalForm.controls.time_to_pay_reason.addValidators(Validators.required);
    } else this.additionalForm.controls.time_to_pay_reason.removeValidators([Validators.required]);
  }

  isCountFormsValid(): boolean {
    if (this.stepper?.selectedIndex < this.countStepIndex) {
      return false;
    }

    let allCountsValid: boolean = this.requestCourtAppearanceFormControl.valid;
    this.counts.forEach(count => {
      let countForm = count.form;
      if (countForm) {
        let valid = countForm.valid;
        if (this.requestCourtAppearanceFormControl.value === this.RequestCourtAppearance.Y) {
          valid = valid && (countForm.value.plea_cd === this.Plea.G || countForm.value.plea_cd === this.Plea.N);
        } else if (this.requestCourtAppearanceFormControl.value === this.RequestCourtAppearance.N) {
          valid = valid && ((countForm.value.request_time_to_pay === this.RequestTimeToPay.Y) || (countForm.value.request_reduction === this.RequestReduction.Y));
        }
        allCountsValid = allCountsValid && (valid || countForm.value.__skip);
      }
    });
    return allCountsValid && !this.isAllCountsSkipped;
  }

  isAdditionalFormValid(): boolean {
    var result = this.stepper?.selectedIndex > this.countStepIndex;
    if (this.additionalForm?.value.represented_by_lawyer === this.RepresentedByLawyer.Y && !this.legalRepresentationForm?.valid) {
      result = false;
    }
    return result && this.additionalForm?.valid;
  }

  onChangeRequestCourtAppearance(value: DisputeRequestCourtAppearanceYn) {
    this.noticeOfDispute.request_court_appearance = value;
    this.counts.forEach(count => {
      count.form.controls.plea_cd.setValue(null);
      count.form.controls.request_reduction.setValue(this.RequestReduction.N);
      count.form.controls.request_time_to_pay.setValue(this.RequestTimeToPay.N);
      count.form.controls.__skip.setValue(false);
    })
  }

  onChangeRepresentedByLawyer(event: MatCheckboxChange) {
    let value = event.checked ? this.RepresentedByLawyer.Y : this.RepresentedByLawyer.N;
    this.additionalForm.controls.represented_by_lawyer.setValue(value);
  }

  onChangeInterpreterRequired(event: MatCheckboxChange) {
    let value = event.checked ? this.InterpreterRequired.Y : this.InterpreterRequired.N;
    this.additionalForm.controls.interpreter_required.setValue(value);
    if (value === this.InterpreterRequired.Y) {
      this.additionalForm.controls.interpreter_language_cd.setValidators([Validators.required]);
    } else {
      this.additionalForm.controls.interpreter_language_cd.setValue(null);
      this.additionalForm.controls.interpreter_language_cd.clearValidators();
      this.additionalForm.controls.interpreter_language_cd.updateValueAndValidity();
    }
  }

  onChangeWitnessPresent(event: MatCheckboxChange) {
    if (event.checked) {
      this.additionalForm.controls.witness_no.setValidators([Validators.min(this.minWitnesses), Validators.max(this.maxWitnesses), Validators.required]);
    } else {
      this.additionalForm.controls.witness_no.setValue(null);
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
    return this.counts?.filter(i => i.form?.value.__skip).length === this.counts.length;
  }

  onSkipChecked(form: DisputeCountFormGroup, value: boolean) {
    if (value) {
      form.controls?.plea_cd.setValue(this.Plea.G);
    } else {
      form.controls?.plea_cd.setValue(null);
    }
    form.controls?.request_reduction.setValue(this.RequestReduction.N);
    form.controls?.request_time_to_pay.setValue(this.RequestTimeToPay.N);

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

  onRemoveFile(file: FileMetadata) {
    const data: DialogOptions = {
      titleKey: "Remove File?",
      messageKey: "Are you sure you want to delete file " + file.fileName + "?",
      actionTextKey: "Delete",
      actionType: "warn",
      cancelTextKey: "Cancel",
      icon: "delete"
    };
    this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.store.dispatch(DisputeStore.Actions.RemoveDocument({ file }));
        }
      });
  }

  onGetFile(file: FileMetadata) {
    if (file.pendingFileStream) {
      var url = URL.createObjectURL(file.__penfingFile);
      window.open(url);
    } else {
      this.store.dispatch(DisputeStore.Actions.GetDocument({ fileId: file.fileId }));
    }
  }

  onUploadClicked() {
    if (this.fileTypeToUpload === this.adjournmentFileType.key && (<NoticeOfDispute>this.ticket).appearance_less_than_14_days) {
      const data: DialogOptions = {
        titleKey: "Court hearing scheduled for less than 14 days",
        messageKey: "You are requesting an adjournment within 14 days of your court date. To help ensure that your request for an adjournment is processed on time, please contact the Violation Ticket Centre at 1-877-661-8026. If your adjournment is not able to be processed, you may be deemed guilty and your dispute closed. Would you like to proceed?",
        actionTextKey: "Yes",
        actionType: "primary",
        cancelTextKey: "No",
      };
      this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
        .subscribe((action: any) => {
          if (action) {
            this.fileInput.nativeElement.click();
          }
        });
    } else {
      this.fileInput.nativeElement.click();
    }
  }

  async onUploadFile(files: FileList) {
    if (files.length <= 0) return;

    const blobToBase64 = file => new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = error => reject(error);
    });

    var pendingFileStream = await blobToBase64(files[0]) as string;
    this.store.dispatch(DisputeStore.Actions.AddDocument({ file: files[0], fileType: this.fileTypeToUpload, pendingFileStream }));
    this.fileInput.nativeElement.value = null;
    this.fileTypeToUpload = this.adjournmentFileType.key;
  }

  submitDispute() {
    this.saveDispute.emit(this.noticeOfDispute);
  }
}

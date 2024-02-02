import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy, Input, Output, EventEmitter, ElementRef } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { MatStepper } from "@angular/material/stepper";
import { MatLegacyCheckboxChange as MatCheckboxChange } from "@angular/material/legacy-checkbox";
import { MatLegacyDialog as MatDialog } from "@angular/material/legacy-dialog";
import { TranslateService } from "@ngx-translate/core";
import { ToastService } from "@core/services/toast.service";
import { UtilsService } from "@core/services/utils.service";
import { FormUtilsService } from "@core/services/form-utils.service";
import { ConfigService } from "@config/config.service";
import { TicketTypes } from "@shared/enums/ticket-type.enum";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { FormErrorStateMatcher } from "@shared/directives/form-error-state-matcher.directive";
import { ViolationTicket, DisputeCountPleaCode, DisputeRepresentedByLawyer, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, Language, ViolationTicketCount, DisputeRequestCourtAppearanceYn, DisputeInterpreterRequired, DisputeContactTypeCd, DisputeSignatoryType } from "app/api";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { NoticeOfDisputeService, NoticeOfDispute, NoticeOfDisputeFormGroup, CountsActions, DisputeCount, Count, DisputeCountFormGroup } from "app/services/notice-of-dispute.service";
import { LookupsService } from "app/services/lookups.service";
import { DisputeFormMode } from "@shared/enums/dispute-form-mode";
import { Observable, firstValueFrom } from "rxjs";
import { DisputeStore } from "app/store";
import { Store } from "@ngrx/store";
import { DisputeService, FileMetadata } from "app/services/dispute.service";
import { AppConfigService } from "app/services/app-config.service";
import { FileUtilsService } from "@shared/services/file-utils.service";
import { filter, map } from 'rxjs/operators';

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

  private state: DisputeStore.State;
  previousButtonIcon = "keyboard_arrow_left";
  previousButtonKey = "stepper.backReview";
  defaultLanguage: string;
  adjournmentFormLink: string;
  DisputeFormMode = DisputeFormMode;
  TicketTypes = TicketTypes;
  Plea = DisputeCountPleaCode;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;
  InterpreterRequired = DisputeInterpreterRequired;
  SignatoryType = DisputeSignatoryType;

  form: NoticeOfDisputeFormGroup;
  requestCourtAppearanceFormControl: FormControl<DisputeRequestCourtAppearanceYn> = new FormControl(null, [Validators.required]);
  counts: Count[];
  additionalForm: NoticeOfDisputeFormGroup;
  legalRepresentationForm: NoticeOfDisputeFormGroup;
  noticeOfDispute: NoticeOfDispute;
  matcher = new FormErrorStateMatcher();

  // Form for Dispuntant/Agent Signature Panel
  signatureBoxForm = new FormGroup({
    signatory_type: new FormControl<DisputeSignatoryType>(null, [Validators.required]),
    disputant_signatory_name: new FormControl(null, [Validators.maxLength(100), Validators.required]),
    agent_signatory_name: new FormControl(null, [Validators.maxLength(100), Validators.required]),
  });

  // TODO: use ViewChild to detect instead of hardcode
  countStepIndex: number = 1;

  // Additional
  countsActions: CountsActions;
  customWitnessOption = false;
  minWitnesses = 1;
  maxWitnesses = 99;

  // Summary
  disableSave = false;

  // Upload
  adjournmentFileType = { key: "Adjournment", value: "Application for Adjournment" };
  fileTypes = [
    this.adjournmentFileType,
    { key: "Other", value: "Other" },
  ]
  fileTypeToUpload: string = this.adjournmentFileType.key;
  acceptFileTypes = [
    "image/jpeg",
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "application/pdf",
    ".pdf",
    ".doc",
    ".docx"
  ]

  // Consume from the service
  languages: Language[] = [];
  private countFormDefaultValue = this.noticeOfDisputeService.countFormDefaultValue;

  constructor(
    private dialog: MatDialog,
    private violationTicketService: ViolationTicketService,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private formUtilsService: FormUtilsService,
    private translateService: TranslateService,
    private toastService: ToastService,
    private config: ConfigService,
    private store: Store,
    private lookups: LookupsService,
    private appConfigService: AppConfigService,
    private fileUtilsService: FileUtilsService,
  ) {
    // config or static
    this.defaultLanguage = this.translateService.getDefaultLang();
    this.adjournmentFormLink = this.appConfigService.adjournmentFormLink;

    this.lookups.languages$.pipe(
      map(languages => languages ? languages.filter(lang => lang.code !== 'UNK') : []), // remove UNK and check for null
      map(filteredLanguages => [
        ...filteredLanguages,
        { code: 'OTH', description: 'Other' } // add OTH
      ])
    ).subscribe(languages => {
      this.languages = languages;
    });
  }

  ngOnInit(): void {
    // build form
    this.form = this.noticeOfDisputeService.getNoticeOfDisputeForm(this.ticket);
    this.requestCourtAppearanceFormControl.setValue((<NoticeOfDispute>this.ticket)?.request_court_appearance); // to be removed
    if (this.mode !== DisputeFormMode.CREATE) {
      this.previousButtonKey = "cancel";
      this.store.select(DisputeStore.Selectors.State).subscribe(state => {
        this.state = state;
      })
    }

    this.counts = this.ticketCounts.map(ticketCount => {
      var dispute_count = this.disputeCounts.filter(i => i.count_no === ticketCount.count_no).shift();
      return {
        ticket_count: ticketCount,
        dispute_count: dispute_count,
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
    if (this.mode !== DisputeFormMode.CREATE) {
      this.disputeService.goToUpdateDisputeLanding(this.state.params);
    } else {
      this.violationTicketService.goToInitiateResolution();
    }
  }

  async onStepSave() {
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

    var signatoryName = null;
    if (this.signatureBoxForm?.value.signatory_type === this.SignatoryType.D) {
      signatoryName = this.signatureBoxForm.get('disputant_signatory_name');
    } else if (this.signatureBoxForm?.value.signatory_type === this.SignatoryType.A) {
      signatoryName = this.signatureBoxForm.get('agent_signatory_name');
    }

    let fileData: FileMetadata[] = [];
    this.fileData$?.subscribe(i => { fileData = i; })
    this.noticeOfDispute = this.noticeOfDisputeService.getNoticeOfDispute(this.ticket, {
      ...this.form.value,
      ...this.additionalForm?.value,
      ...this.legalRepresentationForm?.value,
      request_court_appearance: this.requestCourtAppearanceFormControl.value,
      signatory_name: signatoryName?.value,
      signatory_type: this.signatureBoxForm?.value.signatory_type,
      dispute_counts: this.counts.map(i => i.form.value),
      file_data: fileData
    });
  }

  private setAdditional() {
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.counts.map(i => i.form.value));
    this.additionalForm = this.noticeOfDisputeService.getAdditionalForm(this.ticket);
    if (this.mode === DisputeFormMode.UPDATE) {
      this.additionalForm = this.noticeOfDisputeService.getAdditionalForm(this.noticeOfDispute);
      if (this.additionalForm.controls.witness_no?.value > 0) this.additionalForm.controls.__witness_present.setValue(true);
    }

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
          valid = valid && (this.mode === this.DisputeFormMode.UPDATE || this.isSignatureFormValid) && 
          ((countForm.value.request_time_to_pay === this.RequestTimeToPay.Y) || (countForm.value.request_reduction === this.RequestReduction.Y));
        }
        allCountsValid = allCountsValid && (valid || countForm.value.__skip);
      }
    });
    return allCountsValid && (this.mode === this.DisputeFormMode.UPDATE || !this.isAllCountsSkipped);
  }

  private get isSignatureFormValid(): boolean {
    if (this.signatureBoxForm?.value.signatory_type === this.SignatoryType.D) {
      return this.signatureBoxForm?.get('disputant_signatory_name')?.valid;
    } else if (this.signatureBoxForm?.value.signatory_type === this.SignatoryType.A) {
      return this.signatureBoxForm?.get('agent_signatory_name')?.valid;
    }
  }

  isAdditionalFormValid(): boolean {
    var result = this.stepper?.selectedIndex > this.countStepIndex;
    if (this.additionalForm?.value.represented_by_lawyer === this.RepresentedByLawyer.Y && !this.legalRepresentationForm?.valid) {
      result = false;
    }
    return result && this.additionalForm?.valid;
  }

  onChangeRequestCourtAppearance(value: DisputeRequestCourtAppearanceYn) {
    if (typeof value !== "undefined") {
      this.noticeOfDispute.request_court_appearance = value;
      this.counts.forEach(count => {
        count.form.controls.plea_cd.setValue(null);
        count.form.controls.request_reduction.setValue(this.RequestReduction.N);
        count.form.controls.request_time_to_pay.setValue(this.RequestTimeToPay.N);
        count.form.controls.__skip.setValue(false);
      })
      if (value === this.RequestCourtAppearance.Y) {
        this.signatureBoxForm.controls.signatory_type.setValue(null);
        this.signatureBoxForm.controls.disputant_signatory_name.setValue(null);
        this.signatureBoxForm.controls.agent_signatory_name.setValue(null);
        this.signatureBoxForm.controls.signatory_type.clearValidators();
        this.signatureBoxForm.controls.disputant_signatory_name.clearValidators();
        this.signatureBoxForm.controls.agent_signatory_name.clearValidators();
        this.signatureBoxForm.controls.signatory_type.updateValueAndValidity();
        this.signatureBoxForm.controls.disputant_signatory_name.updateValueAndValidity();
        this.signatureBoxForm.controls.agent_signatory_name.updateValueAndValidity();
      }
    }
  }

  onChangeRepresentedByLawyer(event: MatCheckboxChange) {
    this.changeRepresentedByLawyer(event.checked);
  }

  private changeRepresentedByLawyer(checked: boolean) {
    let value = checked ? this.RepresentedByLawyer.Y : this.RepresentedByLawyer.N;
    this.additionalForm.controls.represented_by_lawyer.setValue(value);

    if (checked && this.form.value.contact_type === DisputeContactTypeCd.Lawyer) {
      this.legalRepresentationForm.controls.law_firm_name.patchValue(this.form.value.contact_law_firm_name);

      var contactNames = [this.form.value.contact_surname, this.form.value.contact_given_names]
      this.legalRepresentationForm.controls.lawyer_full_name.patchValue(contactNames.join(" "));

      var address = [this.form.value.address, this.form.value.address_city, this.form.value.address_province, this.form.value.postal_code]
      this.legalRepresentationForm.controls.lawyer_address.patchValue(address.join(", "));
      this.legalRepresentationForm.controls.lawyer_phone_number.patchValue(this.form.value.home_phone_number);
      this.legalRepresentationForm.controls.lawyer_email.patchValue(this.form.value.email_address);
    } else {
      this.legalRepresentationForm.reset();
    }
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

  /**
   * Handles the change of the signature type.
   *
   * @param {DisputeSignatoryType} value - The new value of the signature type.
   */
  onChangeSignatureType(value: DisputeSignatoryType) {
    if (value === this.SignatoryType.D) {
      this.signatureBoxForm.controls.disputant_signatory_name.setValidators([Validators.maxLength(100), Validators.required]);
      this.signatureBoxForm.controls.agent_signatory_name.setValue(null);
      this.signatureBoxForm.controls.agent_signatory_name.clearValidators();
      this.signatureBoxForm.controls.agent_signatory_name.disable();
      this.signatureBoxForm.controls.disputant_signatory_name.enable();
    } else if (value === this.SignatoryType.A) {
      this.signatureBoxForm.controls.agent_signatory_name.setValidators([Validators.maxLength(100), Validators.required]);
      this.signatureBoxForm.controls.disputant_signatory_name.setValue(null);
      this.signatureBoxForm.controls.disputant_signatory_name.clearValidators();
      this.signatureBoxForm.controls.disputant_signatory_name.disable();
      this.signatureBoxForm.controls.agent_signatory_name.enable();
    }

    this.signatureBoxForm.controls.agent_signatory_name.updateValueAndValidity();
    this.signatureBoxForm.controls.disputant_signatory_name.updateValueAndValidity();
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
        messageKey: `You have selected "Skip this count, no action required" for all counts on your ticket. No dispute request will be created. If you do not pay or dispute your ticket within 30 days, you will be deemed to have plead guilty and you will be required to pay the full offence amount. Please review your selection(s) if you intend to file a dispute.`,
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
        messageKey: "You are requesting an adjournment within 14 days of your court date. To help ensure that your request for an adjournment is processed on time, please contact the Violation Ticket Centre at Courts.TCO@gov.bc.ca. If your adjournment is not able to be processed, you may be deemed guilty and your dispute closed. Would you like to proceed?",
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
    let file = files[0];

    let fileData = await firstValueFrom(this.fileData$)
    if (fileData.length >= 4) {
      this.onUploadFileError("Maximum 4 file uploads per dispute.");
      return;
    }

    let err = this.fileUtilsService.checkFileSize(file.size, 50);
    if (err.length > 0) {
      this.onUploadFileError(err);
      return;
    }

    err = this.fileUtilsService.checkFileType(file, this.acceptFileTypes);
    if (err.length > 0) {
      this.onUploadFileError(err);
      return;
    }

    let pendingFileStream = await firstValueFrom(this.fileUtilsService.readFileAsDataURL(file)) as string;
    this.store.dispatch(DisputeStore.Actions.AddDocument({ file: file, fileType: this.fileTypeToUpload, pendingFileStream }));
    this.fileInput.nativeElement.value = null;
    this.fileTypeToUpload = this.adjournmentFileType.key;
  }

  private onUploadFileError(err: string): void {
    const data: DialogOptions = {
      titleKey: "Warning",
      actionType: "warn",
      messageKey: "File upload error. " + err,
      actionTextKey: "Close",
      cancelHide: true
    };
    this.dialog.open(ConfirmDialogComponent, { data });
  }

  onSubmitClicked() {
    this.submitDispute();
  }

  private submitDispute() {
    this.saveDispute.emit(this.noticeOfDispute);
  }
}

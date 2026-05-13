import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CountryCodeValue, ProvinceCodeValue } from '@config/config.model';
import { MatStepper } from '@angular/material/stepper';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { TranslateService } from '@ngx-translate/core';
import { ConfigService } from '@config/config.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { FormErrorStateMatcher } from '@shared/directives/form-error-state-matcher.directive';
import { DisputeFormMode } from '@shared/enums/dispute-form-mode';
import { TicketTypes } from '@shared/enums/ticket-type.enum';
import { FileUtilsService } from '@shared/services/file-utils.service';
import {
  DisputeContactTypeCd,
  DisputeInterpreterRequired,
  DisputeRepresentedByLawyer,
  DisputeRequestCourtAppearanceYn,
  Language,
  ViolationTicketCount,
} from 'app/api';
import { AppConfigService } from 'app/services/app-config.service';
import { DisputeService, FileMetadata } from 'app/services/dispute.service';
import { LookupsService } from 'app/services/lookups.service';
import {
  DisputeCount,
  NoticeOfDispute,
  NoticeOfDisputeFormGroup,
  NoticeOfDisputeService,
} from 'app/services/notice-of-dispute.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';
import { DisputeStore } from 'app/store';
import { Store } from '@ngrx/store';
import { Observable, firstValueFrom } from 'rxjs';

/**
 * Stepper component for the Update Dispute flow only.
 *
 * Key differences from DisputeStepperComponent:
 * - Every section is opt-in via a checkbox; unchecked sections are excluded
 *   from the submission payload and have no validation requirements.
 * - The Counts step is replaced by instructional text and a file-upload area.
 * - The original DisputeStepperComponent is not touched.
 */
@Component({
  selector: 'app-update-dispute-stepper',
  templateUrl: './update-dispute-stepper.component.html',
  styleUrls: ['./update-dispute-stepper.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UpdateDisputeStepperComponent implements OnInit, AfterViewInit {
  @Input() ticket: NoticeOfDispute;
  @Input() ticketCounts: ViolationTicketCount[] = [];
  @Input() disputeCounts: DisputeCount[] = [];
  @Input() fileData$: Observable<FileMetadata[]>;
  @Input() ticketType: string;
  @Input() mode: DisputeFormMode = DisputeFormMode.UPDATE;
  @Output() saveDispute = new EventEmitter<NoticeOfDispute>();

  @ViewChild(MatStepper) private stepper: MatStepper;
  @ViewChild('fileInput') private fileInput: ElementRef;

  private state: DisputeStore.State;

  // ── Section-enable checkboxes ──────────────────────────────────────────────
  contactSectionEnabled = new FormControl<boolean>(false);
  additionalSectionEnabled = new FormControl<boolean>(false);
  adjournStepEnabled = new FormControl<boolean>(false);
  writtenReasonsStepEnabled = new FormControl<boolean>(false);
  supportingDocsStepEnabled = new FormControl<boolean>(false);

  // ── Additional section radio controls (NO_CHANGE | 'N' | 'Y') ────────────
  lawyerRadio = new FormControl<string>('NO_CHANGE');
  interpreterRadio = new FormControl<string>('NO_CHANGE');
  witnessRadio = new FormControl<string>('NO_CHANGE');

  // ── Enums exposed to the template ─────────────────────────────────────────
  DisputeFormMode = DisputeFormMode;
  TicketTypes = TicketTypes;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  InterpreterRequired = DisputeInterpreterRequired;

  // ── Forms ──────────────────────────────────────────────────────────────────
  form: NoticeOfDisputeFormGroup;
  additionalForm: NoticeOfDisputeFormGroup;
  legalRepresentationForm: NoticeOfDisputeFormGroup;
  matcher = new FormErrorStateMatcher();

  // ── Misc ───────────────────────────────────────────────────────────────────
  defaultLanguage: string;
  adjournmentFormLink: string;
  writtenReasonsFormLink: string;
  languages: Language[] = [];
  minWitnesses = 1;
  maxWitnesses = 99;
  disableSave = false;

  /** Human-readable labels for contact form fields shown in the review step. */
  private readonly contactFieldLabels: Record<string, string> = {
    disputant_surname: 'Last Name',
    disputant_given_names: 'Given Names',
    contact_type: 'Contact Type',
    contact_surname: 'Contact Last Name',
    contact_given_names: 'Contact Given Names',
    contact_law_firm_name: 'Law Firm Name',
    address: 'Address',
    address_city: 'Address City',
    address_province: 'Province / State',
    postal_code: 'Postal Code',
    home_phone_number: 'Home Phone',
    work_phone_number: 'Work Phone',
    email_address: 'Email Address',
    drivers_licence_number: "Driver's Licence Number",
    drivers_licence_province: "Driver's Licence Province / State",
  };

  /**
   * Fields that contain raw IDs/seq numbers whose display is handled by
   * a companion resolved field — skip them in the summary.
   */
  private readonly skipInSummary = new Set([
    'address_country_id',
    'address_province_seq_no',
    'address_province_country_id',
    'drivers_licence_country_id',
    'drivers_licence_province_seq_no',
  ]);

  // ── File upload ────────────────────────────────────────────────────────────
  adjournmentFileType = { key: 'Adjournment', value: 'Application for Adjournment' };
  fileTypes = [this.adjournmentFileType, { key: 'Other', value: 'Other' }];
  fileTypeToUpload: string = this.adjournmentFileType.key;
  acceptFileTypes = [
    'image/jpeg',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'application/pdf',
    '.pdf',
    '.doc',
    '.docx',
  ];

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
    this.defaultLanguage = this.translateService.getDefaultLang();
    this.adjournmentFormLink = this.appConfigService.adjournmentFormLink;
    this.writtenReasonsFormLink = this.appConfigService.writtenReasonsFormLink;
    this.lookups.languages$.subscribe(languages => {
      this.languages = languages;
    });
  }

  // ── Lifecycle ──────────────────────────────────────────────────────────────

  ngOnInit(): void {
    this.store.select(DisputeStore.Selectors.State).subscribe(state => {
      this.state = state;
    });

    // Contact form — start disabled; all validators cleared.
    this.form = this.noticeOfDisputeService.getNoticeOfDisputeForm(this.ticket);
    this.clearAllValidators(this.form);
    this.form.disable();

    // Additional form — start disabled; all validators cleared.
    this.additionalForm = this.noticeOfDisputeService.getAdditionalForm(this.ticket);
    this.clearAllValidators(this.additionalForm);
    this.additionalForm.disable();

    // Legal representation form — start disabled; all validators cleared.
    this.legalRepresentationForm = this.noticeOfDisputeService.getLegalRepresentationForm(this.ticket);
    this.clearAllValidators(this.legalRepresentationForm);
    this.legalRepresentationForm.disable();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.stepper?.selectionChange.subscribe(() => {
        this.scrollToSectionHook();
      });
    }, 0);
  }

  // ── Validation helpers ─────────────────────────────────────────────────────

  /** Every section is optional; the step is valid when unchecked or when its form is valid. */
  isContactStepValid(): boolean {
    return !this.contactSectionEnabled.value || this.form.valid;
  }

  /** Counts step has no form — always considered complete. */
  isCountsStepValid(): boolean {
    return true;
  }

  isAdditionalStepValid(): boolean {
    if (!this.additionalSectionEnabled.value) return true;
    if (this.lawyerRadio.value === 'Y' && !this.legalRepresentationForm?.valid) {
      return false;
    }
    if (this.interpreterRadio.value === 'Y' && !this.additionalForm.controls['interpreter_language_cd']?.value) {
      return false;
    }
    if (this.witnessRadio.value === 'Y' && !this.additionalForm.controls['witness_no']?.valid) {
      return false;
    }
    return this.additionalForm?.valid ?? true;
  }

  // ── Section-enable toggles ─────────────────────────────────────────────────

  onContactSectionToggle(enabled: boolean): void {
    if (enabled) {
      // Rebuild the form to restore all original validators, then patch with current ticket values.
      this.form = this.noticeOfDisputeService.getNoticeOfDisputeForm(this.ticket);
      // disputant_surname and disputant_given_names are not editable in the update flow — remove their required constraint.
      this.form.controls.disputant_surname?.clearValidators();
      this.form.controls.disputant_surname?.updateValueAndValidity();
      this.form.controls.disputant_given_names?.clearValidators();
      this.form.controls.disputant_given_names?.updateValueAndValidity();
      // Default contact_type to Individual if not set (or UNKNOWN) so the dropdown is never empty.
      const ct = this.form.controls.contact_type?.value;
      if (!ct || ct === DisputeContactTypeCd.Unknown) {
        this.form.controls.contact_type?.setValue(DisputeContactTypeCd.Individual);
      }
    } else {
      this.clearAllValidators(this.form);
      this.form.disable();
    }
  }

  onAdditionalSectionToggle(enabled: boolean): void {
    if (enabled) {
      this.additionalForm.enable();
      // legalRepresentationForm stays disabled until lawyer radio = Y
      this.legalRepresentationForm.disable();
    } else {
      // Reset all field values before disabling so stale data is cleared
      this.additionalForm.reset();
      this.legalRepresentationForm.reset();
      this.additionalForm.disable();
      this.legalRepresentationForm.disable();
      // Reset all radios to NO_CHANGE
      this.lawyerRadio.setValue('NO_CHANGE');
      this.interpreterRadio.setValue('NO_CHANGE');
      this.witnessRadio.setValue('NO_CHANGE');
    }
  }

  onLawyerRadioChange(value: string): void {
    if (value === 'Y') {
      // Rebuild to restore validators from legalRepresentationConfigs.
      this.legalRepresentationForm = this.noticeOfDisputeService.getLegalRepresentationForm(this.ticket);
    } else {
      this.legalRepresentationForm.reset();
      this.legalRepresentationForm.disable();
    }
  }

  onInterpreterRadioChange(value: string): void {
    const langCtrl = this.additionalForm.controls['interpreter_language_cd'];
    if (value === 'Y') {
      langCtrl?.setValidators([Validators.required]);
      langCtrl?.updateValueAndValidity();
    } else {
      if (langCtrl) this.formUtilsService.resetAndClearValidators(langCtrl);
    }
  }

  onWitnessRadioChange(value: string): void {
    const witnessCtrl = this.additionalForm.controls['witness_no'];
    if (value === 'Y') {
      witnessCtrl?.setValidators([
        Validators.min(this.minWitnesses),
        Validators.max(this.maxWitnesses),
        Validators.required,
      ]);
      witnessCtrl?.updateValueAndValidity();
    } else {
      if (witnessCtrl) this.formUtilsService.resetAndClearValidators(witnessCtrl);
    }
  }

  // ── Navigation ─────────────────────────────────────────────────────────────

  onStepCancel(): void {
    this.disputeService.goToUpdateDisputeLanding(this.state.params);
  }

  onStepBack(): void {
    this.stepper.previous();
    this.scrollToSectionHook();
  }

  // ── Additional-form sub-controls ───────────────────────────────────────────



  /** Produces label/value pairs for the contact section in the review step.
   *  Fields are emitted in a deliberate, user-friendly order.
   */
  getContactSummaryRows(): { label: string; value: string }[] {
    if (!this.form) return [];
    const raw = this.form.getRawValue();
    const rows: { label: string; value: string }[] = [];

    // Helper to push a row only when the value is non-empty
    const push = (label: string, value: any) => {
      if (value !== null && value !== undefined && value !== '') {
        rows.push({ label, value: String(value) });
      }
    };

    // ── Contact details ─────────────────────────────────────────────────────
    push('Contact Type', raw['contact_type']);
    push('Last Name', raw['disputant_surname']);
    push('Given Names', raw['disputant_given_names']);
    push('Contact Last Name', raw['contact_surname']);
    push('Contact Given Names', raw['contact_given_names']);
    push('Law Firm Name', raw['contact_law_firm_name']);

    // ── Address block: street → country → province → city → postal ──────────
    push('Address', raw['address']);

    const countryId = raw['address_country_id'];
    if (countryId !== null && countryId !== undefined) {
      const country = this.config.countries.find((c: CountryCodeValue) => c.ctryId === +countryId);
      push('Address Country', country?.ctryLongNm ?? String(countryId));
    }

    const provSeqNo = raw['address_province_seq_no'];
    const provCtryId = raw['address_province_country_id'] ?? raw['address_country_id'];
    if (provSeqNo !== null && provSeqNo !== undefined) {
      const prov = this.config.provincesAndStates.find(
        (p: ProvinceCodeValue) => p.provSeqNo === +provSeqNo && (!provCtryId || p.ctryId === +provCtryId)
      );
      push('Province / State', prov?.provNm ?? String(provSeqNo));
    }

    push('Address City', raw['address_city']);
    push('Postal Code', raw['postal_code']);

    // ── Contact details cont. ────────────────────────────────────────────────
    push('Home Phone', raw['home_phone_number']);
    push('Work Phone', raw['work_phone_number']);
    push('Email Address', raw['email_address']);

    // ── Driver's licence ─────────────────────────────────────────────────────
    push("Driver's Licence Number", raw['drivers_licence_number']);

    const dlProvSeqNo = raw['drivers_licence_province_seq_no'];
    const dlCtryId = raw['drivers_licence_country_id'];
    if (dlProvSeqNo !== null && dlProvSeqNo !== undefined) {
      const prov = this.config.provincesAndStates.find(
        (p: ProvinceCodeValue) => p.provSeqNo === +dlProvSeqNo && (!dlCtryId || p.ctryId === +dlCtryId)
      );
      push("Driver's Licence Province / State", prov?.provNm ?? String(dlProvSeqNo));
    }

    return rows;
  }

  /** Produces label/value pairs for the additional section in the review step. */
  getAdditionalSummaryRows(): { label: string; value: string }[] {
    const rows: { label: string; value: string }[] = [];

    if (this.lawyerRadio.value === 'N') {
      rows.push({ label: 'Lawyer or Agent', value: 'I no longer intend to be represented by a lawyer or agent in court' });
    } else if (this.lawyerRadio.value === 'Y') {
      rows.push({ label: 'Lawyer or Agent', value: 'I intend to be represented by a lawyer or agent in court' });
      const raw = this.legalRepresentationForm.getRawValue();
      if (raw['law_firm_name']) rows.push({ label: 'Law Firm Name', value: raw['law_firm_name'] });
      if (raw['lawyer_full_name']) rows.push({ label: 'Lawyer / Agent Name', value: raw['lawyer_full_name'] });
      if (raw['lawyer_address']) rows.push({ label: 'Lawyer / Agent Address', value: raw['lawyer_address'] });
      if (raw['lawyer_phone_number']) rows.push({ label: 'Lawyer / Agent Phone', value: raw['lawyer_phone_number'] });
      if (raw['lawyer_email']) rows.push({ label: 'Lawyer / Agent Email', value: raw['lawyer_email'] });
    }

    if (this.interpreterRadio.value === 'N') {
      rows.push({ label: 'Language Interpreter', value: 'I no longer require a language interpreter at the hearing' });
    } else if (this.interpreterRadio.value === 'Y') {
      const langCd = this.additionalForm?.value.interpreter_language_cd;
      const langName = langCd ? this.lookups.getLanguageDescription(String(langCd)) : '';
      rows.push({ label: 'Language Interpreter', value: `I require a language interpreter at the hearing${langName ? ' — ' + langName : ''}` });
    }

    if (this.witnessRadio.value === 'N') {
      rows.push({ label: 'Witnesses', value: 'I no longer intend to call a witness' });
    } else if (this.witnessRadio.value === 'Y') {
      const count = this.additionalForm?.value.witness_no;
      rows.push({ label: 'Witnesses', value: `I intend to call a witness${count != null ? ' (' + count + ')' : ''}` });
    }

    return rows;
  }

  /** Maps Y/N API values to Yes/No for display. */
  private yesNo(value: string): string {
    if (value === 'Y') return 'Yes';
    if (value === 'N') return 'No';
    return value;
  }

  /** Converts a snake_case key to Title Case with spaces. */
  private humaniseKey(key: string): string {
    return key
      .replace(/_/g, ' ')
      .replace(/\b\w/g, c => c.toUpperCase());
  }

  getToolTipData(data: any): string {
    if (data) {
      let msg = '';
      this.lookups.languages.forEach(res => {
        if (res.code === data.value) {
          msg = res.description;
        }
      });
      return msg;
    }
    return 'please select a language';
  }

  // ── File upload ────────────────────────────────────────────────────────────

  onRemoveFile(file: FileMetadata): void {
    const data: DialogOptions = {
      titleKey: 'Remove File?',
      messageKey: 'Are you sure you want to delete file ' + file.fileName + '?',
      actionTextKey: 'Delete',
      actionType: 'warn',
      cancelTextKey: 'Cancel',
      icon: 'delete',
    };
    this.dialog
      .open(ConfirmDialogComponent, { data, width: '40%' })
      .afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.store.dispatch(DisputeStore.Actions.RemoveDocument({ file }));
        }
      });
  }

  onGetFile(file: FileMetadata): void {
    if (file.pendingFileStream) {
      const url = URL.createObjectURL(file.__penfingFile);
      window.open(url);
    } else {
      this.store.dispatch(DisputeStore.Actions.GetDocument({ fileId: file.fileId }));
    }
  }

  onUploadWithType(type: string): void {
    this.fileTypeToUpload = type;
    this.onUploadClicked();
  }

  onUploadClicked(): void {
    if (this.fileTypeToUpload === this.adjournmentFileType.key && this.ticket?.appearance_less_than_14_days) {
      const data: DialogOptions = {
        titleKey: 'Court hearing scheduled for less than 14 days',
        messageKey:
          'You are requesting an adjournment within 14 days of your court date. To help ensure that your request for an adjournment is processed on time, please contact the Violation Ticket Centre at Courts.TCO@gov.bc.ca. If your adjournment is not able to be processed, you may be deemed guilty and your dispute closed. Would you like to proceed?',
        actionTextKey: 'Yes',
        actionType: 'primary',
        cancelTextKey: 'No',
      };
      this.dialog
        .open(ConfirmDialogComponent, { data, width: '40%' })
        .afterClosed()
        .subscribe((action: any) => {
          if (action) {
            this.fileInput.nativeElement.click();
          }
        });
    } else {
      this.fileInput.nativeElement.click();
    }
  }

  async onUploadFile(files: FileList): Promise<void> {
    if (files.length <= 0) return;
    const file = files[0];

    const fileData = await firstValueFrom(this.fileData$);
    if (fileData.filter(i => !i.deleteRequested).length >= 4) {
      this.onUploadFileError('Maximum 4 file uploads per dispute.');
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

    const pendingFileStream = (await firstValueFrom(
      this.fileUtilsService.readFileAsDataURL(file),
    )) as string;
    this.store.dispatch(
      DisputeStore.Actions.AddDocument({ file, fileType: this.fileTypeToUpload, pendingFileStream }),
    );
    this.fileInput.nativeElement.value = null;
    this.fileTypeToUpload = this.adjournmentFileType.key;
  }

  // ── Submission ─────────────────────────────────────────────────────────────

  onSubmitClicked(): void {
    const payload: NoticeOfDispute = {
      ticket_number: this.ticket?.ticket_number,
      contact_section_enabled: this.contactSectionEnabled.value,
      additional_section_enabled: this.additionalSectionEnabled.value,
    };

    if (this.contactSectionEnabled.value) {
      // Include ALL fields (including empty/null) so the backend can clear them if needed.
      Object.entries(this.form.getRawValue()).forEach(([key, val]) => {
        if (!key.startsWith('__')) {
          payload[key] = val;
        }
      });
      // Split contact_given_names into contact_given_name1/2/3 so they map to backend fields
      this.noticeOfDisputeService.splitContactGivenNames(payload);
      // Split address into address_line1/2/3 so they map to backend fields
      this.noticeOfDisputeService.splitAddressLines(payload);
    }

    if (this.additionalSectionEnabled.value) {
      // Lawyer/Agent — only included when radio is explicitly Y or N
      if (this.lawyerRadio.value !== 'NO_CHANGE') {
        payload['represented_by_lawyer'] = this.lawyerRadio.value as DisputeRepresentedByLawyer; // 'Y' or 'N'
        if (this.lawyerRadio.value === 'Y') {
          Object.entries(this.legalRepresentationForm.getRawValue()).forEach(([key, val]) => {
            payload[key] = val;
          });
          this.noticeOfDisputeService.splitLawyerNames(payload);
        } else {
          // N: clear all lawyer details
          payload['law_firm_name'] = null;
          payload['lawyer_surname'] = null;
          payload['lawyer_given_name1'] = null;
          payload['lawyer_given_name2'] = null;
          payload['lawyer_given_name3'] = null;
          payload['lawyer_address'] = null;
          payload['lawyer_phone_number'] = null;
          payload['lawyer_email'] = null;
        }
      }

      // Interpreter — only included when radio is explicitly Y or N
      if (this.interpreterRadio.value !== 'NO_CHANGE') {
        payload['interpreter_required'] = this.interpreterRadio.value as DisputeInterpreterRequired; // 'Y' or 'N'
        if (this.interpreterRadio.value === 'Y') {
          payload['interpreter_language_cd'] = this.additionalForm.value.interpreter_language_cd;
        } else {
          payload['interpreter_language_cd'] = null;
        }
      }

      // Witnesses — only included when radio is explicitly Y or N
      if (this.witnessRadio.value !== 'NO_CHANGE') {
        if (this.witnessRadio.value === 'Y') {
          payload['witness_no'] = this.additionalForm.value.witness_no;
        } else {
          payload['witness_no'] = 0; // clear
        }
      }
    }

    // File data is always included when present.
    let fileData: FileMetadata[] = [];
    this.fileData$?.subscribe(i => {
      fileData = i;
    });
    if (fileData.length > 0) {
      (payload as any).file_data = fileData;
    }

    this.saveDispute.emit(payload);
  }

  // ── Private helpers ────────────────────────────────────────────────────────

  private clearAllValidators(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const ctrl = formGroup.get(key);
      if (ctrl) {
        ctrl.clearValidators();
        ctrl.updateValueAndValidity({ emitEvent: false });
      }
    });
  }

  private scrollToSectionHook(): void {
    const stepId = this.stepper._getStepLabelId(this.stepper.selectedIndex);
    const stepElement = document.getElementById(stepId);
    if (stepElement) {
      setTimeout(() => {
        stepElement.scrollIntoView({ block: 'start', inline: 'nearest', behavior: 'smooth' });
      }, 250);
    }
  }

  private onUploadFileError(err: string): void {
    const data: DialogOptions = {
      titleKey: 'Warning',
      actionType: 'warn',
      messageKey: 'File upload error. ' + err,
      actionTextKey: 'Close',
      cancelHide: true,
    };
    this.dialog.open(ConfirmDialogComponent, { data });
  }
}

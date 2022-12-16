import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { CountryCodeValue, ProvinceCodeValue } from "@config/config.model";
import { MatStepper } from "@angular/material/stepper";
import { MatCheckboxChange } from "@angular/material/checkbox";
import { MatDialog } from "@angular/material/dialog";
import { TranslateService } from "@ngx-translate/core";
import { ToastService } from "@core/services/toast.service";
import { UtilsService } from "@core/services/utils.service";
import { FormUtilsService } from "@core/services/form-utils.service";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { ConfigService } from "@config/config.service";
import { Address } from "@shared/models/address.model";
import { TicketTypes } from "@shared/enums/ticket-type.enum";
import { AddressAutocompleteComponent } from "@shared/components/address-autocomplete/address-autocomplete.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { FormErrorStateMatcher } from "@shared/directives/form-error-state-matcher.directive";
import { cloneDeep } from "lodash";
import { ViolationTicket, DisputeCountPleaCode, DisputeRepresentedByLawyer, DisputeCountRequestCourtAppearance, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, Language } from "app/api";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { NoticeOfDisputeService, NoticeOfDispute } from "app/services/notice-of-dispute.service";
import { LookupsService } from "app/services/lookups.service";

@Component({
  selector: "app-dispute-ticket-stepper",
  templateUrl: "./dispute-ticket-stepper.component.html",
  styleUrls: ["./dispute-ticket-stepper.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DisputeTicketStepperComponent implements OnInit, AfterViewInit {
  @ViewChild(MatStepper) private stepper: MatStepper;
  @ViewChild(AddressAutocompleteComponent) private addressAutocomplete: AddressAutocompleteComponent;

  languages: Language[] = [];
  provinces: ProvinceCodeValue[];
  states: ProvinceCodeValue[];
  countries: CountryCodeValue[];
  provincesAndStates: ProvinceCodeValue[];

  isMobile: boolean;
  previousButtonIcon = "keyboard_arrow_left";
  defaultLanguage: string;
  ticketTypes = TicketTypes ;
  todayDate: Date = new Date();
  Plea = DisputeCountPleaCode;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  RequestTimeToPay = DisputeCountRequestTimeToPay;
  RequestReduction = DisputeCountRequestReduction;
  selected = null;

  form: FormGroup;
  countForms: FormArray;
  additionalForm: FormGroup;
  legalRepresentationForm: FormGroup;
  ticket: ViolationTicket;
  noticeOfDispute: NoticeOfDispute;
  ticketType;
  matcher = new FormErrorStateMatcher();

  // Disputant
  showManualButton: boolean = true;
  showAddressFields: boolean = true; // temporary preset for testing
  optOut: boolean = false;

  // Count
  countIndexes: number[];

  // Additional
  countsActions: any;
  customWitnessOption = false;
  minWitnesses = 1;
  maxWitnesses = 99;
  additionalIndex: number;

  // Summary
  declared = false;

  // Consume from the service
  bc: ProvinceCodeValue;
  canada: CountryCodeValue;
  usa: CountryCodeValue;
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
    this.isMobile = this.utilsService.isMobile();
    this.defaultLanguage = this.translateService.getDefaultLang();

    this.bc = this.config.bcCodeValue;
    this.canada = this.config.canadaCodeValue;
    this.usa = this.config.usaCodeValue;
    this.countries = this.config.countries;
    this.provincesAndStates = this.config.provincesAndStates;
    this.provinces = this.provincesAndStates.filter(x => x.ctryId === this.canada?.ctryId && x.provSeqNo !== this.bc?.provSeqNo);  // skip BC it will be manually at top of list
    this.states = this.provincesAndStates.filter(x => x.ctryId === this.usa?.ctryId); // USA only

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
    this.ticketType = this.violationTicketService.ticketType;

    // build inner object array before the form
    let countArray = [];
    this.ticket.counts.forEach(count => {
      countArray.push(this.formBuilder.group({ ...count, ...this.countFormFields }));
    })
    this.countForms = this.formBuilder.array(countArray);

    // build form
    this.form = this.formBuilder.group({
      ...this.ticketFormFields,
    });

    // take info from ticket, convert dl number to string
    Object.keys(this.ticket).forEach(key => {
      this.ticket[key] && this.form.get(key)?.patchValue(this.ticket[key]);
    });
    this.ticket.drivers_licence_number && this.form.controls.drivers_licence_number.setValue(this.ticket.drivers_licence_number.toString());

    // search for drivers licence province using abbreviation e.g. BC
    if (this.ticket.drivers_licence_province) {
      let foundProvinces = this.provincesAndStates.filter(x => x.provAbbreviationCd === this.ticket.drivers_licence_province).shift();
      if (foundProvinces) {
        this.form.get("drivers_licence_province_provId").setValue(foundProvinces.provId);
        this.onDLProvinceChange(foundProvinces.provId);
      }
      else {
        this.form.get("drivers_licence_province").setValue(this.ticket.drivers_licence_province);
      }
    } else { // no DL found init to BC
      this.form.get("drivers_licence_province_provId").setValue(this.bc?.provId);
      this.onDLProvinceChange(this.bc?.provId);
    }

    this.form.get("address_province_provId").setValue(this.bc?.provId);
    this.onAddressProvinceChange(this.bc?.provId);

    this.legalRepresentationForm = this.formBuilder.group(this.legalRepresentationFields);

    this.countIndexes = this.ticket.counts.map(i => i.count_no);
    let lastCountInx = this.countIndexes[this.countIndexes.length - 1]
    this.additionalIndex = lastCountInx + 1;
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.addressAutocomplete?.autocomplete.disable(); // disable auto complete component
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

  onCountryChange(ctryId: number) {
    setTimeout(() => {
      this.form.get("postal_code").setValidators([Validators.maxLength(6)]);
      this.form.get("address_province").setValidators([Validators.maxLength(30)]);
      this.form.get("address_province").setValue(null);
      this.form.get("address_province_seq_no").setValidators(null);
      this.form.get("address_province_seq_no").setValue(null);
      this.form.get("address_province_country_id").setValue(ctryId);
      this.form.get("address_province_provId").setValue(null);
      this.form.get("home_phone_number").setValidators([Validators.maxLength(20)]);
      this.form.get("drivers_licence_number").setValidators([Validators.maxLength(20)]);
      this.form.get("drivers_licence_province").setValidators([Validators.maxLength(30)]);

      if ((ctryId === this.canada?.ctryId) || (ctryId === this.usa?.ctryId)) { // canada or usa validators
        this.form.get("address_province_seq_no").addValidators([Validators.required]);
        this.form.get("postal_code").addValidators([Validators.required]);
        this.form.get("home_phone_number").addValidators([Validators.required, FormControlValidators.phone]);
        this.form.get("drivers_licence_number").addValidators([Validators.required]);
        this.form.get("drivers_licence_province_seq_no").addValidators([Validators.required]);
      }

      if (ctryId === this.canada?.ctryId && this.bc) { // pick BC by default if Canada selected
        this.form.get("address_province").setValue(this.bc?.provNm);
        this.form.get("address_province_seq_no").setValue(this.bc?.provSeqNo)
        this.form.get("address_province_provId").setValue(this.bc?.provId);
      }

      this.form.get("postal_code").updateValueAndValidity();
      this.form.get("address_province").updateValueAndValidity();
      this.form.get("address_province_country_id").updateValueAndValidity();
      this.form.get("address_province_seq_no").updateValueAndValidity();
      this.form.get("address_province_provId").updateValueAndValidity();
      this.form.get("home_phone_number").updateValueAndValidity();
      this.form.get("drivers_licence_number").updateValueAndValidity();
      this.form.get("drivers_licence_province").updateValueAndValidity();
    }, 0);
  }

  public onDLProvinceChange(provId: number) {
    setTimeout(() => {
      let prov = this.provincesAndStates.filter(x => x.provId === provId).shift();
      this.form.get("drivers_licence_province").setValue(prov.provNm);
      this.form.get("drivers_licence_country_id").setValue(prov.ctryId);
      this.form.get("drivers_licence_province_seq_no").setValue(prov.provSeqNo);
      if (prov.provAbbreviationCd === this.bc.provAbbreviationCd) {
        this.form.get("drivers_licence_number").setValidators([Validators.maxLength(9)]);
        this.form.get("drivers_licence_number").addValidators([Validators.minLength(7)]);
      } else {
        this.form.get("drivers_licence_number").setValidators([Validators.maxLength(20)]);
      }
      if (prov.ctryId === this.canada?.ctryId || prov.ctryId === this.usa?.ctryId) {
        this.form.get("drivers_licence_number").addValidators([Validators.required]);
      }
      this.form.get("drivers_licence_number").updateValueAndValidity();
    }, 0)
  }

  public onAddressProvinceChange(provId: number) {
    setTimeout(() => {
      let prov = this.provincesAndStates.filter(x => x.provId === provId).shift();
      this.form.get("address_province").setValue(prov.provNm);
      this.form.get("address_province_country_id").setValue(prov.ctryId);
      this.form.get("address_province_seq_no").setValue(prov.provSeqNo);
    }, 0)
  }

  private getCountFormInitValue(count) {
    return { ...this.countFormDefaultValue, ...count };
  }

  private getCountsActions() {
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.countForms.value);
  }

  onAddressAutocomplete({ countryCode, provinceCode, postalCode, address, city }: Address): void {
    // Will be implemented
  }

  onAttendHearingChange(countForm: FormGroup, event): void {
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
      let countForm = this.countForms.controls[countInx];
      if (countForm.value.request_time_to_pay === this.RequestTimeToPay.Y || countForm.value.request_reduction === this.RequestReduction.Y) {
        countForm.patchValue({ plea_cd: this.Plea.G });
      }
      if (countForm.value.__skip) {
        countForm.patchValue({ ...this.getCountFormInitValue(this.ticket.counts[countInx]), __skip: true });
      }
      if (countForm.value.__apply_to_remaining_counts && countInx + 1 < this.countForms.length) {
        let value = this.countForms.controls[countInx].value;
        for (let i = countInx; i < this.countForms.length; i++) {
          this.countForms.controls[i].patchValue({ ...value, ...this.ticket.counts[i], __apply_to_remaining_counts: false })
        }
      }
      this.setAdditional();
    } else if (!isValid) {
      this.utilsService.scrollToErrorSection();
      this.toastService.openErrorToast(this.config.dispute_validation_error);
      return;
    }

    if (isAdditional) {
      this.noticeOfDispute = this.noticeOfDisputeService.getNoticeOfDispute({
        ...this.form.value,
        ...this.additionalForm.value,
        ...this.legalRepresentationForm.value,
        address_country_id: this.form.get("address_country_id").value, // disabled field is not available in this.form.value
        dispute_counts: this.countForms.value
      });
    } else {
      this.noticeOfDispute = null;
    }
  }

  private setAdditional() {
    this.getCountsActions();
    let fields = cloneDeep(this.additionFormFields);
    if (this.countsActions.request_reduction.length > 0 && fields.fine_reduction_reason[1].indexOf(Validators.required) < 0) {
      fields.fine_reduction_reason[1].push(Validators.required);
    }
    if (this.countsActions.request_time_to_pay.length > 0 && fields.time_to_pay_reason[1].indexOf(Validators.required) < 0) {
      fields.time_to_pay_reason[1].push(Validators.required);
    }
    this.additionalForm = this.formBuilder.group({
      ...fields,
    }, {
      validators: [...this.additionFormValidators]
    });
  }

  isValid(countInx?): boolean {
    let countForm = this.countForms?.controls[countInx]
    if (countForm) {
      let valid = countForm.valid || countForm.value.__skip;
      if (countForm.value.request_court_appearance === this.RequestCourtAppearance.Y) {
        valid = valid && (countForm.value.plea_cd === this.Plea.G || countForm.value.plea_cd === this.Plea.N);
      } else if (countForm.value.request_court_appearance === this.RequestCourtAppearance.N) {
        valid = valid && ((countForm.value.request_time_to_pay === this.RequestTimeToPay.Y) || (countForm.value.request_reduction === this.RequestReduction.Y));
      }
      return valid && !this.isAllCountsSkipped;
    }
    return this.form.valid;
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

  onOptOut() {
    let key = "email_address";
    if (this.optOut) {
      this.form.get(key).setValue(null);
      this.form.get(key).disable();
    } else {
      this.form.get(key).enable();
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

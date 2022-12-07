import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatStepper } from "@angular/material/stepper";
import { ActivatedRoute, Router } from "@angular/router";
import { ConfigService, CountryCodeValue, ProvinceCodeValue } from "@config/config.service";
import { UtilsService } from "@core/services/utils.service";
import { TranslateService } from "@ngx-translate/core";
import { Address } from "@shared/models/address.model";
import { FormControlValidators } from "@core/validators/form-control.validators";
import { NoticeOfDispute, ViolationTicket, DisputeCountPleaCode, DisputeRepresentedByLawyer, DisputeCountRequestCourtAppearance, DisputeCountRequestTimeToPay, DisputeCountRequestReduction, Language } from "app/api";
import { ticketTypes } from "@shared/enums/ticket-type.enum";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { Subscription } from "rxjs";
import { MatCheckboxChange } from "@angular/material/checkbox";
import { FormUtilsService } from "@core/services/form-utils.service";
import { ToastService } from "@core/services/toast.service";
import { NoticeOfDisputeService } from "app/services/notice-of-dispute.service";
import { AddressAutocompleteComponent } from "@shared/components/address-autocomplete/address-autocomplete.component";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ConfirmDialogComponent } from "@shared/dialogs/confirm-dialog/confirm-dialog.component";
import { MatDialog } from "@angular/material/dialog";
import { FormErrorStateMatcher } from "@shared/directives/form-error-state-matcher.directive";
import { add, cloneDeep } from "lodash";
import { LookupsService } from "app/services/lookups.service";
import { address } from "faker";
import { throwToolbarMixedModesError } from "@angular/material/toolbar";

@Component({
  selector: "app-dispute-ticket-stepper",
  templateUrl: "./dispute-ticket-stepper.component.html",
  styleUrls: ["./dispute-ticket-stepper.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DisputeTicketStepperComponent implements OnInit, AfterViewInit {
  @ViewChild(MatStepper) private stepper: MatStepper;
  @ViewChild(AddressAutocompleteComponent) private addressAutocomplete: AddressAutocompleteComponent;

  public busy: Subscription;
  public isMobile: boolean;
  public previousButtonIcon = "keyboard_arrow_left";
  public defaultLanguage: string;
  public ticketTypes = ticketTypes;
  public todayDate: Date = new Date();
  public Plea = DisputeCountPleaCode;
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  public RequestTimeToPay = DisputeCountRequestTimeToPay;
  public RequestReduction = DisputeCountRequestReduction;
  public selected = null;
  public countries: CountryCodeValue[] = [];

  public form: FormGroup;
  public countForms: FormArray;
  public additionalForm: FormGroup;
  public legalRepresentationForm: FormGroup;
  public ticket: ViolationTicket;
  public noticeOfDispute: NoticeOfDispute;
  public ticketType;
  public matcher = new FormErrorStateMatcher();

  // Disputant
  public showManualButton: boolean = true;
  public showAddressFields: boolean = true; // temporary preset for testing
  public optOut: boolean = false;

  // Count
  public countIndexes: number[];

  // Additional
  public countsActions: any;
  public customWitnessOption = false;
  public minWitnesses = 1;
  public maxWitnesses = 99;
  public additionalIndex: number;
  public provinces: ProvinceCodeValue[];
  public states: ProvinceCodeValue[];

  // Summary
  public declared = false;

  // Consume from the service
  private ticketFormFields = this.noticeOfDisputeService.ticketFormFields;
  private countFormFields = this.noticeOfDisputeService.countFormFields;
  private countFormDefaultValue = this.noticeOfDisputeService.countFormDefaultValue;
  private additionFormFields = this.noticeOfDisputeService.additionFormFields;
  private additionFormValidators = this.noticeOfDisputeService.additionFormValidators;
  private additionFormDefaultValue = this.noticeOfDisputeService.additionFormDefaultValue;
  private legalRepresentationFields = this.noticeOfDisputeService.legalRepresentationFields;
  public bcFound: ProvinceCodeValue[] = [];
  public canadaFound: CountryCodeValue[] = [];
  public usaFound: CountryCodeValue[] = [];

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected violationTicketService: ViolationTicketService,
    protected noticeOfDisputeService: NoticeOfDisputeService,
    private utilsService: UtilsService,
    private formUtilsService: FormUtilsService,
    private translateService: TranslateService,
    private toastService: ToastService,
    public config: ConfigService,
    private dialog: MatDialog,
    public lookups: LookupsService
  ) {
    // config or static
    this.isMobile = this.utilsService.isMobile();
    this.defaultLanguage = this.translateService.getDefaultLang();

    this.bcFound = this.config.provincesAndStates.filter(x => x.provAbbreviationCd === "BC");
    this.canadaFound = this.config.countries.filter(x => x.ctryLongNm === "Canada");
    this.usaFound = this.config.countries.filter(x => x.ctryLongNm === "USA");
    this.provinces = this.config.provincesAndStates.filter(x => x.ctryId === this.canadaFound[0]?.ctryId && x.provSeqNo !== this.bcFound[0]?.provSeqNo);  // skip BC it will be manually at top of list
    this.states = this.config.provincesAndStates.filter(x => x.ctryId === this.usaFound[0]?.ctryId); // USA only
    this.countries = this.config.countries.filter(x => x.ctryId !== this.canadaFound[0].ctryId && x.ctryId !== this.usaFound[0].ctryId); // skip USA and canada they will be manualy at top of list
    this.countries = this.config.countries;

    this.busy = this.lookups.getLanguages().subscribe((response: Language[]) => {
      this.lookups.languages$.next(response);
    });
  }

  public ngOnInit(): void {
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
    this.ticket.drivers_licence_number && this.form.controls["drivers_licence_number"].setValue(this.ticket.drivers_licence_number.toString());

    // search for drivers licence province using abbreviation e.g. BC
    let foundProvinces = this.config.provincesAndStates.filter(x => x.provAbbreviationCd === this.ticket.drivers_licence_province);
    if (foundProvinces.length > 0) {
      this.onDLProvinceChange(foundProvinces[0].provId);
    }
    else this.form.get("drivers_licence_province").setValue(this.ticket.drivers_licence_province);
    this.form.get("address_province_provId").setValue(this.bcFound[0]?.provId);
    this.onAddressProvinceChange(this.bcFound[0]?.provId);

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

  public onCountryChange(ctryId: number) {
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
      this.form.get("drivers_licence_province").setValue(null);
      this.form.get("drivers_licence_province_seq_no").setValidators(null);
      this.form.get("drivers_licence_province_seq_no").setValue(null);
      this.form.get("drivers_licence_province_provId").setValue(null);
      this.form.get("drivers_licence_country_id").setValue(ctryId);

      if ((ctryId === this.canadaFound[0]?.ctryId) || (ctryId === this.usaFound[0]?.ctryId)) { // canada or usa validators
        this.form.get("address_province_seq_no").addValidators([Validators.required]);
        this.form.get("postal_code").addValidators([Validators.required]);
        this.form.get("home_phone_number").addValidators([Validators.required, FormControlValidators.phone]);
        this.form.get("drivers_licence_number").addValidators([Validators.required]);
        this.form.get("drivers_licence_province_seq_no").addValidators([Validators.required]);
      }

      if (ctryId === this.canadaFound[0]?.ctryId && this.bcFound.length > 0) { // pick BC by default if Canada selected
        this.form.get("address_province").setValue(this.bcFound[0].provNm);
        this.form.get("address_province_seq_no").setValue(this.bcFound[0].provSeqNo)
        this.form.get("address_province_provId").setValue(this.bcFound[0].provId);
        this.form.get("drivers_licence_province").setValue(this.bcFound[0].provNm);
        this.form.get("drivers_licence_province_seq_no").setValue(this.bcFound[0].provSeqNo)
        this.form.get("drivers_licence_province_provId").setValue(this.bcFound[0].provId);
      }

      this.form.get("postal_code").updateValueAndValidity();
      this.form.get("address_province").updateValueAndValidity();
      this.form.get("address_province_country_id").updateValueAndValidity();
      this.form.get("address_province_seq_no").updateValueAndValidity();
      this.form.get("address_province_provId").updateValueAndValidity();
      this.form.get("home_phone_number").updateValueAndValidity();
      this.form.get("drivers_licence_number").updateValueAndValidity();
      this.form.get("drivers_licence_province").updateValueAndValidity();
      this.form.get("drivers_licence_province_seq_no").updateValueAndValidity();
      this.form.get("drivers_licence_province").updateValueAndValidity();
      this.form.get("drivers_licence_country_id").updateValueAndValidity();
    }, 0);
  }

  public onDLProvinceChange(provId: number) {
    setTimeout(() => {
      let provFound = this.config.provincesAndStates.filter(x => x.provId === provId);
      this.form.get("drivers_licence_province").setValue(provFound[0].provNm);
      this.form.get("drivers_licence_country_id").setValue(provFound[0].ctryId);
      this.form.get("drivers_licence_province_seq_no").setValue(provFound[0].provSeqNo);
      if (provFound[0].provAbbreviationCd == "BC") {
        this.form.get("drivers_licence_number").setValidators([Validators.maxLength(9)]);
        this.form.get("drivers_licence_number").addValidators([Validators.minLength(7)]);
      } else {
        this.form.get("drivers_licence_number").setValidators([Validators.maxLength(20)]);
      }
      if (provFound[0].ctryId === this.canadaFound[0]?.ctryId || provFound[0].ctryId === this.usaFound[0]?.ctryId) {
        this.form.get("drivers_licence_number").addValidators([Validators.required]);
      }
      this.form.get("drivers_licence_number").updateValueAndValidity();
    }, 0)
  }

  public onAddressProvinceChange(provId: number) {
    setTimeout(() => {
      let provFound = this.config.provincesAndStates.filter(x => x.provId === provId);
      this.form.get("address_province").setValue(provFound[0].provNm);
      this.form.get("address_province_country_id").setValue(provFound[0].ctryId);
      this.form.get("address_province_seq_no").setValue(provFound[0].provSeqNo);
    }, 0)
  }

  private getCountFormInitValue(count) {
    return { ...this.countFormDefaultValue, ...count };
  }

  private getCountsActions() {
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.countForms.value);
  }

  public onAddressAutocomplete({ countryCode, provinceCode, postalCode, address, city }: Address): void {
    // Will be implemented
  }

  public onStepCancel(): void {
    this.violationTicketService.goToInitiateResolution();
  }

  public onAttendHearingChange(countForm: FormGroup, event): void {
    countForm.patchValue({ ...this.countFormDefaultValue, request_court_appearance: event.value, __skip: false });
  }

  public onStepSave(): void {
    let isAdditional = this.stepper.selectedIndex === this.additionalIndex;
    let isValid = this.formUtilsService.checkValidity(this.form);

    if (this.countIndexes?.indexOf(this.stepper.selectedIndex) > -1) {
      let countInx = this.stepper.selectedIndex - 1;
      let countForm = this.countForms.controls[countInx];
      if (countForm.value.request_time_to_pay === this.RequestTimeToPay.Y || countForm.value.request_reduction === this.RequestReduction.Y) {
        countForm.patchValue({ plea_cd: DisputeCountPleaCode.G });
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

  public isValid(countInx?): boolean {
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

  public onChangeRepresentedByLawyer(event: MatCheckboxChange) {
    this.additionalForm.markAsUntouched();
  }

  public onChangeWitnessPresent(event: MatCheckboxChange) {
    if (event.checked) {
      this.additionalForm.controls.witness_no.setValidators([Validators.min(this.minWitnesses), Validators.max(this.maxWitnesses), Validators.required]);
    } else {
      this.additionalForm.controls.witness_no.clearValidators();
      this.additionalForm.controls.witness_no.updateValueAndValidity();
    }
  }

  public getToolTipDEata(data) {
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
    if (this.optOut) {
      this.form.get("email_address").setValue(null);
      this.form.get("email_address").disable();
    } else {
      this.form.get("email_address").enable();
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

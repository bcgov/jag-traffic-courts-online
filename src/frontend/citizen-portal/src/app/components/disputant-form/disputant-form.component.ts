import { AfterViewInit, Component, OnInit, ViewChild, ChangeDetectionStrategy, Input } from "@angular/core";
import { FormControl, Validators } from "@angular/forms";
import { CountryCodeValue, ProvinceCodeValue } from "@config/config.model";
import { UtilsService } from "@core/services/utils.service";
import { ConfigService } from "@config/config.service";
import { Address } from "@shared/models/address.model";
import { AddressAutocompleteComponent } from "@shared/components/address-autocomplete/address-autocomplete.component";
import { DisputeContactTypeCd, Language } from "app/api";
import { NoticeOfDisputeFormGroup } from "app/services/notice-of-dispute.service";
import { DisputantContactInformationFormGroup } from "app/services/dispute.service";
import { DisputeFormMode } from "@shared/enums/dispute-form-mode";
import { FormControlValidators } from "@core/validators/form-control.validators";

@Component({
  selector: "app-disputant-form",
  templateUrl: "./disputant-form.component.html",
  styleUrls: ["./disputant-form.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DisputantFormComponent implements OnInit, AfterViewInit {
  @Input() isMobile: boolean;
  @Input() mode: DisputeFormMode;
  @Input() form: NoticeOfDisputeFormGroup | DisputantContactInformationFormGroup;
  @ViewChild(AddressAutocompleteComponent) private addressAutocomplete: AddressAutocompleteComponent;

  DisputeFormMode = DisputeFormMode;

  // Form essentials
  countryFormControl: FormControl<CountryCodeValue> = new FormControl(null, [Validators.required]);
  provinceFormControl: FormControl<ProvinceCodeValue> = new FormControl(null, [Validators.required]);
  driversLicenceProvinceFormControl: FormControl<ProvinceCodeValue> = new FormControl(null, [Validators.required]);
  optOut: boolean = false;

  // Form related
  todayDate: Date = new Date();
  showManualButton: boolean = true;
  showAddressFields: boolean = true; // temporary preset for testing
  ContactType = DisputeContactTypeCd;

  // Consume from the service
  languages: Language[] = [];
  provinces: ProvinceCodeValue[];
  states: ProvinceCodeValue[];
  countries: CountryCodeValue[] = this.config.countries;
  provincesAndStates: ProvinceCodeValue[] = this.config.provincesAndStates;
  bc: ProvinceCodeValue = this.config.bcCodeValue;
  canada: CountryCodeValue = this.config.canadaCodeValue;
  usa: CountryCodeValue = this.config.usaCodeValue;

  constructor(
    private utilsService: UtilsService,
    private config: ConfigService,
  ) {
    // config or static
    this.isMobile = this.utilsService.isMobile();

    this.countries = this.config.countries;
    this.provincesAndStates = this.config.provincesAndStates;
    this.provinces = this.provincesAndStates.filter(x => x.ctryId === this.canada.ctryId && x.provSeqNo !== this.bc.provSeqNo);  // skip BC it will be manually at top of list
    this.states = this.provincesAndStates.filter(x => x.ctryId === this.usa.ctryId); // USA only

    this.countryFormControl.valueChanges.subscribe(country => {
      this.onCountryChange(country)
    })

    this.provinceFormControl.valueChanges.subscribe(province => {
      this.onProvinceChange(province)
    })

    this.driversLicenceProvinceFormControl.valueChanges.subscribe(province => {
      this.onDLProvinceChange(province)
    })
  }

  onSelectContactType(newContactType: any) {
    console.log(newContactType);
    if (newContactType == this.ContactType.Lawyer) {
      // make contact law firm name required, blank out contact name
      this.form.get('contact_law_firm_name').clearValidators();
      this.form.get('contact_law_firm_name').addValidators([Validators.required]);
      this.form.get('contact_law_firm_name').setValue("");
      this.form.get('contact_surname').setValue("");
      this.form.get('contact_given_names').setValue("");
    } else if (newContactType == this.ContactType.Individual) {
      // set contact names to disputant names, blank out law firm name
      this.form.get('contact_law_firm_name').clearValidators();
      this.form.get('contact_law_firm_name').setValue("");
      this.form.get('contact_surname').setValue(this.form.get('disputant_surname').value);
      this.form.get('contact_given_names').setValue(this.form.get('disputant_given_names').value);
    } else {
      // set contact names to blank, blank out law firm name
      this.form.get('contact_law_firm_name').clearValidators();
      this.form.get('contact_law_firm_name').setValue("");
      this.form.get('contact_surname').setValue("");
      this.form.get('contact_given_names').setValue("");
    }
    this.form.get('contact_law_firm_name').updateValueAndValidity();
    this.form.get('contact_surname').updateValueAndValidity();
    this.form.get('contact_given_names').updateValueAndValidity();
    this.form.updateValueAndValidity();
  }

  ngOnInit(): void {
    if (!this.form.value.address_country_id) {
      this.countryFormControl.setValue(this.canada);
    } else {
      this.countryFormControl.setValue(this.countries.filter(i => i.ctryId === this.form.value.address_country_id).shift());
    }
    if (!this.form.value.address_province) {
      this.provinceFormControl.setValue(this.bc);
    } else {
      this.provinceFormControl.setValue(this.provincesAndStates.filter(i => i.provAbbreviationCd === this.form.value.address_province).shift());
    }

    let form = this.form as NoticeOfDisputeFormGroup;
    // search for drivers licence province using abbreviation e.g. BC
    if (form.value.drivers_licence_province) {
      let foundProvinces = this.provincesAndStates.filter(x => x.provAbbreviationCd === form.value.drivers_licence_province).shift();
      if (foundProvinces) {
        this.driversLicenceProvinceFormControl.setValue(foundProvinces);
      }
    } else if (form.controls.drivers_licence_province) { // have control but no value
      this.driversLicenceProvinceFormControl.setValue(this.bc);
    }

    // initialize contact type to individual on the ticket
    this.form?.get('contact_type').setValue(this.ContactType.Individual);
    this.form.get('contact_law_firm_name').clearValidators();
    this.form.get('contact_law_firm_name').setValue("");
    this.form.get('contact_surname').setValue(this.form.get('disputant_surname').value);
    this.form.get('contact_given_names').setValue(this.form.get('disputant_given_names').value);

  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.addressAutocomplete?.autocomplete.disable(); // disable auto complete component
    }, 0)
  }

  onCountryChange(country: CountryCodeValue) {
    setTimeout(() => {
      this.form.controls.address_country_id.setValue(country.ctryId);

      this.form.controls.postal_code.setValidators([Validators.maxLength(6)]);
      this.form.controls.address_province.setValidators([Validators.maxLength(30)]);
      this.form.controls.address_province.setValue(null);
      this.form.controls.address_province_seq_no.setValidators(null);
      this.form.controls.address_province_seq_no.setValue(null);
      this.form.controls.address_province_country_id.setValue(country.ctryId);
      this.form.controls.home_phone_number.setValidators([Validators.maxLength(20)]);

      if (this.isCA || this.isUSA) { // canada or usa validators
        this.form.controls.address_province_seq_no.addValidators([Validators.required]);
        this.form.controls.postal_code.addValidators([Validators.required]);
        this.form.controls.home_phone_number.addValidators([Validators.required, FormControlValidators.phone]);

        if (this.isCA) { // pick BC by default if Canada selected
          this.form.controls.address_province.setValue(this.bc.provNm);
          this.form.controls.address_province_seq_no.setValue(this.bc.provSeqNo)
        }
      }

      this.form.controls.postal_code.updateValueAndValidity();
      this.form.controls.address_province.updateValueAndValidity();
      this.form.controls.address_province_country_id.updateValueAndValidity();
      this.form.controls.address_province_seq_no.updateValueAndValidity();
      this.form.controls.home_phone_number.updateValueAndValidity();

      if (this.mode !== DisputeFormMode.UPDATEDISPUTANT) { // Disputant form do not have drivers licence information
        let form = this.form as NoticeOfDisputeFormGroup;
        form.controls.drivers_licence_number.setValidators([Validators.maxLength(20)]);
        form.controls.drivers_licence_province.setValidators([Validators.maxLength(30)]);

        if (this.isCA || this.isUSA) { // canada or usa validators
          form.controls.drivers_licence_number.addValidators([Validators.required]);
          form.controls.drivers_licence_province_seq_no.addValidators([Validators.required]);
        }

        form.controls.drivers_licence_number.updateValueAndValidity();
        form.controls.drivers_licence_province.updateValueAndValidity();
      }
    }, 0);
  }

  onProvinceChange(province: ProvinceCodeValue) {
    setTimeout(() => {
      this.form.controls.address_province.setValue(province.provAbbreviationCd); // for sending two char province or state code to ARC
      this.form.controls.address_province_country_id.setValue(province.ctryId);
      this.form.controls.address_province_seq_no.setValue(province.provSeqNo);
    }, 0)
  }

  onDLProvinceChange(province: ProvinceCodeValue) {
    setTimeout(() => {
      let form = this.form as NoticeOfDisputeFormGroup;
      form.controls.drivers_licence_province.setValue(province.provNm);
      form.controls.drivers_licence_country_id.setValue(province.ctryId);
      form.controls.drivers_licence_province_seq_no.setValue(province.provSeqNo);

      if (province.provId === this.bc.provId) {
        form.controls.drivers_licence_number.setValidators([Validators.maxLength(9)]);
        form.controls.drivers_licence_number.addValidators([Validators.minLength(7)]);
      } else {
        form.controls.drivers_licence_number.setValidators([Validators.maxLength(20)]);
      }

      if (province.ctryId === this.canada.ctryId || province.ctryId === this.usa.ctryId) {
        form.controls.drivers_licence_number.addValidators([Validators.required]);
      }
      form.controls.drivers_licence_number.updateValueAndValidity();
    }, 0)
  }

  onAddressAutocomplete({ countryCode, provinceCode, postalCode, address, city }: Address): void {
    // Will be implemented
  }

  get isCA() {
    return this.countryFormControl.value?.ctryId === this.canada.ctryId;
  }

  get isBC() {
    return this.provinceFormControl.value?.provId === this.bc.provId;
  }

  get isUSA() {
    return this.countryFormControl.value?.ctryId === this.usa.ctryId;
  }

  onOptOut() {
    if (this.optOut) {
      this.form.controls.email_address.setValue(null);
      this.form.controls.email_address.disable();
    } else {
      this.form.controls.email_address.enable();
    }
  }
}

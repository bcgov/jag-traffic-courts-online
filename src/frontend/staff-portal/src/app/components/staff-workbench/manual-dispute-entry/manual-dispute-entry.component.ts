import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { DisputeService, Dispute } from 'app/services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { AuthService } from 'app/services/auth.service';
import { UtilsService } from '@core/services/utils.service';
import { ConfigService } from '@config/config.service';
import { LookupsService, Statute } from 'app/services/lookups.service';
import { 
  DisputeContactTypeCd, 
  DisputeStatus,
  DisputeRequestCourtAppearanceYn,
  DisputeRepresentedByLawyer,
  DisputeInterpreterRequired,
  DisputeSignatoryType,
  DisputeDisputantDetectedOcrIssues,
  DisputeSystemDetectedOcrIssues,
  ViolationTicket,
  ViolationTicketCount,
  DisputeCount,
  DisputeCountRequestCourtAppearance,
  DisputeCountRequestReduction,
  DisputeCountRequestTimeToPay,
  RoadSafetyTicketSearchService,
  Ticket,
  Count
} from 'app/api';
import { ToastService } from '@core/services/toast.service';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';

@Component({
  selector: 'app-manual-dispute-entry',
  templateUrl: './manual-dispute-entry.component.html',
  styleUrls: ['./manual-dispute-entry.component.scss']
})
export class ManualDisputeEntryComponent implements OnInit {
  @Output() public backInbox: EventEmitter<any> = new EventEmitter();

  public isMobile: boolean;
  public ticketDetailsForm: FormGroup;
  public contactInfoForm: FormGroup;
  public disputeInfoForm: FormGroup;
  public provinces: any[];
  public states: any[];
  public countries: any[];
  public bc: any;
  public canada: any;
  public usa: any;
  public todayDate: Date = new Date();
  public maxDateSubmitted: Date = new Date();
  public filteredCount1Statutes: Statute[];
  public filteredCount2Statutes: Statute[];
  public filteredCount3Statutes: Statute[];
  
  public ContactType = DisputeContactTypeCd;
  public RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public InterpreterRequired = DisputeInterpreterRequired;
  public SignatoryType = DisputeSignatoryType;
  public RequestReduction = DisputeCountRequestReduction;
  public RequestTimeToPay = DisputeCountRequestTimeToPay;
  
  // Email opt-out flag
  public optOut: boolean = false;

  // Languages from lookup service
  public languages: any[] = [];

  // Legal representation form
  public legalRepresentationForm: FormGroup;

  // For count management
  public ticketCounts: ViolationTicketCount[] = [];
  public disputeCounts: DisputeCount[] = [];

  // Wizard step management
  public currentStep: number = 0;
  public steps = [
    { label: 'Ticket Details', icon: 'description', completed: false },
    { label: 'Ticket Counts', icon: 'list_alt', completed: false },
    { label: 'Contact Information', icon: 'contact_mail', completed: false },
    { label: 'Dispute Information', icon: 'gavel', completed: false },
    { label: 'Review & Submit', icon: 'check_circle', completed: false }
  ];

  constructor(
    protected formBuilder: FormBuilder,
    private utilsService: UtilsService,
    private disputeService: DisputeService,
    private logger: LoggerService,
    public config: ConfigService,
    public lookupsService: LookupsService,
    private toastService: ToastService,
    private dialog: MatDialog,
    private authService: AuthService,
    private roadSafetyTicketSearchService: RoadSafetyTicketSearchService
  ) {
    this.bc = this.config.bcCodeValue;
    this.canada = this.config.canadaCodeValue;
    this.usa = this.config.usaCodeValue;
    this.isMobile = this.utilsService.isMobile();

    // Subscribe to languages
    this.lookupsService.languages$.subscribe(languages => {
      this.languages = languages || [];
    });

    if (this.config.provincesAndStates) {
      this.provinces = this.config.provincesAndStates.filter(
        x => x.ctryId === this.canada.ctryId && x.provSeqNo !== this.bc.provSeqNo
      );
      this.states = this.config.provincesAndStates.filter(
        x => x.ctryId === this.usa.ctryId
      );
      this.countries = this.config.countries.filter(
        x => x.ctryId !== this.canada.ctryId && x.ctryId !== this.usa.ctryId
      );
    }
  }

  public ngOnInit() {
    this.initializeTicketDetailsForm();
    this.initializeContactInfoForm();
    this.initializeDisputeInfoForm();
    this.initializeLegalRepresentationForm();
    this.addTicketCount(); // Start with one count
  }

  private initializeTicketDetailsForm() {
    this.ticketDetailsForm = this.formBuilder.group({
      ticketNumber: [null, [Validators.required, Validators.pattern(/^[A-Za-z]{2}\d{8}$/)]],
      violationDate: [null, [Validators.required]],
      violationTime: [null, [Validators.required, Validators.pattern(/^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$/)]],
      disputantSurname: [null, [Validators.required, Validators.maxLength(30)]],
      disputantGivenNames: [null, [Validators.required, Validators.maxLength(92)]],
      driversLicenceProvince: [null, [Validators.required]],
      driversLicenceNumber: [null],
      driversLicenceCountry: [null],
      driversLicenceProvinceSeqNo: [null],
      driversLicenceCountryId: [null],
      courtLocation: [null, [Validators.required]]
    });

    // Set up driver's licence validation based on province
    this.ticketDetailsForm.get('driversLicenceProvince').valueChanges.subscribe(value => {
      this.onDLProvinceChange(value);
    });
  }

  private initializeContactInfoForm() {
    this.contactInfoForm = this.formBuilder.group({
      contactTypeCd: [DisputeContactTypeCd.Individual, [Validators.required]],
      contactSurnameNm: [null, [Validators.maxLength(30)]],
      contactGivenNames: [null, [Validators.maxLength(92)]],
      contactLawFirmNm: [null, [Validators.maxLength(200)]],
      address: [null, [Validators.required, Validators.maxLength(100)]],
      addressCity: [null, [Validators.required, Validators.maxLength(30)]],
      addressProvince: [null, [Validators.maxLength(30)]],
      addressProvinceSeqNo: [null],
      addressProvinceCountryId: [null],
      addressCountryId: [this.canada.ctryId, [Validators.required]],
      postalCode: [null, [Validators.maxLength(6)]],
      emailAddress: [null, [Validators.required, Validators.email, Validators.maxLength(100)]],
      homePhoneNumber: [null, [Validators.maxLength(20)]],
      emailVerified: [false]
    });

    // Set up contact type validation
    this.contactInfoForm.get('contactTypeCd').valueChanges.subscribe(value => {
      this.onSelectContactType(value);
    });

    // Set up country change validation
    this.contactInfoForm.get('addressCountryId').valueChanges.subscribe(value => {
      this.onCountryChange(value);
    });

    // Initialize with Canada/BC defaults
    this.onCountryChange(this.canada.ctryId);
    this.contactInfoForm.patchValue({
      addressProvinceSeqNo: this.bc.provSeqNo,
      addressProvinceCountryId: this.canada.ctryId
    });
  }

  private initializeDisputeInfoForm() {
    this.disputeInfoForm = this.formBuilder.group({
      dateSubmitted: [new Date(new Date().toDateString()), [Validators.required]],
      requestCourtAppearance: [null, [Validators.required]],
      representedByLawyer: [DisputeRepresentedByLawyer.N],
      interpreterRequired: [DisputeInterpreterRequired.N],
      interpreterLanguageCd: [null],
      witnessNo: [0],
      signatoryType: [null],
      signatoryName: [null],
      fineReductionReason: [null],
      timeToPayReason: [null]
    });
  }
  private initializeLegalRepresentationForm() {
    this.legalRepresentationForm = this.formBuilder.group({
      lawFirmName: [null],
      lawyerFullName: [null],
      lawyerAddress: [null],
      lawyerPhoneNumber: [null],
      lawyerEmail: [null]
    });
  }

  public addTicketCount() {
    const countNumber = this.ticketCounts.length + 1;
    
    if (countNumber > 3) {
      this.toastService.openErrorToast('Maximum 3 counts allowed');
      return;
    }

    const newCount: ViolationTicketCount = {
      countNo: countNumber,
      description: null,
      actOrRegulationNameCode: null,
      ticketedAmount: null,
      section: null,
      subsection: null,
      paragraph: null,
      subparagraph: null
    };

    const newDisputeCount: DisputeCount = {
      countNo: countNumber,
      requestCourtAppearance: DisputeCountRequestCourtAppearance.N,
      requestReduction: DisputeCountRequestReduction.N,
      requestTimeToPay: DisputeCountRequestTimeToPay.N,
      pleaCode: null
    };

    this.ticketCounts.push(newCount);
    this.disputeCounts.push(newDisputeCount);

    // Add form controls for this count
    const countFormGroup = this.formBuilder.group({
      description: [null, [Validators.required]],
      actOrRegulationNameCode: [null, [Validators.required]],
      ticketedAmount: [null, [Validators.required, Validators.min(0)]],
      section: [null],
      subsection: [null],
      paragraph: [null],
      subparagraph: [null],
      pleaCode: [null],
      requestCourtAppearance: [DisputeCountRequestCourtAppearance.N],
      requestReduction: [DisputeCountRequestReduction.N],
      requestTimeToPay: [DisputeCountRequestTimeToPay.N],
      __skip: [false]
    });

    // Set up watchers to update consolidated reason field validators
    countFormGroup.get('requestReduction').valueChanges.subscribe(() => {
      this.updateReasonValidators();
    });

    countFormGroup.get('requestTimeToPay').valueChanges.subscribe(() => {
      this.updateReasonValidators();
    });

    countFormGroup.get('__skip').valueChanges.subscribe(() => {
      this.updateReasonValidators();
    });

    this.disputeInfoForm.addControl(`count${countNumber}`, countFormGroup);
  }

  public removeTicketCount(countNumber: number) {
    const index = this.ticketCounts.findIndex(c => c.countNo === countNumber);
    if (index > -1) {
      this.ticketCounts.splice(index, 1);
      this.disputeCounts.splice(index, 1);
      this.disputeInfoForm.removeControl(`count${countNumber}`);
      
      // Renumber remaining counts
      this.ticketCounts.forEach((count, idx) => {
        count.countNo = idx + 1;
        this.disputeCounts[idx].countNo = idx + 1;
      });
    }
  }

  public onDLProvinceChange(provId: number) {
    const dlNumberControl = this.ticketDetailsForm.get('driversLicenceNumber');
    
    const selectedProv = this.config.provincesAndStates.find(x => x.provId === provId);
    if (selectedProv) {
      this.ticketDetailsForm.patchValue({
        driversLicenceProvinceSeqNo: selectedProv.provSeqNo,
        driversLicenceCountryId: selectedProv.ctryId,
        driversLicenceCountry: selectedProv.ctryId === this.canada.ctryId ? 'Canada' : 'USA'
      });

      // BC requires 7-9 digit numeric DL
      if (selectedProv.provSeqNo === this.bc.provSeqNo && selectedProv.ctryId === this.canada.ctryId) {
        dlNumberControl.setValidators([Validators.required, Validators.minLength(7), Validators.maxLength(9), Validators.pattern(/^\d+$/)]);
      } else {
        // Other provinces/states: 7-30 characters
        dlNumberControl.setValidators([Validators.minLength(7), Validators.maxLength(30)]);
      }
      dlNumberControl.updateValueAndValidity();
    }
  }

  public onSelectContactType(newContactType: DisputeContactTypeCd) {
    const surnameControl = this.contactInfoForm.get('contactSurnameNm');
    const givenNamesControl = this.contactInfoForm.get('contactGivenNames');
    const lawFirmControl = this.contactInfoForm.get('contactLawFirmNm');

    // Clear all values when changing contact type
    surnameControl.setValue(null);
    givenNamesControl.setValue(null);
    lawFirmControl.setValue(null);

    // Reset validators while preserving max length
    surnameControl.setValidators([Validators.maxLength(30)]);
    givenNamesControl.setValidators([Validators.maxLength(92)]);
    lawFirmControl.setValidators([Validators.maxLength(200)]);

    if (newContactType === DisputeContactTypeCd.Lawyer) {
      // For lawyer: all three fields required
      lawFirmControl.addValidators([Validators.required]);
      surnameControl.addValidators([Validators.required]);
      givenNamesControl.addValidators([Validators.required]);
    } else if (newContactType === DisputeContactTypeCd.Individual) {
      // For individual: no contact info required
    } else if (newContactType === DisputeContactTypeCd.Other) {
      // For agent/other: only names required
      surnameControl.addValidators([Validators.required]);
      givenNamesControl.addValidators([Validators.required]);
    }

    surnameControl.updateValueAndValidity();
    givenNamesControl.updateValueAndValidity();
    lawFirmControl.updateValueAndValidity();
  }

  public onCountryChange(ctryId: number) {
    setTimeout(() => {
      const postalControl = this.contactInfoForm.get('postalCode');
      const provinceControl = this.contactInfoForm.get('addressProvince');
      const provinceSeqControl = this.contactInfoForm.get('addressProvinceSeqNo');
      const provinceCountryIdControl = this.contactInfoForm.get('addressProvinceCountryId');
      const phoneControl = this.contactInfoForm.get('homePhoneNumber');

      // Clear previous validators
      postalControl.clearValidators();
      provinceControl.clearValidators();
      provinceSeqControl.clearValidators();
      phoneControl.clearValidators();

      // Set base validators
      postalControl.setValidators([Validators.maxLength(6)]);
      provinceControl.setValidators([Validators.maxLength(30)]);
      phoneControl.setValidators([Validators.maxLength(20)]);

      // Clear province values if country changes
      provinceControl.setValue(null);
      provinceSeqControl.setValue(null);
      provinceCountryIdControl.setValue(null);

      if (ctryId === this.canada.ctryId || ctryId === this.usa.ctryId) {
        // Canada or USA specific validators
        provinceSeqControl.addValidators([Validators.required]);
        postalControl.addValidators([Validators.required]);
        phoneControl.addValidators([FormControlValidators.phone]);
        
        if (ctryId === this.canada.ctryId) {
          // Canada: Postal code format and set BC as default
          postalControl.setValidators([Validators.required, Validators.minLength(6), Validators.maxLength(6)]);
          provinceCountryIdControl.setValue(ctryId);
          provinceSeqControl.setValue(this.bc.provSeqNo);
        } else {
          // USA: Zip code format
          postalControl.setValidators([Validators.required, Validators.minLength(5), Validators.maxLength(5)]);
        }
      }

      postalControl.updateValueAndValidity();
      provinceControl.updateValueAndValidity();
      provinceSeqControl.updateValueAndValidity();
      phoneControl.updateValueAndValidity();
    }, 5);
  }

  public onAddressProvinceChange(provId: number) {
    const selectedProv = this.config.provincesAndStates.find(x => x.provId === provId);
    if (selectedProv) {
      this.contactInfoForm.patchValue({
        addressProvinceSeqNo: selectedProv.provSeqNo,
        addressProvinceCountryId: selectedProv.ctryId
      });
    }
  }

  public filterStatutes(val: string): Statute[] {
    if (!val) {
      return this.lookupsService.statutes;
    }
    const filterValue = val.toLowerCase();
    return this.lookupsService.statutes.filter(statute => 
      statute.__statuteString?.toLowerCase().includes(filterValue) ||
      statute.code?.toLowerCase().includes(filterValue) ||
      statute.shortDescriptionText?.toLowerCase().includes(filterValue)
    );
  }

  /**
   * Keyup handlers for each count's description field
   */
  public onDescriptionKeyup(countNo: number) {
    const descriptionValue = this.disputeInfoForm.get(`count${countNo}`)?.get('description')?.value;
    
    switch(countNo) {
      case 1:
        this.filteredCount1Statutes = this.filterStatutes(descriptionValue);
        break;
      case 2:
        this.filteredCount2Statutes = this.filterStatutes(descriptionValue);
        break;
      case 3:
        this.filteredCount3Statutes = this.filterStatutes(descriptionValue);
        break;
    }
  }

  /**
   * Updates Act/Regulation and related fields when description changes
   * Called when user selects an option from autocomplete or on change event
   */
  public onDescriptionChange(countNo: number, fullDescription: string) {
    if (!fullDescription || fullDescription.trim() === '') {
      this.clearCountStatuteFields(countNo);
      return;
    }

    const countFormGroup = this.disputeInfoForm.get(`count${countNo}`);
    const parts = fullDescription.split(' '); // act/code description

    // Lookup statute from parts[0] (actCode) and parts[1] (code)
    if (parts && parts.length > 1) {
      const foundStatute = this.lookupsService.statutes?.find(
        (x) =>
          x.actCode?.toUpperCase() === parts[0]?.toUpperCase() &&
          x.code?.toUpperCase() === parts[1]?.toUpperCase()
      );

      if (foundStatute) {
        this.updateCountWithStatute(countNo, foundStatute);
        // Update the description field to show the full statute string
        countFormGroup?.patchValue({
          description: foundStatute.__statuteString
        });
      } else {
        this.clearCountStatuteFields(countNo);
      }
      countFormGroup?.updateValueAndValidity();
    }
  }

  /**
   * Finds the best matching statute from filtered results
   */
  private findMatchingStatute(statutes: Statute[], searchText: string): Statute | null {
    if (!statutes || statutes.length === 0) {
      return null;
    }

    const searchUpper = searchText.toUpperCase().trim();
    
    // First try to find exact match on the full statute string
    const exactMatch = statutes.find(s => 
      s.__statuteString?.toUpperCase() === searchUpper
    );
    
    if (exactMatch) {
      return exactMatch;
    }

    // Try to match by description text
    const descMatch = statutes.find(s => 
      s.shortDescriptionText?.toUpperCase() === searchUpper
    );
    
    if (descMatch) {
      return descMatch;
    }

    // Return first match if searching
    return statutes[0];
  }

  /**
   * Updates all statute-related fields for a count
   */
  private updateCountWithStatute(countNo: number, statute: Statute) {
    const countFormGroup = this.disputeInfoForm.get(`count${countNo}`);
    const count = this.ticketCounts.find(c => c.countNo === countNo);
    
    if (!countFormGroup || !count) {
      return;
    }

    // Update the count object
    count.actOrRegulationNameCode = statute.actCode;
    count.section = statute.sectionText;
    count.subsection = statute.subsectionText;
    count.paragraph = statute.paragraphText;
    count.subparagraph = statute.subparagraphText;
    count.description = statute.shortDescriptionText;

    // Update the form control values
    countFormGroup.patchValue({
      actOrRegulationNameCode: statute.actCode,
      section: statute.sectionText,
      subsection: statute.subsectionText,
      paragraph: statute.paragraphText,
      subparagraph: statute.subparagraphText,
      description: statute.__statuteString || `${statute.actCode} ${statute.code} ${statute.shortDescriptionText}`
    });
  }

  /**
   * Clears all statute-related fields for a count
   */
  private clearCountStatuteFields(countNo: number) {
    const countFormGroup = this.disputeInfoForm.get(`count${countNo}`);
    const count = this.ticketCounts.find(c => c.countNo === countNo);
    
    if (countFormGroup) {
      countFormGroup.patchValue({
        actOrRegulationNameCode: null,
        section: null,
        subsection: null,
        paragraph: null,
        subparagraph: null
      });
    }

    if (count) {
      count.actOrRegulationNameCode = null;
      count.section = null;
      count.subsection = null;
      count.paragraph = null;
      count.subparagraph = null;
    }
  }

  // Wizard Navigation Methods
  public goToStep(stepIndex: number) {
    //if (stepIndex < this.currentStep || this.validateCurrentStep())
    this.currentStep = stepIndex;
    this.scrollToTop();
  }

  public nextStep() {
    if (this.validateCurrentStep()) {
      this.steps[this.currentStep].completed = true;
    }
    if (this.currentStep < this.steps.length - 1) {
        this.currentStep++;
        this.scrollToTop();
    }
  }

  public previousStep() {
    if (this.currentStep > 0) {
      this.currentStep--;
      this.scrollToTop();
    }
  }

  private scrollToTop() {
    // Scroll window to top
    window.scrollTo({ top: 0, behavior: 'smooth' });
    
    // Also try scrolling any parent scroll containers
    setTimeout(() => {
      const wizardContainer = document.querySelector('.wizard-container');
      if (wizardContainer) {
        wizardContainer.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    }, 0);
  }

  public isStepValid(stepIndex: number): boolean {
    switch (stepIndex) {
      case 0: // Ticket Details (basic info only, no counts)
        return this.isTicketBasicInfoValid();
      case 1: // Ticket Counts
        return this.isTicketCountsValid();
      case 2: // Contact Information
        return this.contactInfoForm.valid;
      case 3: // Dispute Information
        return this.disputeInfoForm.valid;
      case 4: // Review & Submit
        return this.validateAllForms();
      default:
        return false;
    }
  }

  private isTicketBasicInfoValid(): boolean {
    const form = this.ticketDetailsForm;
    return form.get('ticketNumber')?.valid &&
           form.get('violationDate')?.valid &&
           form.get('violationTime')?.valid &&
           form.get('courtLocation')?.valid &&
           form.get('disputantSurname')?.valid &&
           form.get('disputantGivenNames')?.valid &&
           form.get('driversLicenceProvince')?.valid &&
           (form.get('driversLicenceProvinceSeqNo')?.value !== this.bc.provSeqNo || 
            form.get('driversLicenceNumber')?.valid);
  }

  private isTicketCountsValid(): boolean {
    if (this.ticketCounts.length === 0) return false;
    
    for (const count of this.ticketCounts) {
      const countFormGroup = this.disputeInfoForm.get(`count${count.countNo}`);
      if (!countFormGroup) return false;
      
      const description = countFormGroup.get('description')?.value;
      const actCode = countFormGroup.get('actOrRegulationNameCode')?.value;
      const amount = countFormGroup.get('ticketedAmount')?.value;
      
      if (!description || !actCode || !amount) {
        return false;
      }
    }
    return true;
  }

  private validateCurrentStep(): boolean {
    const isValid = this.isStepValid(this.currentStep);
    
    if (!isValid) {
      // Mark all fields as touched to show validation errors
      switch (this.currentStep) {
        case 0:
          this.ticketDetailsForm.markAllAsTouched();
          break;
        case 1:
          this.ticketCounts.forEach(count => {
            this.disputeInfoForm.get(`count${count.countNo}`)?.markAllAsTouched();
          });
          break;
        case 2:
          this.contactInfoForm.markAllAsTouched();
          break;
        case 3:
          this.disputeInfoForm.markAllAsTouched();
          break;
      }
      this.toastService.openErrorToast('Please fill in all required fields before proceeding');
    }
    
    return isValid;
  }

  public getReviewData() {
    return {
      ticketDetails: this.ticketDetailsForm.value,
      contactInfo: this.contactInfoForm.value,
      disputeInfo: this.disputeInfoForm.value,
      ticketCounts: this.ticketCounts.map(count => ({
        countNo: count.countNo,
        ...this.disputeInfoForm.get(`count${count.countNo}`)?.value
      }))
    };
  }

  public onBack() {
    this.backInbox.emit();
  }

  public onBackWithConfirmation() {
    const data: DialogOptions = {
      titleKey: "Discard Changes?",
      messageKey: "Are you sure you want to leave this page? All unsaved data will be lost.",
      actionTextKey: "Discard",
      actionType: "warn",
      cancelTextKey: "Cancel",
      icon: "warning",
    };

    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.onBack();
        }
      });
  }

  public onFindTicket() {
    const ticketNumber = this.ticketDetailsForm.get('ticketNumber').value;
    const violationTime = this.ticketDetailsForm.get('violationTime').value;

    if (!ticketNumber || !violationTime) {
      this.toastService.openErrorToast('Please enter both Ticket Number and Violation Time.');
      return;
    }

    this.logger.log('ManualDisputeEntryComponent::onFindTicket - Searching for ticket:', ticketNumber, violationTime);
    
    this.roadSafetyTicketSearchService.apiRoadsafetyticketsearchGet(ticketNumber, violationTime).subscribe({
      next: (result: Ticket) => {
        this.logger.log('ManualDisputeEntryComponent::onFindTicket - Ticket found:', result);
        console.log('Ticket Search Result:', result);
        this.populateFormFromTicket(result);
        this.toastService.openSuccessToast('Ticket found and form populated successfully!');
      },
      error: (error) => {
        this.logger.error('ManualDisputeEntryComponent::onFindTicket - Error:', error);
        console.error('Ticket Search Error:', error);
        this.toastService.openErrorToast('Failed to find ticket. Please verify the ticket number and time.');
      }
    });
  }

  /**
   * Populates the form fields from the ticket search result
   */
  private populateFormFromTicket(ticket: Ticket): void {
    if (!ticket) return;

    // Parse the issued datetime
    if (ticket.issued) {
      const issuedDate = new Date(ticket.issued);
      this.ticketDetailsForm.patchValue({
        violationDate: issuedDate,
        violationTime: this.formatTime(issuedDate)
      });
    }

    // Populate disputant name
    const givenNames = [ticket.firstGivenName, ticket.secondGivenName]
      .filter(name => name && name.trim())
      .join(' ');
    
    this.ticketDetailsForm.patchValue({
      ticketNumber: ticket.number,
      disputantSurname: ticket.surname,
      disputantGivenNames: givenNames
    });

    // Clear existing counts
    while (this.ticketCounts.length > 0) {
      this.removeTicketCount(this.ticketCounts[0].countNo);
    }

    // Add and populate counts from ticket
    if (ticket.counts && ticket.counts.length > 0) {
      ticket.counts.forEach((count: Count, index: number) => {
        // Add a new count
        this.addTicketCount();
        
        const countNo = index + 1;
        const countFormGroup = this.disputeInfoForm.get(`count${countNo}`);
        
        if (countFormGroup) {
          // Determine act or regulation code
          const actOrRegCode = count.act || '';
          
          // Build description string
          const description = count.description || '';
          
          // Patch count form values
          countFormGroup.patchValue({
            description: description,
            actOrRegulationNameCode: actOrRegCode,
            ticketedAmount: count.ticketedAmount,
            section: count.section || null,
            subsection: count.subsection || null,
            paragraph: count.paragraph || null,
            subparagraph: count.subparagraph || null
          });

          // Update the ticket count in the array
          if (this.ticketCounts[index]) {
            this.ticketCounts[index].description = description;
            this.ticketCounts[index].actOrRegulationNameCode = actOrRegCode;
            this.ticketCounts[index].ticketedAmount = count.ticketedAmount;
            this.ticketCounts[index].section = count.section;
            this.ticketCounts[index].subsection = count.subsection;
            this.ticketCounts[index].paragraph = count.paragraph;
            this.ticketCounts[index].subparagraph = count.subparagraph;
          }
        }
      });
    }

    this.logger.log('ManualDisputeEntryComponent::populateFormFromTicket - Form populated with ticket data');
  }

  /**
   * Formats a Date object to HH:MM string
   */
  private formatTime(date: Date): string {
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }

  public onSubmit() {
    if (!this.validateAllForms()) {
      this.toastService.openErrorToast('Please fill in all required fields');
      return;
    }

    const data: DialogOptions = {
      titleKey: "Submit Manual Dispute Entry?",
      messageKey: "Are you sure you want to submit this manually entered dispute? Please verify all information is correct.",
      actionTextKey: "Submit Dispute",
      actionType: "primary",
      cancelTextKey: "Cancel",
      icon: "help",
    };

    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.submitDispute();
        }
      });
  }

  private validateAllForms(): boolean {
    this.ticketDetailsForm.markAllAsTouched();
    this.contactInfoForm.markAllAsTouched();
    this.disputeInfoForm.markAllAsTouched();

    return this.ticketDetailsForm.valid && 
           this.contactInfoForm.valid && 
           this.disputeInfoForm.valid;
  }

  private submitDispute() {
    try {
      const dispute: Dispute = this.buildDisputeObject();
      
      this.logger.log('ManualDisputeEntryComponent::submitDispute - Submitting dispute:', dispute);
      
      this.disputeService.createDispute(dispute).subscribe({
        next: (response: Dispute) => {
          this.logger.info('ManualDisputeEntryComponent::submitDispute - Success:', response);
          this.toastService.openSuccessToast('Dispute created successfully');
          this.backInbox.emit();
        },
        error: (error: any) => {
          this.logger.error('ManualDisputeEntryComponent::submitDispute - Error:', error);
          // Error toast is already shown by the service
        }
      });
    } catch (error) {
      this.logger.error('ManualDisputeEntryComponent::submitDispute - Exception:', error);
      this.toastService.openErrorToast('Failed to build dispute object. Please check all fields.');
    }
  }

  private buildDisputeObject(): Dispute {
    const ticketData = this.ticketDetailsForm.value;
    const contactData = this.contactInfoForm.value;
    const disputeData = this.disputeInfoForm.value;
    const legalRepData = this.legalRepresentationForm.value;

    // Build violation ticket
    const violationTicket: ViolationTicket = {
      ticketNumber: ticketData.ticketNumber,
      disputantSurname: ticketData.disputantSurname,
      disputantGivenNames: ticketData.disputantGivenNames,
      disputantDriversLicenceNumber: ticketData.driversLicenceNumber,
      driversLicenceProvince: this.getProvinceCode(ticketData.driversLicenceProvince),
      driversLicenceCountry: ticketData.driversLicenceCountry,
      issuedTs: this.formatDateTime(ticketData.violationDate, ticketData.violationTime),
      courtLocation: ticketData.courtLocation,
      violationTicketCounts: this.buildViolationTicketCounts()
    };

    // Build dispute counts
    const disputeCounts = this.buildDisputeCounts();

    // Split given names if needed
    const givenNames = ticketData.disputantGivenNames ? ticketData.disputantGivenNames.split(' ') : [];

    // Split lawyer full name into surname and given names
    const lawyerNames = legalRepData.lawyerFullName ? legalRepData.lawyerFullName.split(' ') : [];
    const lawyerSurname = lawyerNames.length > 0 ? lawyerNames[lawyerNames.length - 1] : null;
    const lawyerGivenName1 = lawyerNames.length > 1 ? lawyerNames[0] : null;
    const lawyerGivenName2 = lawyerNames.length > 2 ? lawyerNames[1] : null;
    const lawyerGivenName3 = lawyerNames.length > 3 ? lawyerNames[2] : null;

    const dispute: Dispute = {
      // Ticket information
      ticketNumber: ticketData.ticketNumber,
      issuedTs: violationTicket.issuedTs,
      submittedTs: disputeData.dateSubmitted ? this.formatDateTimeForApi(disputeData.dateSubmitted) : this.formatDateTimeForApi(new Date()),
      
      // Disputant information
      disputantSurname: ticketData.disputantSurname,
      disputantGivenName1: givenNames[0] || null,
      disputantGivenName2: givenNames[1] || null,
      disputantGivenName3: givenNames[2] || null,
      driversLicenceNumber: ticketData.driversLicenceNumber,
      driversLicenceIssuedCountryId: ticketData.driversLicenceCountryId,
      driversLicenceIssuedProvinceSeqNo: ticketData.driversLicenceProvinceSeqNo,
      
      // Contact information
      contactTypeCd: contactData.contactTypeCd,
      contactSurnameNm: contactData.contactSurnameNm,
      contactGiven1Nm: contactData.contactGivenNames?.split(' ')[0] || null,
      contactGiven2Nm: contactData.contactGivenNames?.split(' ')[1] || null,
      contactGiven3Nm: contactData.contactGivenNames?.split(' ')[2] || null,
      contactLawFirmNm: contactData.contactLawFirmNm,
      addressLine1: contactData.address,
      addressCity: contactData.addressCity,
      addressProvince: this.getAddressProvince(contactData),
      addressProvinceSeqNo: contactData.addressProvinceSeqNo,
      addressProvinceCountryId: contactData.addressProvinceCountryId,
      addressCountryId: contactData.addressCountryId,
      postalCode: contactData.postalCode,
      emailAddress: contactData.emailAddress,
      emailAddressVerified: undefined,
      homePhoneNumber: contactData.homePhoneNumber,
      
      // Dispute information
      requestCourtAppearanceYn: disputeData.requestCourtAppearance,
      representedByLawyer: disputeData.representedByLawyer,
      interpreterRequired: disputeData.interpreterRequired,
      interpreterLanguageCd: disputeData.interpreterLanguageCd,
      witnessNo: disputeData.witnessNo || 0,
      signatoryType: disputeData.signatoryType,
      signatoryName: disputeData.signatoryName,
      fineReductionReason: disputeData.fineReductionReason,
      timeToPayReason: disputeData.timeToPayReason,
      
      // Legal representation information (from legalRepresentationForm)
      lawFirmName: legalRepData.lawFirmName || null,
      lawyerSurname: lawyerSurname,
      lawyerGivenName1: lawyerGivenName1,
      lawyerGivenName2: lawyerGivenName2,
      lawyerGivenName3: lawyerGivenName3,
      lawyerAddress: legalRepData.lawyerAddress || null,
      lawyerPhoneNumber: legalRepData.lawyerPhoneNumber || null,
      lawyerEmail: legalRepData.lawyerEmail || null,
      
      // OCR fields - set to 'N' for manual entry (no OCR involved)
      disputantDetectedOcrIssues: DisputeDisputantDetectedOcrIssues.N,
      systemDetectedOcrIssues: DisputeSystemDetectedOcrIssues.N,
      
      // Status
      status: DisputeStatus.New,
      
      // Courthouse
      courtAgenId: this.getCourtAgencyId(ticketData.courtLocation),
      
      // Violation ticket and counts
      violationTicket: violationTicket,
      disputeCounts: disputeCounts,
      
      // Staff entry flag - this is the new flag mentioned in the JIRA
      // Note: This would need to be added to the API model
      // enteredByStaff: true
    };

    return dispute;
  }

  private buildViolationTicketCounts(): ViolationTicketCount[] {
    return this.ticketCounts.map((count, index) => {
      const countFormData = this.disputeInfoForm.get(`count${count.countNo}`).value;
      return {
        countNo: count.countNo,
        description: countFormData.description,
        actOrRegulationNameCode: countFormData.actOrRegulationNameCode,
        ticketedAmount: countFormData.ticketedAmount,
        section: countFormData.section,
        subsection: countFormData.subsection,
        paragraph: countFormData.paragraph,
        subparagraph: countFormData.subparagraph
      };
    });
  }

  private buildDisputeCounts(): DisputeCount[] {
    return this.disputeCounts.map((count, index) => {
      const countFormData = this.disputeInfoForm.get(`count${count.countNo}`).value;
      
      // Determine pleaCode based on requestReduction value:
      // 'Y' = "I agree I committed this offence..." → Guilty plea
      // 'N' = "I do not agree I committed this offence..." → Not Guilty plea
      // 'UNKNOWN' or null = Skip count → Unknown plea
      let pleaCode: any;
      if (countFormData.requestReduction === DisputeCountRequestReduction.Y) {
        pleaCode = 'G'; // Guilty
      } else if (countFormData.requestReduction === DisputeCountRequestReduction.N) {
        pleaCode = 'N'; // Not Guilty
      } else {
        pleaCode = 'UNKNOWN'; // Skip or unknown
      }
      
      return {
        countNo: count.countNo,
        requestCourtAppearance: countFormData.requestCourtAppearance,
        requestReduction: countFormData.requestReduction,
        requestTimeToPay: countFormData.requestTimeToPay,
        pleaCode: pleaCode
      };
    });
  }

  /**
   * Format date to Oracle API format: yyyy-MM-ddTHH:mm:ss (no milliseconds)
   */
  private formatDateTimeForApi(date: Date): string {
    if (!date) return null;
    
    const d = date instanceof Date ? date : new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    const hours = String(d.getHours()).padStart(2, '0');
    const minutes = String(d.getMinutes()).padStart(2, '0');
    const seconds = String(d.getSeconds()).padStart(2, '0');
    
    return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
  }

  private formatDateTime(date: any, time: string): string {
    if (!date || !time) return null;
    
    let dateStr: string;
    if (date instanceof Date) {
      dateStr = date.toISOString().split('T')[0];
    } else if (typeof date === 'string') {
      dateStr = date.split('T')[0];
    } else {
      dateStr = new Date(date).toISOString().split('T')[0];
    }
    
    return `${dateStr}T${time}:00`;
  }

  private getProvinceCode(provId: number): string {
    if (!provId) return null;
    const prov = this.config.provincesAndStates.find(x => x.provId === provId);
    return prov?.provNm || null;
  }

  private getAddressProvince(contactData: any): string {
    // Check if country is Canada or USA (province is selected from dropdown)
    if (contactData.addressCountryId === this.canada.ctryId || contactData.addressCountryId === this.usa.ctryId) {
      // Province/State was selected from dropdown, so addressProvince contains provId
      return this.getProvinceCode(contactData.addressProvince);
    } else {
      // Province/State was entered as free text
      return contactData.addressProvince;
    }
  }

  /**
   * Get country name for display
   */
  public getCountryName(ctryId: number): string {
    if (!ctryId) return '';
    const country = this.config.countries.find(x => x.ctryId === ctryId);
    if (country) return country.ctryLongNm;
    if (ctryId === this.canada.ctryId) return this.canada.ctryLongNm;
    if (ctryId === this.usa.ctryId) return this.usa.ctryLongNm;
    return '';
  }

  /**
   * Get province/state name for display in review
   */
  public getProvinceNameForDisplay(contactData: any): string {
    if (!contactData.addressProvince) return 'Not provided';
    return this.getAddressProvince(contactData);
  }

  private getCourtAgencyId(courtLocation: string): string {
    if (!courtLocation) return null;
    const court = this.lookupsService.courthouseAgencies.find(x => x.name === courtLocation);
    return court?.id || null;
  }

  onKeyPressNumbers(event: any, BCOnly: boolean) {
    const charCode = (event.which) ? event.which : event.keyCode;
    if ((charCode < 48 || charCode > 57) && BCOnly) {
      event.preventDefault();
      return false;
    }
    return true;
  }

  /**
   * Handles email opt-out preference
   */
  public onOptOut() {
    const emailControl = this.contactInfoForm.get('emailAddress');
    if (this.optOut) {
      emailControl.setValue(null);
      emailControl.clearValidators();
      emailControl.setValidators([Validators.email, Validators.maxLength(100)]);
      emailControl.disable();
    } else {
      emailControl.enable();
      emailControl.setValidators([Validators.required, Validators.email, Validators.maxLength(100)]);
    }
    emailControl.updateValueAndValidity();
  }

  /**
   * Handles request court appearance selection
   */
  public onChangeRequestCourtAppearance(value: DisputeRequestCourtAppearanceYn) {
    if (value === this.RequestCourtAppearance.N) {
      // When not requesting court appearance, set signatory type required
      this.disputeInfoForm.get('signatoryType').setValidators([Validators.required]);
      this.disputeInfoForm.get('signatoryType').updateValueAndValidity({ emitEvent: false });
      
      // Reset interpreter and witness for written reasons
      this.disputeInfoForm.patchValue({
        representedByLawyer: DisputeRepresentedByLawyer.N,
        interpreterRequired: DisputeInterpreterRequired.N,
        witnessNo: 0
      });
      
      // Clear plea codes when switching to written reasons
      this.ticketCounts.forEach(count => {
        const countFormGroup = this.disputeInfoForm.get(`count${count.countNo}`);
        if (countFormGroup) {
          countFormGroup.patchValue({ pleaCode: null });
        }
      });
    } else if (value === this.RequestCourtAppearance.Y) {
      // When requesting court appearance, signatory not needed
      this.disputeInfoForm.get('signatoryType').clearValidators();
      this.disputeInfoForm.get('signatoryType').setValue(null);
      this.disputeInfoForm.get('signatoryName').clearValidators();
      this.disputeInfoForm.get('signatoryName').setValue(null);
      this.disputeInfoForm.get('signatoryType').updateValueAndValidity({ emitEvent: false });
      this.disputeInfoForm.get('signatoryName').updateValueAndValidity({ emitEvent: false });
      
      // Clear fine reduction and time to pay when switching to court appearance
      this.ticketCounts.forEach(count => {
        const countFormGroup = this.disputeInfoForm.get(`count${count.countNo}`);
        if (countFormGroup) {
          countFormGroup.patchValue({
            requestReduction: DisputeCountRequestReduction.N,
            requestTimeToPay: DisputeCountRequestTimeToPay.N
          });
        }
      });
    }
  }

  /**
   * Handles signature type change
   */
  public onChangeSignatureType(value: DisputeSignatoryType) {
    const sigNameControl = this.disputeInfoForm.get('signatoryName');
    
    if (value === this.SignatoryType.D || value === this.SignatoryType.A) {
      // Set validators when a signature type is selected
      sigNameControl.setValidators([Validators.required, Validators.maxLength(100)]);
    } else {
      // Clear validators when no type selected
      sigNameControl.clearValidators();
    }
    sigNameControl.updateValueAndValidity({ emitEvent: false });
  }

  /**
   * Handles represented by lawyer checkbox change
   */
  public onChangeRepresentedByLawyer(checked: boolean) {
    const value = checked ? DisputeRepresentedByLawyer.Y : DisputeRepresentedByLawyer.N;
    this.disputeInfoForm.patchValue({ representedByLawyer: value });
    
    if (value === DisputeRepresentedByLawyer.Y) {
      // Set validators when lawyer representation is selected
      this.legalRepresentationForm.get('lawFirmName').setValidators([Validators.required, Validators.maxLength(200)]);
      this.legalRepresentationForm.get('lawyerFullName').setValidators([Validators.required, Validators.maxLength(100)]);
      this.legalRepresentationForm.get('lawyerAddress').setValidators([Validators.required, Validators.maxLength(304)]);
      this.legalRepresentationForm.get('lawyerPhoneNumber').setValidators([Validators.required]);
      this.legalRepresentationForm.get('lawyerEmail').setValidators([Validators.required, Validators.email, Validators.maxLength(100)]);
    } else {
      // Clear validators and reset form
      this.legalRepresentationForm.get('lawFirmName').clearValidators();
      this.legalRepresentationForm.get('lawyerFullName').clearValidators();
      this.legalRepresentationForm.get('lawyerAddress').clearValidators();
      this.legalRepresentationForm.get('lawyerPhoneNumber').clearValidators();
      this.legalRepresentationForm.get('lawyerEmail').clearValidators();
      this.legalRepresentationForm.reset();
    }
    
    // Update validity
    Object.keys(this.legalRepresentationForm.controls).forEach(key => {
      this.legalRepresentationForm.get(key).updateValueAndValidity();
    });
  }

  /**
   * Handles interpreter required checkbox change
   */
  public onChangeInterpreterRequired(checked: boolean) {
    const value = checked ? DisputeInterpreterRequired.Y : DisputeInterpreterRequired.N;
    this.disputeInfoForm.patchValue({ interpreterRequired: value });
    
    const langControl = this.disputeInfoForm.get('interpreterLanguageCd');
    if (value === DisputeInterpreterRequired.Y) {
      langControl.setValidators([Validators.required]);
    } else {
      langControl.clearValidators();
      langControl.setValue(null);
    }
    langControl.updateValueAndValidity();
  }

  /**
   * Check if country is Canada
   */
  public get isCA(): boolean {
    return this.contactInfoForm.get('addressCountryId').value === this.canada.ctryId;
  }

  /**
   * Check if country is USA
   */
  public get isUSA(): boolean {
    return this.contactInfoForm.get('addressCountryId').value === this.usa.ctryId;
  }

  /**
   * Update validators for consolidated reason fields based on selected counts
   */
  private updateReasonValidators() {
    const reductionReasonControl = this.disputeInfoForm.get('fineReductionReason');
    const timeToPayReasonControl = this.disputeInfoForm.get('timeToPayReason');
    
    const countsRequestingReduction = this.getCountsRequestingReduction();
    const countsRequestingTimeToPay = this.getCountsRequestingTimeToPay();
    
    // Update reduction reason validators
    if (countsRequestingReduction.length > 0) {
      reductionReasonControl.setValidators([Validators.required, Validators.maxLength(500)]);
    } else {
      reductionReasonControl.clearValidators();
      reductionReasonControl.setValue(null);
      reductionReasonControl.markAsPristine();
      reductionReasonControl.markAsUntouched();
    }
    reductionReasonControl.updateValueAndValidity({ emitEvent: false });
    
    // Update time to pay reason validators
    if (countsRequestingTimeToPay.length > 0) {
      timeToPayReasonControl.setValidators([Validators.required, Validators.maxLength(500)]);
    } else {
      timeToPayReasonControl.clearValidators();
      timeToPayReasonControl.setValue(null);
      timeToPayReasonControl.markAsPristine();
      timeToPayReasonControl.markAsUntouched();
    }
    timeToPayReasonControl.updateValueAndValidity({ emitEvent: false });
  }

  /**
   * Get counts that are requesting fine reduction
   */
  public getCountsRequestingReduction(): ViolationTicketCount[] {
    return this.ticketCounts.filter(count => {
      const countFormGroup = this.disputeInfoForm.get('count' + count.countNo);
      return countFormGroup && 
             countFormGroup.get('requestReduction').value === DisputeCountRequestReduction.Y &&
             countFormGroup.get('__skip').value !== true;
    });
  }

  /**
   * Get counts that are requesting time to pay
   */
  public getCountsRequestingTimeToPay(): ViolationTicketCount[] {
    return this.ticketCounts.filter(count => {
      const countFormGroup = this.disputeInfoForm.get('count' + count.countNo);
      return countFormGroup && 
             countFormGroup.get('requestTimeToPay').value === DisputeCountRequestTimeToPay.Y &&
             countFormGroup.get('__skip').value !== true;
    });
  }

  /**
   * Get label for fine reduction with count numbers
   */
  public getReductionLabel(): string {
    const counts = this.getCountsRequestingReduction();
    if (counts.length === 0) return 'Reason For Fine Reduction';
    
    const countNumbers = counts.map(count => `Count ${count.countNo}`).join(', ');
    return `Reason For Fine Reduction (${countNumbers})`;
  }

  /**
   * Get label for time to pay with count numbers
   */
  public getTimeToPayLabel(): string {
    const counts = this.getCountsRequestingTimeToPay();
    if (counts.length === 0) return 'Reason For Time To Pay';
    
    const countNumbers = counts.map(count => `Count ${count.countNo}`).join(', ');
    return `Reason For Time To Pay (${countNumbers})`;
  }
}

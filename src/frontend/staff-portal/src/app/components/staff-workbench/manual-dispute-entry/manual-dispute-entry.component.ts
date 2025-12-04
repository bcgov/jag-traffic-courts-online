import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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
  ViolationTicket,
  ViolationTicketCount,
  DisputeCount,
  DisputeCountRequestCourtAppearance,
  DisputeCountRequestReduction,
  DisputeCountRequestTimeToPay,
  RoadSafetyTicketSearchService
} from 'app/api';
import { ToastService } from '@core/services/toast.service';
import { MatDialog } from '@angular/material/dialog';
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
  public bc: any;
  public canada: any;
  public usa: any;
  public todayDate: Date = new Date();
  public maxFilingDate: Date = new Date();
  public filteredCount1Statutes: Statute[];
  public filteredCount2Statutes: Statute[];
  public filteredCount3Statutes: Statute[];
  
  public ContactType = DisputeContactTypeCd;
  public RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public InterpreterRequired = DisputeInterpreterRequired;
  public SignatoryType = DisputeSignatoryType;

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

    if (this.config.provincesAndStates) {
      this.provinces = this.config.provincesAndStates.filter(
        x => x.ctryId === this.canada.ctryId && x.provSeqNo !== this.bc.provSeqNo
      );
      this.states = this.config.provincesAndStates.filter(
        x => x.ctryId === this.usa.ctryId
      );
    }
  }

  public ngOnInit() {
    this.initializeTicketDetailsForm();
    this.initializeContactInfoForm();
    this.initializeDisputeInfoForm();
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
      contactSurnameNm: [null],
      contactGivenNames: [null],
      contactLawFirmNm: [null],
      address: [null, [Validators.required, Validators.maxLength(300)]],
      addressCity: [null, [Validators.required, Validators.maxLength(30)]],
      addressProvince: [null],
      addressProvinceSeqNo: [null],
      addressProvinceCountryId: [null],
      addressCountryId: [this.canada.ctryId, [Validators.required]],
      postalCode: [null],
      emailAddress: [null, [Validators.email, Validators.maxLength(100)]],
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
  }

  private initializeDisputeInfoForm() {
    this.disputeInfoForm = this.formBuilder.group({
      filingDate: [new Date(), [Validators.required]],
      representedByLawyer: [DisputeRepresentedByLawyer.N],
      interpreterRequired: [DisputeInterpreterRequired.N],
      interpreterLanguageCd: [null],
      witnessNo: [0],
      signatoryType: [DisputeSignatoryType.D],
      signatoryName: [null]
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
      requestCourtAppearance: [DisputeCountRequestCourtAppearance.N],
      requestReduction: [DisputeCountRequestReduction.N],
      requestTimeToPay: [DisputeCountRequestTimeToPay.N],
      reductionReason: [null],
      timeToPayReason: [null]
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

      // BC requires 8-digit numeric DL
      if (selectedProv.provSeqNo === this.bc.provSeqNo && selectedProv.ctryId === this.canada.ctryId) {
        dlNumberControl.setValidators([Validators.required, Validators.pattern(/^\d{8}$/)]);
      } else {
        dlNumberControl.setValidators([Validators.maxLength(25)]);
      }
      dlNumberControl.updateValueAndValidity();
    }
  }

  public onSelectContactType(newContactType: DisputeContactTypeCd) {
    const surnameControl = this.contactInfoForm.get('contactSurnameNm');
    const givenNamesControl = this.contactInfoForm.get('contactGivenNames');
    const lawFirmControl = this.contactInfoForm.get('contactLawFirmNm');

    // Reset validators
    surnameControl.clearValidators();
    givenNamesControl.clearValidators();
    lawFirmControl.clearValidators();

    if (newContactType === DisputeContactTypeCd.Lawyer) {
      lawFirmControl.setValidators([Validators.required, Validators.maxLength(200)]);
      surnameControl.setValidators([Validators.required, Validators.maxLength(30)]);
      givenNamesControl.setValidators([Validators.required, Validators.maxLength(92)]);
    } else if (newContactType === DisputeContactTypeCd.Individual) {
      surnameControl.setValidators([Validators.required, Validators.maxLength(30)]);
      givenNamesControl.setValidators([Validators.required, Validators.maxLength(92)]);
    }

    surnameControl.updateValueAndValidity();
    givenNamesControl.updateValueAndValidity();
    lawFirmControl.updateValueAndValidity();
  }

  public onCountryChange(ctryId: number) {
    setTimeout(() => {
      const postalCodeControl = this.contactInfoForm.get('postalCode');
      const provinceControl = this.contactInfoForm.get('addressProvince');
      const provinceSeqNoControl = this.contactInfoForm.get('addressProvinceSeqNo');
      
      provinceControl.setValue(null);
      provinceSeqNoControl.setValue(null);
      this.contactInfoForm.patchValue({
        addressProvinceCountryId: ctryId
      });

      if (ctryId === this.canada.ctryId) {
        postalCodeControl.setValidators([Validators.required, Validators.maxLength(6), Validators.pattern(/^[A-Za-z]\d[A-Za-z]\d[A-Za-z]\d$/)]);
      } else if (ctryId === this.usa.ctryId) {
        postalCodeControl.setValidators([Validators.required, Validators.maxLength(10), Validators.pattern(/^\d{5}(-\d{4})?$/)]);
      } else {
        postalCodeControl.setValidators([Validators.maxLength(10)]);
      }
      postalCodeControl.updateValueAndValidity();
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
      statute.code?.toLowerCase().includes(filterValue) ||
      statute.shortDescriptionText?.toLowerCase().includes(filterValue)
    );
  }

  /**
   * Updates Act/Regulation and related fields when description changes
   * Called on input event from the description field
   */
  public onDescriptionChange(countNo: number, descriptionValue: string) {
    if (!descriptionValue || descriptionValue.trim() === '') {
      this.clearCountStatuteFields(countNo);
      return;
    }

    // Filter statutes based on the description
    const filteredStatutes = this.filterStatutes(descriptionValue);
    
    // Find exact match or best match
    const matchedStatute = this.findMatchingStatute(filteredStatutes, descriptionValue);
    
    if (matchedStatute) {
      this.updateCountWithStatute(countNo, matchedStatute);
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
      description: statute.shortDescriptionText
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
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  public nextStep() {
    if (this.validateCurrentStep()) {
      this.steps[this.currentStep].completed = true;
    }
    if (this.currentStep < this.steps.length - 1) {
        this.currentStep++;
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  public previousStep() {
    if (this.currentStep > 0) {
      this.currentStep--;
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
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

  public onFindTicket() {
    this.roadSafetyTicketSearchService.apiRoadsafetyticketsearchGet('EB02000254', '09:09').subscribe({
      next: (result) => {
        this.logger.log('ManualDisputeEntryComponent::onFindTicket - Ticket found:', result);
        console.log('Ticket Search Result:', result);
      },
      error: (error) => {
        this.logger.error('ManualDisputeEntryComponent::onFindTicket - Error:', error);
        console.error('Ticket Search Error:', error);
        this.toastService.openErrorToast('Failed to find ticket. Please try again.');
      }
    });
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
      actionType: "green",
      cancelTextKey: "Cancel",
      icon: "info",
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
    // TODO: Implement API call once backend endpoint is ready
    const dispute: Dispute = this.buildDisputeObject();
    
    this.logger.log('ManualDisputeEntryComponent::submitDispute', dispute);
    this.toastService.openSuccessToast('Dispute data logged to console (API call pending backend implementation)');
    
    // Uncomment when API is ready:
    // this.disputeService.createDispute(dispute).subscribe({
    //   next: (response: Dispute) => {
    //     this.logger.info('ManualDisputeEntryComponent::submitDispute response', response);
    //     this.toastService.openSuccessToast('Dispute created successfully');
    //     this.backInbox.emit();
    //   },
    //   error: (error: any) => {
    //     this.logger.error('ManualDisputeEntryComponent::submitDispute error', error);
    //     this.toastService.openErrorToast('Failed to create dispute. Please try again.');
    //   }
    // });
  }

  private buildDisputeObject(): Dispute {
    const ticketData = this.ticketDetailsForm.value;
    const contactData = this.contactInfoForm.value;
    const disputeData = this.disputeInfoForm.value;

    // Build violation ticket
    const violationTicket: ViolationTicket = {
      ticketNumber: ticketData.ticketNumber,
      disputantSurname: ticketData.disputantSurname,
      disputantGivenNames: ticketData.disputantGivenNames,
      disputantDriversLicenceNumber: ticketData.driversLicenceNumber,
      driversLicenceProvince: this.getProvinceCode(ticketData.driversLicenceProvince),
      driversLicenceCountry: ticketData.driversLicenceCountry,
      issuedTs: this.formatDateTime(ticketData.violationDate, ticketData.violationTime),
      courtLocation: ticketData.courtLocation
    };

    // Build dispute counts
    const disputeCounts = this.buildDisputeCounts();

    // Split given names if needed
    const givenNames = ticketData.disputantGivenNames ? ticketData.disputantGivenNames.split(' ') : [];

    const dispute: Dispute = {
      // Ticket information
      ticketNumber: ticketData.ticketNumber,
      issuedTs: violationTicket.issuedTs,
      submittedTs: new Date().toISOString(),
      filingDate: disputeData.filingDate ? disputeData.filingDate.toISOString() : new Date().toISOString(),
      
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
      address: contactData.address,
      addressCity: contactData.addressCity,
      addressProvince: this.getProvinceCode(contactData.addressProvince),
      addressProvinceSeqNo: contactData.addressProvinceSeqNo,
      addressProvinceCountryId: contactData.addressProvinceCountryId,
      addressCountryId: contactData.addressCountryId,
      postalCode: contactData.postalCode,
      emailAddress: contactData.emailAddress,
      emailAddressVerified: false,
      homePhoneNumber: contactData.homePhoneNumber,
      
      // Dispute information
      representedByLawyer: disputeData.representedByLawyer,
      interpreterRequired: disputeData.interpreterRequired,
      interpreterLanguageCd: disputeData.interpreterLanguageCd,
      witnessNo: disputeData.witnessNo || 0,
      signatoryType: disputeData.signatoryType,
      signatoryName: disputeData.signatoryName,
      
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
      return {
        countNo: count.countNo,
        requestCourtAppearance: countFormData.requestCourtAppearance,
        requestReduction: countFormData.requestReduction,
        requestTimeToPay: countFormData.requestTimeToPay,
        pleaCode: null
      };
    });
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
}

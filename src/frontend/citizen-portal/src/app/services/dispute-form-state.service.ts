import { Injectable } from '@angular/core';
import {
  AbstractControl,
  FormBuilder, FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { LoggerService } from '@core/services/logger.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { FormGroupValidators } from '@core/validators/form-group.validators';
import { Additional } from '@shared/models/additional.model';
import { Disputant } from '@shared/models/disputant.model';
import { Offence } from '@shared/models/offence.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AbstractFormStateService } from 'app/components/classes/abstract-form-state-service.class';

@Injectable({
  providedIn: 'root',
})
export class DisputeFormStateService extends AbstractFormStateService<TicketDispute> {
  public stepDisputantForm: FormGroup;
  public stepOffence1Form: FormGroup;
  public stepOffence2Form: FormGroup;
  public stepOffence3Form: FormGroup;
  public stepAdditionalForm: FormGroup;
  public stepOverviewForm: FormGroup;

  constructor(
    protected formBuilder: FormBuilder,
    protected logger: LoggerService
  ) {
    super(formBuilder, logger);
    this.initialize();
  }

  /**
   * @description
   * Convert JSON into reactive form abstract controls, which can
   * only be set more than once when explicitly forced.
   *
   * NOTE: Executed by views to populate their form models, which
   * allows for it to be used for setting required values that
   * can't be loaded during instantiation.
   */
  public async setForm(
    ticket: TicketDispute,
    forcePatch: boolean = false
  ): Promise<void> {
    super.setForm(ticket, forcePatch);
  }

  /**
   * @description
   * Convert reactive form abstract controls into JSON.
   */
  public get json(): TicketDispute {
    const stepDisputant = this.stepDisputantForm.getRawValue();
    const stepOffence1 = this.stepOffence1Form.getRawValue();
    const stepOffence2 = this.stepOffence2Form.getRawValue();
    const stepOffence3 = this.stepOffence3Form.getRawValue();
    const stepAdditional = this.stepAdditionalForm.getRawValue();
    const stepOverview = this.stepOverviewForm.getRawValue();

    const ticket = {
      ...stepOverview,
    };
    ticket.disputant = stepDisputant;
    ticket.offences = [];
    ticket.offences.push(stepOffence1);
    ticket.offences.push(stepOffence2);
    ticket.offences.push(stepOffence3);
    ticket.additional = stepAdditional;
    return ticket;
  }

  public get jsonTicketDispute(): TicketDispute {
    const stepDisputant = this.stepDisputantForm.getRawValue();
    const stepOffence1 = this.stepOffence1Form.getRawValue();
    const stepOffence2 = this.stepOffence2Form.getRawValue();
    const stepOffence3 = this.stepOffence3Form.getRawValue();
    const stepAdditional = this.stepAdditionalForm.getRawValue();
    const stepOverview = this.stepOverviewForm.getRawValue();

    const dispute: TicketDispute = {
      ...stepOverview,
    };
    dispute.disputant = stepDisputant;
    dispute.offences = [];

    if (stepOffence1.offenceNumber) {
      const offence1: Offence = { ...stepOffence1 };
      dispute.offences.push(offence1);
    }
    if (stepOffence2.offenceNumber) {
      const offence2: Offence = { ...stepOffence2 };
      dispute.offences.push(offence2);
    }
    if (stepOffence3.offenceNumber) {
      const offence3: Offence = { ...stepOffence3 };
      dispute.offences.push(offence3);
    }

    dispute.additional = stepAdditional;
    return dispute;
  }

  public get disputant(): Disputant {
    return this.stepDisputantForm.getRawValue();
  }

  public get additional(): Additional {
    return this.stepAdditionalForm.getRawValue();
  }

  public get offences(): Offence[] {
    const stepOffence1 = this.stepOffence1Form.getRawValue();
    const stepOffence2 = this.stepOffence2Form.getRawValue();
    const stepOffence3 = this.stepOffence3Form.getRawValue();

    return [stepOffence1, stepOffence2, stepOffence3];
  }

  /**
   * @description
   * Helper for getting a list of dispute forms.
   */
  public get applicableForms(): AbstractControl[] {
    const forms = [this.stepDisputantForm];
    forms.push(this.stepOffence1Form);

    const stepOffence2 = this.stepOffence2Form.getRawValue();
    if (stepOffence2.offenceNumber) {
      forms.push(this.stepOffence2Form);
    }

    const stepOffence3 = this.stepOffence3Form.getRawValue();
    if (stepOffence3.offenceNumber) {
      forms.push(this.stepOffence3Form);
    }

    forms.push(this.stepAdditionalForm);
    forms.push(this.stepOverviewForm);

    return forms;
  }

  /**
   * @description
   * Helper for getting a list of dispute forms.
   */
  public get forms(): AbstractControl[] {
    return [
      this.stepDisputantForm,
      this.stepOffence1Form,
      this.stepOffence2Form,
      this.stepOffence3Form,
      this.stepAdditionalForm,
      this.stepOverviewForm,
    ];
  }

  /**
   * @description
   * Helper for getting a list of offence forms.
   */
  public get offenceForms(): AbstractControl[] {
    return [
      this.stepOffence1Form,
      this.stepOffence2Form,
      this.stepOffence3Form,
    ];
  }

  /**
   * @description
   * Check that all constituent forms are valid.
   */
  public get isValid(): boolean {
    return super.isValid;
  }

  /**
   * @description
   * Check that at least one constituent form is dirty.
   */
  public get isDirty(): boolean {
    return this.forms.reduce(
      (dirty: boolean, form: AbstractControl) => dirty || form.dirty,
      false
    );
  }

  /**
   * @description
   * Mark all constituent forms as pristine.
   */
  public markAsPristine(): void {
    this.forms.forEach((form: AbstractControl) => form.markAsPristine());
  }

  /**
   * @description
   * Initialize and configure the forms for patching, which is also used
   * to clear previous form data from the service.
   */
  protected buildForms() {
    this.stepDisputantForm = this.buildStepDisputantForm();
    this.stepOffence1Form = this.buildStepOffenceForm();
    this.stepOffence2Form = this.buildStepOffenceForm();
    this.stepOffence3Form = this.buildStepOffenceForm();
    this.stepAdditionalForm = this.buildStepAdditionalForm();
    this.stepOverviewForm = this.buildStepOverviewForm();
  }

  /**
   * @description
   * Manage the conversion of JSON to reactive forms.
   */
  protected patchForm(ticket: TicketDispute) {
    if (!ticket) {
      return;
    }

    this.stepDisputantForm.patchValue(ticket);
    this.stepOffence1Form.patchValue(ticket);
    this.stepOffence2Form.patchValue(ticket);
    this.stepOffence3Form.patchValue(ticket);
    this.stepAdditionalForm.patchValue(ticket);
    this.stepOverviewForm.patchValue(ticket);

    // After patching the form is dirty, and needs to be pristine
    // to allow for deactivation modals to work properly
    this.markAsPristine();
  }

  /**
   * Form Builders and Helpers
   */

  public buildStepDisputantForm(): FormGroup {
    return this.formBuilder.group({
      lastName: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      mailingAddress: [null, [Validators.required]],
      postalCode: [null],
      city: [null],
      province: [null],
      driverLicenseNumber: [null],
      driverLicenseProvince: [null],
      emailAddress: [null, [Validators.required, Validators.email]],
      phoneNumber: [null, [FormControlValidators.phone]],
      birthdate: [null, []],
    });
  }

  public buildStepOffenceForm(): FormGroup {
    return this.formBuilder.group({
      offenceNumber: [null],
      offenceAgreementStatus: [null, [Validators.required]],
      reductionAppearInCourt: [false],

      // Here for display purposes
      offenceDescription: [null],
      status: [null],
      ticketedAmount: [null],
      amountDue: [null],
      discountDueDate: [null],
      discountAmount: [null],

      _applyToAllCounts: [false],
      _allowApplyToAllCounts: [false],
      _firstOffence: [false],
      _offenceStatus: [null],
      _offenceStatusDesc: [null],
      _within30days: [null],
      _amountDue: [null],
    },
      {
        validators: [
          FormGroupValidators.requiredIfValue(
            'offenceAgreementStatus',
            'REDUCTION',
            'reductionAppearInCourt'
          )
        ],
      });
  }

  public buildStepAdditionalForm(): FormGroup {
    return this.formBuilder.group({
      lawyerPresent: [false],
      interpreterRequired: [false],
      interpreterLanguage: [null],
      witnessPresent: [false],
      numberOfWitnesses: [null],
      requestReduction: [false],
      requestMoreTime: [false],
      reductionReason: [null],
      moreTimeReason: [null],
      _isCourtRequired: [false],
      _isReductionRequired: [false],
      _isReductionNotInCourt: [false]
    },
      {
        validators: [
          FormGroupValidators.requiredIfTrue(
            'interpreterRequired',
            'interpreterLanguage'
          ),
          FormGroupValidators.requiredIfTrue(
            'witnessPresent',
            'numberOfWitnesses'
          ),
          FormGroupValidators.requiredIfFlags(
            '_isReductionNotInCourt',
            'requestReduction',
            'reductionReason'
          ),
          FormGroupValidators.requiredIfFlags(
            '_isReductionNotInCourt',
            'requestMoreTime',
            'moreTimeReason'
          ),
          FormGroupValidators.atLeastOneCheckedIf(
            '_isReductionRequired',
            'requestReduction',
            'requestMoreTime'
          ),
        ],
      });
  }

  public resetStepAdditionalForm(): void {
    this.stepAdditionalForm.reset();
  }

  public buildStepOverviewForm(): FormGroup {
    return this.formBuilder.group({
      // Here for display purposes
      violationTicketNumber: [null],
      violationTime: [null],
      violationDate: [null],
      discountDueDate: [null],
      discountAmount: [null],
      _outstandingBalanceDue: [null],
      _totalBalanceDue: [null],
      _requestSubmitted: [null],
      _within30days: [null],
    });
  }
}

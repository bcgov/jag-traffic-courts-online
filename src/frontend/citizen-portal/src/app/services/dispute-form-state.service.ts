import { Injectable } from '@angular/core';
import {
  FormBuilder,
  Validators,
  FormGroup,
  AbstractControl,
  FormControl,
} from '@angular/forms';
import { LoggerService } from '@core/services/logger.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { AbstractFormStateService } from 'app/components/classes/abstract-form-state-service.class';
import { Additional } from '@shared/models/additional.model';
import { CountDispute } from '@shared/models/countDispute.model';
import { Disputant } from '@shared/models/disputant.model';
import { Offence } from '@shared/models/offence.model';
import { Ticket } from '@shared/models/ticket.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';

@Injectable({
  providedIn: 'root',
})
export class DisputeFormStateService extends AbstractFormStateService<Ticket> {
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
    ticket: Ticket,
    forcePatch: boolean = false
  ): Promise<void> {
    super.setForm(ticket, forcePatch);
  }

  /**
   * @description
   * Convert reactive form abstract controls into JSON.
   */
  public get json(): Ticket {
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
    dispute.offences.push(stepOffence1);
    dispute.offences.push(stepOffence2);
    dispute.offences.push(stepOffence3);
    dispute.additional = stepAdditional;
    return dispute;
  }

  public get jsonCountDispute(): CountDispute {
    const stepOffence1 = this.stepOffence1Form.getRawValue();
    const stepOffence2 = this.stepOffence2Form.getRawValue();
    const stepOffence3 = this.stepOffence3Form.getRawValue();
    const stepAdditional = this.stepAdditionalForm.getRawValue();
    const stepOverview = this.stepOverviewForm.getRawValue();

    const dispute: CountDispute = {
      ...stepOverview,
    };
    if (stepOffence1.offenceNumber) {
      console.log('aaa');
      dispute.offence = stepOffence1;
    }
    if (stepOffence2.offenceNumber) {
      console.log('bbb');
      dispute.offence = stepOffence2;
    }
    if (stepOffence3.offenceNumber) {
      console.log('ccc');
      dispute.offence = stepOffence3;
    }
    dispute.additional = stepAdditional;
    console.log('jsonCountDispute1', stepOffence1);
    console.log('jsonCountDispute2', stepOffence2);
    console.log('jsonCountDispute3', stepOffence3);
    console.log('jsonCountDispute', dispute);
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
   * Check if any offence forms require court.
   */
  public get isCourtRequired(): boolean {
    // console.log('this.offenceForms', this.offenceForms.length);

    // this.offenceForms.forEach((form: AbstractControl) => {
    //   const offenceNumber = form.get('offenceNumber') as FormControl;
    //   const status = form.get('offenceAgreementStatus') as FormControl;
    //   const include = form.get('includeOffenceInDispute') as FormControl;

    //   console.log(
    //     'XXXXXXXXXX',
    //     offenceNumber.value,
    //     include.value,
    //     status.value
    //   );
    // });

    // const courtRequiredForm = this.offenceForms.find(
    //   (form: AbstractControl) =>
    //     (form.get('offenceNumber') as FormControl).value &&
    //     (form.get('includeOffenceInDispute') as FormControl).value &&
    //     (form.get('offenceAgreementStatus') as FormControl).value &&
    //     (form.get('offenceAgreementStatus') as FormControl).value != '1'
    // );

    // return courtRequiredForm ? true : false;
    return true;
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
  protected patchForm(ticket: Ticket) {
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
      surname: [null], //, [Validators.required]],
      givenNames: [null],
      mailingAddress: [null],
      postal: [null],
      city: [null],
      province: [null],
      license: [null],
      provLicense: [null],
      emailAddress: [null], //, [Validators.required, Validators.email]
      homePhone: [null], //, [FormControlValidators.phone]],
      workPhone: [null], //, [FormControlValidators.phone]],
      birthdate: [null], //, []],
    });
  }

  public buildStepOffenceForm(): FormGroup {
    return this.formBuilder.group({
      offenceNumber: [null],
      includeOffenceInDispute: [false],
      offenceAgreementStatus: [null], //, [Validators.required]],
      requestReduction: [null],
      requestMoreTime: [null],
      reductionReason: [null],
      moreTimeReason: [null],

      // Here for display purposes
      offenceDescription: [null],
      ticketedAmount: [null],
      amountDue: [null],
      discountAmount: [null],
      discountDueDate: [null],
    });
  }

  public buildStepAdditionalForm(): FormGroup {
    return this.formBuilder.group({
      lawyerPresent: [null],
      interpreterRequired: [null],
      interpreterLanguage: [null],
      witnessPresent: [null],
    });
  }

  public resetStepAdditionalForm(): void {
    this.stepAdditionalForm.reset();
  }

  public buildStepOverviewForm(): FormGroup {
    return this.formBuilder.group({
      informationCertified: [null, [FormControlValidators.requiredTruthful]],
    });
  }
}

import { Injectable } from '@angular/core';
import {
  FormBuilder,
  Validators,
  FormGroup,
  AbstractControl,
} from '@angular/forms';
import { LoggerService } from '@core/services/logger.service';
import { AbstractFormStateService } from '@dispute/classes/abstract-form-state-service.class';
import { Dispute } from '@shared/models/dispute.model';

@Injectable({
  providedIn: 'root',
})
export class DisputeFormStateService extends AbstractFormStateService<Dispute> {
  public stepReviewForm: FormGroup;
  public stepOffenceForm: FormGroup;
  public stepCourtForm: FormGroup;
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
    dispute: Dispute,
    forcePatch: boolean = false
  ): Promise<void> {
    super.setForm(dispute, forcePatch);
  }

  /**
   * @description
   * Convert reactive form abstract controls into JSON.
   */
  public get json(): Dispute {
    const stepReview = this.stepReviewForm.getRawValue();
    const stepOffence = this.stepOffenceForm.getRawValue();
    const stepCourt = this.stepCourtForm.getRawValue();
    const stepOverview = this.stepOverviewForm.getRawValue();

    return {
      ...stepReview,
      ...stepOffence,
      ...stepCourt,
      ...stepOverview,
    };
  }

  /**
   * @description
   * Helper for getting a list of dispute forms.
   */
  public get forms(): AbstractControl[] {
    return [
      this.stepReviewForm,
      this.stepOffenceForm,
      this.stepCourtForm,
      this.stepOverviewForm,
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
    this.stepReviewForm = this.buildStepReviewForm();
    this.stepOffenceForm = this.buildStepOffenceForm();
    this.stepCourtForm = this.buildStepCourtForm();
    this.stepOverviewForm = this.buildStepOverviewForm();
  }

  /**
   * @description
   * Manage the conversion of JSON to reactive forms.
   */
  protected patchForm(dispute: Dispute) {
    if (!dispute) {
      return;
    }

    this.stepReviewForm.patchValue(dispute);
    this.stepOffenceForm.patchValue(dispute);
    this.stepCourtForm.patchValue(dispute);
    this.stepOverviewForm.patchValue(dispute);

    // After patching the form is dirty, and needs to be pristine
    // to allow for deactivation modals to work properly
    this.markAsPristine();
  }

  /**
   * Form Builders and Helpers
   */

  public buildStepReviewForm(): FormGroup {
    return this.formBuilder.group({
      violationTicketNumber: [null],
      offenceNumber: [null],
      emailAddress: [null, [Validators.required, Validators.email]],
    });
  }

  public buildStepOffenceForm(): FormGroup {
    return this.formBuilder.group({
      offenceAgreementStatus: [null, [Validators.required]],
      requestReduction: [null],
      requestTime: [null],
      reductionReason: [null],
      timeReason: [null],
    });
  }

  public buildStepCourtForm(): FormGroup {
    return this.formBuilder.group({
      lawyerPresent: [false],
      interpreterRequired: [false],
      interpreterLanguage: [null],
      witnessPresent: [false],
    });
  }

  public resetStepCourtForm(): void {
    this.stepCourtForm.reset();
  }

  public buildStepOverviewForm(): FormGroup {
    return this.formBuilder.group({
      informationCertified: [false, [Validators.required]],
    });
  }
}

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
  public stepCount1Form: FormGroup;
  public stepCount2Form: FormGroup;
  public stepCount3Form: FormGroup;
  public stepCourtForm: FormGroup;
  public stepOverviewForm: FormGroup;

  constructor(protected fb: FormBuilder, protected logger: LoggerService) {
    super(fb, logger);
    this.initialize();
  }

  public getStepCountForm(countIndex: number): FormGroup {
    if (countIndex === 0) return this.stepCount1Form;
    else if (countIndex === 1) return this.stepCount2Form;
    else if (countIndex === 2) return this.stepCount3Form;
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
    // Store required dispute identifiers not captured in forms

    super.setForm(dispute, forcePatch);
  }

  /**
   * @description
   * Convert reactive form abstract controls into JSON.
   */
  public get json(): Dispute {
    const stepReview = this.stepReviewForm.getRawValue();
    const stepCount1 = this.stepCount1Form.getRawValue();
    const stepCount2 = this.stepCount2Form.getRawValue();
    const stepCount3 = this.stepCount3Form.getRawValue();
    const stepCourt = this.stepCourtForm.getRawValue();
    const stepOverview = this.stepOverviewForm.getRawValue();

    return {
      ...stepReview,
      stepCount1: { ...stepCount1 },
      stepCount2: { ...stepCount2 },
      stepCount3: { ...stepCount3 },
      ...stepCourt,
      ...stepOverview,
    };
  }

  /**
   * @description
   * Helper for getting a list of enrolment forms.
   */
  public get forms(): AbstractControl[] {
    return [
      this.stepReviewForm,
      this.stepCount1Form,
      this.stepCount2Form,
      this.stepCount3Form,
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
   * Initialize and configure the forms for patching, which is also used
   * to clear previous form data from the service.
   */
  protected buildForms() {
    this.stepReviewForm = this.buildStepReviewForm();
    this.stepCount1Form = this.buildStepCountForm();
    this.stepCount2Form = this.buildStepCountForm();
    this.stepCount3Form = this.buildStepCountForm();
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

    // After patching the form is dirty, and needs to be pristine
    // to allow for deactivation modals to work properly
    this.markAsPristine();
  }

  /**
   * Form Builders and Helpers
   */

  private buildStepReviewForm(): FormGroup {
    return this.fb.group({
      id: [null],
      infoCorrect: [null],
    });
  }

  private buildStepCountForm(): FormGroup {
    return this.fb.group({
      id: [null],
      statuteId: [null],
      count: [null],
      count1A: [null],
      count1A1: [null],
      count1A2: [null],
      surname: [null],
      reductionReason: [null],
      timeReason: [null],
      count1B1: [null],
      count1B2: [null],
    });
  }

  private buildStepCourtForm(): FormGroup {
    return this.fb.group({
      id: [null],
      lawyerPresent: [false],
      interpreterRequired: [false],
      interpreterLanguage: [null],
      callWitness: [false],
    });
  }

  private buildStepOverviewForm(): FormGroup {
    return this.fb.group({
      id: [null],
      certifyCorrect: [false],
    });
  }
}

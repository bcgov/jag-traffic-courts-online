import { Injectable } from '@angular/core';
import {
  FormBuilder,
  Validators,
  FormGroup,
  AbstractControl,
} from '@angular/forms';
import { LoggerService } from '@core/services/logger.service';
import { AbstractFormStateService } from '@dispute/classes/abstract-form-state-service.class';
import { Count, Dispute } from '@shared/models/dispute.model';

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

  public getStepCountForm(countIndex: number): FormGroup {
    if (countIndex === 1) {
      return this.stepCount2Form;
    } else if (countIndex === 2) {
      return this.stepCount3Form;
    }
    return this.stepCount1Form;
  }

  public getCountSummary(countNo: number): string {
    let countInfo;
    if (countNo === 2) {
      countInfo = this.stepCount2Form.getRawValue();
    } else if (countNo === 2) {
      countInfo = this.stepCount3Form.getRawValue();
    } else {
      countInfo = this.stepCount1Form.getRawValue();
    }

    if (!countInfo) {
      return null;
    }

    let summary: string;
    if (countInfo.count === 'A') {
      summary =
        'I agree that I committed this offence, and I do not want to appear in court.';

      if (countInfo.count1A1) {
        summary +=
          ' I request a reduction of the ticketed amount for the following reason:';
        summary += '<br/><small>';
        summary += countInfo.reductionReason;
        summary += '</small><br/>';
      }
      if (countInfo.count1A2) {
        summary +=
          ' I request time to pay the ticketed amount for the following reason:';
        summary += '<br/><small>';
        summary += countInfo.timeReason;
        summary += '</small><br/>';
      }
    } else if (countInfo.count === 'A') {
      summary =
        'I agree that I committed this offence, and I want to appear in court.';

      if (countInfo.count1B1) {
        summary +=
          'I would like to request a reduction of the ticketed amount.';
      }
      if (countInfo.count1B2) {
        summary += 'I would like to request time to pay the ticketed amount';
      }
    } else {
      summary = 'I do not agree that I committed this offence (allegation)';
    }

    return summary;
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
   * Helper for getting a list of dispute forms.
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

    this.stepReviewForm.patchValue(dispute);

    dispute.counts.forEach((c: Count) => {
      if (c.countNo === 1) {
        this.stepCount1Form.patchValue(c);
      }
      if (c.countNo === 2) {
        this.stepCount2Form.patchValue(c);
      }
      if (c.countNo === 3) {
        this.stepCount3Form.patchValue(c);
      }
    });

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
      id: [null],
      emailAddress: [null, [Validators.required, Validators.email]],
    });
  }

  public buildStepCountForm(): FormGroup {
    return this.formBuilder.group({
      id: [null],
      statuteId: [null],
      countNo: [null],
      description: [null],
      count: [null],
      count1A1: [null],
      count1A2: [null],
      reductionReason: [null],
      timeReason: [null],
      count1B1: [null],
      count1B2: [null],
    });
  }

  public buildStepCourtForm(): FormGroup {
    return this.formBuilder.group({
      id: [null],
      lawyerPresent: [false],
      interpreterRequired: [false],
      interpreterLanguage: [null],
      callWitness: [false],
    });
  }

  public buildStepOverviewForm(): FormGroup {
    return this.formBuilder.group({
      id: [null],
      certifyCorrect: [false],
    });
  }
}

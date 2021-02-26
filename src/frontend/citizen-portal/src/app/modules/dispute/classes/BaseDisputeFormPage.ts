import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { BaseDisputePage } from './BaseDisputePage';

export interface IBaseDisputeFormPage {
  formStepReview: FormGroup;
  formCounts: FormArray;
  formStepCourt: FormGroup;
  formStepOverview: FormGroup;
}

export abstract class BaseDisputeFormPage
  extends BaseDisputePage
  implements IBaseDisputeFormPage {
  public formStepReview: FormGroup;
  public formCounts: FormArray;
  public formStepCourt: FormGroup;
  public formStepOverview: FormGroup;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService
  ) {
    super(route, router);

    this.formStepReview = this.formBuilder.group({
      id: [null],
      userId: [null],
      violationTicketNumber: [null],
      courtLocation: [null],
      violationDate: [null],
      infoCorrect: [null],
    });

    this.formStepCourt = this.formBuilder.group({
      id: [null],
      surname: [null, [Validators.required]],
      lawyerPresent: [false],
      interpreterRequired: [false],
      interpreterLanguage: [null],
      callWitness: [false],
    });

    this.formStepOverview = this.formBuilder.group({
      id: [null],
      certifyCorrect: [false],
    });

    this.formCounts = this.formBuilder.array([]);
    const countGroup = this.formBuilder.group({
      id: [null],
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

    this.formCounts.push(countGroup);
    this.formCounts.push(countGroup);
    this.formCounts.push(countGroup);
  }
}

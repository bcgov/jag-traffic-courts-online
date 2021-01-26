import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { BaseDisputePage } from './BaseDisputePage';

export interface IBaseDisputeFormPage {
  formStep1: FormGroup;
  formStep2: FormGroup;
  formStep3: FormGroup;
  formStep4: FormGroup;
  formStep5: FormGroup;
}

export abstract class BaseDisputeFormPage
  extends BaseDisputePage
  implements IBaseDisputeFormPage {
  formStep1: FormGroup;
  formStep2: FormGroup;
  formStep3: FormGroup;
  formStep4: FormGroup;
  formStep5: FormGroup;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService
  ) {
    super(route, router);

    this.formStep1 = this.formBuilder.group({
      id: [null],
      userId: [null],
      violationTicketNumber: [null, [Validators.required]],
      courtLocation: [null],
      violationDate: [null],
    });

    this.formStep2 = this.formBuilder.group({
      id: [null],
      surname: [null, [Validators.required]],
      givenNames: [null],
      mailing: [null],
      postal: [null],
      city: [null],
      province: [null],
      license: [null],
      provLicense: [null],
      homePhone: [null, [FormControlValidators.phone]],
      workPhone: [null, [FormControlValidators.phone]],
      birthdate: [null, []],
    });

    this.formStep3 = this.formBuilder.group({
      id: [null],
      surname: [null, [Validators.required]],
    });

    this.formStep4 = this.formBuilder.group({
      id: [null],
      lawyerPresent: [false, [FormControlValidators.requiredBoolean]],
      interpreterRequired: [false, [FormControlValidators.requiredBoolean]],
      interpreterLanguage: [null],
      callWitness: [false, [FormControlValidators.requiredBoolean]],
    });

    this.formStep5 = this.formBuilder.group({
      id: [null],
      surname: [null, [Validators.required]],
    });

    // TODO Put in here for now.... move later
    this.getTicket();
  }

  private getTicket(): void {
    this.disputeResource.ticket().subscribe((response) => {
      const key = 'result';
      this.disputeService.ticket$.next(response[key]);
    });
  }
}

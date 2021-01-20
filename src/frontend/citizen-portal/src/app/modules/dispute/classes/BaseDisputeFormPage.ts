import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormControlValidators } from '@core/validators/form-control.validators';

export interface IBaseDisputeFormPage {
  form: FormGroup;
}

export abstract class BaseDisputeFormPage implements IBaseDisputeFormPage {
  form: FormGroup;

  constructor(protected formBuilder: FormBuilder) {
    this.form = this.formBuilder.group({
      id: [null],
      userId: [null],
      violationTicketNumber: [null, [Validators.required]],
      courtLocation: [null],
      violationDate: [null],
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
      lawyerPresent: [false, [FormControlValidators.requiredBoolean]],
      interpreterRequired: [false, [FormControlValidators.requiredBoolean]],
      interpreterLanguage: [null],
      callWitness: [false, [FormControlValidators.requiredBoolean]],
    });
  }
}

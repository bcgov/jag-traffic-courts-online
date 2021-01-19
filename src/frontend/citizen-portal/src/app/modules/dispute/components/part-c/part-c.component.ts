import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { FormGroupValidators } from '@core/validators/form-group.validators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-part-c',
  templateUrl: './part-c.component.html',
  styleUrls: ['./part-c.component.scss'],
})
export class PartCComponent implements OnInit {
  public form: FormGroup;
  public busy: Subscription;

  constructor(protected formBuilder: FormBuilder) {}

  public ngOnInit(): void {
    this.form = this.formBuilder.group(
      {
        lawyerPresent: [false, [FormControlValidators.requiredBoolean]],
        interpreterRequired: [false, [FormControlValidators.requiredBoolean]],
        interpreterLanguage: [null],
        callWitness: [false, [FormControlValidators.requiredBoolean]],
      },
      {
        validators: [
          FormGroupValidators.requiredIfTrue(
            'interpreterRequired',
            'interpreterLanguage'
          ),
        ],
      }
    );
  }

  public onSubmit(): void {
    console.log('onSubmit');
  }

  public onBack() {
    console.log('onBack');
  }

  public get interpreterRequired(): FormControl {
    return this.form.get('interpreterRequired') as FormControl;
  }
}

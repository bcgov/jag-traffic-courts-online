import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ViewportService } from '@core/services/viewport.service';
import { Subscription } from 'rxjs';
import moment from 'moment';
import { FormControlValidators } from '@core/validators/form-control.validators';

export const MINIMUM_AGE = 18;

@Component({
  selector: 'app-part-a',
  templateUrl: './part-a.component.html',
  styleUrls: ['./part-a.component.scss'],
})
export class PartAComponent implements OnInit {
  public form: FormGroup;
  public busy: Subscription;
  public maxDateOfBirth: moment.Moment;

  private MINIMUM_AGE = 18;

  constructor(
    private formBuilder: FormBuilder,
    private viewportService: ViewportService
  ) {
    this.maxDateOfBirth = moment().subtract(this.MINIMUM_AGE, 'years');
  }

  public ngOnInit(): void {
    this.form = this.formBuilder.group({
      surname: [null, [Validators.required]],
      given: [null],
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
  }

  public onSubmit(): void {}
  public onBack() {}

  public get isMobile(): boolean {
    return this.viewportService.isMobile;
  }

  public get homePhone(): FormControl {
    return this.form.get('homePhone') as FormControl;
  }

  public get workPhone(): FormControl {
    return this.form.get('workPhone') as FormControl;
  }

  public get birthdate(): FormControl {
    return this.form.get('birthdate') as FormControl;
  }
}

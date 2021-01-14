import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent implements OnInit {
  public form: FormGroup;
  public busy: Subscription;

  constructor(private formBuilder: FormBuilder) {}

  public ngOnInit(): void {
    this.form = this.formBuilder.group({
      surname: [null, [Validators.required]],
      given: [null, [Validators.required]],
      mailing: [null, [Validators.required]],
      postal: [null],
      doingBusinessAs: [''],
      homePhone: [null, [Validators.required, FormControlValidators.phone]],
      workPhone: [null, [FormControlValidators.phone]],
      birthdate: [null, []],
    });
  }

  public onSubmit(): void {
    this.nextRoute();
  }

  public onBack() {}

  public nextRoute() {}
}

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import moment from 'moment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  public form: FormGroup;
  public busy: Subscription;
  public maxViolationDate: moment.Moment;

  constructor(protected formBuilder: FormBuilder) {
    this.maxViolationDate = moment();
  }

  public ngOnInit(): void {
    this.createFormInstance();
  }

  protected createFormInstance() {
    this.form = this.formBuilder.group({
      violationTicketNumber: [null, [Validators.required]],
      courtLocation: [null],
      violationDate: [null],
    });
  }

  public onSubmit(): void {
    console.log('onSubmit');
  }
}

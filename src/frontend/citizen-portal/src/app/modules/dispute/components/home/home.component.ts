import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';

import moment from 'moment';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { Ticket } from '@shared/models/ticket.model';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent extends BaseDisputeFormPage implements OnInit {
  public form: FormGroup;
  public busy: Subscription;
  public maxViolationDate: moment.Moment;

  constructor(
    protected formBuilder: FormBuilder,
    private mockDisputeService: MockDisputeService
  ) {
    super(formBuilder);

    this.maxViolationDate = moment();
  }

  public ngOnInit(): void {
    this.createFormInstance();
  }

  protected createFormInstance() {
    this.mockDisputeService.ticket$.subscribe((ticket: Ticket) => {
      this.form.patchValue(ticket);
    });
  }

  public onSubmit(): void {}
}

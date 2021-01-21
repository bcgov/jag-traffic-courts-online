import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';

import moment from 'moment';
import { Ticket } from '@shared/models/ticket.model';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';

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
    private service: DisputeResourceService
  ) {
    super(formBuilder);

    this.maxViolationDate = moment();
  }

  public ngOnInit(): void {
    this.createFormInstance();
  }

  protected createFormInstance() {
    this.service.ticket$.subscribe((ticket: Ticket) => {
      this.form.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    console.log('onSubmit');
  }
}

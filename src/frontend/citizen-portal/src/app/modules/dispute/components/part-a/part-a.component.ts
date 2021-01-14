import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ViewportService } from '@core/services/viewport.service';
import { Subscription } from 'rxjs';
import moment from 'moment';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { Ticket } from '@shared/models/ticket.model';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';

export const MINIMUM_AGE = 18;

@Component({
  selector: 'app-part-a',
  templateUrl: './part-a.component.html',
  styleUrls: ['./part-a.component.scss'],
})
export class PartAComponent extends BaseDisputeFormPage implements OnInit {
  public form: FormGroup;
  public busy: Subscription;
  public maxDateOfBirth: moment.Moment;

  private MINIMUM_AGE = 18;

  constructor(
    protected formBuilder: FormBuilder,
    private viewportService: ViewportService,
    private mockDisputeService: MockDisputeService
  ) {
    super(formBuilder);

    this.maxDateOfBirth = moment().subtract(this.MINIMUM_AGE, 'years');
  }

  public ngOnInit(): void {
    this.mockDisputeService.ticket$.subscribe((ticket: Ticket) => {
      this.form.patchValue(ticket);
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

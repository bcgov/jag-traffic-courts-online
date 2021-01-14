import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewportService } from '@core/services/viewport.service';
import { Subscription } from 'rxjs';
import { Ticket } from '@shared/models/ticket.model';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';

@Component({
  selector: 'app-part-d',
  templateUrl: './part-d.component.html',
  styleUrls: ['./part-d.component.scss'],
})
export class PartDComponent extends BaseDisputeFormPage implements OnInit {
  public form: FormGroup;
  public busy: Subscription;

  constructor(
    protected formBuilder: FormBuilder,
    private viewportService: ViewportService,
    private mockDisputeService: MockDisputeService
  ) {
    super(formBuilder);
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
}

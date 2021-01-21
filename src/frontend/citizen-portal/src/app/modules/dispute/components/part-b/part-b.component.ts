import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewportService } from '@core/services/viewport.service';
import { Subscription } from 'rxjs';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { Ticket } from '@shared/models/ticket.model';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';

@Component({
  selector: 'app-part-b',
  templateUrl: './part-b.component.html',
  styleUrls: ['./part-b.component.scss'],
})
export class PartBComponent extends BaseDisputeFormPage implements OnInit {
  public form: FormGroup;
  public busy: Subscription;

  constructor(
    protected formBuilder: FormBuilder,
    private viewportService: ViewportService,
    private service: DisputeResourceService
  ) {
    super(formBuilder);
  }

  public ngOnInit(): void {
    this.service.ticket$.subscribe((ticket: Ticket) => {
      this.form.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    // do nothing for now
  }

  public onBack(): void {
    // do nothing for now
  }

  public onBack() {
    // do nothing for now
  }

  public get isMobile(): boolean {
    return this.viewportService.isMobile;
  }
}

import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ViewportService } from '@core/services/viewport.service';
import { RouteUtils } from '@core/utils/route-utils.class';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Ticket } from '@shared/models/ticket.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-part-b',
  templateUrl: './part-b.component.html',
  styleUrls: ['./part-b.component.scss'],
})
export class PartBComponent extends BaseDisputeFormPage implements OnInit {
  @Input() public stepper: MatStepper;

  public busy: Subscription;
  public nextBtnLabel: string;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    private viewportService: ViewportService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {
    super(route, router, formBuilder, disputeService, disputeResource);
    this.nextBtnLabel = 'Next';
  }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.formStep3.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.formStep3)) {
      this.disputeService.ticket$.next({
        ...this.disputeService.ticket,
        ...this.formStep3.value,
      });
      this.stepper.next();
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }

  public onBack() {
    this.stepper.previous();
  }

  public get isMobile(): boolean {
    return this.viewportService.isMobile;
  }
}

import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
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

import moment from 'moment';
import { Subscription } from 'rxjs';

export const MINIMUM_AGE = 18;

@Component({
  selector: 'app-part-a',
  templateUrl: './part-a.component.html',
  styleUrls: ['./part-a.component.scss'],
})
export class PartAComponent extends BaseDisputeFormPage implements OnInit {
  public busy: Subscription;
  public maxDateOfBirth: moment.Moment;
  public nextBtnLabel : string;
   @Input() public stepper: MatStepper;
  private MINIMUM_AGE = 18;

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
    this.nextBtnLabel = "Next";
    this.maxDateOfBirth = moment().subtract(this.MINIMUM_AGE, 'years');
  }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.formStep2.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.formStep2)) {
      this.disputeService.ticket$.next({
        ...this.disputeService.ticket,
        ...this.formStep2.value,
      });
      this.stepper.next();
      //this.routeNext(RouteUtils.currentRoutePath(this.router.url));
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }

  public onBack() {
    this.stepper.previous();
    //this.routeBack(RouteUtils.currentRoutePath(this.router.url));
  }

  public get isMobile(): boolean {
    return this.viewportService.isMobile;
  }

  public get homePhone(): FormControl {
    return this.formStep2.get('homePhone') as FormControl;
  }

  public get workPhone(): FormControl {
    return this.formStep2.get('workPhone') as FormControl;
  }

  public get birthdate(): FormControl {
    return this.formStep2.get('birthdate') as FormControl;
  }
}

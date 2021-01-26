import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { RouteUtils } from '@core/utils/route-utils.class';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Ticket } from '@shared/models/ticket.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-part-c',
  templateUrl: './part-c.component.html',
  styleUrls: ['./part-c.component.scss'],
})
export class PartCComponent extends BaseDisputeFormPage implements OnInit {
  public busy: Subscription;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService,
  ) {
    super(route, router, formBuilder, disputeService, disputeResource);
  }

  public ngOnInit(): void {

    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.formStep4.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.formStep4)) {
      this.disputeService.ticket$.next({
        ...this.disputeService.ticket,
        ...this.formStep4.value,
      });

      this.routeNext(RouteUtils.currentRoutePath(this.router.url));
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }

  public onBack() {
    this.routeBack(RouteUtils.currentRoutePath(this.router.url));
  }

  public get interpreterRequired(): FormControl {
    return this.formStep4.get('interpreterRequired') as FormControl;
  }
}

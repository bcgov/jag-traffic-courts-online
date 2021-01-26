import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subscription, timer } from 'rxjs';

import moment from 'moment';
import { Ticket } from '@shared/models/ticket.model';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { RouteUtils } from '@core/utils/route-utils.class';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastService } from '@core/services/toast.service';
import { LoggerService } from '@core/services/logger.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { UtilsService } from '@core/services/utils.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent extends BaseDisputeFormPage implements OnInit {
  public busy: Subscription;
  public maxViolationDate: moment.Moment;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    private toastService: ToastService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService,
  ) {
    super(route, router, formBuilder, disputeService, disputeResource);

    this.maxViolationDate = moment();
  }

  public ngOnInit(): void {
    this.createFormInstance();
  }

  protected createFormInstance(): void {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.formStep1.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.formStep1)) {
      this.disputeService.ticket$.next({
        ...this.disputeService.ticket,
        ...this.formStep1.value,
      });

      const source = timer(1000);
      this.busy = source.subscribe((val) => {
        this.toastService.openSuccessToast('Information has been saved');
        this.routeNext(RouteUtils.currentRoutePath(this.router.url));
      });
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }
}

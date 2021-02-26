import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { Subscription, timer } from 'rxjs';
import moment from 'moment';

import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { UtilsService } from '@core/services/utils.service';
import { Ticket } from '@shared/models/ticket.model';

@Component({
  selector: 'app-stepper',
  templateUrl: './stepper.component.html',
  styleUrls: ['./stepper.component.scss'],
})
export class StepperComponent extends BaseDisputeFormPage implements OnInit {
  public busy: Subscription;
  public maxViolationDate: moment.Moment;
  public pageMode: string;

  public newSteps: any[];

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    private toastService: ToastService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {
    super(route, router, formBuilder, disputeService, disputeResource);

    this.maxViolationDate = moment();
    this.pageMode = 'full';

    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      let steps = [];
      steps.push({ title: 'Review', value: null, pageName: 1 });

      let index = 0;
      ticket.counts.forEach((cnt) => {
        steps.push({
          title: 'Offence #' + cnt.countNo,
          description: cnt.description,
          value: index,
          pageName: 2,
        });
        index++;
      });

      // steps.push({ title: 'Court', value: null, pageName: 3 });
      steps.push({ title: 'Overview', value: null, pageName: 5 });

      this.disputeService.steps$.next(steps);
    });
  }

  ngOnInit(): void {
    this.disputeService.steps$.subscribe((stepData) => {
      console.log('STEPS$', stepData);
      this.newSteps = stepData;
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { Subscription, timer } from 'rxjs';

import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { UtilsService } from '@core/services/utils.service';
import { Ticket } from '@shared/models/ticket.model';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';

@Component({
  selector: 'app-stepper',
  templateUrl: './stepper.component.html',
  styleUrls: ['./stepper.component.scss'],
})
export class StepperComponent extends BaseDisputeFormPage implements OnInit {
  public busy: Subscription;
  public pageMode: string;

  public disputeSteps: any[];

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    private toastService: ToastService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {
    super(
      route,
      router,
      formBuilder,
      disputeService,
      disputeResource,
      disputeFormStateService
    );

    this.pageMode = 'full';

    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      console.log('ticket', ticket);

      let steps = [];
      steps.push({
        title: 'Violation Ticket Review',
        value: null,
        pageName: 1,
      });

      let index = 0;
      ticket.counts.forEach((cnt) => {
        steps.push({
          title: 'Offence #' + cnt.countNo + ' Review ',
          description: cnt.description,
          statuteId: cnt.statuteId,
          value: index,
          pageName: 2,
        });
        index++;
      });

      steps.push({ title: 'Dispute Overview', value: null, pageName: 5 });

      this.disputeService.steps$.next(steps);
    });
  }

  ngOnInit(): void {
    this.disputeService.steps$.subscribe((stepData) => {
      this.disputeSteps = stepData;
    });
  }

  public onStepSave($event): void {
    console.log('json', this.disputeFormStateService.json);

    const numberOfSteps = $event.stepper.steps.length;
    const currentStep = $event.stepper.selectedIndex + 1;

    this.logger.info(
      'numberOfSteps',
      numberOfSteps,
      'currentStep',
      currentStep
    );

    const count1 = this.disputeFormStateService.stepCount1Form.controls.count
      .value;
    const count2 = this.disputeFormStateService.stepCount2Form.controls.count
      .value;
    const count3 = this.disputeFormStateService.stepCount3Form.controls.count
      .value;
    const showCourtPage =
      (count1 && count1 != 'A') ||
      (count2 && count2 != 'A') ||
      (count3 && count3 != 'A')
        ? true
        : false;

    let steps = this.disputeService.steps$.value;
    const courtPageExists = steps.some((step) => step.pageName === 3);

    if (showCourtPage && !courtPageExists) {
      // Check if the 'Court' step should be added
      steps.splice(steps.length - 1, 0, {
        title: 'Court Information',
        value: null,
        pageName: 3,
      });
      this.disputeService.steps$.next(steps);
    } else if (!showCourtPage && courtPageExists) {
      for (var i = 0; i < steps.length; i++) {
        if (steps[i].pageName === 3) {
          steps.splice(i, 1);
          i--;
        }
      }
    }

    const source = timer(1000);
    this.busy = source.subscribe((val) => {
      if (numberOfSteps === currentStep) {
        // on last step
        this.toastService.openSuccessToast('Dispute has been submitted');
        this.router.navigate(['/']);
      } else {
        this.toastService.openSuccessToast('Information has been saved');
        $event.stepper.next();
      }
    });
  }
}

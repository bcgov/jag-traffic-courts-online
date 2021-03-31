import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { Subscription, timer } from 'rxjs';

import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { CurrencyPipe } from '@angular/common';
import { TicketDispute } from '@shared/models/ticket-dispute.model';

export class StepData {
  constructor(
    public stepNumber: number,
    public title: string,
    public title2Label?: string,
    public title2?: string,
    public title3Label?: string,
    public title3?: string,
    public title4Label?: string,
    public title4?: string,
    public description?: string
  ) {}
}

export enum StepNumber {
  REVIEW = 1,
  OFFENCE = 2,
  COURT = 3,
  OVERVIEW = 4,
}

@Component({
  selector: 'app-stepper',
  templateUrl: './stepper.component.html',
  styleUrls: ['./stepper.component.scss'],
})
export class StepperComponent extends BaseDisputeFormPage implements OnInit {
  public busy: Subscription;
  public pageMode: string;
  public disputeSteps: StepData[];

  private currentParams: Params;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    private activatedRoute: ActivatedRoute,
    private toastService: ToastService,
    private dialog: MatDialog,
    private formatDatePipe: FormatDatePipe,
    private currencyPipe: CurrencyPipe,
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
  }

  public ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      this.currentParams = params;
    });

    // TODO  hack to temporarily to make sure data always displays (when refresh page)
    if (!this.disputeService.ticketDispute) {
      this.disputeResource.getTicket().subscribe((response) => {
        this.disputeService.ticket$.next(response);
        const ticketDispute = this.disputeService.getDisputeTicket(
          response,
          response.offences[2]
        );
        this.disputeService.ticketDispute$.next(ticketDispute);
      });
    }

    this.disputeService.ticketDispute$.subscribe((ticketDispute) => {
      this.initializeDisputeSteps(ticketDispute);
      this.patchForm();
    });

    this.disputeService.steps$.subscribe((stepData) => {
      this.disputeSteps = stepData;
    });
  }

  public onStepCancel(): void {
    this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.SUMMARY)], {
      queryParams: this.currentParams,
    });
  }

  public onStepSave(stepper: MatStepper): void {
    this.logger.info('Dispute Data:', this.disputeFormStateService.json);

    const numberOfSteps = stepper.steps.length;
    const currentStep = stepper.selectedIndex + 1;
    // const showCourtPage = this.shouldShowCourtPage();

    // const steps = this.disputeService.steps$.value;
    // const courtPageExists = steps.some((step) => step.pageName === 3);

    // if (showCourtPage && !courtPageExists) {
    //   this.addCourtPage(steps);
    // } else if (!showCourtPage && courtPageExists) {
    //   this.removeCourtPage(steps);
    // }

    // on the last step
    if (numberOfSteps === currentStep) {
      this.submitDispute();
    } else {
      this.saveStep(stepper);
    }
  }

  // private shouldShowCourtPage(): boolean {
  //   const count1 = this.disputeFormStateService.stepCount1Form.controls.count
  //     .value;
  //   const count2 = this.disputeFormStateService.stepCount2Form.controls.count
  //     .value;
  //   const count3 = this.disputeFormStateService.stepCount3Form.controls.count
  //     .value;
  //   const shouldShow =
  //     (count1 && count1 !== 'A') ||
  //     (count2 && count2 !== 'A') ||
  //     (count3 && count3 !== 'A')
  //       ? true
  //       : false;

  //   return shouldShow;
  // }

  private initializeDisputeSteps(ticketDispute: TicketDispute): void {
    const offence = ticketDispute?.offence;
    if (!offence) {
      return;
    }
    this.logger.info('initializeDisputeSteps offence', offence);

    const steps = [];
    let stepData = new StepData(StepNumber.REVIEW, 'Violation Ticket Review');
    steps.push(stepData);

    stepData = new StepData(
      StepNumber.OFFENCE,
      'Offence #' + offence.offenceNumber + ' Review and Action',
      'Offence Description',
      offence.description,
      'Ticket Amount',
      this.transformCurrency(offence.ticketAmount),
      'Ticket Amount (if paid by ' + this.transformDate(offence.dueDate) + ' )',
      this.transformCurrency(offence.amountDue),
      offence.description
    );
    steps.push(stepData);

    stepData = new StepData(StepNumber.COURT, 'Court Information');
    steps.push(stepData);

    stepData = new StepData(StepNumber.OVERVIEW, 'Dispute Overview');
    steps.push(stepData);

    this.disputeService.steps$.next(steps);
  }

  private transformDate(date: string) {
    return this.formatDatePipe.transform(date);
  }

  private transformCurrency(amount) {
    return this.currencyPipe.transform(amount);
  }

  // private addCourtPage(steps: StepData[]): void {
  //   const courtStepData = new StepData(3, 'Court Information');
  //   steps.splice(steps.length - 1, 0, courtStepData);
  //   this.disputeService.steps$.next(steps);
  // }

  // private removeCourtPage(steps: StepData[]): void {
  //   for (let i = 0; i < steps.length; i++) {
  //     if (steps[i].pageName === 3) {
  //       steps.splice(i, 1);
  //       i--;
  //     }
  //   }
  //   this.disputeService.steps$.next(steps);
  // }

  /**
   * @description
   * Save the data on the current step
   */
  private saveStep(stepper: MatStepper): void {
    const source = timer(1000);
    this.busy = source.subscribe((val) => {
      this.toastService.openSuccessToast('Information has been saved');
      stepper.next();
    });
  }

  /**
   * @description
   * Submit the dispute
   */
  private submitDispute(): void {
    const data: DialogOptions = {
      title: 'Submit Dispute',
      message:
        'When your dispute is submitted for adjudication, it can no longer be updated. Are you ready to submit your dispute?',
      actionText: 'Submit Dispute',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          const source = timer(1000);
          this.busy = source.subscribe((val) => {
            this.toastService.openSuccessToast('Dispute has been submitted');
            this.router.navigate(
              [DisputeRoutes.routePath(DisputeRoutes.SUCCESS)],
              {
                queryParams: this.currentParams,
              }
            );
          });
        }
      });
  }

  // public onSelectionChange(stepper): void {
  //   this.logger.info('onSelectionChange:', this.disputeFormStateService.json);
  // }
}

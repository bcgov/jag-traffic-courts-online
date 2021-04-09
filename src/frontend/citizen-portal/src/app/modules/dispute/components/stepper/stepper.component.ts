import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';

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
import { UtilsService } from '@core/services/utils.service';

@Component({
  selector: 'app-stepper',
  templateUrl: './stepper.component.html',
  styleUrls: ['./stepper.component.scss'],
})
export class StepperComponent
  extends BaseDisputeFormPage
  implements OnInit, AfterViewInit {
  public busy: Subscription;
  public showAdditionalInformationStep: boolean;
  @ViewChild(MatStepper)
  private stepper: MatStepper;

  public reviewForm: FormGroup;
  public offenceForm: FormGroup;
  public courtForm: FormGroup;
  public overviewForm: FormGroup;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    private utilsService: UtilsService,
    private toastService: ToastService,
    private dialog: MatDialog,
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

    this.showAdditionalInformationStep = false;
  }

  public ngOnInit(): void {
    this.disputeService.ticketDispute$.subscribe((ticketDispute) => {
      if (!ticketDispute) {
        this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
      }

      this.disputeFormStateService.reset();
      this.patchForm();
    });

    const formsList = this.disputeFormStateService.forms;
    [
      this.reviewForm,
      this.offenceForm,
      this.courtForm,
      this.overviewForm,
    ] = formsList as FormGroup[];
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }

  public onStepCancel(): void {
    const ticketDispute = this.disputeService.ticketDispute;
    const params = {
      ticketNumber: ticketDispute.violationTicketNumber,
      time: ticketDispute.violationTime,
    };

    this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.SUMMARY)], {
      queryParams: params,
    });
  }

  public onStepSave(stepper: MatStepper): void {
    this.logger.info('Dispute Data:', this.disputeFormStateService.json);

    const numberOfSteps = stepper.steps.length;
    const currentStep = stepper.selectedIndex + 1;

    this.showAdditionalInformationStep = this.showCourtPage();

    this.logger.info(
      'showAdditionalInformationStep',
      this.showAdditionalInformationStep
    );

    // on the last step
    if (numberOfSteps === currentStep) {
      this.submitDispute();
    } else {
      this.saveStep(stepper);
    }
  }

  private showCourtPage(): boolean {
    const offenceStatus = this.disputeFormStateService.stepOffenceForm.controls
      .offenceAgreementStatus.value;
    console.log('offenceStatus', offenceStatus);
    return offenceStatus && offenceStatus !== '1' ? true : false;
  }

  /**
   * @description
   * Save the data on the current step
   */
  private saveStep(stepper: MatStepper): void {
    // const source = timer(1000);
    // this.busy = source.subscribe((val) => {
    // this.toastService.openSuccessToast('Information has been saved');
    stepper.next();
    // });
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
          this.logger.info('submitDispute', this.disputeFormStateService.json);

          this.busy = this.disputeResource
            .createDispute(this.disputeFormStateService.json)
            .subscribe(() => {
              this.toastService.openSuccessToast(
                'Dispute has been successfully submitted'
              );
              this.router.navigate([
                DisputeRoutes.routePath(DisputeRoutes.SUCCESS),
              ]);
            });
        }
      });
  }

  public onSelectionChange(event): void {
    const stepIndex = event.selectedIndex;
    const stepId = this.stepper._getStepLabelId(stepIndex);
    const stepElement = document.getElementById(stepId);
    if (stepElement) {
      setTimeout(() => {
        stepElement.scrollIntoView({
          block: 'start',
          inline: 'nearest',
          behavior: 'smooth',
        });
      }, 250);
    }
  }
}

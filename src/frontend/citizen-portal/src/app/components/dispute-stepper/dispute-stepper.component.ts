import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { AppRoutes } from 'app/app.routes';
import { BaseDisputeFormPage } from 'app/components/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dispute-stepper',
  templateUrl: './dispute-stepper.component.html',
  styleUrls: ['./dispute-stepper.component.scss'],
})
export class DisputeStepperComponent
  extends BaseDisputeFormPage
  implements OnInit {
  public busy: Subscription;
  @ViewChild(MatStepper)
  private stepper: MatStepper;

  public overviewTicket: TicketDisputeView;

  public disputantForm: FormGroup;
  public offence1Form: FormGroup;
  public offence2Form: FormGroup;
  public offence3Form: FormGroup;
  public additionalForm: FormGroup;
  public overviewForm: FormGroup;

  public offence1Exists: boolean;
  public offence2Exists: boolean;
  public offence3Exists: boolean;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
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

    this.offence1Exists = false;
    this.offence2Exists = false;
    this.offence3Exists = false;
  }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      this.disputeFormStateService.reset();
      this.patchForm();

      const formsList = this.disputeFormStateService.forms;
      [
        this.disputantForm,
        this.offence1Form,
        this.offence2Form,
        this.offence3Form,
        this.additionalForm,
        this.overviewForm,
      ] = formsList as FormGroup[];

      const currentTicket = this.disputeService.ticket;
      if (currentTicket) {
        this.disputantForm.patchValue(currentTicket.disputant);
        this.additionalForm.patchValue(currentTicket.additional);

        this.disputeService.ticket.offences.forEach((offence) => {
          if (offence.offenceNumber === 1) {
            this.offence1Exists = true;
            this.offence1Form.patchValue(offence);
          } else if (offence.offenceNumber === 2) {
            this.offence2Exists = true;
            this.offence2Form.patchValue(offence);
          } else if (offence.offenceNumber === 3) {
            this.offence3Exists = true;
            this.offence3Form.patchValue(offence);
          }
        });
      }
    });
  }

  public onStepCancel(): void {
    const ticket = this.disputeService.ticket;
    const params = {
      ticketNumber: ticket.violationTicketNumber,
      time: ticket.violationTime,
    };

    this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
      queryParams: params,
    });
  }

  public onStepSave(stepper: MatStepper): void {
    this.logger.info('DisputeStepperComponent::onStepSave Dispute Data:', this.disputeFormStateService.json);

    const numberOfSteps = stepper.steps.length;
    const currentStep = stepper.selectedIndex + 1;

    // on the last step
    if (numberOfSteps === currentStep) {
      this.submitDispute();
    } else {
      this.saveStep(stepper);
    }
  }

  /**
   * @description
   * Save the data on the current step
   */
  private saveStep(stepper: MatStepper): void {
    stepper.next();
  }

  /**
   * @description
   * Submit the dispute
   */
  private submitDispute(): void {
    const data: DialogOptions = {
      titleKey: 'Submit request',
      messageKey:
        'When your request is submitted for adjudication, it can no longer be updated. Are you ready to submit your request?',
      actionTextKey: 'Submit request',
      cancelTextKey: 'Cancel',
      icon: null,
    };

    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          const payload = this.disputeFormStateService.jsonTicketDispute;
          payload.violationTicketNumber = this.ticket.violationTicketNumber;
          payload.violationTime = this.ticket.violationTime;

          this.busy = this.disputeResource
            .createTicketDispute(payload)
            .subscribe((newDisputeTicket: TicketDisputeView) => {
              this.disputeService.ticket$.next(newDisputeTicket);

              this.router.navigate([
                AppRoutes.disputePath(AppRoutes.SUBMIT_SUCCESS),
              ]);
            });
        }
      });
  }

  public onSelectionChange(event: StepperSelectionEvent): void {
    this.logger.info('DisputeStepperComponent::onSelectionChange Dispute Data:', this.disputeFormStateService.json);

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

    const numberOfSteps = this.stepper.steps.length;
    const currentStep = event.selectedIndex + 1;
    const previousStep = event.previouslySelectedIndex + 1;
    this.logger.info('DisputeStepperComponent::onSelectionChange currentStep', currentStep, 'previousStep', previousStep);

    if (previousStep === 2) {
      this.updateOffenceForms();
    }

    if (currentStep >= (numberOfSteps - 1)) {
      this.setCourtRequired();
    }

    this.logger.info('DisputeStepperComponent::onSelectionChange After:', this.disputeFormStateService.json);
    this.overviewTicket = this.disputeFormStateService.jsonTicketDispute;
  }

  /**
   * @description
   * Determine if the current step is 'Additional Information' (2nd last step) or the last step, update the courtRequired flag
   */
  private setCourtRequired(): void {
    const offenceForms = this.disputeFormStateService.offenceForms;
    let courtRequired = false;
    let reductionRequired = false;
    let isReductionNotInCourt = false;

    offenceForms.forEach((form: AbstractControl) => {
      const offenceNumber = form.get('offenceNumber') as FormControl;
      if (offenceNumber.value) {
        const offenceAgreementStatus = form.get('offenceAgreementStatus') as FormControl;
        const reductionAppearInCourt = form.get('reductionAppearInCourt') as FormControl;

        if (offenceAgreementStatus.value === 'DISPUTE') {
          courtRequired = true;
        } else if (offenceAgreementStatus.value === 'REDUCTION') {
          reductionRequired = true;
          if (reductionAppearInCourt.value) {
            courtRequired = true;
          } else {
            isReductionNotInCourt = true;
          }
        }
      }
    });

    this.logger.log('onSelectionChange courtRequired', courtRequired);
    this.logger.log('onSelectionChange reductionRequired', reductionRequired);
    this.logger.log('onSelectionChange isReductionNotInCourt', isReductionNotInCourt);

    const additionalForm = this.disputeFormStateService.stepAdditionalForm;

    const isCourtRequiredControl = additionalForm.get('_isCourtRequired') as FormControl;
    isCourtRequiredControl.setValue(courtRequired);

    const isReductionRequiredControl = additionalForm.get('_isReductionRequired') as FormControl;
    isReductionRequiredControl.setValue(reductionRequired);

    const isReductionNotInCourtControl = additionalForm.get('_isReductionNotInCourt') as FormControl;
    isReductionNotInCourtControl.setValue(isReductionNotInCourt);
  }

  /**
   * @description
   * After leaving the FIRST offence screen, if 'applyToAllCounts' is selected, update the appropriate other values in the other counts.
   */
  private updateOffenceForms(): void {
    this.logger.info('DisputeStepperComponent::updateOffenceForms');
    const offenceForms = this.disputeFormStateService.offenceForms;

    let applyToAllCounts = false;
    let offenceAgreementStatus;
    let reductionAppearInCourt;

    offenceForms.forEach((form: AbstractControl) => {
      const offenceNumber = form.get('offenceNumber') as FormControl;
      const firstOffence = form.get('_firstOffence') as FormControl;

      if (offenceNumber.value) {
        const applyToAllCountsControl = form.get('_applyToAllCounts') as FormControl;
        const offenceAgreementStatusControl = form.get('offenceAgreementStatus') as FormControl;
        const reductionAppearInCourtControl = form.get('reductionAppearInCourt') as FormControl;

        // cleanup bad state
        if (offenceAgreementStatusControl.value !== 'REDUCTION') {
          reductionAppearInCourtControl.setValue(null);
        }

        // Get the data from the first offence to copy to the others
        if (firstOffence.value) {
          applyToAllCounts = applyToAllCountsControl.value;

          // cleanup bad state
          if (offenceAgreementStatusControl.value !== 'DISPUTE' && offenceAgreementStatusControl.value !== 'REDUCTION') {
            applyToAllCounts = false;
            applyToAllCountsControl.setValue(applyToAllCounts);
          }

          offenceAgreementStatus = offenceAgreementStatusControl.value;
          reductionAppearInCourt = reductionAppearInCourtControl.value;

        } else {
          applyToAllCountsControl.setValue(applyToAllCounts);
          if (applyToAllCounts) {
            offenceAgreementStatusControl.setValue(offenceAgreementStatus);
            reductionAppearInCourtControl.setValue(reductionAppearInCourt);

            offenceAgreementStatusControl.disable();
            reductionAppearInCourtControl.disable();
            applyToAllCountsControl.disable();
          } else {
            offenceAgreementStatusControl.enable();
            reductionAppearInCourtControl.enable();
            applyToAllCountsControl.enable();
          }
        }
      }
    });
  }
}

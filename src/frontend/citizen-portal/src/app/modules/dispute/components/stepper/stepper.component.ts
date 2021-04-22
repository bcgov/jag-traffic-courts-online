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
  @ViewChild(MatStepper)
  private stepper: MatStepper;

  public disputantForm: FormGroup;
  public offenceForm: FormGroup;
  public additionalForm: FormGroup;
  public overviewForm: FormGroup;
  private currentDisputeOffenceNumber: number;

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

    this.currentDisputeOffenceNumber = this.router.getCurrentNavigation().extras.state?.disputeOffenceNumber;
    this.logger.info(
      'Dispute Offence Number',
      this.currentDisputeOffenceNumber
    );

    if (!this.currentDisputeOffenceNumber) {
      this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
    }
  }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
      }

      this.disputeFormStateService.reset();
      this.patchForm();
    });

    let offence1Form = null;
    let offence2Form = null;
    let offence3Form = null;
    const formsList = this.disputeFormStateService.forms;
    [
      this.disputantForm,
      offence1Form,
      offence2Form,
      offence3Form,
      this.additionalForm,
      this.overviewForm,
    ] = formsList as FormGroup[];

    if (this.disputeService.ticket) {
      this.disputeService.ticket.offences.forEach((offence) => {
        if (offence.offenceNumber === this.currentDisputeOffenceNumber) {
          switch (offence.offenceNumber) {
            case 1:
              offence.includeOffenceInDispute = true;
              offence1Form.patchValue(offence);
              this.offenceForm = offence1Form;
              break;
            case 2:
              offence.includeOffenceInDispute = true;
              offence2Form.patchValue(offence);
              this.offenceForm = offence2Form;
              break;
            case 3:
              offence.includeOffenceInDispute = true;
              offence3Form.patchValue(offence);
              this.offenceForm = offence3Form;
              break;
            default:
              this.logger.error(
                'Invalid disputeOffenceNumber',
                this.currentDisputeOffenceNumber
              );
              this.router.navigate([
                DisputeRoutes.routePath(DisputeRoutes.FIND),
              ]);
          }
        }
      });
    }
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }

  public onStepCancel(): void {
    const ticket = this.disputeService.ticket;
    const params = {
      ticketNumber: ticket.violationTicketNumber,
      time: ticket.violationTime,
    };

    this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.SUMMARY)], {
      queryParams: params,
    });
  }

  public onStepSave(stepper: MatStepper): void {
    this.logger.info('Dispute Data:', this.disputeFormStateService.json);

    const numberOfSteps = stepper.steps.length;
    const currentStep = stepper.selectedIndex + 1;

    // on the last step
    if (numberOfSteps === currentStep) {
      this.submitOffenceDispute();
    } else {
      this.saveStep(stepper);
    }
  }

  // private showCourtPage(): boolean {
  //   const offenceStatus = this.disputeFormStateService.stepOffenceForm.controls
  //     .offenceAgreementStatus.value;
  //   return offenceStatus && offenceStatus !== '1' ? true : false;
  // }

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
  private submitOffenceDispute(): void {
    const data: DialogOptions = {
      title: 'Submit Offence Dispute',
      message:
        'When your dispute of the offence is submitted for adjudication, it can no longer be updated. Are you ready to submit your dispute?',
      actionText: 'Submit Dispute',
    };
    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          const payload = this.disputeFormStateService.jsonCountDispute;
          this.busy = this.disputeResource
            .createCountDispute(payload)
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

import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
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
  selector: 'app-dispute-all-stepper',
  templateUrl: './dispute-all-stepper.component.html',
  styleUrls: ['./dispute-all-stepper.component.scss'],
})
export class DisputeAllStepperComponent
  extends BaseDisputeFormPage
  implements OnInit, AfterViewInit {
  public busy: Subscription;
  @ViewChild(MatStepper)
  private stepper: MatStepper;

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

    this.offence1Exists = false;
    this.offence2Exists = false;
    this.offence3Exists = false;
  }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
      }

      this.disputeFormStateService.reset();
      this.patchForm();
    });

    const formsList = this.disputeFormStateService.forms;
    [
      this.disputantForm,
      this.offence1Form,
      this.offence2Form,
      this.offence3Form,
      this.additionalForm,
      this.overviewForm,
    ] = formsList as FormGroup[];

    if (this.disputeService.ticket) {
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
          const payload = this.disputeFormStateService.jsonTicketDispute;
          this.busy = this.disputeResource
            .createTicketDispute(payload)
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

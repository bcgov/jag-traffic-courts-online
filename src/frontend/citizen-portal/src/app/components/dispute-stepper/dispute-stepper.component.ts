import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';
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
  implements OnInit, AfterViewInit
{
  public busy: Subscription;
  @ViewChild(MatStepper)
  private stepper: MatStepper;

  public overviewTicket: TicketDispute;

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

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
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

          this.logger.info('DisputeStepperComponent::submitDispute', payload);

          this.busy = this.disputeResource
            .createTicketDispute(payload)
            .subscribe((newDisputeTicket: TicketDispute) => {
              this.disputeService.ticket$.next(newDisputeTicket);

              this.router.navigate([
                AppRoutes.disputePath(AppRoutes.SUBMIT_SUCCESS),
              ]);
            });
        }
      });
  }

  public onSelectionChange(event): void {
    this.overviewTicket = this.disputeFormStateService.jsonTicketDispute;

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

  public get isCourtRequired(): boolean {
    return this.disputeFormStateService.isCourtRequired;
  }
}

import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { ViewportService } from '@core/services/viewport.service';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { Ticket } from '@shared/models/ticket.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-step-overview',
  templateUrl: './step-overview.component.html',
  styleUrls: ['./step-overview.component.scss'],
})
export class StepOverviewComponent
  extends BaseDisputeFormPage
  implements OnInit {
  @Input() public stepper: MatStepper;

  public busy: Subscription;
  public nextBtnLabel: string;
  public ticket: Ticket;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    private viewportService: ViewportService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService,
    private dialog: MatDialog,
    private toastService: ToastService
  ) {
    super(route, router, formBuilder, disputeService, disputeResource);
    this.nextBtnLabel = 'Submit';
  }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.ticket = ticket;
    });
  }

  public onBack() {
    this.stepper.previous();
  }

  public onSubmit(): void {
    // if (this.formUtilsService.checkValidity(this.formStepCourt)) {
    const data: DialogOptions = {
      title: 'Submit Dispute',
      message:
        'When your dispute is submitted for adjudication, it can no longer be updated. Are you ready to submit your dispute?',
      actionText: 'Submit Dispute',
    };
    this.busy = this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          this.toastService.openSuccessToast('Dispute has been submitted');
          this.router.navigate(['/']);
        }
      });

    // } else {
    //   this.toastService.openErrorToast('Your dispute has an error that needs to be corrected before you will be able to submit');
    // }
  }
}

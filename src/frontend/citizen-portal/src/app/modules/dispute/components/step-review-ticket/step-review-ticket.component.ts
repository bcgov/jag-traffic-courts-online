import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Ticket } from '@shared/models/ticket.model';
import moment from 'moment';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-step-review-ticket',
  templateUrl: './step-review-ticket.component.html',
  styleUrls: ['./step-review-ticket.component.scss'],
})
export class StepReviewTicketComponent
  extends BaseDisputeFormPage
  implements OnInit {
  @Input() public stepper: MatStepper;

  public busy: Subscription;
  public maxViolationDate: moment.Moment;
  public nextBtnLabel: string;
  public ticket: Ticket;

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
  }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.formStepReview.patchValue(ticket);
      this.ticket = ticket;
    });
    this.nextBtnLabel = 'Next';
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.formStepReview)) {
      // this.disputeService.ticket$.next({
      //   ...this.disputeService.ticket,
      //   ...this.formStepReview.value,
      // });

      console.log('formStepReview', this.formStepReview.value);

      const source = timer(1000);
      this.busy = source.subscribe((val) => {
        this.toastService.openSuccessToast('Information has been saved');
        this.stepper.next();
      });
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }
}

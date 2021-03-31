import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';

@Component({
  selector: 'app-step-review-ticket',
  templateUrl: './step-review-ticket.component.html',
  styleUrls: ['./step-review-ticket.component.scss'],
})
export class StepReviewTicketComponent
  extends BaseDisputeFormPage
  implements OnInit {
  @Input() public stepper: MatStepper;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();
  @Output() public stepCancel: EventEmitter<MatStepper> = new EventEmitter();

  public isSubmitted = false;
  public prevBtnLabel: string;
  public prevBtnIcon: string;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
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
  }

  public ngOnInit() {
    this.form = this.disputeFormStateService.stepReviewForm;
    this.ticketDispute = this.disputeService.ticketDispute;

    this.prevBtnLabel = 'Cancel';
    this.prevBtnIcon = 'close';
  }

  public onBack() {
    this.stepCancel.emit();
  }

  public onSubmit(): void {
    this.isSubmitted = true;
    if (this.formUtilsService.checkValidity(this.form)) {
      this.stepSave.emit(this.stepper);
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }

  public get emailAddress(): FormControl {
    return this.form.get('emailAddress') as FormControl;
  }
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
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
  selector: 'app-step-single-count',
  templateUrl: './step-single-count.component.html',
  styleUrls: ['./step-single-count.component.scss'],
})
export class StepSingleCountComponent
  extends BaseDisputeFormPage
  implements OnInit {
  @Input() public stepper: MatStepper;
  @Input() public stepControl: FormGroup;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();
  @Output() public stepCancel: EventEmitter<MatStepper> = new EventEmitter();

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
    this.form = this.stepControl;
    this.patchForm();

    this.prevBtnLabel = 'Cancel';
    this.prevBtnIcon = 'close';
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.form)) {
      this.stepSave.emit(this.stepper);
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }

  public onBack() {
    this.stepCancel.emit();
  }

  public get offenceAgreementStatus(): FormControl {
    return this.form.get('offenceAgreementStatus') as FormControl;
  }

  public get requestReduction(): FormControl {
    return this.form.get('requestReduction') as FormControl;
  }

  public get requestMoreTime(): FormControl {
    return this.form.get('requestMoreTime') as FormControl;
  }

  public get reductionReason(): FormControl {
    return this.form.get('reductionReason') as FormControl;
  }

  public get moreTimeReason(): FormControl {
    return this.form.get('moreTimeReason') as FormControl;
  }

  public get offenceDescription(): FormControl {
    return this.form.get('offenceDescription') as FormControl;
  }

  public get offenceNumber(): FormControl {
    return this.form.get('offenceNumber') as FormControl;
  }

  public get ticketedAmount(): FormControl {
    return this.form.get('ticketedAmount') as FormControl;
  }

  public get amountDue(): FormControl {
    return this.form.get('amountDue') as FormControl;
  }

  public get discountAmount(): FormControl {
    return this.form.get('discountAmount') as FormControl;
  }

  public get discountDueDate(): FormControl {
    return this.form.get('discountDueDate') as FormControl;
  }
}

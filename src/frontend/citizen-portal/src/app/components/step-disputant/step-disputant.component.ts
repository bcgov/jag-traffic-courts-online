import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { BaseDisputeFormPage } from 'app/components/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-step-disputant',
  templateUrl: './step-disputant.component.html',
  styleUrls: ['./step-disputant.component.scss'],
})
export class StepDisputantComponent
  extends BaseDisputeFormPage
  implements OnInit
{
  @Input() public stepper: MatStepper;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();
  @Output() public stepCancel: EventEmitter<MatStepper> = new EventEmitter();

  public previousButtonIcon = 'close';
  public previousButtonKey = 'stepper.cancel';
  public saveButtonKey = 'stepper.next';

  public maxDateOfBirth: Date;

  private MINIMUM_AGE = 18;

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
    const today = new Date();
    this.maxDateOfBirth = new Date();
    this.maxDateOfBirth.setFullYear(today.getFullYear() - this.MINIMUM_AGE);
  }

  public ngOnInit() {
    this.form = this.disputeFormStateService.stepDisputantForm;
    this.patchForm();
  }

  public onBack() {
    this.stepCancel.emit();
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.form)) {
      this.stepSave.emit(this.stepper);
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }

  public get homePhone(): FormControl {
    return this.form.get('homePhone') as FormControl;
  }

  public get workPhone(): FormControl {
    return this.form.get('workPhone') as FormControl;
  }

  public get emailAddress(): FormControl {
    return this.form.get('emailAddress') as FormControl;
  }
}

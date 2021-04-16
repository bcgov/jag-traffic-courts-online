import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';

@Component({
  selector: 'app-step-overview',
  templateUrl: './step-overview.component.html',
  styleUrls: ['./step-overview.component.scss'],
})
export class StepOverviewComponent
  extends BaseDisputeFormPage
  implements OnInit {
  @Input() public stepper: MatStepper;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();

  public nextBtnLabel: string;

  public reviewForm: FormGroup;
  public offenceForm: FormGroup;
  public courtForm: FormGroup;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService,
    private toastService: ToastService
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
    const formsList = this.disputeFormStateService.forms;
    [
      this.reviewForm,
      this.offenceForm,
      this.courtForm,
      this.form,
    ] = formsList as FormGroup[];

    this.patchForm();
    this.nextBtnLabel = 'Submit';
  }

  public onBack() {
    this.stepper.previous();
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.form)) {
      if (this.disputeFormStateService.isValid) {
        this.stepSave.emit(this.stepper);
        return;
      } else {
        this.toastService.openErrorToast(
          'Your dispute has an error that needs to be corrected before you will be able to submit'
        );
      }
    }
    this.utilsService.scrollToErrorSection();
  }

  public get informationCertified(): FormControl {
    return this.form.get('informationCertified') as FormControl;
  }

  public get emailAddress(): FormControl {
    return this.reviewForm.get('emailAddress') as FormControl;
  }

  public get offenceAgreementStatus(): FormControl {
    return this.offenceForm.get('offenceAgreementStatus') as FormControl;
  }

  public get requestReduction(): FormControl {
    return this.offenceForm.get('requestReduction') as FormControl;
  }

  public get requestMoreTime(): FormControl {
    return this.offenceForm.get('requestMoreTime') as FormControl;
  }

  public get reductionReason(): FormControl {
    return this.offenceForm.get('reductionReason') as FormControl;
  }

  public get moreTimeReason(): FormControl {
    return this.offenceForm.get('moreTimeReason') as FormControl;
  }

  public get courtDataExists(): boolean {
    return (
      this.lawyerPresent.value ||
      this.interpreterRequired.value ||
      this.witnessPresent.value
    );
  }

  public get lawyerPresent(): FormControl {
    return this.courtForm.get('lawyerPresent') as FormControl;
  }

  public get interpreterRequired(): FormControl {
    return this.courtForm.get('interpreterRequired') as FormControl;
  }

  public get interpreterLanguage(): FormControl {
    return this.courtForm.get('interpreterLanguage') as FormControl;
  }

  public get witnessPresent(): FormControl {
    return this.courtForm.get('witnessPresent') as FormControl;
  }
}

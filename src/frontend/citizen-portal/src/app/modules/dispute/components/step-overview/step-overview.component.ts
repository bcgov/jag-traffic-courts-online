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
import { Additional } from '@shared/models/additional.model';
import { Disputant } from '@shared/models/disputant.model';

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

  public disputantForm: FormGroup;
  public offence1Form: FormGroup;
  public offence2Form: FormGroup;
  public offence3Form: FormGroup;
  public additionalForm: FormGroup;

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
    this.form = this.disputeFormStateService.stepOverviewForm;
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

  public get offenceFormsList(): any[] {
    return this.disputeFormStateService.offences;
  }

  public get disputantInfo(): Disputant {
    return this.disputeFormStateService.disputant;
  }

  public get additionalInfo(): Additional {
    return this.disputeFormStateService.additional;
  }

  public get informationCertified(): FormControl {
    return this.form.get('informationCertified') as FormControl;
  }

  public get offenceAgreementStatus(): FormControl {
    return this.offence1Form.get('offenceAgreementStatus') as FormControl;
  }

  public get requestReduction(): FormControl {
    return this.offence1Form.get('requestReduction') as FormControl;
  }

  public get requestMoreTime(): FormControl {
    return this.offence1Form.get('requestMoreTime') as FormControl;
  }

  public get reductionReason(): FormControl {
    return this.offence1Form.get('reductionReason') as FormControl;
  }

  public get moreTimeReason(): FormControl {
    return this.offence1Form.get('moreTimeReason') as FormControl;
  }

  public get lawyerPresent(): FormControl {
    return this.additionalForm.get('lawyerPresent') as FormControl;
  }

  public get interpreterRequired(): FormControl {
    return this.additionalForm.get('interpreterRequired') as FormControl;
  }

  public get interpreterLanguage(): FormControl {
    return this.additionalForm.get('interpreterLanguage') as FormControl;
  }

  public get witnessPresent(): FormControl {
    return this.additionalForm.get('witnessPresent') as FormControl;
  }
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { BaseDisputeFormPage } from 'app/components/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Additional } from '@shared/models/additional.model';
import { Disputant } from '@shared/models/disputant.model';
import { ConfigService } from '@config/config.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-step-overview',
  templateUrl: './step-overview.component.html',
  styleUrls: ['./step-overview.component.scss'],
})
export class StepOverviewComponent
  extends BaseDisputeFormPage
  implements OnInit
{
  @Input() public stepper: MatStepper;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();

  public defaultLanguage: string;
  public previousButtonIcon = 'keyboard_arrow_left';
  public previousButtonKey = 'stepper.back';
  public saveButtonKey = 'stepper.submit';

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
    private toastService: ToastService,
    private configService: ConfigService,
    private translateService: TranslateService
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
    this.defaultLanguage = this.translateService.getDefaultLang();
    this.form = this.disputeFormStateService.stepOverviewForm;
    this.patchForm();
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
          this.configService.dispute_validation_error
        );
      }
    }
    this.utilsService.scrollToErrorSection();
  }

  public get offenceFormsList(): any[] {
    return this.disputeFormStateService.offences;
  }

  // public get disputantInfo(): Disputant {
  //   return this.disputeFormStateService.disputant;
  // }

  public get additionalInfo(): Additional {
    return this.disputeFormStateService.additional;
  }

  // public get informationCertified(): FormControl {
  //   return this.form.get('informationCertified') as FormControl;
  // }

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

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { Config } from '@config/config.model';
import { ConfigService } from '@config/config.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { BaseDisputeFormPage } from 'app/components/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-step-additional',
  templateUrl: './step-additional.component.html',
  styleUrls: ['./step-additional.component.scss'],
})
export class StepAdditionalComponent
  extends BaseDisputeFormPage
  implements OnInit {
  @Input() public stepper: MatStepper;
  @Input() public isShowCheckbox: any;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();

  public previousButtonIcon = 'keyboard_arrow_left';
  public previousButtonKey = 'stepper.back';
  public saveButtonKey = 'stepper.next';

  public languages: Config<string>[];
  /**
   * Form field behaviour, customWitnessOption == true shows number input
   * and allows user to type, otherwise use original select options 1 through 5
   */
  public customWitnessOption = false;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private configService: ConfigService,
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

    this.languages = this.configService.languages;
  }

  public ngOnInit() {
    this.form = this.disputeFormStateService.stepAdditionalForm;
    this.customWitnessOption = this.form.getRawValue().numberOfWitnesses >= 6;
    this.form.patchValue({ numberOfWitnesses: this.form.getRawValue().numberOfWitnesses });
    this.patchForm();
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.form)) {
      this.stepSave.emit(this.stepper);
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }

  public onBack() {
    this.stepper.previous();
  }

  public onChangeCallWitnesses(event: MatCheckboxChange) {
    if (event.checked) {
      this.form.controls.numberOfWitnesses.setValidators([Validators.min(0), Validators.required]);
    } else {
      this.form.controls.numberOfWitnesses.clearValidators();
      this.form.controls.numberOfWitnesses.updateValueAndValidity();
    }
  }

  public get interpreterLanguage(): FormControl {
    return this.form.get('interpreterLanguage') as FormControl;
  }

  public get interpreterRequired(): FormControl {
    return this.form.get('interpreterRequired') as FormControl;
  }

  public get witnessPresent(): FormControl {
    return this.form.get('witnessPresent') as FormControl;
  }

  public get isCourtRequired(): FormControl {
    return this.form.get('_isCourtRequired') as FormControl;
  }

  public get isReductionRequired(): FormControl {
    return this.form.get('_isReductionRequired') as FormControl;
  }

  public get isReductionNotInCourt(): FormControl {
    return this.form.get('_isReductionNotInCourt') as FormControl;
  }

  public get numberOfWitnesses(): FormControl {
    return this.form.get('numberOfWitnesses') as FormControl;
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
  public get lawyerPresent(): FormControl {
    return this.form.get('lawyerPresent') as FormControl;
  }
  public get skills() : FormArray {
    return this.form.get('skills') as FormArray;
  }
  newSkill(value1, value2): FormGroup {
    return this.formBuilder.group({
      reductionAppearInCourt: value1,
      reductionAppearInCourtDoNot: value2,
    });
  }
}

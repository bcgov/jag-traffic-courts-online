import { Component, EventEmitter, Input, OnInit, Output, OnChanges, SimpleChanges, } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigService } from '@config/config.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ViolationTicket } from 'app/api';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-step-additional',
  templateUrl: './step-additional.component.html',
  styleUrls: ['./step-additional.component.scss'],
})
export class StepAdditionalComponent implements OnInit {
  @Input() public input: any;
  @Input() public stepper: MatStepper;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();

  // should be passed from parent
  public form: FormGroup;
  public legalRepresentationForm: FormGroup;
  public ticket: ViolationTicket;
  public isShowCheckbox: any;

  // configs
  public languages = this.configService.languages;
  public previousButtonIcon = 'keyboard_arrow_left';
  public previousButtonKey = 'stepper.back';
  public saveButtonKey = 'stepper.next';

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
  }

  public ngOnInit() {
    Object.assign(this, this.input); // copy from input directly and keep the object linkage
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
}

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
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
import { ConfigService } from '@config/config.service';
import { Config } from '@config/config.model';

@Component({
  selector: 'app-step-additional',
  templateUrl: './step-additional.component.html',
  styleUrls: ['./step-additional.component.scss'],
})
export class StepAdditionalComponent
  extends BaseDisputeFormPage
  implements OnInit {
  @Input() public stepper: MatStepper;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();

  public languages: Config<string>[];

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
    // TODO remove this once bug is fixed
    // this.configService.load().toPromise();
    this.languages = this.configService.languages;
    console.log('languages', this.languages);
  }

  public ngOnInit() {
    this.form = this.disputeFormStateService.stepAdditionalForm;
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

  // public isRequired(addressLine: AddressLine): boolean {
  //   return this.formUtilsService.isRequired(this.form, addressLine);
  // }

  public get interpreterLanguage(): FormControl {
    return this.form.get('interpreterLanguage') as FormControl;
  }

  public get interpreterRequired(): FormControl {
    return this.form.get('interpreterRequired') as FormControl;
  }

  public get isCourtRequired(): boolean {
    return this.disputeFormStateService.isCourtRequired;
  }
}

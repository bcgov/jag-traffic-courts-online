import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ViewportService } from '@core/services/viewport.service';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { StepData } from '../stepper/stepper.component';

@Component({
  selector: 'app-step-count',
  templateUrl: './step-count.component.html',
  styleUrls: ['./step-count.component.scss'],
})
export class StepCountComponent extends BaseDisputeFormPage implements OnInit {
  @Input() public stepper: MatStepper;
  @Input() public step: StepData;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    private viewportService: ViewportService,
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
    this.form = this.disputeFormStateService.stepOffenceForm;
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

  public get isMobile(): boolean {
    return this.viewportService.isMobile;
  }

  public get count(): FormControl {
    return this.form.get('count') as FormControl;
  }

  public get count1A1(): FormControl {
    return this.form.get('count1A1') as FormControl;
  }

  public get count1A2(): FormControl {
    return this.form.get('count1A2') as FormControl;
  }

  public get count1B1(): FormControl {
    return this.form.get('count1B1') as FormControl;
  }

  public get count1B2(): FormControl {
    return this.form.get('count1B2') as FormControl;
  }
}

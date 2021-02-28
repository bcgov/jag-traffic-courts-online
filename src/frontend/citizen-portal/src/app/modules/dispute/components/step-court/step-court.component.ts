import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
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
import { Ticket } from '@shared/models/ticket.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-step-court',
  templateUrl: './step-court.component.html',
  styleUrls: ['./step-court.component.scss'],
})
export class StepCourtComponent extends BaseDisputeFormPage implements OnInit {
  @Input() public stepper: MatStepper;
  @Output() public stepSave: EventEmitter<{
    stepper: MatStepper;
    formGroup: FormGroup;
    formArray: FormArray;
  }> = new EventEmitter();

  public busy: Subscription;
  public nextBtnLabel: string;
  public ticket: Ticket;

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
    this.createFormInstance();
    // this.patchForm();
    this.initForm();
    this.nextBtnLabel = 'Next';
  }

  protected createFormInstance() {
    this.form = this.disputeFormStateService.stepCourtForm;
    console.log('StepCourtComponent form', this.form.value);
  }

  protected initForm() {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.ticket = ticket;
      this.form.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.form)) {
      this.stepSave.emit({
        stepper: this.stepper,
        formGroup: this.form,
        formArray: null,
      });
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

  public get interpreterRequired(): FormControl {
    return this.form.get('interpreterRequired') as FormControl;
  }
}

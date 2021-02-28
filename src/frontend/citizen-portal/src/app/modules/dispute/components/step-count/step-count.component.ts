import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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
  selector: 'app-step-count',
  templateUrl: './step-count.component.html',
  styleUrls: ['./step-count.component.scss'],
})
export class StepCountComponent extends BaseDisputeFormPage implements OnInit {
  @Input() public stepper: MatStepper;
  @Input() public step: any;
  @Output() public stepSave: EventEmitter<{
    stepper: MatStepper;
    formGroup: FormGroup;
    formArray: FormArray;
  }> = new EventEmitter();

  public busy: Subscription;
  // public formGroupCount: FormGroup;
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
    this.nextBtnLabel = 'Next';
  }

  public ngOnInit() {
    this.createFormInstance();
    // this.patchForm();
    this.initForm();
    this.nextBtnLabel = 'Next';
  }

  protected createFormInstance() {
    this.form = this.disputeFormStateService.getStepCountForm(this.step.value);
    console.log('StepCountComponent form', this.form.value);
  }

  protected initForm() {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.ticket = ticket;
      this.form.patchValue(ticket);
    });
  }
  public onSubmit(): void {
    // console.log('formCounts', this.formCounts.value);
    // this.formCounts.forEach((cnt) => {
    // });
    //TODO fix this
    // let steps = this.disputeService.steps$.value;
    // const exists = steps.some((step) => step.pageName === 3);
    // if (!exists) {
    //   steps.splice(steps.length - 1, 0, {
    //     title: 'Court',
    //     value: null,
    //     pageName: 3,
    //   });
    //   this.disputeService.steps$.next(steps);
    // }
    // '({count1} and {count1} != "A") or ({count2} and {count2} != "A") or ({count3} and {count3} != "A")',
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

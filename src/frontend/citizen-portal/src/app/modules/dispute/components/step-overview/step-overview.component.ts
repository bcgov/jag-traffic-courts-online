import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Count, Dispute } from '@shared/models/dispute.model';
import { Ticket } from '@shared/models/ticket.model';

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
  public dispute: Dispute;
  public ticket: Ticket;

  public count1Form: FormGroup;
  public count2Form: FormGroup;
  public count3Form: FormGroup;
  public courtForm: FormGroup;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    private formUtilsService: FormUtilsService,
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
    // this.patchForm();

    this.count1Form = this.disputeFormStateService.stepCount1Form;
    this.count2Form = this.disputeFormStateService.stepCount2Form;
    this.count3Form = this.disputeFormStateService.stepCount3Form;
    this.courtForm = this.disputeFormStateService.stepCourtForm;

    this.dispute = this.disputeService.dispute;
    this.ticket = this.disputeService.dispute?.ticket;

    // this.disputeFormStateService.setForm(this.disputeFormStateService.json);
    this.nextBtnLabel = 'Submit';
  }

  public onBack() {
    this.stepper.previous();
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.form)) {
      this.stepSave.emit(this.stepper);
    } else {
      this.toastService.openErrorToast(
        'Your dispute has an error that needs to be corrected before you will be able to submit'
      );
    }
  }

  public get certifyCorrect(): FormControl {
    return this.form.get('certifyCorrect') as FormControl;
  }

  public get cnt1_countNo(): FormControl {
    return this.count1Form.get('countNo') as FormControl;
  }

  public get cnt1_description(): FormControl {
    return this.count1Form.get('description') as FormControl;
  }

  public get cnt1_count(): FormControl {
    return this.count1Form.get('count') as FormControl;
  }

  public get cnt1_count1A1(): FormControl {
    return this.count1Form.get('count1A1') as FormControl;
  }

  public get cnt1_count1A2(): FormControl {
    return this.count1Form.get('count1A2') as FormControl;
  }

  public get cnt1_reductionReason(): FormControl {
    return this.count1Form.get('reductionReason') as FormControl;
  }

  public get cnt1_timeReason(): FormControl {
    return this.count1Form.get('timeReason') as FormControl;
  }

  public get cnt1_count1B1(): FormControl {
    return this.count1Form.get('count1B1') as FormControl;
  }

  public get cnt1_count1B2(): FormControl {
    return this.count1Form.get('count1B2') as FormControl;
  }

  public get cnt2_countNo(): FormControl {
    return this.count2Form.get('countNo') as FormControl;
  }

  public get cnt2_description(): FormControl {
    return this.count2Form.get('description') as FormControl;
  }

  public get cnt2_count(): FormControl {
    return this.count2Form.get('count') as FormControl;
  }

  public get cnt2_count1A1(): FormControl {
    return this.count2Form.get('count1A1') as FormControl;
  }

  public get cnt2_count1A2(): FormControl {
    return this.count2Form.get('count1A2') as FormControl;
  }

  public get cnt2_reductionReason(): FormControl {
    return this.count2Form.get('reductionReason') as FormControl;
  }

  public get cnt2_timeReason(): FormControl {
    return this.count2Form.get('timeReason') as FormControl;
  }

  public get cnt2_count1B1(): FormControl {
    return this.count2Form.get('count1B1') as FormControl;
  }

  public get cnt2_count1B2(): FormControl {
    return this.count2Form.get('count1B2') as FormControl;
  }

  public get cnt3_countNo(): FormControl {
    return this.count3Form.get('countNo') as FormControl;
  }

  public get cnt3_description(): FormControl {
    return this.count3Form.get('description') as FormControl;
  }

  public get cnt3_count(): FormControl {
    return this.count3Form.get('count') as FormControl;
  }

  public get cnt3_count1A1(): FormControl {
    return this.count3Form.get('count1A1') as FormControl;
  }

  public get cnt3_count1A2(): FormControl {
    return this.count3Form.get('count1A2') as FormControl;
  }

  public get cnt3_reductionReason(): FormControl {
    return this.count3Form.get('reductionReason') as FormControl;
  }

  public get cnt3_timeReason(): FormControl {
    return this.count3Form.get('timeReason') as FormControl;
  }

  public get cnt3_count1B1(): FormControl {
    return this.count3Form.get('count1B1') as FormControl;
  }

  public get cnt3_count1B2(): FormControl {
    return this.count3Form.get('count1B2') as FormControl;
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

  public get callWitness(): FormControl {
    return this.courtForm.get('callWitness') as FormControl;
  }
}

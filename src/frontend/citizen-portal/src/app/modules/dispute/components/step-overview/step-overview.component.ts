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

  private count1Form: FormGroup;
  private count2Form: FormGroup;
  private count3Form: FormGroup;
  private courtForm: FormGroup;

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

  public get count1Summary(): string {
    return this.disputeFormStateService.getCountSummary(1);
  }

  public get count2Summary(): string {
    return this.disputeFormStateService.getCountSummary(2);
  }

  public get count3Summary(): string {
    return this.disputeFormStateService.getCountSummary(3);
  }

  public get count1Data(): Count {
    return this.count1Form.value;
  }

  public get count2Data(): Count {
    return this.count2Form.value;
  }

  public get count3Data(): Count {
    return this.count3Form.value;
  }

  public get court(): any {
    return this.courtForm.value;
  }
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
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
import { Ticket } from '@shared/models/ticket.model';
import moment from 'moment';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-step-review-ticket',
  templateUrl: './step-review-ticket.component.html',
  styleUrls: ['./step-review-ticket.component.scss'],
})
export class StepReviewTicketComponent
  extends BaseDisputeFormPage
  implements OnInit {
  @Input() public stepper: MatStepper;
  @Output() public stepSave: EventEmitter<{
    stepper: MatStepper;
    formGroup: FormGroup;
    formArray: FormArray;
  }> = new EventEmitter();

  public busy: Subscription;
  public nextBtnLabel: string;
  public ticket: Ticket;
  public isSubmitted = false;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    private toastService: ToastService,
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
    this.form = this.disputeFormStateService.stepReviewForm;
    console.log('StepReviewTicketComponent form', this.form.value);
  }

  protected initForm() {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.ticket = ticket;
      this.form.patchValue(ticket);
    });

    // if (!this.enrolmentService.enrolment) {
    //   this.getUser$()
    //     .subscribe((enrollee: Enrollee) => {
    //       this.dateOfBirth.enable();
    //       this.form.patchValue(enrollee);
    //     });
    // }
  }

  public onSubmit(): void {
    this.isSubmitted = true;
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

  public get infoCorrect(): FormControl {
    return this.form.get('infoCorrect') as FormControl;
  }
}

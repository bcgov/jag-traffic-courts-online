import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  OnChanges,
  SimpleChanges,
  ChangeDetectionStrategy,
} from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { TranslateService } from '@ngx-translate/core';
import { ticketTypes } from '@shared/enums/ticket-type.enum';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';
import { BaseDisputeFormPage } from 'app/components/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-step-count',
  templateUrl: './step-count.component.html',
  styleUrls: ['./step-count.component.scss'],
})
export class StepCountComponent extends BaseDisputeFormPage implements OnInit,OnChanges {
  @Input() public stepper: MatStepper;
  @Input() public stepControl: FormGroup;
  @Input() public showDoNothingOption = true;
  @Input() public ticketName: string;
  @Input() public isShowCheckbox: boolean;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();
  @Output() public stepCancel: EventEmitter<MatStepper> = new EventEmitter();
  @Input() public isSelectedChekcError:String;
  public errorMsg:string = "display here";
  public defaultLanguage: string;
  public previousButtonIcon = 'keyboard_arrow_left';
  public previousButtonKey = 'stepper.back';
  public saveButtonKey = 'stepper.next';
  public ticketTypesEnum = ticketTypes;

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
    private translateService: TranslateService,
    private ticketTypePipe: TicketTypePipe,
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
    this.form = this.stepControl;
    this.patchForm();
    this.ticketName = this.ticketTypePipe.transform(this.ticketName?.charAt(0));
  }
  
  ngOnChanges(changes: SimpleChanges) {
    if(changes.isSelectedChekcError && changes.isSelectedChekcError.currentValue){
      this.errorMsg = changes.isSelectedChekcError.currentValue
    }
    console.log('-----------------------',changes);
  }
  public onSubmit(): void {
    this.stepSave.emit(this.stepper);
    // TODO: As the reduction appear in court functionality is moved to additional,
    // the validation needs to be done there
    // if (this.formUtilsService.checkValidity(this.form)) {

    // } else {
    //   this.utilsService.scrollToErrorSection();
    // }
  }

  public onBack() {
    this.stepper.previous();
  }

  public get firstOffence(): FormControl {
    return this.form.get('_firstOffence') as FormControl;
  }

  public get allowApplyToAllCounts(): FormControl {
    return this.form.get('_allowApplyToAllCounts') as FormControl;
  }

  public get applyToAllCounts(): FormControl {
    return this.form.get('_applyToAllCounts') as FormControl;
  }

  public get offenceAgreementStatus(): FormControl {
    return this.form.get('offenceAgreementStatus') as FormControl;
  }

  public get reductionAppearInCourt(): FormControl {
    return this.form.get('reductionAppearInCourt') as FormControl;
  }
  public get reductionAppearInCourtDo(): FormControl {
    return this.form.get('reductionAppearInCourtDo') as FormControl;
  }
  public get reductionAppearInCourtDoNot(): FormControl {
    return this.form.get('reductionAppearInCourtDoNot') as FormControl;
  }

  public get disputeAppearInCourtDo(): FormControl {
    return this.form.get('disputeAppearInCourtDo') as FormControl;
  }
  public get disputeAppearInCourtDoNot(): FormControl {
    return this.form.get('disputeAppearInCourtDoNot') as FormControl;
  }
  public get offenceDescription(): FormControl {
    return this.form.get('offenceDescription') as FormControl;
  }

  public get offenceNumber(): FormControl {
    return this.form.get('offenceNumber') as FormControl;
  }

  public get ticketedAmount(): FormControl {
    return this.form.get('ticketedAmount') as FormControl;
  }

  public get amountDue(): FormControl {
    return this.form.get('amountDue') as FormControl;
  }

  public get discountAmount(): FormControl {
    return this.form.get('discountAmount') as FormControl;
  }

  public get discountDueDate(): FormControl {
    return this.form.get('discountDueDate') as FormControl;
  }

  public get within30days(): FormControl {
    return this.form.get('_within30days') as FormControl;
  }
}

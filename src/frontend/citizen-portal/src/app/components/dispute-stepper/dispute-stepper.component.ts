import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { AppRoutes } from 'app/app.routes';
import { BaseDisputeFormPage } from 'app/components/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dispute-stepper',
  templateUrl: './dispute-stepper.component.html',
  styleUrls: ['./dispute-stepper.component.scss'],
})
export class DisputeStepperComponent
  extends BaseDisputeFormPage
  implements OnInit {
  public busy: Subscription;
  @ViewChild(MatStepper)
  private stepper: MatStepper;

  public overviewTicket: TicketDisputeView;

  public disputantForm: FormGroup;
  public offence1Form: FormGroup;
  public offence2Form: FormGroup;
  public offence3Form: FormGroup;
  public additionalForm: FormGroup;
  public overviewForm: FormGroup;
  public ticketName: string;

  public offence1Exists: boolean;
  public offence2Exists: boolean;
  public offence3Exists: boolean;
  public isCheckBoxSelected: any = {
    
  };
  public isErrorCheckMsg1: string;
  public countDataList:any;
  public offenceCount:any = 0;
  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService,
    protected violationTicketService: ViolationTicketService,
    private dialog: MatDialog,
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

    this.offence1Exists = false;
    this.offence2Exists = false;
    this.offence3Exists = false;
  }

  public ngOnInit(): void {
    
    this.isCheckBoxSelected = {
      reductionAppearInCourt:[],
      reductionAppearInCourtDoNot:[],
      disputeAppearInCourt:[]

    };
    this.violationTicketService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      this.disputeFormStateService.reset();
      this.patchForm();

      const formsList = this.disputeFormStateService.forms;
      [
        this.disputantForm,
        this.offence1Form,
        this.offence2Form,
        this.offence3Form,
        this.additionalForm,
        this.overviewForm,
      ] = formsList as FormGroup[];

      const currentTicket = this.violationTicketService.ticket;
      this.offenceCount =0
      if (currentTicket) {
        // this.disputantForm.patchValue(currentTicket.);
        // this.additionalForm.patchValue(currentTicket.additional);

        this.disputeService.ticket.offences.forEach((offence) => {
          if (offence.offenceNumber === 1) {
            this.offence1Exists = true;
            this.offenceCount+=1
            this.offence1Form.patchValue(offence);
          } else if (offence.offenceNumber === 2) {
            this.offence2Exists = true;
            this.offenceCount+=1
            this.offence2Form.patchValue(offence);
          } else if (offence.offenceNumber === 3) {
            this.offence3Exists = true;
            this.offenceCount+=1
            this.offence3Form.patchValue(offence);
          }
        });
      }
    });
    this.ticketName = this.disputeService?.ticket?.violationTicketNumber;
  }

  public onStepCancel(): void {
    const ticket = this.disputeService.ticket;
    const params = {
      ticketNumber: ticket.violationTicketNumber,
      time: ticket.violationTime,
    };

    this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
      queryParams: params,
    });
  }
public getCountData(newObj):void{
  this.countDataList = newObj;
}
  public onStepSave(stepper: MatStepper): void {
    let stepsObjects ={
      2:'offence1Form',
      3:'offence2Form',
      4:'offence3Form',
    }
  
    this.logger.info('DisputeStepperComponent::onStepSave Dispute Data:', this.disputeFormStateService.json);
    const numberOfSteps = stepper.steps.length;
    const currentStep = stepper.selectedIndex + 1;
    if(currentStep ==1){
      this.offence1Form.patchValue({
        _allowApplyToAllCounts: false
      });
    }
    if(this[stepsObjects[currentStep]] && this[stepsObjects[currentStep]].value._skip){
      this[stepsObjects[currentStep]].patchValue({
        reductionAppearInCourt : false,
        reductionAppearInCourtDoNot:false,
        disputeAppearInCourt : false,
        disputeAppearInCourtDoNot: false,
        offenceAgreementStatus: null,
        _applyToAllCounts:false
      });

    }
    
    if(currentStep ==2 && this[stepsObjects[currentStep]] && this[stepsObjects[currentStep]].value._allowApplyToAllCounts){
      let applyAll = this[stepsObjects[currentStep]].value._allowApplyToAllCounts
      let listArray = [2,3,4].filter((ele)=> ele != currentStep)
      
      listArray.map((ele)=>{

        this[stepsObjects[ele]].patchValue({
          reductionAppearInCourt : this[stepsObjects[currentStep]].value.reductionAppearInCourt,
          reductionAppearInCourtDoNot:this[stepsObjects[currentStep]].value.reductionAppearInCourtDoNot,
          disputeAppearInCourt : this[stepsObjects[currentStep]].value.disputeAppearInCourt,
          disputeAppearInCourtDoNot:this[stepsObjects[currentStep]].value.disputeAppearInCourtDoNot,
          offenceAgreementStatus: this[stepsObjects[currentStep]].value.offenceAgreementStatus,
          _applyToAllCounts:this[stepsObjects[currentStep]].value.offenceAgreementStatus
        });
      })
    
    } 
    if(currentStep ==2 && this[stepsObjects[currentStep]] && !this[stepsObjects[currentStep]].value._allowApplyToAllCounts){
      let applyAll = this[stepsObjects[currentStep]].value._allowApplyToAllCounts
      let listArray = [2,3,4].filter((ele)=> ele != currentStep)
      
      listArray.map((ele)=>{

        this[stepsObjects[ele]].patchValue({
          reductionAppearInCourt : false,
          reductionAppearInCourtDoNot:false,
          disputeAppearInCourt : false,
          disputeAppearInCourtDoNot:false,
          offenceAgreementStatus: false,
          // _applyToAllCounts:this[stepsObjects[currentStep]].value.offenceAgreementStatus
        });
      })
    }
    if(currentStep ==3 && this[stepsObjects[currentStep]] && this[stepsObjects[currentStep]].value._applyToAllCounts ){
      let applyAll = this[stepsObjects[currentStep]].value._allowApplyToAllCounts
      let listArray = [3,4].filter((ele)=> ele != currentStep)
      
      listArray.map((ele)=>{

        this[stepsObjects[ele]].patchValue({
          reductionAppearInCourt : this[stepsObjects[currentStep]].value.reductionAppearInCourt,
          reductionAppearInCourtDoNot:this[stepsObjects[currentStep]].value.reductionAppearInCourtDoNot,
          disputeAppearInCourt : this[stepsObjects[currentStep]].value.disputeAppearInCourt,
          disputeAppearInCourtDoNot:this[stepsObjects[currentStep]].value.disputeAppearInCourtDoNot,
          offenceAgreementStatus: this[stepsObjects[currentStep]].value.offenceAgreementStatus,
          _applyToAllCounts:this[stepsObjects[currentStep]].value._applyToAllCounts
        });
      })
    
    }

      if(currentStep ==3 && this[stepsObjects[currentStep]] && !this[stepsObjects[currentStep]].value._applyToAllCounts ){
      let applyAll = this[stepsObjects[currentStep]].value._allowApplyToAllCounts
      let listArray = [3,4].filter((ele)=> ele != currentStep)
      
      listArray.map((ele)=>{

        this[stepsObjects[ele]].patchValue({
          reductionAppearInCourt : false,
          reductionAppearInCourtDoNot:false,
          disputeAppearInCourt : false,
          disputeAppearInCourtDoNot:false,
          offenceAgreementStatus: false,
          // _applyToAllCounts:this[stepsObjects[currentStep]].value._applyToAllCounts
        });
      })
    
    }
    this.addAddition()
    // if(currentStep ==2 && this.offence1Form.value.offenceAgreementStatus == "REDUCTION" && !this.offence1Form.value.reductionAppearInCourt && !this.offence1Form.value.reductionAppearInCourt){
    //   this.isErrorCheckMsg1 = 'select atleast one checkbox';
    //   return;
    // }else{
    //   this.isErrorCheckMsg1 = "";
    // }
    // if(currentStep ==3 && this.offence1Form.value.offenceAgreementStatus == "REDUCTION" && !this.offence2Form.value.reductionAppearInCourt && !this.offence2Form.value.reductionAppearInCourt){
    //   this.isErrorCheckMsg1 = 'select atleast one checkbox';
    //   return;
    // }
    if (currentStep === 3){
      // this.isCheckBoxSelected[currentStep] = {
      //   reductionAppearInCourt : this.offence2Form.value.reductionAppearInCourt,
      //   reductionAppearInCourtDoNot: this.offence2Form.value.reductionAppearInCourtDoNot,
      //   count:2
      // };
      // this.isCheckBoxSelected= this.offence1Form.value.reductionAppearInCourt
    }
    if (currentStep === 2){
     
      // this.isCheckBoxSelected[currentStep] = {
      //   reductionAppearInCourt : this.offence1Form.value.reductionAppearInCourt,
      //   reductionAppearInCourtDoNot: this.offence1Form.value.reductionAppearInCourtDoNot,
      //   count:1
      // };
    
    }
    // let addForm = this.disputeFormStateService.stepAdditionalForm;
    // let add = addForm.get('countData') as FormArray;
    // add.push(this.createItem());
    // on the last step
    if (numberOfSteps === currentStep) {
      this.submitDispute();
    } else {
      this.saveStep(stepper);
    }
  }

  addAddition(){
    this.isCheckBoxSelected = {
      reductionAppearInCourt:[],
      reductionAppearInCourtDoNot:[],
      disputeAppearInCourt:[]

    };
    let stepsObjects ={
      2:'offence1Form',
      3:'offence2Form',
      4:'offence3Form',
    };

    [2,3,4].map((currentStep)=>{
      if(this[stepsObjects[currentStep]] && this[stepsObjects[currentStep]].value.reductionAppearInCourt && this[stepsObjects[currentStep]].value.offenceAgreementStatus == 'REDUCTION'){

        this.isCheckBoxSelected['reductionAppearInCourt'].push(currentStep-1);

    } else {

    }
    if(this[stepsObjects[currentStep]] && this[stepsObjects[currentStep]].value.reductionAppearInCourtDoNot && this[stepsObjects[currentStep]].value.offenceAgreementStatus == 'REDUCTION'){

      this.isCheckBoxSelected['reductionAppearInCourtDoNot'].push(currentStep-1);
      
  }
  if(this[stepsObjects[currentStep]] && this[stepsObjects[currentStep]].value.disputeAppearInCourt && this[stepsObjects[currentStep]].value.offenceAgreementStatus == 'DISPUTE'){

    this.isCheckBoxSelected['disputeAppearInCourt'].push(currentStep-1);
    
}
    })
  }
  createItem(): FormGroup {
    return this.formBuilder.group({
      name:''
    });
  }
  /**
   * @description
   * Save the data on the current step
   */
  private saveStep(stepper: MatStepper): void {
    stepper.next();
  }

  /**
   * @description
   * Submit the dispute
   */
  private submitDispute(): void {
    const data: DialogOptions = {
      titleKey: 'Submit request',
      messageKey:
        'When your request is submitted for adjudication, it can no longer be updated. Are you ready to submit your request?',
      actionTextKey: 'Submit request',
      cancelTextKey: 'Cancel',
      icon: null,
    };

    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          const payload = this.disputeFormStateService.jsonTicketDispute;
          payload.violationTicketNumber = this.ticket.violationTicketNumber;
          payload.violationTime = this.ticket.violationTime;

          this.busy = this.disputeResource
            .createTicketDispute(payload)
            .subscribe((newDisputeTicket: TicketDisputeView) => {
              // newDisputeTicket.additional = {
              //   _isCourtRequired:true,
              //   _isReductionRequired: true,
              //   _isReductionNotInCourt: true
              // }
              newDisputeTicket.countList = this.countDataList;
              this.disputeService.ticket$.next(newDisputeTicket);

              this.router.navigate([
                AppRoutes.disputePath(AppRoutes.SUBMIT_SUCCESS),
              ],{
                queryParams: this.countDataList,
              });
            });
        }
      });
  }

  public onSelectionChange(event: StepperSelectionEvent): void {
    this.logger.info('DisputeStepperComponent::onSelectionChange Dispute Data:', this.disputeFormStateService.json);

    const stepIndex = event.selectedIndex;
    const stepId = this.stepper._getStepLabelId(stepIndex);
    const stepElement = document.getElementById(stepId);
    if (stepElement) {
      setTimeout(() => {
        stepElement.scrollIntoView({
          block: 'start',
          inline: 'nearest',
          behavior: 'smooth',
        });
      }, 250);
    }

    const numberOfSteps = this.stepper.steps.length;
    const currentStep = event.selectedIndex + 1;
    const previousStep = event.previouslySelectedIndex + 1;
    this.logger.info('DisputeStepperComponent::onSelectionChange currentStep', currentStep, 'previousStep', previousStep);

    if (previousStep === 2) {
      this.updateOffenceForms();
    }

    if (currentStep >= (numberOfSteps - 1)) {
      this.setCourtRequired();
    }

    this.logger.info('DisputeStepperComponent::onSelectionChange After:', this.disputeFormStateService.json);
    this.overviewTicket = this.disputeFormStateService.jsonTicketDispute;
  }

  /**
   * @description
   * Determine if the current step is 'Additional Information' (2nd last step) or the last step, update the courtRequired flag
   */
  private setCourtRequired(): void {
    const offenceForms = this.disputeFormStateService.offenceForms;
    let courtRequired = false;
    let reductionRequired = false;
    let isReductionNotInCourt = false;

    offenceForms.forEach((form: AbstractControl) => {
      const offenceNumber = form.get('offenceNumber') as FormControl;
      if (offenceNumber.value) {
        const offenceAgreementStatus = form.get('offenceAgreementStatus') as FormControl;
        const reductionAppearInCourt = form.get('reductionAppearInCourt') as FormControl;

        if (offenceAgreementStatus.value === 'DISPUTE') {
          courtRequired = true;
        } else if (offenceAgreementStatus.value === 'REDUCTION') {
          reductionRequired = true;
          if (reductionAppearInCourt.value) {
            courtRequired = true;
          } else {
            isReductionNotInCourt = true;
          }
        }
      }
    });

    this.logger.log('onSelectionChange courtRequired', courtRequired);
    this.logger.log('onSelectionChange reductionRequired', reductionRequired);
    this.logger.log('onSelectionChange isReductionNotInCourt', isReductionNotInCourt);

    const additionalForm = this.disputeFormStateService.stepAdditionalForm;

    const isCourtRequiredControl = additionalForm.get('_isCourtRequired') as FormControl;
    isCourtRequiredControl.setValue(courtRequired);

    const isReductionRequiredControl = additionalForm.get('_isReductionRequired') as FormControl;
    isReductionRequiredControl.setValue(reductionRequired);

    const isReductionNotInCourtControl = additionalForm.get('_isReductionNotInCourt') as FormControl;
    isReductionNotInCourtControl.setValue(isReductionNotInCourt);
  }

  /**
   * @description
   * After leaving the FIRST offence screen, if 'applyToAllCounts' is selected, update the appropriate other values in the other counts.
   */
  private updateOffenceForms(): void {
    this.logger.info('DisputeStepperComponent::updateOffenceForms');
    const offenceForms = this.disputeFormStateService.offenceForms;

    let applyToAllCounts = false;
    let offenceAgreementStatus;
    let reductionAppearInCourt;

    offenceForms.forEach((form: AbstractControl) => {
      const offenceNumber = form.get('offenceNumber') as FormControl;
      const firstOffence = form.get('_firstOffence') as FormControl;

      if (offenceNumber.value) {
        const applyToAllCountsControl = form.get('_applyToAllCounts') as FormControl;
        const offenceAgreementStatusControl = form.get('offenceAgreementStatus') as FormControl;
        const reductionAppearInCourtControl = form.get('reductionAppearInCourt') as FormControl;

        // cleanup bad state
        if (offenceAgreementStatusControl.value !== 'REDUCTION') {
          reductionAppearInCourtControl.setValue(null);
        }

        // Get the data from the first offence to copy to the others
        if (firstOffence.value) {
          applyToAllCounts = applyToAllCountsControl.value;

          // cleanup bad state
          if (offenceAgreementStatusControl.value !== 'DISPUTE' && offenceAgreementStatusControl.value !== 'REDUCTION') {
            applyToAllCounts = false;
            applyToAllCountsControl.setValue(applyToAllCounts);
          }

          offenceAgreementStatus = offenceAgreementStatusControl.value;
          reductionAppearInCourt = reductionAppearInCourtControl.value;

        } else {
          if (applyToAllCounts) {
            offenceAgreementStatusControl.setValue(offenceAgreementStatus);
            reductionAppearInCourtControl.setValue(reductionAppearInCourt);
          }
        }
      }
    });
  }
}

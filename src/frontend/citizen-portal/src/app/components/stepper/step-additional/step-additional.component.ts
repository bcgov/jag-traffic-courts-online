import { Component,
   EventEmitter,
   Input,
  OnInit,
   Output,
   OnChanges,
   SimpleChanges,
   ChangeDetectionStrategy,
 } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { Config } from '@config/config.model';
import { ConfigService } from '@config/config.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { BaseDisputeFormPage } from 'app/components/classes/BaseDisputeFormPage';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-step-additional',
  templateUrl: './step-additional.component.html',
  styleUrls: ['./step-additional.component.scss'],
})
export class StepAdditionalComponent extends BaseDisputeFormPage implements OnInit,OnChanges {
  @Input() public stepper: MatStepper;
  @Input() public isShowCheckbox: any;
  @Output() public stepSave: EventEmitter<MatStepper> = new EventEmitter();
  @Output() public countEmit: EventEmitter<any> = new EventEmitter();
  

  public previousButtonIcon = 'keyboard_arrow_left';
  public previousButtonKey = 'stepper.back';
  public saveButtonKey = 'stepper.next';

  public languages: Config<string>[];
  public countFormList:any;
  public countFormList2:any;
  public dispute={

  };
  public newObj={
      count1:'',
      count: null,
      reductionAppearInCourt :'',
      reductionAppearInCourtDoNot:''
  };
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
    super(
      route,
      router,
      formBuilder,
      disputeService,
      disputeResource,
      disputeFormStateService
    );

    this.languages = this.configService.languages;
    this.languages = [{name:'french',code:'fr'},{name:'spanish',code:'sp'},{name:'arabic',code:'Ar'}];
  }

  public ngOnInit() {
    this.form = this.disputeFormStateService.stepAdditionalForm;
    this.customWitnessOption = this.form.getRawValue().numberOfWitnesses >= 6;
    this.form.patchValue({ numberOfWitnesses: this.form.getRawValue().numberOfWitnesses });
    this.patchForm();
  }
  ngOnChanges(changes: SimpleChanges) {
    let array = [...this.isShowCheckbox.reductionAppearInCourt,...this.isShowCheckbox.reductionAppearInCourtDoNot]
    this.newObj.count = [...new Set(array)]
    this.newObj.count1 = this.isShowCheckbox.disputeAppearInCourt
   
    // if(changes.isShowCheckbox && changes.isShowCheckbox.currentValue && changes.isShowCheckbox.currentValue[2]){
    //   this.countFormList = this.form.get('countData') as FormArray;
    //  if(!this.countFormList){
    //   this.countFormList =[];
     
    //  }
    //  this.countFormList.push(this.createItem());
     
    // }
    // if(changes.isShowCheckbox && changes.isShowCheckbox.currentValue && changes.isShowCheckbox.currentValue[3]){
    //   this.countFormList2 = this.form.get('countData2') as FormArray;
    //   if(!this.countFormList2){
    //     this.countFormList2 =[];
    //   }
    //   this.countFormList2.push(this.createItem());
    // }
   
  }
  createItem(): FormGroup {
    return this.formBuilder.group({
      name:''
    });
  }
  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.form)) {
      debugger 
      let obj = {
        newObj : this.newObj,
        dispute:this.dispute
      }
      this.countEmit.emit(obj);
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

  public get interpreterLanguage(): FormControl {
    return this.form.get('interpreterLanguage') as FormControl;
  }

  public get interpreterRequired(): FormControl {
    return this.form.get('interpreterRequired') as FormControl;
  }

  public get witnessPresent(): FormControl {
    return this.form.get('witnessPresent') as FormControl;
  }

  public get isCourtRequired(): FormControl {
    return this.form.get('_isCourtRequired') as FormControl;
  }

  public get isReductionRequired(): FormControl {
    return this.form.get('_isReductionRequired') as FormControl;
  }

  public get isReductionNotInCourt(): FormControl {
    return this.form.get('_isReductionNotInCourt') as FormControl;
  }

  public get numberOfWitnesses(): FormControl {
    return this.form.get('numberOfWitnesses') as FormControl;
  }

  public get requestReduction(): FormControl {
    return this.form.get('requestReduction') as FormControl;
  }

  public get requestMoreTime(): FormControl {
    return this.form.get('requestMoreTime') as FormControl;
  }

  public get reductionReason(): FormControl {
    return this.form.get('reductionReason') as FormControl;
  }

  public get moreTimeReason(): FormControl {
    return this.form.get('moreTimeReason') as FormControl;
  }
  public get lawyerPresent(): FormControl {
    return this.form.get('lawyerPresent') as FormControl;
  }
  public get countData() : FormArray {
    return this.form.get('countData') as FormArray;
  }
  public get countData2() : FormArray {
    return this.form.get('countData2') as FormArray;
  }
  newSkill(value1, value2): FormGroup {
    return this.formBuilder.group({
      reductionAppearInCourt: value1,
      reductionAppearInCourtDoNot: value2,
    });
  }
}

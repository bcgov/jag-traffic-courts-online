import { Component, EventEmitter, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JJDispute } from '../../../services/jj-dispute.service';
import { JJDisputedCount, JJDisputedCountAppearInCourt, JJDisputedCountIncludesSurcharge, JJDisputedCountLatestPlea, JJDisputedCountPlea, JJDisputedCountRequestReduction, JJDisputedCountRequestTimeToPay, JJDisputedCountRoPAbatement, JJDisputedCountRoPDismissed, JJDisputedCountRoPFinding, JJDisputedCountRoPForWantOfProsecution, JJDisputedCountRoPJailIntermittent, JJDisputedCountRoPWithdrawn, JJDisputeHearingType, JJDisputeStatus } from 'app/api';
import { MatLegacyRadioChange as MatRadioChange } from '@angular/material/legacy-radio';
import { LookupsService, Statute } from 'app/services/lookups.service';
import { CustomDatePipe } from '@shared/pipes/custom-date.pipe';

@Component({
  selector: 'app-jj-count',
  templateUrl: './jj-count.component.html',
  styleUrls: ['./jj-count.component.scss']
})
export class JJCountComponent implements OnInit, OnChanges {
  @Input() jjDisputeInfo: JJDispute;
  @Input() count: number;
  @Input() type: string;
  @Input() isViewOnly: boolean = false;
  /** Admin Staff Support edit mode */
  @Input() isSSEditMode: boolean = false;
  @Input() jjDisputedCount: JJDisputedCount;
  @Output() jjDisputedCountUpdate: EventEmitter<JJDisputedCount> = new EventEmitter<JJDisputedCount>();

  // Enums
  IncludesSurcharge = JJDisputedCountIncludesSurcharge;
  Plea = JJDisputedCountPlea;
  RequestReduction = JJDisputedCountRequestReduction;
  RequestTimeToPay = JJDisputedCountRequestTimeToPay;
  AppearInCourt = JJDisputedCountAppearInCourt;
  HearingType = JJDisputeHearingType;
  JailIntermittent = JJDisputedCountRoPJailIntermittent;
  Abatement = JJDisputedCountRoPAbatement;
  Withdrawn = JJDisputedCountRoPWithdrawn;
  ForWantOfProsecution = JJDisputedCountRoPForWantOfProsecution;
  Dismissed = JJDisputedCountRoPDismissed;
  Finding = JJDisputedCountRoPFinding;
  LatestPlea = JJDisputedCountLatestPlea;

  // Variables
  todayDate: Date = new Date();
  form: FormGroup;
  countForm: FormGroup = this.formBuilder.group({
    appearInCourt: [null],
    requestReduction: [null],
    requestTimeToPay: [null],
    latestPlea: [null],
    lesserOrGreaterAmount: [null],
    includesSurcharge: [null],
    revisedDueDate: [null],
    comments: [null, Validators.maxLength(4000)],
  });
  countRoPForm: FormGroup = this.formBuilder.group({
    finding: [null],
    ssProbationDuration: [null, Validators.maxLength(500)],
    ssProbationConditions: [null, Validators.maxLength(500)],
    jailDuration: [null, Validators.maxLength(500)],
    _jailIntermittent: [false],
    probationDuration: [null, Validators.maxLength(500)],
    probationConditions: [null, Validators.maxLength(1000)],
    drivingProhibition: [null, Validators.maxLength(500)],
    drivingProhibitionMVASection: [null, Validators.maxLength(240)],
    _dismissed: [false],
    _forWantOfProsecution: [false],
    _withdrawn: [false],
    _abatement: [false],
    stayOfProceedingsBy: [null, Validators.maxLength(500)],
    other: [null, Validators.maxLength(500)]
  });
  timeToPay: string = "";
  fineReduction: string = "";
  inclSurcharge: string = "";
  showDateHint: boolean = true;
  lesserOrGreaterAmount: number = 0;
  surcharge: number = 0;
  lesserDescriptionFilteredStatutes: Statute[];
  drivingProhibitionFilteredStatutes: Statute[];
  DisputeStatus = JJDisputeStatus;

  constructor(
    private lookupsService: LookupsService,
    private formBuilder: FormBuilder,
    private datePipe: CustomDatePipe
  ) {
  }

  ngOnInit(): void {
    this.initForm();
    this.initFormData();
    this.initListeners();
  }

  private initForm() {
    this.form = this.formBuilder.group({
        totalFineAmount: [null, [Validators.max(9999), Validators.min(0)]],
        lesserOrGreaterAmount: [null, [Validators.max(9999), Validators.min(0)]],
        revisedDueDate: [null],
        comments: [{ value: null, disabled: !this.jjDisputedCount }],
        latestPlea: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
        latestPleaUpdateTs: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
        jjDisputedCountRoP: this.formBuilder.group({
          finding: [{ value: null, disabled: !this.jjDisputedCount }],
          lesserDescription: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          id: [{ value: null }],
          ssProbationCheckbox: [{ value: false, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          ssProbationDuration: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          ssProbationConditions: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          jailCheckbox: [{ value: false, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          jailDuration: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          jailIntermittent: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          probationCheckbox: [{ value: false, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          probationDuration: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          probationConditions: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          drivingProhibitionCheckbox: [{ value: false, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          drivingProhibition: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          drivingProhibitionMVASection: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          dismissed: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          forWantOfProsecution: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          withdrawn: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          abatement: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          stayOfProceedingsByCheckbox: [{ value: false, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          stayOfProceedingsBy: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          otherCheckbox: [{ value: false, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
          other: [{ value: null, disabled: !this.jjDisputedCount || this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons }],
        })
      });
  }

  private initFormData(): void {
    // initialize if no value
    if (this.jjDisputedCount && this.form) {
      if (this.jjDisputedCount.totalFineAmount != null && this.jjDisputedCount.totalFineAmount > 0) {
        this.surcharge = this.jjDisputedCount.totalFineAmount / 1.15 * 0.15;
      }

      if (this.isViewOnly &&
        (this.jjDisputedCount.jjDisputedCountRoP.finding === JJDisputedCountRoPFinding.NotGuilty
          || this.jjDisputedCount.jjDisputedCountRoP.finding === JJDisputedCountRoPFinding.Cancelled)) {
        this.jjDisputedCount.ticketedFineAmount = null;
        this.jjDisputedCount.lesserOrGreaterAmount = null;
        this.jjDisputedCount.totalFineAmount = null;
        this.jjDisputedCount.dueDate = null;
        this.jjDisputedCount.revisedDueDate = null;
      }

      // initialize form, radio buttons
      this.form.patchValue(this.jjDisputedCount);
      this.jjDisputedCount.lesserOrGreaterAmount === null ?? this.form.controls.lesserOrGreaterAmount.setValue(this.jjDisputedCount.ticketedFineAmount);
      this.inclSurcharge = this.jjDisputedCount ? (this.jjDisputedCount.includesSurcharge == this.IncludesSurcharge.Y ? "yes" : 
        (this.jjDisputedCount.includesSurcharge == this.IncludesSurcharge.N ? "no" : "")) : "";
      this.fineReduction = this.jjDisputedCount ? (this.jjDisputedCount.totalFineAmount || this.jjDisputedCount.lesserOrGreaterAmount ? 
        (this.jjDisputedCount.lesserOrGreaterAmount !== null && this.jjDisputedCount.lesserOrGreaterAmount != this.jjDisputedCount.ticketedFineAmount ? "yes" : "no") : "") : "";
      this.timeToPay = this.jjDisputedCount ? (this.jjDisputedCount.revisedDueDate ? 
        (this.jjDisputedCount.dueDate != this.jjDisputedCount.revisedDueDate ? "yes" : "no") : "") : "";
      this.updateInclSurcharge(this.inclSurcharge);

      // TCVP-2467
      if (!this.isViewOnly &&
        (this.jjDisputedCount.jjDisputedCountRoP.finding === JJDisputedCountRoPFinding.NotGuilty
          || this.jjDisputedCount.jjDisputedCountRoP.finding === JJDisputedCountRoPFinding.Cancelled)) {
        this.form.controls.lesserOrGreaterAmount.setValue(null);
        this.form.controls.totalFineAmount.setValue(null);
        this.form.controls.revisedDueDate.setValue(null);
        this.jjDisputedCount.lesserOrGreaterAmount = null;
        this.jjDisputedCount.revisedDueDate = null;
        this.jjDisputedCount.totalFineAmount = null;
      }

      // Make sure the date hint label is shown if there is a revised date/time
      this.showDateHint = this.timeToPay === "yes";

      if (this.jjDisputeInfo.hearingType === this.HearingType.CourtAppearance) {
        // Finding
        if (this.jjDisputedCount?.jjDisputedCountRoP?.finding !== this.Finding.GuiltyLesser) {
          this.form.get('jjDisputedCountRoP')?.get('lesserDescription')?.disable();
        }

        // suspended sentence probation fields
        if (this.jjDisputedCount?.jjDisputedCountRoP?.ssProbationDuration || this.jjDisputedCount?.jjDisputedCountRoP?.ssProbationConditions) {
          this.form.get('jjDisputedCountRoP').get('ssProbationCheckbox').setValue(true);
        } else {
          this.form.get('jjDisputedCountRoP').get('ssProbationDuration').disable();
          this.form.get('jjDisputedCountRoP').get('ssProbationConditions').disable();
        }

        // Jail fields
        if (this.jjDisputedCount?.jjDisputedCountRoP?.jailDuration || this.jjDisputedCount?.jjDisputedCountRoP?.jailIntermittent === this.JailIntermittent.Y) {
          this.form.get('jjDisputedCountRoP').get('jailCheckbox').setValue(true);
        } else {
          this.form.get('jjDisputedCountRoP').get('jailDuration').disable();
          this.form.get('jjDisputedCountRoP').get('jailIntermittent').disable();
        }

        // Probation fields
        if (this.jjDisputedCount?.jjDisputedCountRoP?.probationDuration || this.jjDisputedCount?.jjDisputedCountRoP?.probationConditions) {
          this.form.get('jjDisputedCountRoP').get('probationCheckbox').setValue(true);
        } else {
          this.form.get('jjDisputedCountRoP').get('probationDuration').disable();
          this.form.get('jjDisputedCountRoP').get('probationConditions').disable();
        }

        // Driving Prohibition fields
        if (this.jjDisputedCount?.jjDisputedCountRoP?.drivingProhibition || this.jjDisputedCount?.jjDisputedCountRoP?.drivingProhibitionMVASection) {
          this.form.get('jjDisputedCountRoP').get('drivingProhibitionCheckbox').setValue(true);
        } else {
          this.form.get('jjDisputedCountRoP').get('drivingProhibition').disable();
          this.form.get('jjDisputedCountRoP').get('drivingProhibitionMVASection').disable();
        }

        // Stay of Proceedings fields
        if (this.jjDisputedCount?.jjDisputedCountRoP?.stayOfProceedingsBy) {
          this.form.get('jjDisputedCountRoP').get('stayOfProceedingsByCheckbox').setValue(true);
        } else {
          this.form.get('jjDisputedCountRoP').get('stayOfProceedingsBy').disable();
        }

        // Other fields
        if (this.jjDisputedCount?.jjDisputedCountRoP?.other) {
          this.form.get('jjDisputedCountRoP').get('otherCheckbox').setValue(true);
        } else {
          this.form.get('jjDisputedCountRoP').get('other').disable();
        }

        // Latest Plea
        if (this.jjDisputedCount?.latestPlea) {
          this.bindLatestPlea(this.jjDisputedCount.latestPlea);
        }

        // Latest Plea UpdatedTs
        if (this.jjDisputedCount?.latestPleaUpdateTs) {
          this.bindLatestPleaUpdateTs(this.jjDisputedCount.latestPleaUpdateTs);
        }
      }

      if (this.isViewOnly || !this.jjDisputedCount) {
        this.form.disable();
      }

      this.countForm.patchValue(this.jjDisputedCount);

      if (this.jjDisputedCount.jjDisputedCountRoP) {
        this.countRoPForm.patchValue(this.jjDisputedCount.jjDisputedCountRoP);
        this.countRoPForm.patchValue({
          _jailIntermittent: this.jjDisputedCount.jjDisputedCountRoP.jailIntermittent === this.JailIntermittent.Y,
          _dismissed: this.jjDisputedCount.jjDisputedCountRoP.dismissed === this.Dismissed.Y,
          _forWantOfProsecution: this.jjDisputedCount.jjDisputedCountRoP.forWantOfProsecution === this.ForWantOfProsecution.Y,
          _withdrawn: this.jjDisputedCount.jjDisputedCountRoP.withdrawn === this.Withdrawn.Y,
          _abatement: this.jjDisputedCount.jjDisputedCountRoP.abatement === this.Abatement.Y
        });
      }
    }
  }

  private initListeners() {
    if (this.jjDisputedCount) {
      // listen for form changes
      this.form.valueChanges.subscribe(() => {
        Object.assign(this.jjDisputedCount, this.form.getRawValue()); // get raw value includes disabled fields
        if (this.jjDisputedCount.latestPleaUpdateTs) {
          this.jjDisputedCount.latestPleaUpdateTs = new Date(this.jjDisputedCount.latestPleaUpdateTs).toISOString();
        }
        this.jjDisputedCount.includesSurcharge = (this.inclSurcharge === "yes" ? this.IncludesSurcharge.Y : this.IncludesSurcharge.N);
        this.jjDisputedCountUpdate.emit(this.jjDisputedCount);
      });

      this.countForm.valueChanges.subscribe(() => {
        this.jjDisputedCount = { ...this.jjDisputedCount, ...this.countForm.value };
        this.inclSurcharge = (this.jjDisputedCount.includesSurcharge === this.IncludesSurcharge.Y ? "yes" : "no");
        this.jjDisputedCountUpdate.emit(this.jjDisputedCount);
      });

      this.countRoPForm.valueChanges.subscribe(() => {
        this.jjDisputedCount.jjDisputedCountRoP = { ...this.jjDisputedCount.jjDisputedCountRoP, ...this.countRoPForm.value };
        this.jjDisputedCount.jjDisputedCountRoP.jailIntermittent = this.countRoPForm.value._jailIntermittent ? this.JailIntermittent.Y : this.JailIntermittent.N;
        this.jjDisputedCount.jjDisputedCountRoP.dismissed = this.countRoPForm.value._dismissed ? this.Dismissed.Y : this.Dismissed.N;
        this.jjDisputedCount.jjDisputedCountRoP.forWantOfProsecution = this.countRoPForm.value._forWantOfProsecution ? this.ForWantOfProsecution.Y : this.ForWantOfProsecution.N;
        this.jjDisputedCount.jjDisputedCountRoP.withdrawn = this.countRoPForm.value._withdrawn ? this.Withdrawn.Y : this.Withdrawn.N;
        this.jjDisputedCount.jjDisputedCountRoP.abatement = this.countRoPForm.value._abatement ? this.Abatement.Y : this.Abatement.N;
        this.jjDisputedCountUpdate.emit(this.jjDisputedCount);
      });
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes?.jjDisputeInfo?.currentValue) {
      this.jjDisputeInfo = { ...this.jjDisputeInfo, ...this.jjDisputeInfo };
      this.initFormData();
    }
    if (changes?.jjDisputedCount?.currentValue) {
      this.jjDisputedCount = { ...this.jjDisputedCount, ...this.jjDisputedCount };
      this.initFormData();
    }
    if (changes?.isSSEditMode?.currentValue) {
      this.initFormData();
    }
  }

  onSuspSentenceCheck(checked: boolean) {
    if (!checked) {
      this.form.get('jjDisputedCountRoP').get('ssProbationDuration').setValue(null);
      this.form.get('jjDisputedCountRoP').get('ssProbationDuration').disable();
      this.form.get('jjDisputedCountRoP').get('ssProbationConditions').setValue(null);
      this.form.get('jjDisputedCountRoP').get('ssProbationConditions').disable();
    } else {
      this.form.get('jjDisputedCountRoP').get('ssProbationDuration').enable();
      this.form.get('jjDisputedCountRoP').get('ssProbationConditions').enable();
    }
  }

  onJailCheck(checked: boolean) {
    if (!checked) {
      this.form.get('jjDisputedCountRoP').get('jailDuration').setValue(null);
      this.form.get('jjDisputedCountRoP').get('jailDuration').disable();
      this.form.get('jjDisputedCountRoP').get('jailIntermittent').setValue(this.JailIntermittent.N);
      this.form.get('jjDisputedCountRoP').get('jailIntermittent').disable();
    } else {
      this.form.get('jjDisputedCountRoP').get('jailDuration').enable();
      this.form.get('jjDisputedCountRoP').get('jailIntermittent').enable();
    }
  }

  onDrivingProhibitionCheck(checked: boolean) {
    if (!checked) {
      this.form.get('jjDisputedCountRoP').get('drivingProhibition').setValue(null);
      this.form.get('jjDisputedCountRoP').get('drivingProhibition').disable();
      this.form.get('jjDisputedCountRoP').get('drivingProhibitionMVASection').setValue(null);
      this.form.get('jjDisputedCountRoP').get('drivingProhibitionMVASection').disable();
    } else {
      this.form.get('jjDisputedCountRoP').get('drivingProhibition').enable();
      this.form.get('jjDisputedCountRoP').get('drivingProhibitionMVASection').enable();
    }
  }

  onStayOfProceedingsByCheck(checked: boolean) {
    if (!checked) {
      this.form.get('jjDisputedCountRoP').get('stayOfProceedingsBy').setValue(null);
      this.form.get('jjDisputedCountRoP').get('stayOfProceedingsBy').disable();
    } else {
      this.form.get('jjDisputedCountRoP').get('stayOfProceedingsBy').enable();
    }
  }

  onOtherCheck(checked: boolean) {
    if (!checked) {
      this.form.get('jjDisputedCountRoP').get('other').setValue(null);
      this.form.get('jjDisputedCountRoP').get('other').disable();
    } else {
      this.form.get('jjDisputedCountRoP').get('other').enable();
    }
  }

  onProbationCheck(checked: boolean) {
    if (!checked) {
      this.form.get('jjDisputedCountRoP').get('probationDuration').setValue(null);
      this.form.get('jjDisputedCountRoP').get('probationDuration').disable();
      this.form.get('jjDisputedCountRoP').get('probationConditions').setValue(null);
      this.form.get('jjDisputedCountRoP').get('probationConditions').disable();
    } else {
      this.form.get('jjDisputedCountRoP').get('probationDuration').enable();
      this.form.get('jjDisputedCountRoP').get('probationConditions').enable();
    }
  }

  onDismissedChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('dismissed').setValue(value === true ? this.Dismissed.Y : this.Dismissed.N);
  }

  onAbatementChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('abatement').setValue(value === true ? this.Abatement.Y : this.Abatement.N);
  }

  onWithdrawnChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('withdrawn').setValue(value === true ? this.Withdrawn.Y : this.Withdrawn.N);
  }

  onForWantOfProsecutionChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('forWantOfProsecution').setValue(value === true ? this.ForWantOfProsecution.Y : this.ForWantOfProsecution.N);
  }

  onJailIntermittentChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('jailIntermittent').setValue(value === true ? this.JailIntermittent.Y : this.JailIntermittent.N);
  }

  onLesserDescriptionKeyup() {
    this.lesserDescriptionFilteredStatutes = this.filterStatutes(this.form.get('jjDisputedCountRoP').get('lesserDescription').value);
  }

  onFindingChange(value: JJDisputedCountRoPFinding) {
    if (value == this.Finding.GuiltyLesser) {
      this.form.get('jjDisputedCountRoP').get('lesserDescription').enable();
    } else {
      this.form.get('jjDisputedCountRoP').get('lesserDescription').setValue(null);
      this.form.get('jjDisputedCountRoP').get('lesserDescription').disable();
      // TCVP-2986 - set total fine amount and due date to null if not guilty
      if (value == this.Finding.NotGuilty) {
        this.form.get('totalFineAmount').setValue(null);
        this.form.get('lesserOrGreaterAmount').setValue(null);
        this.form.get('revisedDueDate').setValue(null);
        this.inclSurcharge = "";
        this.fineReduction = "";
        this.timeToPay = "";
        this.surcharge = 0;
        this.lesserOrGreaterAmount = 0;
      } else {
        this.form.get('totalFineAmount').setValue(this.jjDisputedCount?.ticketedFineAmount);
        this.form.get('lesserOrGreaterAmount').setValue(this.jjDisputedCount?.ticketedFineAmount);
        this.form.get('revisedDueDate').setValue(this.jjDisputedCount?.revisedDueDate);
        this.inclSurcharge = this.jjDisputedCount?.includesSurcharge == this.IncludesSurcharge.Y ? "yes" : "no";
        this.updateInclSurcharge(this.inclSurcharge);
      }
    }
  }

  onDrivingProhibitionMVASectionKeyup() {
    this.drivingProhibitionFilteredStatutes = this.filterStatutes(this.form.get('jjDisputedCountRoP').get('drivingProhibitionMVASection').value);
  }

  // return a filtered list of statutes
  filterStatutes(val: string): Statute[] {
    if (!this.lookupsService.statutes || this.lookupsService.statutes.length == 0) return [];
    return this.lookupsService.statutes.filter(option => option.__statuteString.toLowerCase().indexOf(val.toLowerCase()) >= 0);
  }

  onChangelesserOrGreaterAmount() {
    this.updateInclSurcharge(this.inclSurcharge);
  }

  updateFineAmount(event: MatRadioChange) {
    // if they select no set it back to ticketed Amount & includes surcharge
    // do nothing if yes
    if (event.value == "no") {
      this.form.get('totalFineAmount').setValue(this.jjDisputedCount?.ticketedFineAmount);
      this.form.get('lesserOrGreaterAmount').setValue(this.jjDisputedCount?.ticketedFineAmount);
      this.inclSurcharge = "yes";
      this.updateInclSurcharge("yes");
    }
  }

  updateInclSurcharge(eventValue: string) {
    // surcharge is always 15%
    if (eventValue == "yes") {
      this.form.get('totalFineAmount').setValue(Math.round(this.form.get('lesserOrGreaterAmount').value));
      this.lesserOrGreaterAmount = this.form.get('lesserOrGreaterAmount').value / 1.15;
      this.surcharge = 0.15 * this.lesserOrGreaterAmount;
    } else {
      const lesserOrGreaterAmountValue = this.form.get('lesserOrGreaterAmount').value;
      if (lesserOrGreaterAmountValue !== null && lesserOrGreaterAmountValue > 0) {
        var surcharge = Math.round(lesserOrGreaterAmountValue * 0.15);
        var newTotalFineAmount = lesserOrGreaterAmountValue + surcharge;
        this.form.get('totalFineAmount').setValue(newTotalFineAmount);
      } else {
        this.form.get('totalFineAmount').setValue(this.jjDisputedCount?.totalFineAmount);
      }
      this.lesserOrGreaterAmount = this.form.get('lesserOrGreaterAmount').value;
      this.surcharge = Math.round(this.form.get('lesserOrGreaterAmount').value * 0.15);
    }
  }

  updateRevisedDueDate(event: MatRadioChange) {
    // if they select no set it back to passed in due date
    if (event.value == "no") {
      this.form.get('revisedDueDate').setValue(this.jjDisputedCount?.dueDate);
    }
  }

  //Latest Plea
  bindLatestPlea(value) {
    this.form.controls.latestPlea.setValue(value);
  }

  bindLatestPleaUpdateTs(value) {
    this.form.controls.latestPleaUpdateTs.setValue(this.datePipe.transform(new Date(value), "YYYY-MM-dd HH:mm"));
  }

  isEmpty(value) {
    return (value == null || (typeof value === "string" && value.trim().length === 0));
  }

}



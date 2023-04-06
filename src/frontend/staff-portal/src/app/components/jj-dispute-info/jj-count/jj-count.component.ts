import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JJDispute } from '../../../services/jj-dispute.service';
import { JJDisputedCount, JJDisputedCountAppearInCourt, JJDisputedCountIncludesSurcharge, JJDisputedCountPlea, JJDisputedCountRequestReduction, JJDisputedCountRequestTimeToPay, JJDisputedCountRoPAbatement, JJDisputedCountRoPDismissed, JJDisputedCountRoPFinding, JJDisputedCountRoPForWantOfProsecution, JJDisputedCountRoPJailIntermittent, JJDisputedCountRoPWithdrawn, JJDisputeHearingType } from 'app/api';
import { MatRadioChange } from '@angular/material/radio';
import { LookupsService, Statute } from 'app/services/lookups.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-jj-count',
  templateUrl: './jj-count.component.html',
  styleUrls: ['./jj-count.component.scss']
})
export class JJCountComponent implements OnInit {
  @Input() jjDisputeInfo: JJDispute;
  @Input() count: number;
  @Input() type: string;
  @Input() isViewOnly: boolean = false;
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

  // Variables
  busy: Subscription;
  todayDate: Date = new Date();
  form: FormGroup;
  timeToPay: string = "";
  fineReduction: string = "";
  inclSurcharge: string = "";
  showDateHint: boolean = true;
  lesserOrGreaterAmount: number = 0;
  surcharge: number = 0;
  lesserDescriptionFilteredStatutes: Statute[];
  drivingProhibitionFilteredStatutes: Statute[];

  constructor(
    private lookupsService: LookupsService,
    private formBuilder: FormBuilder,
  ) {
    this.busy = this.lookupsService.getStatutes().subscribe((response: Statute[]) => {
      this.lookupsService.statutes$.next(response);
    });
  }

  ngOnInit(): void {
    this.form = this.jjDisputeInfo.hearingType == this.HearingType.WrittenReasons ?
      this.formBuilder.group({
        totalFineAmount: [null, [Validators.required, Validators.max(9999.99), Validators.min(0.00)]],
        lesserOrGreaterAmount: [null, [Validators.required, Validators.max(9999.99), Validators.min(0.00)]],
        revisedDueDate: [null, [Validators.required]],
        comments: [{ value: null, disabled: !this.jjDisputedCount }]
      }) :
      this.formBuilder.group({
        totalFineAmount: [null, [Validators.max(9999.99), Validators.min(0.00)]],
        lesserOrGreaterAmount: [null, [Validators.max(9999.99), Validators.min(0.00)]],
        revisedDueDate: [null],
        comments: [{ value: null, disabled: !this.jjDisputedCount }],
        jjDisputedCountRoP: this.formBuilder.group({
          finding:  [{ value: null, disabled: !this.jjDisputedCount }],
          lesserDescription:  [{ value: null, disabled: !this.jjDisputedCount  }],
          id: [{ value: null }],
          ssProbationCheckbox: [{value: false, disabled: !this.jjDisputedCount  }],
          ssProbationDuration:  [{ value: null, disabled: !this.jjDisputedCount  }],
          ssProbationConditions:  [{ value: null, disabled: !this.jjDisputedCount }],
          jailCheckbox: [{value: false, disabled: !this.jjDisputedCount }],
          jailDuration:  [{ value: null, disabled: !this.jjDisputedCount  }],
          jailIntermittent:  [{ value: null, disabled: !this.jjDisputedCount  }],
          probationCheckbox: [{value: false, disabled: !this.jjDisputedCount  }],
          probationDuration:  [{ value: null, disabled: !this.jjDisputedCount  }],
          probationConditions:  [{ value: null, disabled: !this.jjDisputedCount  }],
          drivingProhibitionCheckbox: [{value: false, disabled: !this.jjDisputedCount }],
          drivingProhibition:  [{ value: null, disabled: !this.jjDisputedCount  }],
          drivingProhibitionMVASection:  [{ value: null, disabled: !this.jjDisputedCount  }],
          dismissed:  [{ value: null, disabled: !this.jjDisputedCount  }],
          forWantOfProsecution:  [{ value: null, disabled: !this.jjDisputedCount  }],
          withdrawn:  [{ value: null, disabled: !this.jjDisputedCount  }],
          abatement:  [{ value: null , disabled: !this.jjDisputedCount }],
          stayOfProceedingsByCheckbox: [{value: false, disabled: !this.jjDisputedCount }],
          stayOfProceedingsBy:  [{ value: null, disabled: !this.jjDisputedCount  }],
          otherCheckbox: [{value: false, disabled: !this.jjDisputedCount }],
          other:  [{ value: null, disabled: !this.jjDisputedCount  }],
        })
      });

    // initialize if no value
    if (this.jjDisputedCount) {
      if (this.jjDisputedCount.totalFineAmount) {
        this.surcharge = this.jjDisputedCount.totalFineAmount / 1.15 * 0.15;
      } else {
        this.jjDisputedCount.totalFineAmount = this.jjDisputedCount.ticketedFineAmount;
      }
      if (!this.jjDisputedCount.revisedDueDate) this.jjDisputedCount.revisedDueDate = this.jjDisputedCount.dueDate;

      // initialize form, radio buttons
      this.form.patchValue(this.jjDisputedCount);
      if (!this.jjDisputedCount.lesserOrGreaterAmount) this.form.controls.lesserOrGreaterAmount.setValue(this.jjDisputedCount.ticketedFineAmount);
      this.inclSurcharge = this.jjDisputedCount ? (this.jjDisputedCount.includesSurcharge == this.IncludesSurcharge.Y ? "yes" : "no") : "";
      this.fineReduction = this.jjDisputedCount ? (this.jjDisputedCount.totalFineAmount != this.jjDisputedCount.ticketedFineAmount ? "yes" : "no") : "";
      this.timeToPay = this.jjDisputedCount ? (this.jjDisputedCount.dueDate != this.jjDisputedCount.revisedDueDate ? "yes" : "no") : "";
      this.updateInclSurcharge(this.inclSurcharge);
      this.form.controls.revisedDueDate.setValue(this.jjDisputedCount.revisedDueDate);

      if (this.jjDisputedCount?.jjDisputedCountRoP) {
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
        if (this.jjDisputedCount?.jjDisputedCountRoP?.stayOfProceedingsBy ) {
          this.form.get('jjDisputedCountRoP').get('stayOfProceedingsByCheckbox').setValue(true);
        } else {
          this.form.get('jjDisputedCountRoP').get('stayOfProceedingsBy').disable();
        }

        // Other fields
        if (this.jjDisputedCount?.jjDisputedCountRoP?.other ) {
          this.form.get('jjDisputedCountRoP').get('otherCheckbox').setValue(true);
        } else {
          this.form.get('jjDisputedCountRoP').get('other').disable();
        }
    }

      if(this.isViewOnly || !this.jjDisputedCount) {
        this.form.disable();
      }

      // listen for form changes
      this.form.valueChanges.subscribe(() => {
        Object.assign(this.jjDisputedCount, this.form.value);
        this.jjDisputedCount.includesSurcharge = this.inclSurcharge == "yes" ? this.IncludesSurcharge.Y : this.IncludesSurcharge.N;
        this.jjDisputedCountUpdate.emit(this.jjDisputedCount);
      });
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
    }
  }

  onDrivingProhibitionMVASectionKeyup() {
    this.drivingProhibitionFilteredStatutes = this.filterStatutes(this.form.get('jjDisputedCountRoP').get('drivingProhibitionMVASection').value);
  }

  // return a filtered list of statutes
  filterStatutes(val: string): Statute[] {
    if (!this.lookupsService.statutes || this.lookupsService.statutes.length == 0) return [];
    return this.lookupsService.statutes.filter(option => option.__statuteString.indexOf(val) >= 0);
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
      this.form.get('totalFineAmount').setValue(this.form.get('lesserOrGreaterAmount').value);
      this.lesserOrGreaterAmount = this.form.get('lesserOrGreaterAmount').value / 1.15;
      this.surcharge = 0.15 * this.lesserOrGreaterAmount;
    } else {
      this.form.get('totalFineAmount').setValue(this.form.get('lesserOrGreaterAmount').value * 1.15);
      this.lesserOrGreaterAmount = this.form.get('lesserOrGreaterAmount').value;
      this.surcharge = this.form.get('lesserOrGreaterAmount').value * 0.15;
    }
  }

  updateRevisedDueDate(event: MatRadioChange) {
    // if they select no set it back to passed in due date
    if (event.value == "no") {
      this.form.get('revisedDueDate').setValue(this.jjDisputedCount?.dueDate);
    }
  }
}



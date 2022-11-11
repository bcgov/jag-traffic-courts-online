import { Component, EventEmitter, Input, Output, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { UtilsService } from '@core/services/utils.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { JJDisputeService } from '../../../services/jj-dispute.service';
import { JJDispute, JJDisputedCount, JJDisputedCountAppearInCourt, JJDisputedCountIncludesSurcharge, JJDisputedCountRequestReduction, JJDisputedCountRequestTimeToPay, JJDisputedCountRoPAbatement, JJDisputedCountRoPDismissed, JJDisputedCountRoPFinding, JJDisputedCountRoPForWantOfProsecution, JJDisputedCountRoPJailIntermittent, JJDisputedCountRoPWithdrawn, JJDisputeHearingType } from 'app/api';
import { MatRadioChange } from '@angular/material/radio';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { MoreOptionsDialogComponent } from '@shared/dialogs/more-options-dialog/more-options-dialog.component';
import { LookupsService, StatuteView } from 'app/services/lookups.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-jj-count',
  templateUrl: './jj-count.component.html',
  styleUrls: ['./jj-count.component.scss']
})
export class JJCountComponent implements OnInit {
  @Input() public jjDisputeInfo: JJDispute;
  @Input() public count: number;
  @Input() public type: string;
  @Input() public jjDisputedCount: JJDisputedCount;
  @Output() public jjDisputedCountUpdate: EventEmitter<JJDisputedCount> = new EventEmitter<JJDisputedCount>();
  public isMobile: boolean;
  public todayDate: Date = new Date();
  public violationDate: string = "";
  public form: FormGroup;
  public timeToPay: string = "no";
  public fineReduction: string = "no";
  public inclSurcharge: string = "yes";
  public lesserOrGreaterAmount: number = 0;
  public surcharge: number = 0;
  public IncludesSurcharge = JJDisputedCountIncludesSurcharge;
  public RequestReduction = JJDisputedCountRequestReduction;
  public RequestTimeToPay = JJDisputedCountRequestTimeToPay;
  public AppearInCourt = JJDisputedCountAppearInCourt;
  public HearingType = JJDisputeHearingType;
  public JailIntermittent = JJDisputedCountRoPJailIntermittent;
  public Abatement = JJDisputedCountRoPAbatement;
  public Withdrawn = JJDisputedCountRoPWithdrawn;
  public ForWantOfProsecution = JJDisputedCountRoPForWantOfProsecution;
  public Dismissed = JJDisputedCountRoPDismissed;
  public busy: Subscription;
  public Finding = JJDisputedCountRoPFinding;
  public lesserDescriptionFilteredStatutes: StatuteView[];
  public drivingProhibitionFilteredStatutes: StatuteView[];

  constructor(
    protected route: ActivatedRoute,
    private dialog: MatDialog,
    private utilsService: UtilsService,
    public lookupsService: LookupsService,
    public mockConfigService: MockConfigService,
    public jjDisputeService: JJDisputeService,
    protected formBuilder: FormBuilder,
  ) {
    const today = new Date();
    this.isMobile = this.utilsService.isMobile();

    this.busy = this.lookupsService.getStatutes().subscribe((response: StatuteView[]) => {
      this.lookupsService.statutes$.next(response);
    });

  }

  ngOnInit(): void {
    if (this.jjDisputedCount) this.violationDate = this.jjDisputeInfo.violationDate.split("T")[0];
    if (this.jjDisputedCount?.revisedDueDate?.split('T')?.length > 0) this.jjDisputedCount.revisedDueDate = this.jjDisputedCount.revisedDueDate.split('T')[0];
    if (this.jjDisputedCount?.dueDate?.split('T')?.length > 0) this.jjDisputedCount.dueDate = this.jjDisputedCount.dueDate.split('T')[0];

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
          finding: null,
          lesserDescription: null,
          ssProbationDuration: null,
          ssProbationConditions: null,
          jailDuration: null,
          jailIntermittent: null,
          probationDuration: null,
          probationConditions: null,
          drivingProhibition: null,
          drivingProhibitionMVASection: null,
          dismissed: null,
          forWantOfProsecution: null,
          withdrawn: null,
          abatement: null,
          stayOfProceedingsBy: null,
          other: null,
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
      this.inclSurcharge = this.jjDisputedCount.includesSurcharge == this.IncludesSurcharge.Y ? "yes" : "no";
      this.fineReduction = this.jjDisputedCount.totalFineAmount != this.jjDisputedCount.ticketedFineAmount ? "yes" : "no";
      this.timeToPay = this.jjDisputedCount.dueDate != this.jjDisputedCount.revisedDueDate ? "yes" : "no";
      this.updateInclSurcharge(this.inclSurcharge);
      this.form.get('revisedDueDate').setValue(this.jjDisputedCount.revisedDueDate);

      // listen for form changes
      this.form.valueChanges.subscribe(() => {
        this.jjDisputedCount.comments = this.form.get('comments').value;
        this.jjDisputedCount.revisedDueDate = this.form.get('revisedDueDate').value;
        this.jjDisputedCount.lesserOrGreaterAmount = this.form.get('lesserOrGreaterAmount').value;
        this.jjDisputedCount.totalFineAmount = this.form.get('totalFineAmount').value;
        this.jjDisputedCount.includesSurcharge = this.inclSurcharge == "yes" ? this.IncludesSurcharge.Y : this.IncludesSurcharge.N;
        if (this.jjDisputeInfo.hearingType === this.HearingType.CourtAppearance) {
          this.jjDisputedCount.jjDisputedCountRoP.finding = this.form.get('jjDisputedCountRoP').get('finding').value;
          this.jjDisputedCount.jjDisputedCountRoP.lesserDescription = this.form.get('jjDisputedCountRoP').get('lesserDescription').value;
          this.jjDisputedCount.jjDisputedCountRoP.ssProbationDuration = this.form.get('jjDisputedCountRoP').get('ssProbationDuration').value;
          this.jjDisputedCount.jjDisputedCountRoP.ssProbationConditions = this.form.get('jjDisputedCountRoP').get('ssProbationConditions').value;
          this.jjDisputedCount.jjDisputedCountRoP.jailDuration = this.form.get('jjDisputedCountRoP').get('jailDuration').value;
          this.jjDisputedCount.jjDisputedCountRoP.jailIntermittent = this.form.get('jjDisputedCountRoP').get('jailIntermittent').value;
          this.jjDisputedCount.jjDisputedCountRoP.probationDuration = this.form.get('jjDisputedCountRoP').get('probationDuration').value;
          this.jjDisputedCount.jjDisputedCountRoP.probationConditions = this.form.get('jjDisputedCountRoP').get('probationConditions').value;
          this.jjDisputedCount.jjDisputedCountRoP.drivingProhibition = this.form.get('jjDisputedCountRoP').get('drivingProhibition').value;
          this.jjDisputedCount.jjDisputedCountRoP.drivingProhibitionMVASection = this.form.get('jjDisputedCountRoP').get('drivingProhibitionMVASection').value;
          this.jjDisputedCount.jjDisputedCountRoP.dismissed = this.form.get('jjDisputedCountRoP').get('dismissed').value;
          this.jjDisputedCount.jjDisputedCountRoP.forWantOfProsecution = this.form.get('jjDisputedCountRoP').get('forWantOfProsecution').value;
          this.jjDisputedCount.jjDisputedCountRoP.withdrawn = this.form.get('jjDisputedCountRoP').get('withdrawn').value;
          this.jjDisputedCount.jjDisputedCountRoP.abatement = this.form.get('jjDisputedCountRoP').get('abatement').value;
          this.jjDisputedCount.jjDisputedCountRoP.stayOfProceedingsBy = this.form.get('jjDisputedCountRoP').get('stayOfProceedingsBy').value;
          this.jjDisputedCount.jjDisputedCountRoP.other = this.form.get('jjDisputedCountRoP').get('other').value;
        }
        this.jjDisputedCountUpdate.emit(this.jjDisputedCount);
      });
    }
  }

  onDismissedChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('dismissed').setValue(value === true ? this.Dismissed.Y : this.Dismissed.N );
  }

  onAbatementChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('abatement').setValue(value === true ? this.Abatement.Y : this.Abatement.N );
  }

  onWithdrawnChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('withdrawn').setValue(value === true ? this.Withdrawn.Y : this.Withdrawn.N );
  }

  onForWantOfProsecutionChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('forWantOfProsecution').setValue(value === true ? this.ForWantOfProsecution.Y : this.ForWantOfProsecution.N );
  }

  onJailIntermittentChange(value: any) {
    this.form.get('jjDisputedCountRoP').get('jailIntermittent').setValue(value === true ? this.JailIntermittent.Y : this.JailIntermittent.N );
  }

  onLesserDescriptionKeyup() {
    this.lesserDescriptionFilteredStatutes = this.filterStatutes(this.form.get('jjDisputedCountRoP').get('lesserDescription').value);
  }

  onDrivingProhibitionMVASectionKeyup() {
    this.drivingProhibitionFilteredStatutes = this.filterStatutes(this.form.get('jjDisputedCountRoP').get('drivingProhibitionMVASection').value);
  }

  // return a filtered list of statutes
  public filterStatutes(val: string): StatuteView[] {
    if (!this.lookupsService.statutes || this.lookupsService.statutes.length == 0) return [];
    return this.lookupsService.statutes.filter(option => option.__statuteString.indexOf(val) >= 0);
  }

  // decompose string into subparagraph, section, subsection, paragraph
  public unLegalParagraph(statuteLegalParagraphing: string): { subparagraph: string, section: string, subsection: string, paragraph: string } {
    let allParts = statuteLegalParagraphing.split("(");
    let subparagraph = "";
    let section = "";
    let subsection = "";
    let paragraph = "";

    // parts are section(section)(subsection)(paragraph)(subparagraph) if all are present
    // extract substrings but dont include final ')' of each part
    if (allParts.length > 0) section = allParts[0].substring(0, allParts[0].length);
    if (allParts.length > 1) subsection = allParts[1].substring(0, allParts[1].length - 1);
    if (allParts.length > 2) paragraph = allParts[2].substring(0, allParts[2].length - 1);
    if (allParts.length > 3) subparagraph = allParts[3].substring(0, allParts[3].length - 1);

    return { subparagraph: subparagraph, section: section, subsection: subsection, paragraph: paragraph };
  }

  onMoreOptions() {
    const data: DialogOptions = {
      titleKey: "Response to Written Reasons Dispute",
      messageKey: "",
      actionTextKey: "Require court hearing",
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline",
    };
    this.dialog.open(MoreOptionsDialogComponent, { data, width: "50%" }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          // TODO: fill in to do actions depending on choice
        }
      });
  }

  onChangelesserOrGreaterAmount() {
    // surcharge is always 15%
    if (this.inclSurcharge == "yes") {
      this.form.get('totalFineAmount').setValue(this.form.get('lesserOrGreaterAmount').value);
      this.lesserOrGreaterAmount = this.form.get('lesserOrGreaterAmount').value / 1.15;
      this.surcharge = 0.15 * this.lesserOrGreaterAmount;
    } else {
      this.form.get('totalFineAmount').setValue(this.form.get('lesserOrGreaterAmount').value * 1.15);
      this.lesserOrGreaterAmount = this.form.get('lesserOrGreaterAmount').value;
      this.surcharge = this.form.get('lesserOrGreaterAmount').value * 0.15;
    }
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



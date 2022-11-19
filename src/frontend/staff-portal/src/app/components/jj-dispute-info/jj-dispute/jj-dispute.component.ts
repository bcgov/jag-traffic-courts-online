import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CustomDatePipe as DatePipe } from '@shared/pipes/custom-date.pipe';
import { ActivatedRoute } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { JJDisputeService, JJDisputeView as JJDispute } from '../../../services/jj-dispute.service';
import { Subscription } from 'rxjs';
import { JJDisputedCount, JJDisputeStatus, JJDisputedCountRequestReduction, JJDisputedCountRequestTimeToPay, JJDisputeHearingType, JJDisputeCourtAppearanceRoP, JJDisputeCourtAppearanceRoPApp, JJDisputeCourtAppearanceRoPCrown } from 'app/api/model/models';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { UserRepresentation } from 'app/api/model/models';
import { AuthService } from 'app/services/auth.service';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-jj-dispute',
  templateUrl: './jj-dispute.component.html',
  styleUrls: ['./jj-dispute.component.scss']
})
export class JJDisputeComponent implements OnInit {
  @Input() public jjDisputeInfo: JJDispute
  @Input() public type: string;
  @Input() public isViewOnly: boolean = false;
  @Output() public onBack: EventEmitter<any> = new EventEmitter();

  public isMobile: boolean;
  public busy: Subscription;
  public courtAppearanceForm: FormGroup;
  public lastUpdatedJJDispute: JJDispute;
  public todayDate: Date = new Date();
  public retrieving: boolean = true;
  public violationDate: string = "";
  public violationTime: string = "";
  public timeToPayCountsHeading: string = "";
  public fineReductionCountsHeading: string = "";
  public remarks: string = "";
  public jjList: UserRepresentation[];
  public selectedJJ: string;
  public RequestTimeToPay = JJDisputedCountRequestTimeToPay;
  public RequestReduction = JJDisputedCountRequestReduction;
  public HearingType = JJDisputeHearingType;
  public RoPApp = JJDisputeCourtAppearanceRoPApp;
  public RoPCrown = JJDisputeCourtAppearanceRoPCrown;

  constructor(
    protected route: ActivatedRoute,
    private utilsService: UtilsService,
    public mockConfigService: MockConfigService,
    public formBuilder: FormBuilder,
    private datePipe: DatePipe,
    private jjDisputeService: JJDisputeService,
    private dialog: MatDialog,
    private logger: LoggerService,
    public authService: AuthService
  ) {
    this.isMobile = this.utilsService.isMobile();
    this.jjList = this.jjDisputeService.jjList;
  }

  public ngOnInit() {
    this.getJJDispute();

    this.courtAppearanceForm = this.formBuilder.group({
      appearanceTs: [null],
      room: [null],
      reason: [null],
      app: [null],
      noAppTs: [null],
      clerkRecord: [null],
      defenseCounsel: [null],
      crown: [null],
      jjSeized: [null],
      adjudicator: [null],
      comments: [null]
    });
  }

  public onSubmit(): void {
    this.lastUpdatedJJDispute.status = JJDisputeStatus.Confirmed;  // Send to VTC Staff for review
    this.lastUpdatedJJDispute.jjDecisionDate = this.datePipe.transform(new Date(), "yyyy-MM-dd"); // record date of decision
    this.putJJDispute();
  }

  public updateAppearanceTs() {
    this.courtAppearanceForm.get('appearanceTs').setValue(new Date());
  }

  public updateNoAPPTs() {
    this.courtAppearanceForm.get('noAppTs').setValue(new Date());
  }

  public onSave(): void {
    // Update status to in progress unless status is set to review in which case do not change
    if (this.lastUpdatedJJDispute.status !== JJDisputeStatus.Review) {
      this.lastUpdatedJJDispute.status = JJDisputeStatus.InProgress;
      this.putJJDispute();
    } else {
      this.putJJDispute();
    }
  }

  private onAccept(): void {
    const data: DialogOptions = {
      titleKey: "Submit to JUSTIN?",
      messageKey: "Are you sure this dispute is ready to be submitted to JUSTIN?",
      actionTextKey: "Submit",
      actionType: "primary",
      cancelTextKey: "Go back",
      icon: ""
    };
    this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.jjDisputeService.apiJjTicketNumberAcceptPut(this.lastUpdatedJJDispute.ticketNumber, this.type === "ticket").subscribe(response => {
            this.onBackClicked();
          });
        }
      });
  }

  returnToJJ(): void {
    const data: DialogOptions = {
      titleKey: "Return to Judicial Justice?",
      messageKey: "Are you sure you want to send this dispute decision to the selected judicial justice?",
      actionTextKey: "Send to jj",
      actionType: "primary",
      cancelTextKey: "Go back",
      icon: ""
    };

    this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.jjDisputeService.apiJjTicketNumberReviewPut(this.lastUpdatedJJDispute.ticketNumber, this.type === "ticket", this.remarks).subscribe(() => {
            this.jjDisputeService.apiJjAssignPut([this.lastUpdatedJJDispute.ticketNumber], this.selectedJJ).subscribe(response => {
              this.onBackClicked();
            })
          })
        }
      });
  }

  private putJJDispute(): void {
    // update court appearance data
    if (this.lastUpdatedJJDispute.hearingType === this.HearingType.CourtAppearance) {
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0] = this.courtAppearanceForm.value;
    }
    this.busy = this.jjDisputeService.putJJDispute(this.lastUpdatedJJDispute.ticketNumber, this.lastUpdatedJJDispute, this.type === "ticket", this.remarks).subscribe((response: JJDispute) => {
      this.lastUpdatedJJDispute = response;
      this.logger.info(
        'JJDisputeComponent::putJJDispute response',
        response
      );
      this.onBackClicked();
    });
  }

  // get dispute by id
  getJJDispute(): void {
    this.logger.log('JJDisputeComponent::getJJDispute');

    this.busy = this.jjDisputeService.getJJDispute(this.jjDisputeInfo.ticketNumber, this.type === "ticket").subscribe((response: JJDispute) => {
      this.retrieving = false;
      this.logger.info(
        'JJDisputeComponent::getJJDispute response',
        response
      );

      this.lastUpdatedJJDispute = response;

      // set violation date and time
      let violationDate = this.lastUpdatedJJDispute.violationDate.split("T");
      this.violationDate = violationDate[0];
      this.violationTime = violationDate[1].split(":")[0] + ":" + violationDate[1].split(":")[1];

      // set up headings for written reasons
      this.lastUpdatedJJDispute.jjDisputedCounts.forEach(disputedCount => {
        if (disputedCount.requestTimeToPay === this.RequestTimeToPay.Y) this.timeToPayCountsHeading += "Count " + disputedCount.count.toString() + ", ";
        if (disputedCount.requestReduction === this.RequestReduction.Y) this.fineReductionCountsHeading += "Count " + disputedCount.count.toString() + ", ";
      });
      if (this.timeToPayCountsHeading.length > 0) {
        this.timeToPayCountsHeading = this.timeToPayCountsHeading.substring(0, this.timeToPayCountsHeading.lastIndexOf(","));
      }
      if (this.fineReductionCountsHeading.length > 0) {
        this.fineReductionCountsHeading = this.fineReductionCountsHeading.substring(0, this.fineReductionCountsHeading.lastIndexOf(","));
      }

      if (this.lastUpdatedJJDispute?.jjDisputeCourtAppearanceRoPs?.length > 0) {
        this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs = this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs.sort((a: JJDisputeCourtAppearanceRoP, b: JJDisputeCourtAppearanceRoP) => {
          return Date.parse(a.appearanceTs) - Date.parse(b.appearanceTs)
        });
        this.courtAppearanceForm.patchValue(this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0]);
      }
    });
  }

  getJJDisputedCount(count: number) {
    return this.lastUpdatedJJDispute.jjDisputedCounts.filter(x => x.count == count)[0];
  }

  // get from child component
  updateFinalDispositionCount(updatedJJDisputedCount: JJDisputedCount) {
    this.lastUpdatedJJDispute.jjDisputedCounts.forEach(jjDisputedCount => {
      if (jjDisputedCount.count == updatedJJDisputedCount.count) {
        jjDisputedCount = updatedJJDisputedCount;
      }
    });
  }

  public onBackClicked() {
    this.onBack.emit();
  }
}

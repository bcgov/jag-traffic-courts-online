import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CustomDatePipe as DatePipe } from '@shared/pipes/custom-date.pipe';
import { LoggerService } from '@core/services/logger.service';
import { JJDisputeService, JJDispute } from '../../../services/jj-dispute.service';
import { Subscription } from 'rxjs';
import { JJDisputedCount, JJDisputeStatus, JJDisputedCountRequestReduction, JJDisputedCountRequestTimeToPay, JJDisputeHearingType, JJDisputeCourtAppearanceRoPApp, JJDisputeCourtAppearanceRoPCrown, Language } from 'app/api/model/models';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { MatDialog } from '@angular/material/dialog';
import { UserRepresentation } from 'app/services/auth.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { LookupsService } from 'app/services/lookups.service';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-jj-dispute',
  templateUrl: './jj-dispute.component.html',
  styleUrls: ['./jj-dispute.component.scss']
})
export class JJDisputeComponent implements OnInit {
  @Input() jjDisputeInfo: JJDispute
  @Input() type: string;
  @Input() isViewOnly = false;
  @Output() onBack: EventEmitter<any> = new EventEmitter();

  busy: Subscription;
  courtAppearanceForm: FormGroup;
  lastUpdatedJJDispute: JJDispute;
  todayDate: Date = new Date();
  retrieving: boolean = true;
  violationDate: string = "";
  violationTime: string = "";
  timeToPayCountsHeading: string = "";
  fineReductionCountsHeading: string = "";
  remarks: string = "";
  jjList: UserRepresentation[];
  selectedJJ: string;
  RequestTimeToPay = JJDisputedCountRequestTimeToPay;
  RequestReduction = JJDisputedCountRequestReduction;
  HearingType = JJDisputeHearingType;
  RoPApp = JJDisputeCourtAppearanceRoPApp;
  RoPCrown = JJDisputeCourtAppearanceRoPCrown;
  DisputeStatus = JJDisputeStatus;
  requireCourtHearingReason: string = "";

  constructor(
    private formBuilder: FormBuilder,
    private datePipe: DatePipe,
    private jjDisputeService: JJDisputeService,
    private dialog: MatDialog,
    private logger: LoggerService,
    private lookups: LookupsService
  ) {
    this.jjDisputeService.jjList$.subscribe(result => {
      this.jjList = result;
    });

    this.busy = this.lookups.getLanguages().subscribe((response: Language[]) => {
      this.lookups.languages$.next(response);
    });
  }

  ngOnInit() {
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
    });;
  }

  public onSubmit(): void {
    this.lastUpdatedJJDispute.status = this.DisputeStatus.Confirmed;  // Send to VTC Staff for review
    this.lastUpdatedJJDispute.jjDecisionDate = this.datePipe.transform(new Date(), "yyyy-MM-dd"); // record date of decision
    this.putJJDispute();
  }

  onRequireCourtHearing() {
    const data: DialogOptions = {
      titleKey: "Require court hearing?",
      messageKey:
        "Please enter the reason this request requires a court hearing. This information will be shared with staff only.",
      actionTextKey: "Require court hearing",
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline",
      message: this.requireCourtHearingReason
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action?: any) => {
        if (action?.output?.response) {
          this.requireCourtHearingReason = action.output.reason; // update on form for appearances

          // udate the reason entered, reject dispute and return to TRM home
          this.busy = this.jjDisputeService.apiJjRequireCourtHearingPut(this.lastUpdatedJJDispute.ticketNumber, this.requireCourtHearingReason).subscribe({
            next: response => {
              this.lastUpdatedJJDispute.status = this.DisputeStatus.RequireCourtHearing;
              this.onBackClicked();
            },
            error: err => { },
            complete: () => { }
          });
        }
      });
  }

  updateAppearanceTs() {
    this.courtAppearanceForm.controls.appearanceTs.setValue(new Date());
  }

  updateNoAPPTs() {
    this.courtAppearanceForm.controls.noAppTs.setValue(new Date());
  }

  onSave(): void {
    // Update status to in progress unless status is set to review in which case do not change
    if (this.lastUpdatedJJDispute.status !== this.DisputeStatus.Review) {
      this.lastUpdatedJJDispute.status = this.DisputeStatus.InProgress;
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
              this.jjDisputeService.refreshDisputes.emit();
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
    this.busy = this.jjDisputeService.putJJDispute(this.lastUpdatedJJDispute.ticketNumber, this.lastUpdatedJJDispute, this.type === "ticket", this.remarks).subscribe(response => {
      this.lastUpdatedJJDispute = response;
      this.logger.info(
        'JJDisputeComponent::putJJDispute response',
        response
      );
      this.jjDisputeService.refreshDisputes.emit();
      this.onBackClicked();
    });
  }

  // get dispute by id
  getJJDispute(): void {
    this.logger.log('JJDisputeComponent::getJJDispute');

    this.busy = this.jjDisputeService.getJJDispute(this.jjDisputeInfo.ticketNumber, this.type === "ticket").subscribe(response => {
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
        this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs = this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs.sort((a, b) => {
          return Date.parse(a.appearanceTs) - Date.parse(b.appearanceTs)
        });
        this.courtAppearanceForm.patchValue(this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0]);
      }
    });
  }

  getJJDisputedCount(count: number) {
    return this.lastUpdatedJJDispute.jjDisputedCounts.filter(x => x.count == count).shift();
  }

  // get from child component
  updateFinalDispositionCount(updatedJJDisputedCount: JJDisputedCount) {
    this.lastUpdatedJJDispute.jjDisputedCounts.forEach(jjDisputedCount => {
      if (jjDisputedCount.count == updatedJJDisputedCount.count) {
        jjDisputedCount = updatedJJDisputedCount;
      }
    });
  }

  getLanguageDesc(code: string): string {
    return this.lookups.getLanguageDescription(code);
  }

  onBackClicked() {
    this.onBack.emit();
  }
}

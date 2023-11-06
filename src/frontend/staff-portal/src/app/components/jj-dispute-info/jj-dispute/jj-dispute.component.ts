import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { JJDisputeService, JJDispute } from '../../../services/jj-dispute.service';
import { Observable, map } from 'rxjs';
import { JJDisputedCount, JJDisputeStatus, JJDisputedCountRequestReduction, JJDisputedCountRequestTimeToPay, JJDisputeHearingType, JJDisputeCourtAppearanceRoPAppCd, JJDisputeCourtAppearanceRoPCrown, JJDisputeCourtAppearanceRoPDattCd, JJDisputeCourtAppearanceRoPJjSeized, FileMetadata, JJDisputeElectronicTicketYn, JJDisputeNoticeOfHearingYn, TicketImageDataJustinDocumentReportType, DocumentType, JJDisputeContactType, JJDisputedCountRoPFinding } from 'app/api/model/models';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { LookupsService } from 'app/services/lookups.service';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { ConfigService } from '@config/config.service';
import { DocumentService } from 'app/api/api/document.service';
import { HistoryRecordService } from 'app/services/history-records.service';
import { PrintOptions } from '@shared/models/print-options.model';
import { CustomDatePipe } from '@shared/pipes/custom-date.pipe';

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
  printOptions: PrintOptions = new PrintOptions();

  RequestTimeToPay = JJDisputedCountRequestTimeToPay;
  Finding = JJDisputedCountRoPFinding;
  RequestReduction = JJDisputedCountRequestReduction;
  HearingType = JJDisputeHearingType;
  RoPApp = JJDisputeCourtAppearanceRoPAppCd;
  RoPCrown = JJDisputeCourtAppearanceRoPCrown;
  RoPDatt = JJDisputeCourtAppearanceRoPDattCd;
  RoPSeized = JJDisputeCourtAppearanceRoPJjSeized;
  ElectronicTicket = JJDisputeElectronicTicketYn;
  NoticeOfHearing = JJDisputeNoticeOfHearingYn;
  ReportType = TicketImageDataJustinDocumentReportType;
  DocumentType = DocumentType;
  DisputeStatus = JJDisputeStatus;
  ContactType = JJDisputeContactType;

  courtAppearanceForm: FormGroup = this.formBuilder.group({
    appCd: [{ value: null, disabled: true }],
    room: [{ value: null, disabled: true }],
    reason: [null],
    noAppTs: [null],
    _noAppTs: [null],
    clerkRecord: [null],
    defenceCounsel: [null],
    crown: [null],
    jjSeized: [null],
    adjudicator: [null],
    comments: [null],
    dattCd: [null],
    adjudicatorName: [{ value: null, disabled: true }]
  });

  infoHeight: number = window.innerHeight - 150; // less size of other fixed elements
  infoWidth: number = window.innerWidth;
  lastUpdatedJJDispute: JJDispute;
  jjIDIR: string;
  jjName: string;
  retrieving: boolean = true;
  violationDate: string = "";
  violationTime: string = "";
  timeToPayCountsHeading: string = "";
  fineReductionCountsHeading: string = "";
  remarks: string = "";
  jjList: UserRepresentation[];
  selectedJJ: string;
  fileTypeToUpload: string = "Certified Extract";
  filesToUpload: any[] = [];
  requireCourtHearingReason: string = "";
  concludeStatusOnly: boolean = false;
  cancelStatusOnly: boolean = false;
  isNoAppEnabled: boolean = true;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private jjDisputeService: JJDisputeService,
    private dialog: MatDialog,
    private logger: LoggerService,
    private lookups: LookupsService,
    private config: ConfigService,
    private documentService: DocumentService,
    private historyRecordService: HistoryRecordService,
    private datePipe: CustomDatePipe
  ) {
    this.authService.jjList$.subscribe(result => {
      this.jjList = result;
    });
  }

  ngOnInit() {
    this.getJJDispute();

    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = userProfile.idir;
        this.jjName = userProfile.fullName;
      }
    })
  }

  // get dispute by id
  getJJDispute(): void {
    this.logger.log('JJDisputeComponent::getJJDispute');

    this.jjDisputeService.getJJDispute(this.jjDisputeInfo.id, this.jjDisputeInfo.ticketNumber, this.type === "ticket").subscribe(response => {
      this.retrieving = false;
      this.logger.info(
        'JJDisputeComponent::getJJDispute response',
        response
      );

      this.lastUpdatedJJDispute = response;

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

      let dLProvinceFound = this.config.provincesAndStates.filter(x => x.ctryId == +this.lastUpdatedJJDispute.drvLicIssuedCtryId && x.provSeqNo == +this.lastUpdatedJJDispute.drvLicIssuedProvSeqNo);
      this.lastUpdatedJJDispute.driversLicenceProvince = dLProvinceFound.length > 0 ? dLProvinceFound[0].provNm : "Unknown";

      this.lastUpdatedJJDispute.interpreterLanguage = this.lookups.getLanguageDescription(this.lastUpdatedJJDispute.interpreterLanguageCd);

      if (this.lastUpdatedJJDispute?.mostRecentCourtAppearance) {
        if (!this.lastUpdatedJJDispute.mostRecentCourtAppearance.jjSeized) this.lastUpdatedJJDispute.mostRecentCourtAppearance.jjSeized = 'N';

        if (!this.isViewOnly) {
          this.lastUpdatedJJDispute.mostRecentCourtAppearance.adjudicator = this.jjIDIR;
          this.lastUpdatedJJDispute.mostRecentCourtAppearance["adjudicatorName"] = this.jjName; // force to add property
          this.lastUpdatedJJDispute.jjAssignedToName = this.jjName;
          if (this.lastUpdatedJJDispute.jjAssignedTo != this.jjIDIR) {
            this.lastUpdatedJJDispute.jjAssignedTo = this.jjIDIR;
            this.jjDisputeService.apiJjAssignPut([this.lastUpdatedJJDispute.ticketNumber], this.jjIDIR).subscribe(response => { }); // assign JJ who opened it
          }
        }
        this.courtAppearanceForm.patchValue(this.lastUpdatedJJDispute.mostRecentCourtAppearance);
        this.determineIfConcludeOrCancel();
      }
    });
  }

  goTo(id: string) {
    const element = document.getElementById(id);
    element?.scrollIntoView(true);
  }

  onConfirm(): void {
    const data: DialogOptions = {
      titleKey: "Submit to VTC Staff?",
      messageKey: "Are you sure this dispute is ready to be submitted to VTC Staff?",
      actionTextKey: "Confirm",
      actionType: "primary",
      cancelTextKey: "Go back",
      icon: ""
    };
    this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.lastUpdatedJJDispute.jjDecisionDate = new Date().toISOString();
          this.putJJDispute().subscribe(response => {
            this.jjDisputeService.apiJjTicketNumberConfirmPut(this.lastUpdatedJJDispute.ticketNumber).subscribe(response => {
              this.onBackClicked();
            });
          });
        }
      });
  }

  onRequireCourtHearing() {
    const data: DialogOptions = {
      titleKey: this.lastUpdatedJJDispute.hearingType === this.HearingType.WrittenReasons ? "Require court hearing?" : "Adjourn / Continue?",
      messageKey: this.lastUpdatedJJDispute.hearingType === this.HearingType.WrittenReasons ?
        "Please enter the reason this request requires a court hearing. This information will be shared with staff only."
        : "Please enter the reason this request requires an additional court hearing. This information will be shared with staff only.",
      actionTextKey: "OK",
      actionType: "warn",
      cancelTextKey: "Go back",
      icon: "error_outline",
      message: this.requireCourtHearingReason
    };
    this.dialog.open(ConfirmReasonDialogComponent, { data }).afterClosed()
      .subscribe((action?: any) => {
        if (action?.output?.response) {
          this.requireCourtHearingReason = action.output.reason; // update on form for appearances

          // update the reason entered, reject dispute and return to TRM home
          this.putJJDispute().subscribe(response => {
            this.jjDisputeService.apiJjRequireCourtHearingPut(this.lastUpdatedJJDispute.ticketNumber, this.requireCourtHearingReason).subscribe({
              next: response => {
                this.onBackClicked();
              },
              error: err => { },
              complete: () => { }
            });
          });
        }
      });
  }

  onUpdateAPPCd() {
    if (JJDisputeCourtAppearanceRoPAppCd.P === this.courtAppearanceForm.value.appCd
      || JJDisputeCourtAppearanceRoPAppCd.A === this.courtAppearanceForm.value.appCd) {
      this.courtAppearanceForm.controls.noAppTs.setValue(null);
      this.isNoAppEnabled = false;
    }
    else {
      this.isNoAppEnabled = true;
    }
  }

  updateNoAppTs(date: Date) {
    this.courtAppearanceForm.controls.noAppTs.setValue(this.datePipe.transform(date, "MM/dd/yyyy HH:mm:ss") + " UTC");
  }

  updateNoAppTsToNow() {
    this.updateNoAppTs(new Date());
  }

  updateNoAppDateTime(value) {
    this.updateNoAppTs(new Date(value));
  }

  onSave(): void {
    // Update status to in progress unless status is set to review in which case do not change
    if (this.lastUpdatedJJDispute.status !== this.DisputeStatus.Review) {
      this.lastUpdatedJJDispute.status = this.DisputeStatus.InProgress;
    }
    this.putJJDispute().subscribe(response => {
      if (this.remarks) {
        this.remarks = "";
      }
      const data: DialogOptions = {
        titleKey: "Saved",
        messageKey: "Dispute saved",
        actionTextKey: "Ok",
        actionType: "primary",
        icon: "done"
      };
      this.dialog.open(ConfirmDialogComponent, { data, width: "200px" });
    });
  }

  onAccept(): void {
    const data: DialogOptions = {
      titleKey: "Submit to JUSTIN?",
      messageKey: "Are you sure this dispute is ready to be submitted to JUSTIN?",
      actionTextKey: "Accept",
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
      messageKey: "Are you sure you want to send this dispute decision to the selected judicial justice? Please provide a reason why.",
      actionTextKey: "Send to jj",
      actionType: "primary",
      cancelTextKey: "Go back",
      message: this.remarks,
      icon: ""
    };

    this.dialog.open(ConfirmReasonDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        if (action?.output?.response) {
          this.remarks = action.output.reason;
          this.jjDisputeService.apiJjDisputeIdReviewPut(this.lastUpdatedJJDispute.ticketNumber, this.type === "ticket", this.remarks).subscribe(() => {
            this.jjDisputeService.apiJjAssignPut([this.lastUpdatedJJDispute.ticketNumber], this.selectedJJ).subscribe(response => {
              this.onBackClicked();
            })
          })
        }
      });
  }

  private putJJDispute(): Observable<any> {
    // update court appearance data
    if (this.lastUpdatedJJDispute.hearingType === this.HearingType.CourtAppearance) {
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0] = { ...this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0], ...this.courtAppearanceForm.value };
    }
    return this.jjDisputeService.putJJDispute(this.lastUpdatedJJDispute.ticketNumber, this.lastUpdatedJJDispute.id, this.lastUpdatedJJDispute, this.type === "ticket", this.remarks).pipe(
      map(
        response => {
          this.lastUpdatedJJDispute = response;
          this.logger.info(
            'JJDisputeComponent::putJJDispute response',
            response
          );
        }));
  }

  refreshFileHistory() {
    // reset ticket number to trigger file history refresh
    this.historyRecordService.refreshFileHistory.emit(this.lastUpdatedJJDispute.ticketNumber);
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
    this.determineIfConcludeOrCancel();
  }

  determineIfConcludeOrCancel() {
    this.concludeStatusOnly = false;
    this.cancelStatusOnly = false;
    let cancelledCount = 0;
    let countCount = 0;
    let paidPriorToAppearancCount = 0;
    this.lastUpdatedJJDispute.jjDisputedCounts.forEach(jjDisputedCount => {
      countCount++;
      if (jjDisputedCount.jjDisputedCountRoP.finding === this.Finding.Cancelled) cancelledCount++;
      if (jjDisputedCount.jjDisputedCountRoP.finding === this.Finding.PaidPriorToAppearance) paidPriorToAppearancCount++;
    });
    if (cancelledCount === countCount && countCount > 0) this.cancelStatusOnly = true;
    else if (cancelledCount + paidPriorToAppearancCount >= countCount && countCount > 0) this.concludeStatusOnly = true;
  }

  onCancelled() {
    const data: DialogOptions = {
      titleKey: "Cancel Dispute",
      messageKey: "All counts have been recorded as cancelled.  The dispute will be recorded as cancelled.",
      actionTextKey: "Ok",
      actionType: "green",
      icon: "info",
      cancelHide: true
    };
    this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        this.putJJDispute().subscribe(response => {
          this.jjDisputeService.apiJjTicketNumberCancelPut(this.lastUpdatedJJDispute.ticketNumber, false).subscribe(response => {
            this.onBackClicked();
          });
        });
      });
  }

  onConcluded() {
    const data: DialogOptions = {
      titleKey: "Conclude Dispute",
      messageKey: "All counts have been recorded as cancelled or paid prior to appearance.  The dispute will be recorded as concluded.",
      actionTextKey: "Ok",
      actionType: "green",
      icon: "info",
      cancelHide: true
    };
    this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        this.putJJDispute().subscribe(response => {
          this.jjDisputeService.apiJjTicketNumberConcludePut(this.lastUpdatedJJDispute.ticketNumber, false).subscribe(response => {
            this.onBackClicked();
          });
        });
      });
  }

  onRemoveFile(fileId: string, fileName: string) {
    const data: DialogOptions = {
      titleKey: "Remove File?",
      messageKey: "Are you sure you want to delete file " + fileName + "?",
      actionTextKey: "Delete",
      actionType: "warn",
      cancelTextKey: "Cancel",
      icon: "delete"
    };
    this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.lastUpdatedJJDispute.fileData = this.lastUpdatedJJDispute.fileData.filter(x => x.fileId !== fileId);
          this.documentService.apiDocumentDelete(fileId).subscribe(any => {
            // dont need to update the JJ Dispute after the document is removed, line 88 is just to update UX
            this.refreshFileHistory();
          });
        }
      });
  }

  onGetFile(fileId: string) {
    this.jjDisputeService.getFileBlob(fileId).subscribe(result => {
      // TODO: remove the custom function here and replace with generated api call once staff API method
      // has proper response type documented in swagger json
      if (result != null) {
        var url = URL.createObjectURL(result);
        window.open(url);
      } else alert("File contents not found");
    });
  }

  onGetJustinDocument(documentType: DocumentType) {
    this.jjDisputeService.getJustinDocument(this.lastUpdatedJJDispute.ticketNumber, documentType).subscribe(result => {
      var url = URL.createObjectURL(result);
      window.open(url);
    });
  }

  onUpload(files: FileList) {
    if (files.length <= 0) return;

    // upload to coms
    this.documentService.apiDocumentPost(this.lastUpdatedJJDispute.id, this.lastUpdatedJJDispute.noticeOfDisputeGuid, this.fileTypeToUpload, files[0])
      .subscribe(fileId => {

        // add to display of files in DCF
        let item: FileMetadata = { fileId: fileId, fileName: files[0].name, virusScanStatus: "waiting for virus scan..." };
        this.lastUpdatedJJDispute.fileData.push(item);
        this.refreshFileHistory();
      });
  }

  onPrint() {
    this.jjDisputeService.apiJjDisputeIdPrintGet(this.lastUpdatedJJDispute.id).subscribe(result => {
      if (result != null) {
        var url = URL.createObjectURL(result);
        window.open(url);
      } else alert("File contents not found");
    });
  }

  onBackClicked() {
    this.jjDisputeService.refreshDisputes.emit();
    this.onBack.emit();
  }
}

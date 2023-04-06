import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CustomDatePipe as DatePipe } from '@shared/pipes/custom-date.pipe';
import { LoggerService } from '@core/services/logger.service';
import { JJDisputeService, JJDispute } from '../../../services/jj-dispute.service';
import { Observable, Subscription, map } from 'rxjs';
import { JJDisputedCount, JJDisputeStatus, JJDisputedCountRequestReduction, JJDisputedCountRequestTimeToPay, JJDisputeHearingType, JJDisputeCourtAppearanceRoPAppCd, JJDisputeCourtAppearanceRoPCrown, Language, JJDisputeCourtAppearanceRoPDattCd, JJDisputeCourtAppearanceRoPJjSeized, FileMetadata, JJDisputeElectronicTicketYn, JJDisputeNoticeOfHearingYn, TicketImageDataJustinDocumentReportType, DocumentType, JJDisputeContactType } from 'app/api/model/models';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { MatDialog } from '@angular/material/dialog';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { LookupsService } from 'app/services/lookups.service';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { ConfigService } from '@config/config.service';
import { DocumentService } from 'app/api/api/document.service';
import { HistoryRecordService } from 'app/services/history-records.service';

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

  public printDispute: boolean = true;
  public printUploadedDocuments: boolean = true;
  public printFileHistory: boolean = true;
  public printFileRemarks: boolean = true;

  busy: Subscription;
  courtAppearanceForm: FormGroup;
  infoHeight: number = window.innerHeight - 150; // less size of other fixed elements
  infoWidth: number = window.innerWidth;
  lastUpdatedJJDispute: JJDispute;
  jjIDIR: string;
  jjName: string;
  todayDate: Date = new Date();
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
  dLProvince: string;
  RequestTimeToPay = JJDisputedCountRequestTimeToPay;
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
  requireCourtHearingReason: string = "";

  constructor(
    private formBuilder: FormBuilder,
    private datePipe: DatePipe,
    private authService: AuthService,
    private jjDisputeService: JJDisputeService,
    private dialog: MatDialog,
    private logger: LoggerService,
    private lookups: LookupsService,
    public config: ConfigService,
    private documentService: DocumentService,
    private historyRecordService: HistoryRecordService
  ) {
    this.jjDisputeService.jjList$.subscribe(result => {
      this.jjList = result;
    });

    this.busy = this.lookups.getLanguages().subscribe((response: Language[]) => {
      this.lookups.languages$.next(response);
    });
  }

  public goTo(id: string) {
    const element = document.getElementById(id);
    element?.scrollIntoView(true);
  }

  onRemove(fileId: string, fileName: string) {
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

  ngOnInit() {
    this.getJJDispute();

    this.courtAppearanceForm = this.formBuilder.group({
      appearanceTs: [null],
      room: [null],
      createdBy: [null],
      reason: [null],
      appCd: [null],
      noAppTs: [null],
      clerkRecord: [null],
      defenceCounsel: [null],
      crown: [null],
      jjSeized: [null],
      adjudicator: [null],
      comments: [null],
      dattCd: [null],
      adjudicatorName: [null]
    });

    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = userProfile.idir;
        this.jjName = userProfile.fullName;
      }
    })
  }

  public onConfirm(): void {
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
          this.lastUpdatedJJDispute.jjDecisionDate = this.datePipe.transform(new Date(), "yyyy-MM-dd") + "T" + this.datePipe.transform(new Date(), "HH:mm:ss") + ".000+00:00"; // record date of decision
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
            this.jjDisputeService.apiJjRequireCourtHearingPut(this.lastUpdatedJJDispute.ticketNumber, this.lastUpdatedJJDispute.id, this.requireCourtHearingReason).subscribe({
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
    const data: DialogOptions = {
      titleKey: "Saved",
      messageKey: "Dispute saved",
      actionTextKey: "Ok",
      actionType: "primary",
      icon: "done"
    };
    this.dialog.open(ConfirmDialogComponent, { data, width: "200px" });
  }

  public onAccept(): void {
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

      // update fields in latest court appearance
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].appCd = this.courtAppearanceForm.value.appCd;
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].noAppTs = this.courtAppearanceForm.value.noAppTs;
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].clerkRecord = this.courtAppearanceForm.value.clerkRecord;
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].defenceCounsel = this.courtAppearanceForm.value.defenceCounsel;
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].dattCd = this.courtAppearanceForm.value.dattCd;
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].crown = this.courtAppearanceForm.value.crown;
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].jjSeized = this.courtAppearanceForm.value.jjSeized;
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].adjudicator = this.courtAppearanceForm.value.adjudicator;
      this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].comments = this.courtAppearanceForm.value.comments;
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

  // get dispute by id
  getJJDispute(): void {
    this.logger.log('JJDisputeComponent::getJJDispute');

    this.busy = this.jjDisputeService.getJJDispute(this.jjDisputeInfo.id, this.jjDisputeInfo.ticketNumber, this.type === "ticket").subscribe(response => {
      this.retrieving = false;
      this.logger.info(
        'JJDisputeComponent::getJJDispute response',
        response
      );

      this.lastUpdatedJJDispute = response;

      // set violation date and time
      let violationDate = this.lastUpdatedJJDispute.issuedTs.split("T");
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

      let dLProvinceFound = this.config.provincesAndStates.filter(x => x.ctryId == +this.lastUpdatedJJDispute.drvLicIssuedCtryId && x.provSeqNo == +this.lastUpdatedJJDispute.drvLicIssuedProvSeqNo);
      this.dLProvince = dLProvinceFound.length > 0 ? dLProvinceFound[0].provNm : "Unknown";

      if (this.lastUpdatedJJDispute?.jjDisputeCourtAppearanceRoPs?.length > 0) {
        this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs = this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs.sort((a, b) => {
          return Date.parse(b.appearanceTs) - Date.parse(a.appearanceTs)
        });
        if (!this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].jjSeized) this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].jjSeized = 'N';
        this.courtAppearanceForm.patchValue(this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0]);
        this.courtAppearanceForm.controls.appearanceTs.setValue(new Date(this.lastUpdatedJJDispute.jjDisputeCourtAppearanceRoPs[0].appearanceTs));
        if (!this.isViewOnly) {
          this.courtAppearanceForm.controls.adjudicator.setValue(this.jjIDIR);
          this.courtAppearanceForm.controls.adjudicatorName.setValue(this.jjName);
          this.lastUpdatedJJDispute.jjAssignedToName = this.jjName;
          if (this.lastUpdatedJJDispute.jjAssignedTo != this.jjIDIR) {
            this.lastUpdatedJJDispute.jjAssignedTo = this.jjIDIR;
            this.jjDisputeService.apiJjAssignPut([this.lastUpdatedJJDispute.ticketNumber], this.jjIDIR).subscribe(response => { }); // assign JJ who opened it
          }
        }
      }
    });
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
  }

  getLanguageDesc(code: string): string {
    return this.lookups.getLanguageDescription(code);
  }

  onBackClicked() {
    this.jjDisputeService.refreshDisputes.emit();
    this.onBack.emit();
  }
}

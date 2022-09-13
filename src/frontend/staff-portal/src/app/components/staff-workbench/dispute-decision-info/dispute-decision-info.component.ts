import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { throwToolbarMixedModesError } from '@angular/material/toolbar';
import { LoggerService } from '@core/services/logger.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { JJDispute, JJDisputeStatus } from 'app/api';
import { JJDisputeService, JJTeamMember } from 'app/services/jj-dispute.service';

@Component({
  selector: 'app-dispute-decision-info',
  templateUrl: './dispute-decision-info.component.html',
  styleUrls: ['./dispute-decision-info.component.scss']
})
export class DisputeDecisionInfoComponent implements OnInit {
  @Output() public backInbox: EventEmitter<any> = new EventEmitter();
  @Input() public jjDisputeInfo: JJDispute;

  public printDispute: boolean = true;
  public printUploadedDocuments: boolean = true;
  public printFileHistory: boolean = true;
  public printFileRemarks: boolean = true;
  public jjList: JJTeamMember[];
  public selectedJJ: JJTeamMember;

  constructor(
    private jjDisputeService: JJDisputeService,
    private dialog: MatDialog,
    private logger: LoggerService,
  ) { }

  ngOnInit(): void {
    console.log("Dispute Decision Details");
    this.jjList = this.jjDisputeService.jjList;
  }

  public onSubmit(): void {
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
          this.jjDisputeInfo.status = JJDisputeStatus.Accepted;
          this.putJJDispute();
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
          this.jjDisputeInfo.status = JJDisputeStatus.Review;
          this.jjDisputeInfo.jjAssignedTo = this.selectedJJ.idir;
          this.putJJDispute();
        }
      });
  }

  private putJJDispute() : void {
    this.jjDisputeService.putJJDispute(this.jjDisputeInfo.ticketNumber, this.jjDisputeInfo, true).subscribe((response: JJDispute) => {
      this.jjDisputeInfo = response;
      this.logger.info(
        'DisputeDecisionInfoComponent::putJJDispute response',
        response
      );
    });
  }

  public onBack() {
    this.backInbox.emit();
  }

  public goTo(id: string) {
    document.getElementById(id)?.scrollIntoView();
  }
}

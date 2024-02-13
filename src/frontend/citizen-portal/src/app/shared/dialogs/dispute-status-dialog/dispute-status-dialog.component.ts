import { Component, Inject } from '@angular/core';
import { MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';
import { QueryParamsForSearch } from '@shared/models/query-params-for-search.model';
import { DisputeStatus, JJDisputeHearingType, JJDisputeStatus, SearchDisputeResult } from 'app/api';
import { StatusStepType } from 'app/services/dispute.service';
import { DisputeStore } from 'app/store';

@Component({
  selector: 'app-dispute-status-dialog',
  templateUrl: './dispute-status-dialog.component.html',
  styleUrls: ['./dispute-status-dialog.component.scss']
})
export class DisputeStatusDialogComponent {
  dispute: SearchDisputeResult;
  params: QueryParamsForSearch;
  steps: StatusStep[] = [];
  selectedStep = 0;
  JJDisputeStatus = JJDisputeStatus;
  DisputeStatus = DisputeStatus;

  constructor(
    @Inject(MAT_DIALOG_DATA) private dialogData: DisputeStore.State,
  ) {
    this.dispute = this.dialogData.result;
    this.params = this.dialogData.params;
    this.markStepCompleted();
  }

  private markStepCompleted(): void {
    Object.values(StatusStepType).forEach((type, index) => {
      let step: StatusStep = { seq: index, type, isCompleted: false };
      switch (step.type) {
        case StatusStepType.SUBMITTED: { // must be submitted
          step.isCompleted = true;
          this.steps.push(step);
          break;
        }
        case StatusStepType.PROCESSING: {
          step.isCompleted = this.checkProcessingCompleted();
          if ([DisputeStatus.Cancelled].indexOf(this.dispute?.dispute_status) <= -1) this.steps.push(step);
          break;
        }
        case StatusStepType.SCHEDULED: {
          step.isCompleted = this.dispute?.hearing_type === JJDisputeHearingType.CourtAppearance && this.checkScheduledCompleted();
          if (this.dispute?.hearing_type === JJDisputeHearingType.CourtAppearance) this.steps.push(step);
          break;
        }
        case StatusStepType.CONCLUDED: {
          step.isCompleted = this.checkConcluded();
          this.steps.push(step);
          break;
        }
      }
      if (step.isCompleted) this.selectedStep = this.steps.length -1;
    });
  }

  private checkProcessingCompleted(): boolean {
    if (this.dispute?.hearing_type === JJDisputeHearingType.CourtAppearance && this.checkScheduledCompleted()) return true;
    else {
      let jjStatuses: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.Confirmed, JJDisputeStatus.Review, JJDisputeStatus.InProgress];
      let statuses: DisputeStatus[] = [DisputeStatus.Processing];
      if (jjStatuses.indexOf(this.dispute?.jjdispute_status) > -1 || statuses.indexOf(this.dispute?.dispute_status) > -1) {
        return true;
      } else return false;
    }
  }

  private checkScheduledCompleted(): boolean {
    if (this.checkConcluded()) return true;
    else {
      let jjStatuses: JJDisputeStatus[] = [
        JJDisputeStatus.HearingScheduled,
        JJDisputeStatus.InProgress,
        JJDisputeStatus.RequireCourtHearing,
        JJDisputeStatus.RequireMoreInfo,
        JJDisputeStatus.Review,
        JJDisputeStatus.Confirmed
      ];
      if (jjStatuses.indexOf(this.dispute?.jjdispute_status) > -1) {
        return true;
      } else return false;
    }
  }

  private checkCancelled(): boolean {
    if ([DisputeStatus.Cancelled].indexOf(this.dispute?.dispute_status) > -1 || [JJDisputeStatus.Cancelled].indexOf(this.dispute?.jjdispute_status) > -1) return true;
    else return false;
  }
  
  private checkRejected(): boolean {
    if ([DisputeStatus.Rejected].indexOf(this.dispute?.dispute_status) > -1) return true;
    else return false;
  }

  private checkConcluded(): boolean {
    if ([JJDisputeStatus.Concluded].indexOf(this.dispute?.jjdispute_status) > -1) return true;
    else if ([DisputeStatus.Concluded].indexOf(this.dispute?.dispute_status) > -1) return true;
    else if ([JJDisputeStatus.Accepted].indexOf(this.dispute?.jjdispute_status) > -1) return true;
    else if (this.checkCancelled()) return true;
    else if (this.checkCancelled()) return true;
    else return false;
  }
}

export interface StatusStep {
  seq: number
  type: StatusStepType
  isCompleted: boolean
}

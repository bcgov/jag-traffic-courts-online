import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { QueryParamsForSearch } from '@shared/models/query-params-for-search.model';
import { DisputeStatus, JJDisputeStatus, SearchDisputeResult } from 'app/api';
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

  constructor(
    @Inject(MAT_DIALOG_DATA) private dialogData: DisputeStore.State,
  ) {
    this.dispute = this.dialogData.result;
    this.params = this.dialogData.params;
    Object.values(StatusStepType).forEach((type, index) => {
      let step: StatusStep = { seq: index, type, isCompleted: false };
      this.steps.push(step);
    })
    this.markStepCompleted();
  }

  private markStepCompleted(): void {
    let steps = [...this.steps].reverse();
    let lastCompletedStepIndex = 0;
    steps.forEach((step, i) => {
      let isCompleted = false;
      let index = steps.length - 1 - i;
      switch (step.type) {
        case StatusStepType.SUBMITTED: { // must be submitted
          isCompleted = true;
        }
        case StatusStepType.PROCESSING: {
          let statuses: DisputeStatus[] = [DisputeStatus.Processing];
          if (statuses.indexOf(this.dispute?.dispute_status) > -1) {
            isCompleted = true;
          }
          break;
        }
        case StatusStepType.SCHEDULED: {
          let jjStatuses: JJDisputeStatus[] = [
            JJDisputeStatus.HearingScheduled,
            JJDisputeStatus.InProgress,
            JJDisputeStatus.RequireCourtHearing,
            JJDisputeStatus.RequireMoreInfo,
            JJDisputeStatus.Review,
          ];

          if (jjStatuses.indexOf(this.dispute?.jjdispute_status) > -1) {
            isCompleted = true;
          }
          break;
        }
        case StatusStepType.CONFIRMED: {
          let jjStatuses: JJDisputeStatus[] = [JJDisputeStatus.Confirmed];
          if (jjStatuses.indexOf(this.dispute?.jjdispute_status) > -1) {
            isCompleted = true;
          }
          break;
        }
      }
      if (isCompleted || index < lastCompletedStepIndex) {
        step.isCompleted = true;
        lastCompletedStepIndex = lastCompletedStepIndex === 0 ? index : lastCompletedStepIndex;
      }
    })
    this.selectedStep = lastCompletedStepIndex;
  }
}

export interface StatusStep {
  seq: number
  type: StatusStepType
  isCompleted: boolean
}

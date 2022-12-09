import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { QueryParamsForSearch } from '@shared/models/query-params-for-search.model';
import { SearchDisputeResult } from 'app/api';
import { DisputeStore } from 'app/store';

@Component({
  selector: 'app-dispute-status-dialog',
  templateUrl: './dispute-status-dialog.component.html',
  styleUrls: ['./dispute-status-dialog.component.scss']
})
export class DisputeStatusDialogComponent {
  data: SearchDisputeResult;
  params: QueryParamsForSearch;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) private dialogData: DisputeStore.StateData,
  ) {
    this.data = this.dialogData.dispute;
    this.params = this.dialogData.params;
  }
}

import { Component, ViewChild, Input, OnDestroy } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { FileHistory, JJDisputeRemark } from 'app/api';
import { HistoryRecordService } from 'app/services/history-records.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-jj-dispute-remarks',
  templateUrl: './jj-dispute-remarks.component.html',
  styleUrls: ['./jj-dispute-remarks.component.scss'],
})
export class JJDisputeRemarksComponent implements OnDestroy {
  @Input() data: JJDisputeRemark[];
  @ViewChild(MatSort) sort = new MatSort();

  dataSource = new MatTableDataSource<JJDisputeRemark>();
  displayedColumns: string[] = [
    "createdTs",
    "userFullName",
    "note",
  ];
  subscriptions: Subscription[] = [];

  constructor(
    private historyRecordService: HistoryRecordService
  ) {
    this.subscriptions.push(this.historyRecordService.FileHistories$.subscribe(fileHistories => {
      this.refreshData();
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    })
  }

  refreshData(): void {
    // Add ticket validation saving remarks
    this.historyRecordService.FileHistories?.filter(i => i.auditLogEntryType === "FRMK").forEach((fileHistory: FileHistory) => {
      if(this.data && fileHistory.comment) {
        this.data.push(<JJDisputeRemark>{
          createdTs: fileHistory.createdTs,
          userFullName: fileHistory.actionByApplicationUser,
          note: fileHistory.comment
        })
      }      
    })

    this.data = this.data?.sort((a: JJDisputeRemark, b: JJDisputeRemark) => {
      return Date.parse(a.createdTs) - Date.parse(b.createdTs)
    });
    this.dataSource = new MatTableDataSource<JJDisputeRemark>(this.data);
  }
}
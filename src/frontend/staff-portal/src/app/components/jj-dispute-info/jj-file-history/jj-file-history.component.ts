import { Component, OnInit, ViewChild, Input, OnDestroy } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { FileHistory, EmailHistory, EmailHistorySuccessfullySent, JJDisputeRemark } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { HistoryRecordService } from 'app/services/history-records.service';
import { Observable, Subscription, forkJoin } from 'rxjs';


@Component({
  selector: 'app-jj-file-history',
  templateUrl: './jj-file-history.component.html',
  styleUrls: ['./jj-file-history.component.scss'],
})
export class JJFileHistoryComponent implements OnInit, OnDestroy {
  @Input() ticketNumber: string;
  @Input() remarks: JJDisputeRemark[];
  @ViewChild(MatSort) sort = new MatSort();

  dataSource: MatTableDataSource<HistoryRecord> = new MatTableDataSource<HistoryRecord>();
  data: HistoryRecord[] = [];

  fileHistory: FileHistory[] = [];
  emailHistory: EmailHistory[] = [];

  displayedColumns: string[] = [
    "createdTs",
    "actionByApplicationUser",
    "recordType",
    "eventDescription",
  ]

  subscriptions: Subscription[] = [];

  constructor(
    private logger: LoggerService,
    private historyRecordService: HistoryRecordService
  ) {
    this.subscriptions.push(this.historyRecordService.refreshFileHistory.subscribe(ticketNumber => {
      this.getAllHistories();
    }));
  }

  ngOnInit(): void {
    this.getAllHistories();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => {
      subscription.unsubscribe();
    })
  }

  getAllHistories() {
    this.data = [];
    let observables: Observable<any>[] = [];
    observables.push(this.historyRecordService.getFileHistories(this.ticketNumber))
    observables.push(this.historyRecordService.getEmailHistories(this.ticketNumber))

    this.subscriptions.push(
      forkJoin(observables).subscribe({
        next: (responses: any[]) => {
          this.logger.info('FileHistoryComponent::getAllHistories', responses);
          this.fileHistory = <FileHistory[]>responses[0];
          this.emailHistory = <EmailHistory[]>responses[1];
          this.setDisplayHistory();
        }
      }))
  }

  setDisplayHistory() {
    // file history events
    this.fileHistory.forEach(fileHistoryRecord => {
      this.data.push({
        createdTs: fileHistoryRecord.createdTs,
        recordType: "Event",
        actionByApplicationUser: fileHistoryRecord.actionByApplicationUser,
        eventDescription: fileHistoryRecord.description
      })
    });

    // add email history
    this.emailHistory.forEach(emailHistoryRecord => {
      this.data.push({
        createdTs: emailHistoryRecord.createdTs,
        recordType: emailHistoryRecord.successfullySent == EmailHistorySuccessfullySent.Y ? "Email Sent" : "Email Not Sent",
        actionByApplicationUser: emailHistoryRecord.toEmailAddress,
        eventDescription: emailHistoryRecord.subject
      })
    })

    // add remarks
    this.remarks.forEach(remark => {
      this.data.push({
        createdTs: remark.createdTs,
        recordType: "Remark",
        actionByApplicationUser: remark.userFullName,
        eventDescription: remark.note
      })
    })

    this.dataSource.data = this.data;

    // sort by timestamp
    this.dataSource.data = this.dataSource.data.sort((a, b) => {
      return (a.createdTs > b.createdTs) ? 1 : -1;
    })
  }
}

export interface HistoryRecord {
  createdTs?: string;
  recordType: string;
  actionByApplicationUser: string;
  eventDescription?: string;
}

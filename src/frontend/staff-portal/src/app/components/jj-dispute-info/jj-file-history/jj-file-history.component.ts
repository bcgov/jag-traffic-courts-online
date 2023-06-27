import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { FileHistory, EmailHistory, EmailHistorySuccessfullySent } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { HistoryRecordService } from 'app/services/history-records.service';


@Component({
  selector: 'app-jj-file-history',
  templateUrl: './jj-file-history.component.html',
  styleUrls: ['./jj-file-history.component.scss'],
})
export class JJFileHistoryComponent implements OnInit {
  @Input() ticketNumber: string;
  @ViewChild(MatSort) sort = new MatSort();

  dataSource = new MatTableDataSource<HistoryRecord>();

  fileHistory: FileHistory[] = [];
  emailHistory: EmailHistory[] = [];

  displayedColumns: string[] = [
    "createdTs",
    "actionByApplicationUser",
    "recordType",
    "eventDescription",
  ]

  constructor(
    private logger: LoggerService,
    private historyRecordService: HistoryRecordService
  ) {
    this.historyRecordService.refreshFileHistory.subscribe(ticketNumber => {
      this.ticketNumber = ticketNumber;
      this.dataSource = new MatTableDataSource<HistoryRecord>();
      this.getAllFileHistory();
    });
  }

  ngOnInit(): void {
    this.getAllFileHistory();
  }

  getAllFileHistory() {
    this.historyRecordService.getFileHistories(this.ticketNumber).subscribe((response: FileHistory[]) => {
      this.logger.info('FileHistoryComponent::getAllFileHistory', response)
      this.fileHistory = response;
      this.getAllEmailHistory();
    });
  }

  getAllEmailHistory() {
    this.historyRecordService.getEmailHistories(this.ticketNumber).subscribe((response: EmailHistory[]) => {
      this.logger.info('FileHistoryComponent::getAllEmailHistory', response)
      this.emailHistory = response;
      this.setDisplayHistory();
    });
  }

  setDisplayHistory() {

    // file history events
    this.fileHistory.forEach(fileHistoryRecord => {
      this.dataSource.data.push({
        createdTs: new Date(fileHistoryRecord.createdTs),
        recordType: "Event",
        actionByApplicationUser: fileHistoryRecord.actionByApplicationUser,
        eventDescription: fileHistoryRecord.description
       })
    });

    // add email history
    this.emailHistory.forEach(emailHistoryRecord => {
      this.dataSource.data.push({
        createdTs: new Date(emailHistoryRecord.createdTs),
        recordType: emailHistoryRecord.successfullySent == EmailHistorySuccessfullySent.Y ? "Email Sent" : "Email Not Sent",
        actionByApplicationUser: emailHistoryRecord.toEmailAddress,
        eventDescription: emailHistoryRecord.subject
      })
    })

    // sort by timestamp
    this.dataSource.data = this.dataSource.data.sort((a,b) => {
      return (a.createdTs > b.createdTs) ? 1 : -1;
    })
  }
}

export interface HistoryRecord {
  createdTs?: Date;
  recordType: string;
  actionByApplicationUser: string;
  eventDescription?: string;
}

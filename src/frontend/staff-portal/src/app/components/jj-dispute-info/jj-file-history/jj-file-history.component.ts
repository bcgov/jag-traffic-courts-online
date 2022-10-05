import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { FileHistory, EmailHistory, FileHistoryService, EmailHistoryService } from 'app/api';
import { catchError, map } from 'rxjs/operators';
import { LoggerService } from '@core/services/logger.service';


@Component({
  selector: 'app-jj-file-history',
  templateUrl: './jj-file-history.component.html',
  styleUrls: ['./jj-file-history.component.scss'],
})
export class JJFileHistoryComponent implements OnInit {
  @Input() public ticketNumber: string;

  dataSource = new MatTableDataSource<HistoryRecord>();
  @ViewChild(MatSort) sort = new MatSort();

  public fileHistory: FileHistory[] = [];
  public emailHistory: EmailHistory[] = [];

  public displayedColumns: string[] = [
    "createdTs",
    "recordType",
    "eventDescription",
  ]

  constructor(
    private fileHistoryService: FileHistoryService,
    private logger: LoggerService,
    private emailHistorySerivce: EmailHistoryService
  ) {
  }

  ngOnInit(): void {
    this.getAllFileHistory();
  }

  getAllFileHistory() {
    this.fileHistoryService.apiFilehistoryFilehistoryGet(this.ticketNumber)
    .pipe(
      map((response: any) => {
        console.log(this.ticketNumber, this.fileHistory);
        this.logger.info('FileHistoryComponent::getAllFileHistory', response)
        this.fileHistory = response;
        this.getAllEmailHistory();
      }),
      catchError((error: any) => {
        this.logger.error(
          'FileHistoryComponent::getAllFileHistory error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  getAllEmailHistory() {
    this.emailHistorySerivce.apiEmailhistoryEmailhistoryGet(this.ticketNumber)
    .pipe(
      map((response: any) => {
        console.log(this.ticketNumber, this.emailHistory);
        this.logger.info('FileHistoryComponent::getAllEmailHistory', response)
        this.emailHistory = response;
        this.setDisplayHistory();
      }),
      catchError((error: any) => {
        this.logger.error(
          'FileHistoryComponent::getAllEmailHistory error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public setDisplayHistory() {

    // file history events
    this.fileHistory.forEach(fileHistoryRecord => {
      this.dataSource.data.push({
        createdTs: new Date(fileHistoryRecord.createdTs),
        recordType: "Event",
        eventDescription: fileHistoryRecord.description
       })
    });

    // add email history
    this.emailHistory.forEach(emailHistoryRecord => {
      this.dataSource.data.push({
        createdTs: new Date(emailHistoryRecord.createdTs),
        recordType: "Email Sent",
        eventDescription: emailHistoryRecord.subject
      })
    })


    // sort by timestamp
    this.dataSource.data.sort((a,b) => {
      return (a.createdTs > b.createdTs) ? 1 : -1;
    })

    console.log("display history", this.dataSource.data);

  }

}

export interface HistoryRecord {
  createdTs?: Date;
  recordType: string;
  eventDescription?: string;
}

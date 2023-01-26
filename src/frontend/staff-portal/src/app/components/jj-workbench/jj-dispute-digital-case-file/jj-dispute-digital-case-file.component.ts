import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { JJDisputeStatus, JJDisputeHearingType } from 'app/api';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-jj-dispute-digital-case-file',
  templateUrl: './jj-dispute-digital-case-file.component.html',
  styleUrls: ['./jj-dispute-digital-case-file.component.scss']
})
export class JJDisputeDigitalCaseFileComponent implements OnInit, AfterViewInit {
  @Input() data$: Observable<JJDispute[]>;
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  HearingType = JJDisputeHearingType;
  jjAssignedToFilter: string;
  tableHeight: number = window.innerHeight - 300; // less size of other fixed elements
  filterText: string;
  data = [] as JJDispute[];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
  displayedColumns: string[] = [
    "ticketNumber",
    "dateSubmitted",
    "violationDate",
    "fullName",
    "courthouseLocation",
    "status",
  ];

  constructor(
    private jjDisputeService: JJDisputeService,
  ) {
    this.dataSource.filterPredicate = function (record, filter) {
      return record.fullName?.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1
        || record.ticketNumber?.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }
  }

  calcTableHeight(heightOther) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1)*60)
  }

  ngOnInit(): void {
    this.data$.subscribe(data => {
      this.refreshData(data);
    })
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  applyFilter() {
    this.dataSource.filter = this.filterText;
    this.tableHeight = this.calcTableHeight(300);
  }

  refreshData(jjDisputes: JJDispute[]): void {
    this.data = jjDisputes;
    // only show status NEW, IN_PROGRESS, CONFIRMED, REVIEW, REQUIRE_COURT_HEARING, REQUIRE_MORE_INFO
    this.data = this.data.filter(x => x.status);
    this.dataSource.data = this.data;
    console.log(this.data, this.dataSource.data);
    // initially sort by submitted date within status
    this.dataSource.data = this.dataSource.data.sort((a, b) => {
      // if they have the same status
      if (a.status === b.status) {
        if (a.submittedTs > b.submittedTs) { return 1; } else { return -1; }
      }

      // compare statuses
      else {
        if (this.jjDisputeService.jjDisputeStatusesSorted.indexOf(a.status) > this.jjDisputeService.jjDisputeStatusesSorted.indexOf(b.status)) { return 1; } else { return -1; }
      }
    });

    this.applyFilter();
  }
}

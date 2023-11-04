import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { JJDisputeHearingType, JJDisputeStatus } from 'app/api';
import { Observable, Subscription } from 'rxjs';
import { DateUtil } from '@shared/utils/date-util';

@Component({
  selector: 'app-jj-dispute-digital-case-file',
  templateUrl: './jj-dispute-digital-case-file.component.html',
  styleUrls: ['../../../app.component.scss', './jj-dispute-digital-case-file.component.scss']
})
export class JJDisputeDigitalCaseFileComponent implements OnInit, AfterViewInit {
  @Input() data$: Observable<JJDispute[]>;
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  HearingType = JJDisputeHearingType;
  jjAssignedToFilter: string;
  tableHeight: number = window.innerHeight - 350; // less size of other fixed elements
  filterText: string;
  // data = [] as JJDispute[];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
  dataFilters = {
    "dateFrom": "",
    "dateTo": "",
    "ticketNumber": "",
    "occamDisputantName": "",
    "courthouseLocation": ""
  };
  displayedColumns: string[] = [
    "ticketNumber",
    "submittedTs",
    "violationDate",
    "occamDisputantName",
    "courthouseLocation",
    "status",
  ];
  subscription: Subscription;

  constructor(
    private jjDisputeService: JJDisputeService,
  ) {
    this.dataSource.filterPredicate = this.searchFilter;
  }

  calcTableHeight(heightOther: number) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 60)
  }

  ngOnInit(): void {    
    if (!this.subscription) {
      this.subscription = this.data$.subscribe(data => {
        this.dataSource.data = data;
        // this.dataSource.data = this.sampleData();
      });
    }
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  onApplyFilter() {
    this.dataSource.filter = JSON.stringify(this.dataFilters);
    this.tableHeight = this.calcTableHeight(350);
  }

  resetSearchFilters() {
    // Will update search filters in UI
    this.dataFilters = {
      "dateFrom": "",
      "dateTo": "",
      "ticketNumber": "",
      "occamDisputantName": "",
      "courthouseLocation": ""
    };

    // Will re-execute the filter function, but will block UI rendering
    // Put this call in a Timeout to keep UI responsive.
    setTimeout(() => {
      //this.data = this.sampleData();
      this.dataSource.filter = "{}";
      this.tableHeight = this.calcTableHeight(350);
    }, 100);
  }
    
  searchFilter = function (record: JJDispute, filter: string) {
    let searchTerms = JSON.parse(filter);
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("dateFrom" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrAfter(record.submittedTs, value);
      }
      else if ("dateTo" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrBefore(record.submittedTs, value);
      }
      else {
        return record[field].toLocaleLowerCase().indexOf(value.trim().toLocaleLowerCase()) != -1;
      }
    });
  };
  
  // for development/testing
  sampleData() {
    let sampleData: JJDispute[] = [
      {
        ticketNumber: "AK12345678",
        submittedTs: "2023-10-07",
        jjDecisionDate: "2023-10-31",
        jjAssignedToName: "Sam Smith",
        violationDate: "2023-09-15",
        occamDisputantName: "Timmons, Tim",
        courthouseLocation: "Vancouver Law Courts",
        status: JJDisputeStatus.Confirmed
      },
      {
        ticketNumber: "AX11112222",
        submittedTs: "2023-09-25",
        jjDecisionDate: "2023-10-15",
        jjAssignedToName: "Jon Jones",
        violationDate: "2023-09-20",
        occamDisputantName: "Russel, Rick",
        courthouseLocation: "North Vancouver Court",
        status: JJDisputeStatus.Confirmed
      },
      {
        ticketNumber: "AJ11223344",
        submittedTs: "2023-10-08",
        jjDecisionDate: "2023-10-25",
        jjAssignedToName: "Amanda Ada",
        violationDate: "2023-09-25",
        occamDisputantName: "Zeldic, Zack",
        courthouseLocation: "Port Coquitlam Court",
        status: JJDisputeStatus.Confirmed
      }
    ];
    return sampleData;
  }
}

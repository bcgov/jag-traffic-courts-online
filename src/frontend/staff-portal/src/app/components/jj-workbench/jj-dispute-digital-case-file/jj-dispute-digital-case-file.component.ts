import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { JJDispute } from 'app/services/jj-dispute.service';
import { JJDisputeHearingType } from 'app/api';
import { Observable, Subscription } from 'rxjs';
import { DateUtil } from '@shared/utils/date-util';
import { TableFilter, TableFilterKeys } from '@shared/models/table-filter-options.model';
import { TableFilterService } from 'app/services/table-filter.service';

@Component({
  selector: 'app-jj-dispute-digital-case-file',
  templateUrl: './jj-dispute-digital-case-file.component.html',
  styleUrls: ['./jj-dispute-digital-case-file.component.scss']
})
export class JJDisputeDigitalCaseFileComponent implements OnInit, AfterViewInit {
  @Input() data$: Observable<JJDispute[]>;
  @Input() tabIndex: number;
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  HearingType = JJDisputeHearingType;
  jjAssignedToFilter: string;
  filterText: string;
  // data = [] as JJDispute[];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
  tableFilterKeys: TableFilterKeys[] = ["dateSubmittedFrom", "dateSubmittedTo", "occamDisputantName", "courthouseLocation"];

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
    private tableFilterService: TableFilterService,
  ) {
    this.dataSource.filterPredicate = this.searchFilter;
  }

  ngOnInit(): void {    
    if (!this.subscription) {
      this.subscription = this.data$.subscribe(data => {
        this.dataSource.data = data;
        this.onApplyFilter(this.tableFilterService.tableFilters[this.tabIndex]);
      });
    }
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  onApplyFilter(dataFilters: TableFilter) {
    this.dataSource.filter = JSON.stringify(dataFilters);
  }
    
  searchFilter = function (record: JJDispute, filter: string) {
    let searchTerms = JSON.parse(filter);
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("dateSubmittedFrom" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrAfter(record.submittedTs, value);
      }
      else if ("dateSubmittedTo" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrBefore(record.submittedTs, value);
      }
      else {
        return record[field].toLocaleLowerCase().indexOf(value.trim().toLocaleLowerCase()) != -1;
      }
    });
  };
}

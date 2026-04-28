import { Component, OnInit, ViewChild, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { DisputeCaseFileSummary, PagedDisputeCaseFileSummaryCollection, SortDirection } from 'app/api';
import { TableFilter, TableFilterKeys } from '@shared/models/table-filter-options.model';
import { TableFilterService } from 'app/services/table-filter.service';
import { LoggerService } from '@core/services/logger.service';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';

@Component({
  selector: 'app-jj-dispute-digital-case-file',
  templateUrl: './jj-dispute-digital-case-file.component.html',
  styleUrls: ['./jj-dispute-digital-case-file.component.scss'],
  standalone: false,
})
export class JJDisputeDigitalCaseFileComponent implements OnInit {
  @Input() tabIndex: number;
  @Output() tcoDisputeInfo: EventEmitter<DisputeCaseFileSummary> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  tcoDisputes: DisputeCaseFileSummary[] = [];
  tcoDisputesCollection: PagedDisputeCaseFileSummaryCollection = {};
  dataSource = new MatTableDataSource(this.tcoDisputes);
  tableFilterKeys: TableFilterKeys[] = ["dateSubmittedFrom", "dateSubmittedTo", "surname", 
    "courthouseLocation", "ticketNumber"];

  displayedColumns: string[] = [
    "ticketNumber",
    "submittedTs",
    "violationDate",
    "surnameOrOrgName",
    "toBeHeardAtCourthouseName",
    "status",
  ];
  currentPage: number = 1;
  totalPages: number = 1;
  sortBy: string = "submittedTs";
  sortDirection: SortDirection = SortDirection.Desc;
  filters: TableFilter = new TableFilter();
  previousFilters: TableFilter = new TableFilter();
  disputeStatus = DisputeStatus;

  constructor(
    private tableFilterService: TableFilterService,
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService
  ) {
  }

  ngOnInit(): void {    
    let dataFilter: TableFilter = this.tableFilterService.tableFilters[this.tabIndex];
    //dataFilter.status = dataFilter.status ?? "";
    this.filters = dataFilter; 
    this.previousFilters = { ...dataFilter };
    this.currentPage = this.tableFilterService.currentPage[this.tabIndex];
    this.getTCODisputes();    
  }

  getTCODisputes() {
    this.logger.log('JJDisputeDigitalCaseFileComponent::getTCODisputes');
    const params = {
      submittedFrom: this.filters.dateSubmittedFrom,
      submittedThru: this.filters.dateSubmittedTo,
      ticketNumber: this.filters.ticketNumber ? this.filters.ticketNumber.toUpperCase() : "",
      surnameOrOrgName: this.filters.surname ?? "",
      toBeHeardAtCourthouseIds: this.filters.courthouseLocation && this.filters.courthouseLocation.length > 0 ? 
        this.filters.courthouseLocation.map(x => x.id).join(",") : "",
      sortBy: this.sortDirection === SortDirection.Asc ? this.sortBy : "-" + this.sortBy,
      pageNumber: this.currentPage,
      pageSize: 25,
      fetchPendingAdjournments: false,
    };
    this.jjDisputeService.getTCODisputes(params).subscribe((response) => {
      this.tcoDisputes = [];
      this.logger.log('JJDisputeDigitalCaseFileComponent::getTCODisputes response');
      this.tcoDisputesCollection = response;
      this.currentPage = response.pageNumber;
      this.totalPages = response.totalPages;
      if(!this.totalPages){
        this.currentPage = 0;
      }
      this.tcoDisputes = response.items;
      this.dataSource.data = this.tcoDisputes;
    });
  }

  backWorkbench(element: DisputeCaseFileSummary) {
    this.tcoDisputeInfo.emit(element);
  }

  onApplyFilter(dataFilters: TableFilter) {
    if (JSON.stringify(this.previousFilters) !== JSON.stringify(dataFilters)) { // Add this line
      this.currentPage = 1;
      this.tableFilterService.currentPage[this.tabIndex] = 1;
    }
    this.filters = dataFilters;
    this.previousFilters = { ...dataFilters };
    this.getTCODisputes();
  }

  sortData(sort: Sort){
    this.sortBy = sort.active;
    this.sortDirection = sort.direction ? sort.direction as SortDirection : SortDirection.Desc;
    this.currentPage = 1;
    this.tableFilterService.currentPage[this.tabIndex] = 1;
    this.getTCODisputes();
  }

  onPageChange(event: number) {
    this.currentPage = event;
    this.tableFilterService.currentPage[this.tabIndex] = event;
    this.getTCODisputes();
  }

  isConcludedStatus(status: DisputeStatus): boolean {
    const concludedStatuses = new Set([
      DisputeStatus.Accepted,
      DisputeStatus.Cancelled,
      DisputeStatus.Concluded]);
    return concludedStatuses.has(status);
  }
}
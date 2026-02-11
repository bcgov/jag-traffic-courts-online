import { Component, OnInit, ViewChild, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { DisputeCaseFileSummary, PagedDisputeCaseFileSummaryCollection, SortDirection } from 'app/api';
import { AuthService } from 'app/services/auth.service';
import { LookupsService } from 'app/services/lookups.service';
import { TableFilter, TableFilterKeys } from '@shared/models/table-filter-options.model';
import { TableFilterService } from 'app/services/table-filter.service';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';

@Component({
  selector: 'app-dispute-decision-inbox',
  templateUrl: './dispute-decision-inbox.component.html',
  styleUrls: ['./dispute-decision-inbox.component.scss'],
})
export class DisputeDecisionInboxComponent implements OnInit {
  @Input() tabIndex: number;
  courthouseTeamNames = ["A", "B", "C", "D"];

  @Output() tcoDisputeInfo: EventEmitter<DisputeCaseFileSummary> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  IDIR: string = "";
  courthouseTeams = {};
  tcoDisputes: DisputeCaseFileSummary[] = [];
  tcoDisputesCollection: PagedDisputeCaseFileSummaryCollection = {};
  dataSource = new MatTableDataSource(this.tcoDisputes);
  tableFilterKeys: TableFilterKeys[] = ["decisionDateFrom", "decisionDateTo", "surname", "courthouseLocation", "ticketNumber", "team"];

  displayedColumns: string[] = [
    "ticketNumber",
    "jjDecisionDate",
    "signatoryName",
    "violationDate",
    "surnameOrOrgName",
    "toBeHeardAtCourthouseName",
    "appearanceRoomCode",
    "status",
    "vtcAssignedTo"
  ];
  currentPage: number = 1;
  totalPages: number = 1;
  sortBy: string = "jjDecisionDate";
  sortDirection: SortDirection = SortDirection.Asc;
  filters: TableFilter = new TableFilter();
  previousFilters: TableFilter = new TableFilter();
  disputeStatus = DisputeStatus;

  constructor(
    private logger: LoggerService,
    private authService: AuthService,
    private lookupsService: LookupsService,
    private tableFilterService: TableFilterService,
    private jjDisputeService: JJDisputeService
  ) {
    this.getCourthouseAgencyIds();
  }

  getCourthouseAgencyIds() {
    this.courthouseTeamNames.forEach(teamName => {
      let matchingTeams = this.lookupsService.courthouseTeams.filter(x => x.__team === teamName);
      let ids = matchingTeams.flatMap(team => 
        this.lookupsService.courthouseAgencies.filter(agency => agency.name.toLowerCase() === team.name.toLowerCase()).map(agency => agency.id));
      this.courthouseTeams[teamName] = ids;
    });
  }

  public ngOnInit() {
    let dataFilter: TableFilter = this.tableFilterService.tableFilters[this.tabIndex];
    //dataFilter.status = dataFilter.status ?? "";
    this.filters = dataFilter;
    this.previousFilters = { ...dataFilter };
    this.currentPage = this.tableFilterService.currentPage[this.tabIndex];
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.IDIR = userProfile.idir;
        this.getTCODisputes();
      }
    });
  }

  backWorkbench(value: DisputeCaseFileSummary) {
    this.tcoDisputeInfo.emit(value);
  }

  getTCODisputes(): void {
    this.logger.log('JJDisputeDecisionInboxComponent::getTCODisputes');

    const params = {
      appearances: true,
      jjDecisionDtFrom: this.filters.decisionDateFrom,
      jjDecisionDtThru: this.filters.decisionDateTo,
      ticketNumber: this.filters.ticketNumber ? this.filters.ticketNumber.toUpperCase() : "",
      surnameOrOrgName: this.filters.surname ?? "",
      disputeStatusCodes: DisputeStatus.Confirmed + "," + DisputeStatus.RequireCourtHearing,
      toBeHeardAtCourthouseIds: this.filters.courthouseLocation && this.filters.courthouseLocation.length > 0 ? 
        this.filters.courthouseLocation.map(x => x.id).join(",") : 
        (this.filters.team ? this.courthouseTeams[this.filters.team].join(",") : ""),
      sortBy: this.sortDirection === SortDirection.Asc ? this.sortBy : "-" + this.sortBy,
      pageNumber: this.currentPage,
      pageSize: 25
    };
    this.jjDisputeService.getTCODisputes(params).subscribe((response) => {
      this.tcoDisputes = [];
      this.logger.log('JJDisputeDecisionInboxComponent::getTCODisputes response');
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
}


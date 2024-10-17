import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { Observable, filter } from 'rxjs';
import { JJDisputeStatus } from 'app/api';
import { AuthService } from 'app/services/auth.service';
import { LookupsService } from 'app/services/lookups.service';
import { AppState } from 'app/store/app.state';
import { Store } from '@ngrx/store';
import { DateUtil } from '@shared/utils/date-util';
import { TableFilter, TableFilterKeys } from '@shared/models/table-filter-options.model';
import { TableFilterService } from 'app/services/table-filter.service';

@Component({
  selector: 'app-dispute-decision-inbox',
  templateUrl: './dispute-decision-inbox.component.html',
  styleUrls: ['./dispute-decision-inbox.component.scss'],
})
export class DisputeDecisionInboxComponent implements OnInit, AfterViewInit {
  @Input() tabIndex: number;
  courthouseTeamNames = ["A", "B", "C", "D"];

  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  IDIR: string = "";
  currentTeam: string = "All";
  courthouseTeams = {};
  data$: Observable<JJDispute[]>;
  data = [] as JJDispute[];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
  tableFilterKeys: TableFilterKeys[] = ["decisionDateFrom", "decisionDateTo", "occamDisputantName", "courthouseLocation", "ticketNumber", "team"];

  displayedColumns: string[] = [
    "ticketNumber",
    "jjDecisionDate",
    "jjAssignedTo",
    "violationDate",
    "occamDisputantName",
    "courthouseLocation",
    "mostRecentCourtAppearance.room",
    "status",
    "vtcAssignedTo"
  ];

  constructor(
    private logger: LoggerService,
    private authService: AuthService,
    private lookupsService: LookupsService,
    private store: Store<AppState>,
    private tableFilterService: TableFilterService,
  ) {
    this.data$ = this.store.select(state => state.jjDispute.data).pipe( filter(i => !!i));
    this.courthouseTeamNames.forEach(teamName => {
      this.courthouseTeams[teamName] = this.lookupsService.courthouseTeams.filter(x => x.__team === teamName).map(x => x.name.toLocaleLowerCase());
    })
  }

  public ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.IDIR = userProfile.idir;
        this.data$.subscribe(jjDisputes => {
          this.data = jjDisputes
            .map(jjDispute => {
              return {
                ...jjDispute,
                __status: jjDispute.status
                  .replace(/^[-_]*(.)/, (_, c) => c)
                  .replace(/[-_]+(.)/g, (_, c) => ' ' + c)
                  .replace(
                    /\w\S*/g,
                    function (txt) {
                      return txt.charAt(0).toUpperCase() + txt.substring(1).toLowerCase();
                    })
              };
            });
          this.getAll();
        })
      }
    });
  }

  onApplyFilter(dataFilters: TableFilter) {
    this.dataSource.filter = JSON.stringify(dataFilters);
  }

  searchFilter = function (record: JJDispute, filter: string) {
    let searchTerms = JSON.parse(filter);
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("decisionDateFrom" === field) {
        return !value || !DateUtil.isValid(value) || DateUtil.isDateOnOrAfter(record.jjDecisionDate, value);
      }
      else if ("decisionDateTo" === field) {
        return !value || !DateUtil.isValid(value) || DateUtil.isDateOnOrBefore(record.jjDecisionDate, value);
      }
      else if ("team" === field) {
        if (value === '' || value === "All") {
          return true;
        }
        else if (this.courthouseTeamNames.includes(value)) {
          return this.courthouseTeams[value].includes(record.courthouseLocation.toLocaleLowerCase());
        }
        return false;
      }
      else if (record[field]) {
        return record[field].toLocaleLowerCase().indexOf(value.trim().toLocaleLowerCase()) != -1;
      }
      return true;
    });
  }.bind(this);

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backWorkbench(value: JJDispute) {
    this.jjDisputeInfo.emit(value);
  }

  getAll(): void {
    this.logger.log('JJDisputeDecisionInboxComponent::getAllDisputes');

    // filter jj disputes only show those in CONFIRMED status or REQUIRE_COURT_HEARING
    this.data = this.data.filter(x => x.status == JJDisputeStatus.Confirmed || x.status === JJDisputeStatus.RequireCourtHearing);
    this.dataSource.data = this.data;
    this.dataSource.filterPredicate = this.searchFilter;

    // initially sort by decision date within status
    this.dataSource.data = this.dataSource.data.sort((a, b) => {
      if (a.jjDecisionDate > b.jjDecisionDate) { return 1; } else { return -1; }
    });

    this.onApplyFilter(this.tableFilterService.tableFilters[this.tabIndex]);
  }
}

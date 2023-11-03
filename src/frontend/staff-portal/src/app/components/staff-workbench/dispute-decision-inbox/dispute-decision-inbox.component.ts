import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { Observable, filter } from 'rxjs';
import { JJDisputeStatus } from 'app/api';
import { AuthService } from 'app/services/auth.service';
import { LookupsService } from 'app/services/lookups.service';
import { AppState } from 'app/store/app.state';
import { Store, select } from '@ngrx/store';
import { DateUtil } from '@shared/utils/date-util';

@Component({
  selector: 'app-dispute-decision-inbox',
  templateUrl: './dispute-decision-inbox.component.html',
  styleUrls: ['../../../app.component.scss', './dispute-decision-inbox.component.scss'],
})
export class DisputeDecisionInboxComponent implements OnInit, AfterViewInit {
  private courthouseTeamNames = ["A", "B", "C", "D"];

  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  tableHeight: number = window.innerHeight - 325; // less size of other fixed elements

  IDIR: string = "";
  currentTeam: string = "All";
  courthouseTeams = {};
  data$: Observable<JJDispute[]>;
  data = [] as JJDispute[];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
  dataFilters = {
    "dateFrom": "",
    "dateTo": "",
    "ticketNumber": "",
    "occamDisputantName": "",
    "courthouseLocation": "",
    "team": ""
  };
  displayedColumns: string[] = [
    "ticketNumber",
    "jjDecisionDate",
    "jjAssignedTo",
    "violationDate",
    "fullName",
    "courthouseLocation",
    "status",
    "vtcAssignedTo"
  ];

  constructor(
    private logger: LoggerService,
    private authService: AuthService,
    private lookupsService: LookupsService,
    private store: Store<AppState>
  ) {
    this.data$ = this.store.pipe(select(state => state.jjDispute.data), filter(i => !!i));
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
          
          // load dataFilters (Decision Validation tab) in session for page reload
          let __dataFilter = sessionStorage.getItem("dataFilters-DV");
          if (__dataFilter) {
            this.dataFilters = JSON.parse(__dataFilter);
          }
          
          this.getAll();
        })
      }
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  onApplyFilter(filterName: string, value: string) {    
    if (filterName) {
      const filterValue = value;
      this.dataFilters[filterName] = filterValue;
    }
    this.dataSource.filter = JSON.stringify(this.dataFilters);
    
    // cache dataFilters (for Decision Validation tab) in session for page reload
    sessionStorage.setItem("dataFilters-DV", this.dataSource.filter);

    this.tableHeight = this.calcTableHeight(352);
  }

  resetSearchFilters() {
    // Will update search filters in UI
    this.dataFilters = {
      "dateFrom": "",
      "dateTo": "",
      "ticketNumber": "",
      "occamDisputantName": "",
      "courthouseLocation": "",
      "team": ""
    };

    // Will re-execute the filter function, but will block UI rendering
    // Put this call in a Timeout to keep UI responsive.
    setTimeout(() => {
      this.dataSource.filter = "{}";
      this.tableHeight = this.calcTableHeight(352);
    }, 100);
  }

  searchFilter = function (record: JJDispute, filter: string) {
    let searchTerms = JSON.parse(filter);
    return Object.entries(searchTerms).every(([field, value]: [string, string]) => {
      if ("dateFrom" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrAfter(record.jjDecisionDate, value);
      }
      else if ("dateTo" === field) {
        return !DateUtil.isValid(value) || DateUtil.isDateOnOrBefore(record.jjDecisionDate, value);
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
      else {
        return record[field].toLocaleLowerCase().indexOf(value.trim().toLocaleLowerCase()) != -1;
      }
    });
  }.bind(this);

  calcTableHeight(heightOther: number) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 60)
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

    this.onApplyFilter(null, null);
  }
}

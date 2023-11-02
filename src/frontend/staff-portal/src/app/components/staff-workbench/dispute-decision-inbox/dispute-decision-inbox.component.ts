import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { Observable, filter } from 'rxjs';
import { JJDisputeStatus } from 'app/api';
import { AuthService } from 'app/services/auth.service';
import { CourthouseTeam, LookupsService } from 'app/services/lookups.service';
import { AppState } from 'app/store/app.state';
import { Store, select } from '@ngrx/store';
import { DateUtil } from '@shared/utils/date-util';

@Component({
  selector: 'app-dispute-decision-inbox',
  templateUrl: './dispute-decision-inbox.component.html',
  styleUrls: ['../../../app.component.scss', './dispute-decision-inbox.component.scss'],
})
export class DisputeDecisionInboxComponent implements OnInit, AfterViewInit {
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  tableHeight: number = window.innerHeight - 325; // less size of other fixed elements

  IDIR: string = "";
  currentTeam: string = "All";
  courthouseTeams = {"A": [], "B": [], "C": [], "D": [] };
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
    "vtcAssignedTo"
  ];

  constructor(
    private logger: LoggerService,
    private authService: AuthService,
    private lookupsService: LookupsService,
    private store: Store<AppState>
  ) {
    this.data$ = this.store.pipe(select(state => state.jjDispute.data), filter(i => !!i));
    this.courthouseTeams['A'] = lookupsService.courthouseTeams.filter(x => x.__team === 'A').map(x => x.name.toLocaleLowerCase());
    this.courthouseTeams['B'] = lookupsService.courthouseTeams.filter(x => x.__team === 'B').map(x => x.name.toLocaleLowerCase());
    this.courthouseTeams['C'] = lookupsService.courthouseTeams.filter(x => x.__team === 'C').map(x => x.name.toLocaleLowerCase());
    this.courthouseTeams['D'] = lookupsService.courthouseTeams.filter(x => x.__team === 'D').map(x => x.name.toLocaleLowerCase());
  }

  public ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.IDIR = userProfile.idir;
        this.data$.subscribe(jjDisputes => {
          this.data = jjDisputes
            .map(jjDispute => { return { ...jjDispute } });
          this.getAll();
        })
      }
    });
  }

  onApplyFilter(filterName: string, value: string) {
    const filterValue = value;
    this.dataFilters[filterName] = filterValue;
    this.dataSource.filter = JSON.stringify(this.dataFilters);
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
      //this.data = this.sampleData();
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
        else if (['A', 'B', 'C', 'D'].includes(value)) {
          return this.courthouseTeams[value].includes(record['courthouseLocation'].toLocaleLowerCase());
        }
        return false;
      }
      else {
        return record[field].toLocaleLowerCase().indexOf(value.trim().toLocaleLowerCase()) != -1;
      }
    });
  }.bind(this);

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  calcTableHeight(heightOther: number) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 60)
  }

  backWorkbench(value: JJDispute) {
    this.jjDisputeInfo.emit(value);
  }

  getAll(): void {
    this.logger.log('JJDisputeDecisionInboxComponent::getAllDisputes');

    //this.data = this.sampleData();

    // filter jj disputes only show those in CONFIRMED status or REQUIRE_COURT_HEARING
    this.data = this.data.filter(x => x.status == JJDisputeStatus.Confirmed || x.status === JJDisputeStatus.RequireCourtHearing);
    this.dataSource.data = this.data;
    this.dataSource.filterPredicate = this.searchFilter;

    // initially sort by decision date within status
    this.dataSource.data = this.dataSource.data.sort((a, b) => {
      if (a.jjDecisionDate > b.jjDecisionDate) { return 1; } else { return -1; }
    });

    this.tableHeight = this.calcTableHeight(325);
  }

  // for development/testing
  sampleData() {
    let sampleData: JJDispute[] = [
      {
        ticketNumber: "AK12345678",
        jjDecisionDate: "2023-10-31",
        jjAssignedToName: "Sam Smith",
        violationDate: "2023-09-15",
        occamDisputantName: "Timmons, Tim",
        courthouseLocation: "Vancouver Law Courts",
        status: JJDisputeStatus.Confirmed
      },
      {
        ticketNumber: "AX11112222",
        jjDecisionDate: "2023-10-15",
        jjAssignedToName: "Jon Jones",
        violationDate: "2023-09-20",
        occamDisputantName: "Russel, Rick",
        courthouseLocation: "North Vancouver Court",
        status: JJDisputeStatus.Confirmed
      },
      {
        ticketNumber: "AJ11223344",
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

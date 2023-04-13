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

@Component({
  selector: 'app-dispute-decision-inbox',
  templateUrl: './dispute-decision-inbox.component.html',
  styleUrls: ['./dispute-decision-inbox.component.scss'],
})
export class DisputeDecisionInboxComponent implements OnInit, AfterViewInit {
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  tableHeight: number = window.innerHeight - 325; // less size of other fixed elements

  IDIR: string = "";
  currentTeam: string = "All";
  data$: Observable<JJDispute[]>;
  data = [] as JJDispute[];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
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
  }

  filterByTeam(team: string) {
    let teamCourthouses = this.lookupsService.courthouseTeams.filter(x => (x.__team === team || team === "All"));
    this.dataSource.data = this.data.filter(x => teamCourthouses.filter(y => y.name === x.courthouseLocation || y.id === x.courthouseLocation).length > 0);
    this.tableHeight = this.calcTableHeight(325);
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

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  calcTableHeight(heightOther) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 60)
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  getAll(): void {
    this.logger.log('JJDisputeDecisionInboxComponent::getAllDisputes');

    // filter jj disputes only show those in CONFIRMED status or REQUIRE_COURT_HEARING
    this.data = this.data.filter(x => x.status == JJDisputeStatus.Confirmed || x.status === JJDisputeStatus.RequireCourtHearing);
    this.dataSource.data = this.data;

    // initially sort by decision date within status
    this.dataSource.data = this.dataSource.data.sort((a, b) => {
      if (a.jjDecisionDate > b.jjDecisionDate) { return 1; } else { return -1; }
    });
    this.tableHeight = this.calcTableHeight(325);
  }
}

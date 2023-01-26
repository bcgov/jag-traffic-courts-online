import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { CourthouseConfig } from '@config/config.model';
import { JJDisputeStatus } from 'app/api';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-dispute-decision-inbox',
  templateUrl: './dispute-decision-inbox.component.html',
  styleUrls: ['./dispute-decision-inbox.component.scss'],
})
export class DisputeDecisionInboxComponent implements OnInit, AfterViewInit {
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  tableHeight: number = window.innerHeight - 325; // less size of other fixed elements

  busy: Subscription;
  courtLocations: CourthouseConfig[];
  IDIR: string = "";
  currentTeam: string = "All";
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
    private jjDisputeService: JJDisputeService,
    private authService: AuthService,
    private mockConfigService: MockConfigService,
  ) {
    if (this.mockConfigService.courtLocations) {
      this.courtLocations = this.mockConfigService.courtLocations;
    }
    this.jjDisputeService.refreshDisputes.subscribe(x => { this.getAll(); })
  }

  filterByTeam(team: string) {
    let teamCourthouses = this.courtLocations.filter(x => (x.jjTeam === team || team === "All"));
    this.dataSource.data = this.data.filter(x => teamCourthouses.filter(y => y.name === x.courthouseLocation).length > 0);
    this.tableHeight = this.calcTableHeight(325);
  }

  public ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.IDIR = userProfile.idir;
      }
    });
    this.getAll();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  calcTableHeight(heightOther) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1)*60)
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  getAll(): void {
    this.logger.log('JJDisputeDecisionInboxComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputes().subscribe((response) => {
      // filter jj disputes only show those in CONFIRMED status or REQUIRE_COURT_HEARING
      this.data = response.filter(x => x.status == JJDisputeStatus.Confirmed || x.status === JJDisputeStatus.RequireCourtHearing);
      this.dataSource.data = this.data;

      // initially sort by decision date within status
      this.dataSource.data = this.dataSource.data.sort((a, b) => {
        if (a.jjDecisionDate > b.jjDecisionDate) { return 1; } else { return -1; }
      });
      this.tableHeight = this.calcTableHeight(325);
    });
  }
}

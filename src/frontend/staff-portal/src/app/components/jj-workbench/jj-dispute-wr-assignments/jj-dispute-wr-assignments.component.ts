import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { CourthouseConfig } from '@config/config.model';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { LoggerService } from '@core/services/logger.service';
import { filter, Observable, Subscription } from 'rxjs';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { JJDisputeHearingType } from 'app/api';
import { UserRepresentation } from 'app/services/auth.service';
import { AppState } from 'app/store';
import { select, Store } from '@ngrx/store';

@Component({
  selector: 'app-jj-dispute-wr-assignments',
  templateUrl: './jj-dispute-wr-assignments.component.html',
  styleUrls: ['./jj-dispute-wr-assignments.component.scss'],
})
export class JJDisputeWRAssignmentsComponent implements OnInit, AfterViewInit {
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  busy: Subscription;
  data$: Observable<JJDispute[]>;
  data = [] as JJDispute[];
  courtLocations: CourthouseConfig[];
  currentTeam: string = "A";
  valueOfUnassigned: string = "";
  bulkjjAssignedTo: string = this.valueOfUnassigned;
  HearingType = JJDisputeHearingType;
  teamCounts: teamCounts[] = [];
  jjList: UserRepresentation[];
  assignedDataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
  unassignedDataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
  displayedColumns: string[] = [
    "assignedIcon",
    "jjAssignedTo",
    "bulkAssign",
    "ticketNumber",
    "submittedTs",
    "fullName",
    "courthouseLocation",
    "policeDetachment",
    "timeToPayReason",
  ];

  constructor(
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private mockConfigService: MockConfigService,
    private store: Store<AppState>
  ) {
    this.jjDisputeService.jjList$.subscribe(result => {
      this.jjList = result;
    });

    if (this.mockConfigService.courtLocations) {
      this.courtLocations = this.mockConfigService.courtLocations;
    }

    this.data$ = this.store.pipe(select(state => state.jjDispute.data), filter(i => !!i));
    this.data$.subscribe(jjDisputes => {
      this.data = jjDisputes.map(jjDispute => { return { ...jjDispute } });
      this.getAll("A");
    })
  }

  ngOnInit(): void {
    this.getAll("A");
  }

  ngAfterViewInit() {
    this.assignedDataSource.sort = this.sort;
    this.unassignedDataSource.sort = this.sort;
  }

  sortData(event: Sort) {
    this.assignedDataSource.sort = this.sort;
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  getType(element: JJDispute): string {
    if (element.timeToPayReason && element.fineReductionReason)
      return "Time to pay/Fine";
    else if (element.timeToPayReason)
      return "Time to pay";
    else return "Fine";
  }

  filterByTeam(team: string) {
    let teamCourthouses = this.courtLocations.filter(x => x.jjTeam === team);
    this.assignedDataSource.data = this.data.filter(x => x.jjAssignedTo && x.jjAssignedTo !== this.valueOfUnassigned && teamCourthouses.filter(y => y.name === x.courthouseLocation).length > 0);
    this.unassignedDataSource.data = this.data.filter(x => (!x.jjAssignedTo || x.jjAssignedTo === this.valueOfUnassigned) && teamCourthouses.filter(y => y.name === x.courthouseLocation).length > 0);
    this.currentTeam = team;
  }

  getCurrentTeamCounts(): teamCounts {
    return this.teamCounts.filter(x => x.team == this.currentTeam)[0];
  }

  getTeamCount(team: string): teamCounts {
    let teamCourthouses = this.courtLocations.filter(x => x.jjTeam === team);
    let teamDisputes = this.data.filter(x => teamCourthouses.filter(y => y.name === x.courthouseLocation).length > 0);
    let teamCounts = { team: team, assignedCount: 0, unassignedCount: 0 } as teamCounts;
    if (teamDisputes) {
      let unassignedTeamCounts = teamDisputes.filter(x => !x.jjAssignedTo || x.jjAssignedTo === this.valueOfUnassigned);
      if (unassignedTeamCounts.length > 0) teamCounts.unassignedCount = unassignedTeamCounts.length;
      let assignedTeamCounts = teamDisputes.filter(x => x.jjAssignedTo && x.jjAssignedTo !== this.valueOfUnassigned);
      if (assignedTeamCounts.length > 0) teamCounts.assignedCount = assignedTeamCounts.length;
      return teamCounts;
    }
    else return teamCounts;
  }

  getAll(team: string): void {
    // filter jj disputes only show new, review, in_progress
    this.data = this.data.filter(x => (this.jjDisputeService.jjDisputeStatusEditable.indexOf(x.status) >= 0) && x.hearingType === this.HearingType.WrittenReasons);
    this.data = this.data.sort((a, b) => { if (a.submittedTs > b.submittedTs) { return -1; } else { return 1 } });
    this.resetAssignedUnassigned();

    this.filterByTeam(team); // initialize
  }

  resetAssignedUnassigned() {
    this.assignedDataSource.data = null;
    this.unassignedDataSource.data = null;

    this.assignedDataSource.data = this.data.filter(x => x.jjAssignedTo && x.jjAssignedTo !== this.valueOfUnassigned);
    this.unassignedDataSource.data = this.data.filter(x => !x.jjAssignedTo || x.jjAssignedTo === this.valueOfUnassigned);
    this.unassignedDataSource.data.forEach(jjDispute => {
      let index = this.unassignedDataSource.data.indexOf(jjDispute);
      jjDispute.jjAssignedTo = this.valueOfUnassigned;
      this.unassignedDataSource.data[index] = jjDispute;
    });

    this.teamCounts = [];
    this.teamCounts.push(this.getTeamCount("A"));
    this.teamCounts.push(this.getTeamCount("B"));
    this.teamCounts.push(this.getTeamCount("C"));
    this.teamCounts.push(this.getTeamCount("D"));

    this.filterByTeam(this.currentTeam);
  }

  onAssign(element: JJDispute): void {
    this.bulkUpdateJJAssignedTo([element.ticketNumber], element.jjAssignedTo);
  }

  onSelectAll(event: MatCheckboxChange) {
    if (event.checked) {
      this.assignedDataSource.data.forEach(x => x.bulkAssign);
      this.unassignedDataSource.data.forEach(x => x.bulkAssign);
    } else {
      this.assignedDataSource.data.forEach(x => !x.bulkAssign);
      this.unassignedDataSource.data.forEach(x => !x.bulkAssign);
    }
  }

  bulkUpdateJJAssignedTo(ticketNumbers: string[], assignTo: string) {
    assignTo = !assignTo || assignTo === this.valueOfUnassigned ? null : assignTo;
    this.busy = this.jjDisputeService.apiJjAssignPut(ticketNumbers, assignTo).subscribe((response) => {
      this.logger.info(
        'JJDisputeWRAssignmentsComponent::onBulkAssign response',
        response
      );
      this.getAll("A");
      this.bulkjjAssignedTo = this.valueOfUnassigned;
    });
  }

  getBulkButtonDisabled() {
    if (this.assignedDataSource.data.filter(x => x.bulkAssign)?.length == 0 &&
      this.unassignedDataSource.data.filter(x => x.bulkAssign)?.length === 0)
      return true;
    else return false;
  }

  onBulkAssign() {
    let ticketNumbers = [];
    this.assignedDataSource.data.forEach(jjDispute => {
      if (jjDispute.bulkAssign) ticketNumbers.push(jjDispute.ticketNumber);
    })
    this.unassignedDataSource.data.forEach(jjDispute => {
      if (jjDispute.bulkAssign) ticketNumbers.push(jjDispute.ticketNumber);
    });
    this.bulkUpdateJJAssignedTo(ticketNumbers, this.bulkjjAssignedTo);
  }
}

export interface teamCounts {
  team: string;
  assignedCount: number;
  unassignedCount: number;
}

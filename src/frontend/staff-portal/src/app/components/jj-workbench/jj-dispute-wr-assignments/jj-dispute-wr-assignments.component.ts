import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort } from '@angular/material/sort';
import { LookupsService } from 'app/services/lookups.service';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { filter, Observable } from 'rxjs';
import { MatLegacyCheckboxChange as MatCheckboxChange } from '@angular/material/legacy-checkbox';
import { JJDisputeAccidentYn, JJDisputeHearingType } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { AppState } from 'app/store';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-jj-dispute-wr-assignments',
  templateUrl: './jj-dispute-wr-assignments.component.html',
  styleUrls: ['./jj-dispute-wr-assignments.component.scss'],
})
export class JJDisputeWRAssignmentsComponent implements OnInit, AfterViewInit {
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  Accident = JJDisputeAccidentYn;
  data$: Observable<JJDispute[]>;
  data = [] as JJDispute[];
  currentTeam: string = "A";
  valueOfUnassigned: string = "";
  bulkjjAssignedTo: string = this.valueOfUnassigned;
  HearingType = JJDisputeHearingType;
  teamCounts: teamCounts[] = [];
  jjList: UserRepresentation[];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
  displayedColumns: string[] = [
    "assignedIcon",
    "jjAssignedToName",
    "bulkAssign",
    "ticketNumber",
    "submittedTs",
    "occamDisputantSurnameNm",
    "courthouseLocation",
    "policeDetachment",
    "timeToPayReason",
    "accidentYn",
  ];

  constructor(
    private authService: AuthService,
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private store: Store<AppState>,
    private lookupsService: LookupsService
  ) {
    this.authService.jjList$.subscribe(result => {
      this.jjList = result;
    });

    this.data$ = this.store.select(state => state.jjDispute.data).pipe(filter(i => !!i));
    this.data$.subscribe(jjDisputes => {
      this.data = jjDisputes.map(jjDispute => { return { ...jjDispute } });
      this.getAll(this.currentTeam);
    })
  }

  ngOnInit(): void {
    this.getAll(this.currentTeam);
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.sort.active = 'ticketNumber';

    // custom sorting on columns
    this.dataSource.sortingDataAccessor = (data: any, sortHeaderId: string): string => {
      if (sortHeaderId === 'timeToPayReason'){
        return this.getType(data);
      } else if (typeof data[sortHeaderId] === 'string') {
        return data[sortHeaderId].toLocaleLowerCase();
      }    
      return data[sortHeaderId];
    };
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
    let teamCourthouses = this.lookupsService.courthouseTeams.filter(x => x.__team === team);
    // let team D have all courthouse locations not found in list so these are not lost
    this.dataSource.data = this.data.filter(x =>
    ((teamCourthouses.filter(y => y.id === x.courtAgenId).length > 0) // court agency id found in team ist of courthouses
      || (team === 'D' && this.lookupsService.courthouseTeams.filter(y => y.id === x.courtAgenId).length <= 0))); // or team D and court agency id not found in complete list of courthouses
    this.currentTeam = team;
  }

  getCurrentTeamCounts(): teamCounts {
    return this.teamCounts.filter(x => x.team == this.currentTeam)[0];
  }

  getTeamCount(team: string): teamCounts {
    let teamCourthouses = this.lookupsService.courthouseTeams.filter(x => x.__team === team);
    let teamDisputes = this.data.filter(x =>
    ((teamCourthouses.filter(y => y.id === x.courtAgenId).length > 0) // court agency id found in team ist of courthouses
      || (team === 'A' && this.lookupsService.courthouseTeams.filter(y => y.id === x.courtAgenId).length <= 0))); // or team A and court agency id not found in complete list of courthouses
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
    this.data.forEach(jjDispute => {
      if (!jjDispute.jjAssignedTo) {
        jjDispute.jjAssignedTo = this.valueOfUnassigned;
      }
    });

    this.dataSource.data = this.data;
    this.resetCounts();
    this.filterByTeam(team); // initialize
  }

  resetCounts() {
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
      this.dataSource.data.forEach(x => x.bulkAssign = true);
    } else {
      this.dataSource.data.forEach(x => x.bulkAssign = false);
    }
  }

  bulkUpdateJJAssignedTo(ticketNumbers: string[], assignTo: string) {
    assignTo = !assignTo || assignTo === this.valueOfUnassigned ? null : assignTo;
    this.jjDisputeService.apiJjAssignPut(ticketNumbers, assignTo).subscribe((response) => {
      this.logger.info(
        'JJDisputeWRAssignmentsComponent::onBulkAssign response',
        response
      );
      this.getAll(this.currentTeam);
      this.bulkjjAssignedTo = this.valueOfUnassigned;
    });
  }

  getBulkButtonDisabled() {
    if (this.dataSource.data.filter(x => x.bulkAssign)?.length == 0)
      return true;
    else return false;
  }

  onBulkAssign() {
    let ticketNumbers = [];
    this.dataSource.data.forEach(jjDispute => {
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

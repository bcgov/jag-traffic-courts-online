import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { LookupsService } from 'app/services/lookups.service';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { filter, Observable } from 'rxjs';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { JJDisputeHearingType } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
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

  tableHeight: number = window.innerHeight - 425; // less size of other fixed elements
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
    "jjAssignedTo",
    "bulkAssign",
    "ticketNumber",
    "submittedTs",
    "occamDisputantName",
    "courthouseLocation",
    "policeDetachment",
    "timeToPayReason",
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

    this.data$ = this.store.pipe(select(state => state.jjDispute.data), filter(i => !!i));
    this.data$.subscribe(jjDisputes => {
      this.data = jjDisputes.map(jjDispute => { return { ...jjDispute } });
      this.getAll("A");
    })
  }

  ngOnInit(): void {
    this.getAll("A");
    // override the default sortData with our custom sort function
    this.dataSource.sortData = this.customSortData('jjAssignedTo');
  }

  calcTableHeight(heightOther) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 80)
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.sort.active = 'ticketNumber';
  }

  // custom sort function
  customSortData(firstKey: string) {
    let sortFunction = (items: JJDispute[], sort: MatSort): JJDispute[] => {
      if (!sort.active || sort.direction === '') {
        return items;
      }
      return this.sortDataByTwoKeys(
        items,
        firstKey,
        sort.active,
        sort.direction
      );
    };
    return sortFunction;
  }

  // Compare logic
  sortDataByTwoKeys(
    data: JJDispute[],
    firstKey: string,
    secondKey: string,
    orderBy: 'asc' | 'desc' = 'asc'
  ) {
    return data.sort((item1, item2) => {
      if (((item1[firstKey] === this.valueOfUnassigned || !item1[firstKey]) && (item2[firstKey] === this.valueOfUnassigned || !item2[firstKey]))
        || (item1[firstKey] !== this.valueOfUnassigned && item2[firstKey] !== this.valueOfUnassigned)) {

        if (orderBy === 'asc' && item1[secondKey] <= item2[secondKey]) return -1;
        if (orderBy === 'asc' && item1[secondKey] > item2[secondKey]) return 1;
        if (orderBy === 'desc' && item1[secondKey] < item2[secondKey]) return 1;
        if (orderBy === 'desc' && item1[secondKey] < item2[secondKey]) return -1;
      }
      let firstKeyResult = 1;
      if (item1[firstKey] == this.valueOfUnassigned || !item1[firstKey]) firstKeyResult = -1;
      return firstKeyResult;
    });
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
    this.tableHeight = this.calcTableHeight(425);
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
    this.dataSource.sort = this.sort;

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
      this.getAll("A");
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

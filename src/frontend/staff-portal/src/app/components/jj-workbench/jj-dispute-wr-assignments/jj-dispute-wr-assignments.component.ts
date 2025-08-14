import { Component, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort, Sort } from '@angular/material/sort';
import { LookupsService } from 'app/services/lookups.service';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { MatLegacyCheckboxChange as MatCheckboxChange } from '@angular/material/legacy-checkbox';
import { DisputeCaseFileSummary, PagedDisputeCaseFileSummaryCollection, SortDirection, YesNo, Agency } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { HearingType } from '@shared/consts/HearingType.model';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';

@Component({
  selector: 'app-jj-dispute-wr-assignments',
  templateUrl: './jj-dispute-wr-assignments.component.html',
  styleUrls: ['./jj-dispute-wr-assignments.component.scss'],
})
export class JJDisputeWRAssignmentsComponent implements OnInit {
  @Output() tcoDisputeInfo: EventEmitter<DisputeCaseFileSummary> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  currentTeam: string = "A";
  courthouseTeamCounts: teamCounts[] = [];
  valueOfUnassigned: string = "";
  bulkjjAssignedTo: string = this.valueOfUnassigned;
  jjList: UserRepresentation[];
  tcoDisputes: DisputeCaseFileSummaryTeam[] = [];
  tcoDisputesCollection: PagedDisputeCaseFileSummaryCollection = {};
  dataSource = new MatTableDataSource(this.tcoDisputes);
  displayedColumns: string[] = [
    "assignedIcon",
    "jjAssignedTo",
    "bulkAssign",
    "ticketNumber",
    "submittedTs",
    "surnameOrOrgName",
    "toBeHeardAtCourthouseName",
    "policeDetachment",
    "timeToPayReason",
    "accidentYn",
  ];
  sortBy: string = "submittedTs";
  sortDirection: SortDirection = SortDirection.Desc;
  yesNo = YesNo;
  courthouseTeamIds = {};

  constructor(
    private authService: AuthService,
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private lookupsService: LookupsService
  ) {
    this.authService.jjList$.subscribe(result => {
      this.jjList = result;
    });
  }

  initializeCourthouseTeamCounts(): void {
    this.courthouseTeamCounts = [
      { team: 'A', assignedCount: 0, unassignedCount: 0 },
      { team: 'B', assignedCount: 0, unassignedCount: 0 },
      { team: 'C', assignedCount: 0, unassignedCount: 0 },
      { team: 'D', assignedCount: 0, unassignedCount: 0 }
    ];
  }

  getCourthouseAgencyIds() {
    this.lookupsService.getCourthouseAgencies().subscribe((agencies: Agency[]) => {
      this.courthouseTeamCounts.forEach(courthouseTeamCount => {
        let matchingTeams = this.lookupsService.courthouseTeams.filter(x => x.__team === courthouseTeamCount.team);
        let ids = matchingTeams.flatMap(team => 
          agencies.filter(agency => agency.name.toLowerCase() === team.name.toLowerCase()).map(agency => agency.id));
        this.courthouseTeamIds[courthouseTeamCount.team] = ids;
      });
      this.getTCODisputes();
    });
  }

  ngOnInit(): void {
    this.initializeCourthouseTeamCounts();
    this.getCourthouseAgencyIds();   
  }

  getTCODisputes() {
    this.logger.log('JJDisputeWRAssignmentsComponent::getTCODisputes');
    const params = {
      disputeStatusCodes: [DisputeStatus.New, DisputeStatus.Review, DisputeStatus.InProgress].join(","),
      hearingTypeCd: HearingType.WrittenReasons,
      toBeHeardAtCourthouseIds: this.courthouseTeamIds[this.currentTeam] ? 
        this.courthouseTeamIds[this.currentTeam].join(",") : "",
      sortBy: this.sortDirection === SortDirection.Asc ? this.sortBy : "-" + this.sortBy,
      pageNumber: 1,
      pageSize: -1
    };
    this.jjDisputeService.getTCODisputes(params).subscribe((response) => {
      this.tcoDisputes = [];
      this.logger.log('JJDisputeWRAssignmentsComponent::getTCODisputes response');
      this.tcoDisputesCollection = response;
      this.tcoDisputes = response.items.map(item => ({
        ...item,
        bulkAssign: false
      })) as DisputeCaseFileSummaryTeam[];
      this.dataSource.data = this.tcoDisputes;
      if (this.sortBy === 'timeToPayReason') {
        this.sortByType();
      }
      let courthouseTeamCount = this.courthouseTeamCounts.find(x => x.team === this.currentTeam);
      courthouseTeamCount.assignedCount = this.tcoDisputes.filter(x => x.jjAssignedTo).length;
      courthouseTeamCount.unassignedCount = this.tcoDisputes.filter(x => !x.jjAssignedTo).length;
    });
  }

  backWorkbench(element: DisputeCaseFileSummary) {
    this.tcoDisputeInfo.emit(element);
  }

  getType(element: DisputeCaseFileSummary): string {
    if (element.timeToPayReason && element.fineReductionReason)
      return "Time to pay/Fine";
    else if (element.timeToPayReason)
      return "Time to pay";
    else return "Fine";
  }

  filterByTeam(team: string) {    
    this.currentTeam = team;
    this.getTCODisputes();
  }

  onAssign(element: DisputeCaseFileSummary): void {
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
    assignTo = assignTo ? assignTo : null;
    this.jjDisputeService.apiJjAssignPut(ticketNumbers, assignTo).subscribe((response) => {
      this.logger.info(
        'JJDisputeWRAssignmentsComponent::onBulkAssign response',
        response
      );
      this.getTCODisputes();
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
    this.dataSource.data.forEach(tcoDispute => {
      if (tcoDispute.bulkAssign) ticketNumbers.push(tcoDispute.ticketNumber);
    });
    this.bulkUpdateJJAssignedTo(ticketNumbers, this.bulkjjAssignedTo);
  }

  getName(jjAssignedTo: string) {
    if (this.jjList) {
      const jj = this.jjList.find(j => j.idir === jjAssignedTo);
      return jj ? jj.jjDisplayName : '';
    }
  }

  sortData(sort: Sort){
    this.sortBy = sort.active;
    this.sortDirection = sort.direction ? sort.direction as SortDirection : SortDirection.Desc;
    if (this.sortBy === 'timeToPayReason') {
      this.sortByType();
    } else {
      this.getTCODisputes();
    }
  }

  getAssignedCount() {
    return this.courthouseTeamCounts.find(x => x.team === this.currentTeam).assignedCount;
  }

  getUnassignedCount() {
    return this.courthouseTeamCounts.find(x => x.team === this.currentTeam).unassignedCount;
  }

  sortByType() {
    this.dataSource.data = this.tcoDisputes.sort((a, b) => {
      const typeA = this.getType(a);
      const typeB = this.getType(b);
      if (typeA.toLowerCase() < typeB.toLowerCase()) {
        return this.sortDirection === 'asc' ? -1 : 1;
      } else if (typeA.toLowerCase() > typeB.toLowerCase()) {
        return this.sortDirection === 'asc' ? 1 : -1;
      } else {
        return 0;
      }
    });
  }
}

export interface teamCounts {
  team: string;
  assignedCount: number;
  unassignedCount: number;
}

export interface DisputeCaseFileSummaryTeam extends DisputeCaseFileSummary {
  bulkAssign: boolean;
}

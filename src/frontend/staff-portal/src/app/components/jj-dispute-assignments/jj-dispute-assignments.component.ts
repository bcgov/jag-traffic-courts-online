import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { CourthouseConfig } from '@config/config.model';
import { JJDisputeService, JJTeamMember } from 'app/services/jj-dispute.service';
import { JJDispute } from '../../api/model/jJDispute.model';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { MatCheckboxChange } from '@angular/material/checkbox';

@Component({
  selector: 'app-jj-dispute-assignments',
  templateUrl: './jj-dispute-assignments.component.html',
  styleUrls: ['./jj-dispute-assignments.component.scss'],
})
export class JJDisputeAssignmentsComponent implements OnInit, AfterViewInit {
  @Output() public jjPage: EventEmitter<string> = new EventEmitter();

  busy: Subscription;
  jjDisputeInfo: JJDispute;

  data = [] as JJDisputeView[];
  public courtLocations: CourthouseConfig[];
  public currentTeam: string = "A";
  showDispute: boolean = false;
  public bulkjjAssignedTo: string = "unassigned";
  public teamCounts: teamCounts[] = [];
  assignedDataSource = new MatTableDataSource();
  unassignedDataSource = new MatTableDataSource();
  @ViewChild(MatSort) sort = new MatSort();
  displayedColumns: string[] = [
    "assignedIcon",
    "jjAssignedTo",
    "ticketNumber",
    "submittedDate",
    "surname",
    "courthouseLocation",
    "policeDetachment",
    "timeToPayReason",
    "status"
  ];

  constructor(
    public jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    public mockConfigService: MockConfigService,

  ) {

    if (this.mockConfigService.courtLocations) {
      this.courtLocations = this.mockConfigService.courtLocations;
    }
   }

  ngOnInit(): void {
    this.jjDisputeService.jjList = this.jjDisputeService.jjList.sort((a: JJTeamMember, b: JJTeamMember) => { if (a.name < b.name) { return -1; } else { return 1 } });
    this.getAll("A");
  }

  ngAfterViewInit() {
    this.assignedDataSource.sort = this.sort;
    this.unassignedDataSource.sort = this.sort;
  }

  sortData(event: Sort) {
    this.assignedDataSource.sort = this.sort;
  }

  backTicketList(element) {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage.emit("Dispute Details");
    else this.jjPage.emit("Assignments");
    this.jjDisputeInfo = element;
    if (!this.showDispute) this.getAll(this.currentTeam);  // refresh list
  }

  backTicketpage() {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage.emit("Dispute Details");
    else this.jjPage.emit("Assignments");
    if (!this.showDispute) this.getAll(this.currentTeam); // refresh list
  }

  getType(element: JJDispute): string {
    if (element.timeToPayReason && element.fineReductionReason)
      return "Time to pay/fine";
    else if (element.timeToPayReason)
      return "Time to pay";
    else return "Fine";
  }

  filterByTeam(team: string) {
    let teamCourthouses = this.courtLocations.filter(x => x.jjTeam === team);
    this.currentTeam = team;
    this.assignedDataSource.data = this.data.filter(x => x.jjAssignedTo !== null && x.jjAssignedTo !== "unassigned" && teamCourthouses.filter(y => y.name === x.courthouseLocation).length > 0);
    this.unassignedDataSource.data = this.data.filter(x => (x.jjAssignedTo === null || x.jjAssignedTo === "unassigned") && teamCourthouses.filter(y => y.name === x.courthouseLocation).length > 0);
  }

  getCurrentTeamCounts(): teamCounts {
    return this.teamCounts.filter(x => x.team == this.currentTeam)[0];
  }

  getTeamCount(team: string): teamCounts {
    let teamCourthouses = this.courtLocations.filter(x => x.jjTeam === team);
    let teamDisputes = this.data.filter(x => teamCourthouses.filter(y => y.name === x.courthouseLocation).length > 0);
    let teamCounts = { team: team, assignedCount: 0, unassignedCount: 0 } as teamCounts;
    if (teamDisputes) {
      let unassignedTeamCounts = teamDisputes.filter(x => x.jjAssignedTo === null || x.jjAssignedTo === "unassigned");
      if (unassignedTeamCounts.length > 0) teamCounts.unassignedCount = unassignedTeamCounts.length;
      let assignedTeamCounts = teamDisputes.filter(x => x.jjAssignedTo !== null && x.jjAssignedTo !== "unassigned");
      if (assignedTeamCounts.length > 0) teamCounts.assignedCount = assignedTeamCounts.length;
      return teamCounts;
    }
    else return teamCounts;
  }

  getAll(team: string): void {
    this.logger.log('JJDisputeAssignmentsComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputes().subscribe((response: JJDisputeView[]) => {
      // filter jj disputes only show new
      this.data = response.filter(x => this.jjDisputeService.JJDisputeStatusEditable.indexOf(x.status) > 0);
      this.data = this.data.sort((a: JJDisputeView, b: JJDisputeView) => { if (a.submittedDate > b.submittedDate) { return -1; } else { return 1 } });
      this.data.forEach(x => {
          x.jjAssignedToName = this.jjDisputeService.jjList.filter(y => y.idir === x.jjAssignedTo)[0]?.name;
          x.bulkAssign = false;
        });
      this.resetAssignedUnassigned();

      this.filterByTeam(team); // initialize
    });
  }

  public resetAssignedUnassigned() {
    this.assignedDataSource.data = null; this.unassignedDataSource.data = null;
    this.assignedDataSource.data = this.data.filter(x => x.jjAssignedTo !== null && x.jjAssignedTo !== "unassigned" && x.jjAssignedTo !== undefined) as JJDispute[];
    this.unassignedDataSource.data = this.data.filter(x => x.jjAssignedTo === null || x.jjAssignedTo === "unassigned" || x.jjAssignedTo === undefined) as JJDispute[];
    this.unassignedDataSource.data.forEach((jjDispute: JJDispute) => {
      let index = this.unassignedDataSource.data.indexOf(jjDispute);
      jjDispute.jjAssignedTo = "unassigned";
      this.unassignedDataSource.data[index] = jjDispute;
    });

    this.teamCounts = [];
    this.teamCounts.push(this.getTeamCount("A"));
    this.teamCounts.push(this.getTeamCount("B"));
    this.teamCounts.push(this.getTeamCount("C"));
    this.teamCounts.push(this.getTeamCount("D"));

    this.filterByTeam(this.currentTeam);
  }

  public onAssign(element: JJDisputeView): void {
    let updateDispute = this.data.filter(x => x.ticketNumber === element.ticketNumber)[0];
    if (element.jjAssignedTo === "unassigned") updateDispute.jjAssignedTo = null;
    else updateDispute.jjAssignedTo = element.jjAssignedTo;
    this.busy = this.jjDisputeService.putJJDispute(updateDispute.ticketNumber, updateDispute).subscribe((response: JJDispute) => {
      this.resetAssignedUnassigned();
      this.logger.info(
        'JJDisputeAssignmentsComponent::putJJDispute response',
        response
      );
    });
  }

  onSelectAll(event: MatCheckboxChange) {
    if (event.checked === true) {
      this.assignedDataSource.data.forEach((x: JJDisputeView) => x.bulkAssign = true);
      this.unassignedDataSource.data.forEach((x: JJDisputeView)=> x.bulkAssign = true);
    } else {
      this.assignedDataSource.data.forEach((x: JJDisputeView) => x.bulkAssign = false);
      this.unassignedDataSource.data.forEach((x: JJDisputeView)=> x.bulkAssign = false);
    }
  }

  bulkUpdateJJAssignedTo(jjDispute: JJDisputeView) {
    let updateDispute = this.data.filter(x => x.ticketNumber === jjDispute.ticketNumber )[0];
    let index = this.data.indexOf(updateDispute);
    if (this.bulkjjAssignedTo === "unassigned") {
      updateDispute.jjAssignedTo = null;
    }
    else {
      updateDispute.jjAssignedTo = this.bulkjjAssignedTo;
    }

    this.busy = this.jjDisputeService.putJJDispute(updateDispute.ticketNumber, updateDispute).subscribe((response: JJDispute) => {
      this.logger.info(
        'JJDisputeAssignmentsComponent::putJJDispute response',
        response
      );
      updateDispute.jjAssignedToName = this.jjDisputeService.jjList.filter(y => y.idir === updateDispute.jjAssignedTo)[0]?.name;
      this.data[index] = updateDispute;
      this.resetAssignedUnassigned();
    });
  }

  getBulkButtonDisabled() {
    if (this.assignedDataSource.data.filter((x: JJDisputeView) => x.bulkAssign === true)?.length == 0 &&
    this.unassignedDataSource.data.filter((x: JJDisputeView)=> x.bulkAssign === true)?.length === 0)
      return true;
    else return false;
  }

  onBulkAssign () {
    this.assignedDataSource.data.forEach((jjDispute: JJDisputeView) => {
     if (jjDispute.bulkAssign === true) this.bulkUpdateJJAssignedTo(jjDispute);
    })
    this.unassignedDataSource.data.forEach((jjDispute: JJDisputeView) => {
      if (jjDispute.bulkAssign === true) this.bulkUpdateJJAssignedTo(jjDispute);
    });
  }
}

export interface teamCounts {
  team: string;
  assignedCount: number;
  unassignedCount: number;
}
export interface JJDisputeView extends JJDispute {
  jjAssignedToName: string;
  bulkAssign: boolean;
}

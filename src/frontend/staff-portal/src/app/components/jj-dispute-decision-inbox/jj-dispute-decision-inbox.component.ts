import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { JJDispute } from '../../api/model/jJDispute.model';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { JJDisputeStatus } from 'app/api';

@Component({
  selector: 'app-jj-dispute-decision-inbox',
  templateUrl: './jj-dispute-decision-inbox.component.html',
  styleUrls: ['./jj-dispute-decision-inbox.component.scss'],
})
export class JJDisputeDecisionInboxComponent implements OnInit, AfterViewInit {
  @Output() public jjPage: EventEmitter<string> = new EventEmitter();
  @Input() public jjIDIR: string;

  busy: Subscription;
  jjDisputeInfo: JJDispute;

  editableStatuses: JJDisputeStatus[] = [ JJDisputeStatus.New, JJDisputeStatus.InProgress, JJDisputeStatus.Review];

  data = [] as JJDispute[];
  showDispute: boolean = false;
  dataSource = new MatTableDataSource();
  @ViewChild(MatSort) sort = new MatSort();
  displayedColumns: string[] = [
    "ticketNumber",
    "dateSubmitted",
    "violationDate",
    "disputantName",
    "courthouseLocation",
    "policeDetachment",
    "status",
  ];

  constructor(
    public jjDisputeService: JJDisputeService,
    private logger: LoggerService,
  ) { }

  ngOnInit(): void {
    this.getAll();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backTicketList(element) {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage.emit("Dispute Details");
    else this.jjPage.emit("Inbox");
    this.jjDisputeInfo = element;
    if (!this.showDispute) this.getAll();  // refresh list
  }

  backTicketpage() {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage.emit("Dispute Details");
    else this.jjPage.emit("Inbox");
    if (!this.showDispute) this.getAll(); // refresh list
  }

  getAll(): void {
    this.logger.log('JJWorkbenchDashboardComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputes().subscribe((response: JJDispute[]) => {
      // filter jj disputes only show those for the current JJ
      this.data = response.filter(x => x.jjAssignedTo == this.jjIDIR); // current IDIR
      this.dataSource.data = this.data;

      // initially sort by submitted date within status
      this.dataSource.data = this.dataSource.data.sort((a: JJDispute, b: JJDispute) =>
        {
          // if they have the same status
          if (a.status === b.status) {
            if (a.submittedDate > b.submittedDate) { return 1; } else { return -1; }
          }

          // compare statuses
          else {
            if (this.jjDisputeService.jjDisputeStatusesSorted.indexOf(a.status) > this.jjDisputeService.jjDisputeStatusesSorted.indexOf(b.status)) { return 1; } else { return -1; }
          }
        });
    });
  }
}

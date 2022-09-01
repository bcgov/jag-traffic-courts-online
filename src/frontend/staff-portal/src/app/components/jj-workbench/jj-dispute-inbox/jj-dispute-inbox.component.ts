import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { JJDispute } from '../../../api/model/jJDispute.model';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { JJDisputeStatus } from 'app/api';

@Component({
  selector: 'app-jj-dispute-inbox',
  templateUrl: './jj-dispute-inbox.component.html',
  styleUrls: ['./jj-dispute-inbox.component.scss'],
})
export class JJDisputeInboxComponent implements OnInit, AfterViewInit {
  @Output() public jjPage: EventEmitter<string> = new EventEmitter();
  @Input() public jjIDIR: string;

  busy: Subscription;
  jjDisputeInfo: JJDispute;

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
    else this.jjPage.emit("WR Inbox");
    this.jjDisputeInfo = element;
    if (!this.showDispute) this.getAll();  // refresh list
  }

  backTicketpage() {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage.emit("Dispute Details");
    else this.jjPage.emit("WR Inbox");
    if (!this.showDispute) this.getAll(); // refresh list
  }

  getAll(): void {
    this.logger.log('JJDisputeDecisionComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputesByIDIR(this.jjIDIR).subscribe((response: JJDispute[]) => {
      // filter jj disputes only show those for the current JJ
      this.data = response; // current IDIR
      this.dataSource.data = this.data;

      // only show status NEW, IN_PROGRESS, CONFIRMED, REVIEW, REQUIRE_COURT_HEARING, REQUIRE_MORE_INFO
      this.data = this.data.filter(x => x.status === JJDisputeStatus.New || x.status === JJDisputeStatus.Confirmed || x.status === JJDisputeStatus.InProgress || x.status === JJDisputeStatus.Review || x.status === JJDisputeStatus.RequireCourtHearing || x.status === JJDisputeStatus.RequireMoreInfo );

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

import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { JJDisputeStatus, JJDisputeHearingType } from 'app/api';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-jj-dispute-wr-inbox',
  templateUrl: './jj-dispute-wr-inbox.component.html',
  styleUrls: ['./jj-dispute-wr-inbox.component.scss'],
})
export class JJDisputeWRInboxComponent implements OnInit, AfterViewInit {
  @Output() public jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @Input() public jjIDIR: string;
  public HearingType = JJDisputeHearingType;
  busy: Subscription;
  data = [] as JJDispute[];
  dataSource = new MatTableDataSource();
  @ViewChild(MatSort) sort = new MatSort();
  displayedColumns: string[] = [
    "ticketNumber",
    "dateSubmitted",
    "violationDate",
    "fullName",
    "courthouseLocation",
    "policeDetachment",
    "status",
  ];

  constructor(
    public jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private authService: AuthService
  ) {
    // listen for when to refresh from db
    this.jjDisputeService.refreshDisputes.subscribe(x => {
      this.getAll();
    });
  }

  ngOnInit(): void {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = this.authService.userProfile.idir;
        this.getAll();
      }
    })
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  getAll(): void {
    this.logger.log('JJDisputeWRInboxComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputesByIDIR(this.jjIDIR).subscribe((response: JJDispute[]) => {
      // filter jj disputes only show those for the current JJ
      this.data = response; // current IDIR

      // only show status NEW, IN_PROGRESS, CONFIRMED, REVIEW, REQUIRE_COURT_HEARING, REQUIRE_MORE_INFO
      this.data = this.data.filter(x => (x.status === JJDisputeStatus.New || x.status === JJDisputeStatus.Confirmed || x.status === JJDisputeStatus.InProgress || x.status === JJDisputeStatus.Review
        || x.status === JJDisputeStatus.RequireCourtHearing || x.status === JJDisputeStatus.RequireMoreInfo) && x.hearingType === this.HearingType.WrittenReasons);
      this.dataSource.data = this.data;

      // initially sort by submitted date within status
      this.dataSource.data = this.dataSource.data.sort((a: JJDispute, b: JJDispute) =>
        {
          // if they have the same status
          if (a.status === b.status) {
            if (a.submittedTs > b.submittedTs) { return 1; } else { return -1; }
          }

          // compare statuses
          else {
            if (this.jjDisputeService.jjDisputeStatusesSorted.indexOf(a.status) > this.jjDisputeService.jjDisputeStatusesSorted.indexOf(b.status)) { return 1; } else { return -1; }
          }
        });
    });
  }
}

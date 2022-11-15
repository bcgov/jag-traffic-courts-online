import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService, JJDisputeView as JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { JJDisputeStatus, JJDisputeHearingType, JJDisputeCourtAppearanceRoP } from 'app/api';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-jj-dispute-digital-case-file',
  templateUrl: './jj-dispute-digital-case-file.component.html',
  styleUrls: ['./jj-dispute-digital-case-file.component.scss'],
})
export class JJDisputeDigitalCaseFileComponent implements OnInit, AfterViewInit {
  @Output() public jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @Input() public jjIDIR: string;
  public HearingType = JJDisputeHearingType;
  public jjAssignedToFilter: string;
  public filterText: string;
  busy: Subscription;
  data = [] as JJDispute[];
  dataSource = new MatTableDataSource();
  @ViewChild(MatSort) sort = new MatSort();
  displayedColumns: string[] = [
    "ticketNumber",
    "dateSubmitted",
    "violationDate",
    "surname",
    "courthouseLocation",
    "status",
  ];

  displayStatus: JJDisputeStatus[] = [
    JJDisputeStatus.HearingScheduled,
    JJDisputeStatus.Confirmed,
    JJDisputeStatus.InProgress,
    JJDisputeStatus.Review,
    JJDisputeStatus.RequireCourtHearing,
    JJDisputeStatus.RequireMoreInfo,
  ]

  constructor(
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private authService: AuthService
  ) {
    this.dataSource.filterPredicate = function (record: JJDispute, filter) {
      return record.givenNames?.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1
        || record.ticketNumber?.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
    }
    // listen for when to refresh from db
    this.jjDisputeService.refreshDisputes.subscribe(x => {
      this.getAll();
    });
  }

  ngOnInit(): void {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = this.authService.userIDIRLogin;
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

  // called on change in selection of JJ
  applyFilter() {
    this.dataSource.filter = this.filterText;
  }

  getAll(): void {
    this.logger.log('JJDisputeHearingInboxComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputes().subscribe((response: JJDispute[]) => {
      this.data = response;
      // only show status NEW, IN_PROGRESS, CONFIRMED, REVIEW, REQUIRE_COURT_HEARING, REQUIRE_MORE_INFO
      this.data = this.data.filter(x => this.displayStatus.indexOf(x.status) > -1 && x.hearingType === this.HearingType.CourtAppearance);
      this.dataSource.data = this.data;

      // show court appearance fields for most recent court appearance
      this.dataSource.data.forEach((jjDispute: JJDispute) => {
        if (jjDispute.jjDisputeCourtAppearanceRoPs?.length > 0) {
          let mostRecentCourtAppearance = jjDispute.jjDisputeCourtAppearanceRoPs.sort((a, b) => { if (a.appearanceTs > b.appearanceTs) { return -1; } else { return 1 } })[0];
          jjDispute.room = mostRecentCourtAppearance.room;
          jjDispute.duration = mostRecentCourtAppearance.duration;
          jjDispute.appearanceTs = new Date(mostRecentCourtAppearance.appearanceTs);
        }
      });

      // initially sort by submitted date within status
      this.dataSource.data = this.dataSource.data.sort((a: JJDispute, b: JJDispute) => {
        // if they have the same status
        if (a.status === b.status) {
          if (a.submittedDate > b.submittedDate) { return 1; } else { return -1; }
        }

        // compare statuses
        else {
          if (this.jjDisputeService.jjDisputeStatusesSorted.indexOf(a.status) > this.jjDisputeService.jjDisputeStatusesSorted.indexOf(b.status)) { return 1; } else { return -1; }
        }
      });

      this.applyFilter();
    });
  }
}

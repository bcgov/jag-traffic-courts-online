import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { filter, map, Observable, Subscription } from 'rxjs';
import { JJDisputeStatus, JJDisputeHearingType } from 'app/api';
import { AuthService } from 'app/services/auth.service';
import { AppState } from 'app/store';
import { select, Store } from '@ngrx/store';

@Component({
  selector: 'app-jj-dispute-wr-inbox',
  templateUrl: './jj-dispute-wr-inbox.component.html',
  styleUrls: ['./jj-dispute-wr-inbox.component.scss'],
})
export class JJDisputeWRInboxComponent implements OnInit, AfterViewInit {
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();
  busy: Subscription;

  jjIDIR: string;
  tableHeight: number = window.innerHeight - 300; // less size of other fixed elements
  HearingType = JJDisputeHearingType;
  statusComplete = this.jjDisputeService.jjDisputeStatusComplete;
  statusDisplay: JJDisputeStatus[] = this.jjDisputeService.jjDisputeStatusDisplay;
  data$: Observable<JJDispute[]>;
  data = [] as JJDispute[];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
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
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private authService: AuthService,
    private store: Store<AppState>
  ) {
    this.data$ = this.store.pipe(select(state => state.jjDispute.data), filter(i => !!i));
  }

  ngOnInit(): void {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = userProfile.idir;
        this.data$.subscribe(jjDisputes => {
          this.data = jjDisputes
            .map(jjDispute => { return { ...jjDispute } })
            .filter(jjDispute => jjDispute.jjAssignedTo === this.jjIDIR);
          this.getAll();
        })
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
    this.logger.log('JJDisputeWRInboxComponent::getJJDisputesByIDIR');

    // only show status NEW, IN_PROGRESS, CONFIRMED, REVIEW, REQUIRE_COURT_HEARING, REQUIRE_MORE_INFO
    this.data = this.data.filter(x => this.statusDisplay.indexOf(x.status) > -1 && x.hearingType === this.HearingType.WrittenReasons);
    this.dataSource.data = this.data;

    // initially sort by submitted date within status
    this.dataSource.data = this.dataSource.data.sort((a, b) => {
      // if they have the same status
      if (a.status === b.status) {
        if (a.submittedTs > b.submittedTs) { return 1; } else { return -1; }
      }

      // compare statuses
      else {
        if (this.jjDisputeService.jjDisputeStatusesSorted.indexOf(a.status) > this.jjDisputeService.jjDisputeStatusesSorted.indexOf(b.status)) { return 1; } else { return -1; }
      }
    });
  }
}

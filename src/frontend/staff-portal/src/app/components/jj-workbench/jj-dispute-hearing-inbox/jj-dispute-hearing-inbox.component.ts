import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService, JJDisputeView } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { JJDisputeStatus, JJDispute, JJDisputeHearingType, JJDisputeCourtAppearanceRoP } from 'app/api';
import { AuthService } from 'app/services/auth.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-jj-dispute-hearing-inbox',
  templateUrl: './jj-dispute-hearing-inbox.component.html',
  styleUrls: ['./jj-dispute-hearing-inbox.component.scss'],
})
export class JJDisputeHearingInboxComponent implements OnInit, AfterViewInit {
  @Output() public jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @Input() public jjIDIR: string;
  public HearingType = JJDisputeHearingType;
  public filterValues: any = {
    jjAssignedTo: '',
    appearanceTs: new Date()
  }
  busy: Subscription;
  public appearanceDateFilter = new FormControl('');
  public jjAssignedToFilter = new FormControl('');
  data = [] as JJDisputeView[];
  dataSource = new MatTableDataSource();
  @ViewChild(MatSort) sort = new MatSort();
  displayedColumns: string[] = [
    "jjAssignedTo",
    "ticketNumber",
    "dateSubmitted",
    "violationDate",
    "courthouseLocation",
    "appearanceTs",
    "duration",
    "room",
    "status",
  ];

  constructor(
    public jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    public authService: AuthService
  ) {
    // listen for when to refresh from db
    this.jjDisputeService.refreshDisputes.subscribe(x => {
      this.getAll();
    });

    // listen for changes in jj Assigned
    this.jjAssignedToFilter.valueChanges
      .subscribe(
        value => {
          this.filterValues.jjAssignedTo = this.jjAssignedToFilter.value;
          this.dataSource.filter = JSON.stringify(this.filterValues);
          console.log("jj assigned changed", this.filterValues);
        }
      )

    // listen for changes in appearance Date
    this.appearanceDateFilter.valueChanges
      .subscribe(
        value => {
          this.filterValues.appearanceTs = this.appearanceDateFilter.value;
          this.dataSource.filter =JSON.stringify(this.filterValues);
          console.log("appearance date changed", value, this.filterValues);
        }
      )
  }

  ngOnInit(): void {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = this.authService.userIDIRLogin;
        this.getAll();
      }
    })
  }

  public onAssign(element: JJDispute): void {
    let updateDispute = this.data.filter(x => x.ticketNumber === element.ticketNumber)[0];
    if (element.jjAssignedTo === "unassigned") updateDispute.jjAssignedTo = null;
    else updateDispute.jjAssignedTo = element.jjAssignedTo;

    // update most recent court appearance rop
    if (updateDispute.jjDisputeCourtAppearanceRoPs?.length > 0) {
      let mostRecentCourtAppearance = updateDispute.jjDisputeCourtAppearanceRoPs.sort((a,b) => {if (a.appearanceTs > b.appearanceTs) { return -1; } else { return 1 } })[0];
      let i = updateDispute.jjDisputeCourtAppearanceRoPs.indexOf(mostRecentCourtAppearance);
      updateDispute.jjDisputeCourtAppearanceRoPs[i].adjudicator = element.jjAssignedTo;
    }

    this.busy = this.jjDisputeService.putJJDispute(updateDispute.ticketNumber, updateDispute, false).subscribe((response: JJDispute) => {
      this.logger.info(
        'JJDisputeHearingInboxComponent::putJJDispute response',
        response
      );
    });
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  private createFilter(): (record: JJDisputeView, filter: string) => boolean {
    let filterFunction = function (record, filter): boolean {
      let searchTerms = JSON.parse(filter);
      let searchDate = new Date(searchTerms.appearanceTs);

      return (record.jjAssignedTo?.toLocaleLowerCase().indexOf(searchTerms.jjAssignedTo?.toLocaleLowerCase()) > -1)
        && record.appearanceTs?.getFullYear() === searchDate.getFullYear()
        && record.appearanceTs?.getMonth() === searchDate.getMonth()
        && record.appearanceTs?.getDate() === searchDate.getDate();
    }

    return filterFunction;
  }


  function (record: JJDisputeView, filter) {
  }


  getAll(): void {
    this.logger.log('JJDisputeHearingInboxComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputes().subscribe((response: JJDispute[]) => {
      // filter jj disputes only show those for the current JJ
      this.data = response; // current IDIR

      // only show status NEW, IN_PROGRESS, CONFIRMED, REVIEW, REQUIRE_COURT_HEARING, REQUIRE_MORE_INFO
      this.data = this.data.filter(x => (x.status === JJDisputeStatus.HearingScheduled || x.status === JJDisputeStatus.Confirmed || x.status === JJDisputeStatus.InProgress || x.status === JJDisputeStatus.Review
        || x.status === JJDisputeStatus.RequireCourtHearing || x.status === JJDisputeStatus.RequireMoreInfo) && x.hearingType === this.HearingType.CourtAppearance);
      this.dataSource.data = this.data;

      // show court appearance fields for most recent court appearance
      this.dataSource.data.forEach((jjDispute: JJDisputeView) => {
        if (jjDispute.jjDisputeCourtAppearanceRoPs?.length > 0) {
          let mostRecentCourtAppearance = jjDispute.jjDisputeCourtAppearanceRoPs.sort((a,b) => {if (a.appearanceTs > b.appearanceTs) { return -1; } else { return 1 } })[0];
          jjDispute.room = mostRecentCourtAppearance.room;
          jjDispute.duration = mostRecentCourtAppearance.duration;
          jjDispute.appearanceTs = new Date(mostRecentCourtAppearance.appearanceTs);
        }
      });

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

      // this section allows filtering only on jj IDIR
      this.dataSource.filterPredicate = this.createFilter();

      this.jjAssignedToFilter.setValue(this.jjIDIR);
      this.appearanceDateFilter.setValue(new Date());
    });
  }
}

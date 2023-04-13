import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { filter, Observable } from 'rxjs';
import { JJDisputeStatus, JJDisputeHearingType } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { FormControl } from '@angular/forms';
import { select, Store } from '@ngrx/store';
import { AppState } from 'app/store';

@Component({
  selector: 'app-jj-dispute-hearing-inbox',
  templateUrl: './jj-dispute-hearing-inbox.component.html',
  styleUrls: ['./jj-dispute-hearing-inbox.component.scss'],
})
export class JJDisputeHearingInboxComponent implements OnInit, AfterViewInit {
  @Output() jjDisputeInfo: EventEmitter<JJDispute> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  jjIDIR: string;
  HearingType = JJDisputeHearingType;
  filterValues: any = {
    jjAssignedTo: '',
    appearanceTs: new Date()
  }
  appearanceDateFilter = new FormControl(new Date());
  tableHeight: number = window.innerHeight - 300; // less size of other fixed elements
  jjAssignedToFilter = new FormControl('');
  statusComplete = this.jjDisputeService.jjDisputeStatusComplete;
  statusDisplay: JJDisputeStatus[] = this.jjDisputeService.jjDisputeStatusDisplay;
  jjList: UserRepresentation[];
  data$: Observable<JJDispute[]>;
  data: JJDispute[] = [];
  dataSource: MatTableDataSource<JJDispute> = new MatTableDataSource();
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
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private authService: AuthService,
    private store: Store<AppState>
  ) {
    this.authService.jjList$.subscribe(result => {
      this.jjList = result;
    });

    // listen for changes in jj Assigned
    this.jjAssignedToFilter.valueChanges
      .subscribe(
        value => {
          this.filterValues.jjAssignedTo = this.jjAssignedToFilter.value;
          this.dataSource.filter = JSON.stringify(this.filterValues);
          this.tableHeight = this.calcTableHeight(300);
        }
      )

    // listen for changes in appearance Date
    this.appearanceDateFilter.valueChanges
      .subscribe(
        value => {
          this.filterValues.appearanceTs = this.appearanceDateFilter.value;
          this.dataSource.filter = JSON.stringify(this.filterValues);
          this.tableHeight = this.calcTableHeight(300);
        }
      )

    this.data$ = this.store.pipe(select(state => state.jjDispute.data), filter(i => !!i));
  }

  ngOnInit(): void {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = userProfile.idir;
        this.data$.subscribe(jjDisputes => {
          this.data = jjDisputes.map(jjDispute => { return { ...jjDispute } });
          this.getAll();
        })
      }
    })
  }

  calcTableHeight(heightOther) {
    return Math.min(window.innerHeight - heightOther, (this.dataSource.filteredData.length + 1) * 60)
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backWorkbench(element) {
    this.jjDisputeInfo.emit(element);
  }

  private createFilter(): (record: JJDispute, filter: string) => boolean {
    let filterFunction = function (record, filter): boolean {
      let searchTerms = JSON.parse(filter);
      let searchDate = new Date(searchTerms.appearanceTs);

      return (record.jjAssignedTo?.toLocaleLowerCase().indexOf(searchTerms.jjAssignedTo?.toLocaleLowerCase()) > -1 || searchTerms?.jjAssignedTo === '' && !record.jjAssignedto)
        && record.appearanceTs?.getFullYear() === searchDate.getFullYear()
        && record.appearanceTs?.getMonth() === searchDate.getMonth()
        && record.appearanceTs?.getDate() === searchDate.getDate();
    }

    return filterFunction;
  }

  getAll(): void {
    // only show status HEARING_SCHEDULED, IN_PROGRESS, REVIEW, REQUIRE_MORE_INFO
    this.data = this.data.filter(x => this.statusDisplay.indexOf(x.status) > -1 && x.hearingType == this.HearingType.CourtAppearance);
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

    // this section allows filtering only on jj IDIR
    this.dataSource.filterPredicate = this.createFilter();

    this.jjAssignedToFilter.setValue(this.jjIDIR);
    this.appearanceDateFilter.setValue(new Date());

    this.tableHeight = this.calcTableHeight(300);
  }
}

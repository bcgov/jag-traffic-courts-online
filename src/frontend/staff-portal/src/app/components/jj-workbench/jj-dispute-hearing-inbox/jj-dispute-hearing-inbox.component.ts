import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { SortDirection, YesNo, DisputeCaseFileSummary, PagedDisputeCaseFileSummaryCollection } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { FormControl } from '@angular/forms';
import { MatDatepicker } from '@angular/material/datepicker';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';
import { HearingType } from '@shared/consts/HearingType.model';
import { LoggerService } from '@core/services/logger.service';

@Component({
  selector: 'app-jj-dispute-hearing-inbox',
  templateUrl: './jj-dispute-hearing-inbox.component.html',
  styleUrls: ['./jj-dispute-hearing-inbox.component.scss'],
  standalone: false,
})
export class JJDisputeHearingInboxComponent implements OnInit, AfterViewInit {
  @Output() tcoDisputeInfo: EventEmitter<DisputeCaseFileSummary> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  @ViewChild('fauxPicker') private readonly fauxPicker: MatDatepicker<null>; // Temp fix for DatetimePicker styles

  filterValues: any = {
    jjAssignedTo: '',
    appearanceTs: new Date()
  }
  appearanceDateFilter = new FormControl(null);
  jjAssignedToFilter = new FormControl('');
  jjList: UserRepresentation[];
  tcoDisputes: DisputeCaseFileSummary[] = [];
  tcoDisputesCollection: PagedDisputeCaseFileSummaryCollection = {};
  dataSource = new MatTableDataSource(this.tcoDisputes);
  displayedColumns: string[] = [
    "jjAssignedTo",
    "ticketNumber",
    "violationDate",
    "toBeHeardAtCourthouseName",
    "appearanceTs",  
    "appearanceRoomCode",
    "appearanceDuration",
    "accidentYn",
    "multipleOfficersYn",
    "pendingAdjournmentRequestsYn",
    "status",
  ];
  currentPage: number = 1;
  totalPages: number = 1;
  sortBy: string = "toBeHeardAtCourthouseName";
  sortDirection: SortDirection = SortDirection.Asc;
  disputeStatus = DisputeStatus;
  hearingType = HearingType;
  yesNo = YesNo;

  constructor(
    private jjDisputeService: JJDisputeService,
    private authService: AuthService,
    private logger: LoggerService,
    private readonly changeDetectorRef: ChangeDetectorRef, // Temp fix for DatetimePicker styles
  ) {
    this.authService.jjList$.subscribe(result => {
      this.jjList = result;
    });

    // listen for changes in jj Assigned
    this.jjAssignedToFilter.valueChanges
      .subscribe(
        value => {
          this.getTCODisputes();
        }
      )

    // listen for changes in appearance Date
    this.appearanceDateFilter.valueChanges
      .subscribe(
        value => {
          this.getTCODisputes();
        }
      )
  }

  ngOnInit(): void {
    this.getTCODisputes();
  }

  getTCODisputes() {
    this.logger.log('JJDisputeHearingInboxComponent::getTCODisputes');
    const params = {
      appearances: true,
      multipleOfficersYn: true,
      jjAssignedTo: this.jjAssignedToFilter.value,
      disputeStatusCodes: [DisputeStatus.HearingScheduled, DisputeStatus.InProgress, DisputeStatus.Review].join(","),
      hearingTypeCd: HearingType.CourtAppearance,
      appearanceDtFrom: this.appearanceDateFilter.value,
      appearanceDtThru: this.appearanceDateFilter.value,
      sortBy: this.sortDirection === SortDirection.Asc ? this.sortBy : "-" + this.sortBy,
      pageNumber: this.currentPage,
      pageSize: 25,
      fetchPendingAdjournments: true,
    };
    this.jjDisputeService.getTCODisputes(params).subscribe((response) => {
      this.tcoDisputes = [];
      this.logger.log('JJDisputeHearingInboxComponent::getTCODisputes response');
      this.tcoDisputesCollection = response;
      this.currentPage = response.pageNumber;
      this.totalPages = response.totalPages;
      if (!this.totalPages) {
        this.currentPage = 0;
      }
      this.tcoDisputes = response.items;
      this.dataSource.data = this.tcoDisputes;
    });
  }

  ngAfterViewInit() {
    if (this.fauxPicker !== undefined) { // Temp fix for DatetimePicker styles
      this.fauxPicker.open()
      this.changeDetectorRef.detectChanges()
      this.fauxPicker.close()
      this.changeDetectorRef.detectChanges()
    }
  }

  backWorkbench(element: DisputeCaseFileSummary) {
    this.tcoDisputeInfo.emit(element);
  }

  sortData(sort: Sort){
    this.sortBy = sort.active;
    this.sortDirection = sort.direction ? sort.direction as SortDirection : SortDirection.Desc;
    this.currentPage = 1;
    this.getTCODisputes();
  }

  onPageChange(event: number) {
    this.currentPage = event;
    this.getTCODisputes();
  }

  getName(jjAssignedTo: string) {
    if (this.jjList) {
      const jj = this.jjList.find(j => j.idir === jjAssignedTo);
      return jj ? jj.jjDisplayName : '';
    }
  }

  isEditable(element: DisputeCaseFileSummary){
    const editableStatuses = new Set([DisputeStatus.New, DisputeStatus.Review, DisputeStatus.InProgress, 
      DisputeStatus.HearingScheduled]);
    return editableStatuses.has(element.disputeStatus.code as DisputeStatus);
  }
}

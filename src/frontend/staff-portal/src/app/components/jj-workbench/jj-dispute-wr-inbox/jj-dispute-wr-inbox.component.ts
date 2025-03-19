import { Component, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { MatSort, Sort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { SortDirection, DisputeCaseFileSummary, PagedDisputeCaseFileSummaryCollection, YesNo } from 'app/api';
import { AuthService } from 'app/services/auth.service';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';
import { HearingType } from '@shared/consts/HearingType.model';

@Component({
  selector: 'app-jj-dispute-wr-inbox',
  templateUrl: './jj-dispute-wr-inbox.component.html',
  styleUrls: ['./jj-dispute-wr-inbox.component.scss'],
})
export class JJDisputeWRInboxComponent implements OnInit {
  @Output() tcoDisputeInfo: EventEmitter<DisputeCaseFileSummary> = new EventEmitter();
  @ViewChild(MatSort) sort = new MatSort();

  jjIDIR: string;
  tcoDisputes: DisputeCaseFileSummary[] = [];
  tcoDisputesCollection: PagedDisputeCaseFileSummaryCollection = {};
  dataSource = new MatTableDataSource(this.tcoDisputes);
  displayedColumns: string[] = [
    "ticketNumber",
    "submittedTs",
    "violationDate",
    "surnameOrOrgName",
    "toBeHeardAtCourthouseName",
    "policeDetachment",
    "accidentYn",
    "status",
  ];
  currentPage: number = 1;
  totalPages: number = 1;
  sortBy: string = "submittedTs";
  sortDirection: SortDirection = SortDirection.Desc;
  disputeStatus = DisputeStatus;
  hearingType = HearingType;
  accident = YesNo;

  constructor(
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private authService: AuthService
  ) {
  }

  ngOnInit(): void {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = userProfile.idir;
        this.getTCODisputes();
      }
    })
  }

  getTCODisputes() {
    this.logger.log('JJDisputeWRInboxComponent::getTCODisputes');
    const params = {
      timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
      jjAssignedTo: this.jjIDIR,
      disputeStatusCodes: [DisputeStatus.New, DisputeStatus.InProgress, DisputeStatus.Review].join(","),
      hearingTypeCd: HearingType.WrittenReasons,
      sortBy: this.sortDirection === SortDirection.Asc ? this.sortBy : "-" + this.sortBy,
      pageNumber: this.currentPage,
      pageSize: 25
    };
    this.jjDisputeService.getTCODisputes(params).subscribe((response) => {
      this.tcoDisputes = [];
      this.logger.log('JJDisputeWRInboxComponent::getTCODisputes response');
      this.tcoDisputesCollection = response;
      this.currentPage = response.pageNumber;
      this.totalPages = response.totalPages;
      if(!this.totalPages){
        this.currentPage = 0;
      }
      this.tcoDisputes = response.items;
      this.dataSource.data = this.tcoDisputes;
    });
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

  isEditable(element: DisputeCaseFileSummary){
    const editableStatuses = new Set([DisputeStatus.New, DisputeStatus.Review, DisputeStatus.InProgress, 
      DisputeStatus.HearingScheduled]);
    return editableStatuses.has(element.disputeStatus.code as DisputeStatus);
  }
}

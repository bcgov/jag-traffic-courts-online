import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { SortDirection, YesNo, DisputeCaseFileSummary, PagedDisputeCaseFileSummaryCollection } from 'app/api';
import { AuthService, UserRepresentation } from 'app/services/auth.service';
import { FormControl } from '@angular/forms';
import { MatDatepicker } from '@angular/material/datepicker';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';
import { LookupsService } from 'app/services/lookups.service';
import { HearingType } from '@shared/consts/HearingType.model';
import { LoggerService } from '@core/services/logger.service';
import { HearingInboxFilterService } from 'app/services/hearing-inbox-filter.service';

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
  courthouseLocationFilter = new FormControl({ value: '', disabled: true });
  appearanceRoomCodeFilter = new FormControl('');
  jjList: UserRepresentation[];
  tcoDisputes: DisputeCaseFileSummary[] = [];
  appearanceRoomCodes: string[] = [];
  tcoDisputesCollection: PagedDisputeCaseFileSummaryCollection = {};
  dataSource = new MatTableDataSource(this.tcoDisputes);
  displayedColumns: string[] = [
    "jjAssignedTo",
    "ticketNumber",
    "violationDate",
    "surnameOrOrgName",
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
    public lookupsService: LookupsService,
    private hearingInboxFilterService: HearingInboxFilterService
  ) {
    this.authService.jjList$.subscribe(result => {
      this.jjList = result;
    });


    // listen for changes in appearance Date
    this.appearanceDateFilter.valueChanges
      .subscribe(
        value => {
          if (value) {
            this.courthouseLocationFilter.enable({ emitEvent: false });
          } else {
            this.courthouseLocationFilter.setValue('', { emitEvent: false });
            this.courthouseLocationFilter.disable({ emitEvent: false });
          }
        }
      )

    //listen for changes in courthouse location
    this.courthouseLocationFilter.valueChanges
      .subscribe(
        value => {
          this.getTCODisputes(false);
        }
      )

    // listen for changes in court room
    this.appearanceRoomCodeFilter.valueChanges
      .subscribe(
        value => {
          this.getTCODisputes(true);
        }
      )

      
  }

  ngOnInit(): void {
    const savedFilters = this.hearingInboxFilterService.filters;

    if (savedFilters.appearanceDate) {
      this.appearanceDateFilter.setValue(savedFilters.appearanceDate, { emitEvent: false });
      this.courthouseLocationFilter.enable({ emitEvent: false });
    }

    if (savedFilters.courthouseLocation) {
      this.courthouseLocationFilter.setValue(savedFilters.courthouseLocation, { emitEvent: false });
      this.appearanceRoomCodeFilter.enable({ emitEvent: false });
    }

    if (savedFilters.appearanceRoomCode) {
      this.appearanceRoomCodeFilter.setValue(savedFilters.appearanceRoomCode, { emitEvent: false });
      this.getTCODisputes(true);
    }

  }

  getTCODisputes(bind: boolean) {
    this.logger.log('JJDisputeHearingInboxComponent::getTCODisputes');
    const params = {
      appearances: true,
      multipleOfficersYn: true,
      jjAssignedTo: this.jjAssignedToFilter.value,
      disputeStatusCodes: [DisputeStatus.HearingScheduled, DisputeStatus.InProgress, DisputeStatus.Review].join(","),
      hearingTypeCd: HearingType.CourtAppearance,
      appearanceCourthouseIds: this.courthouseLocationFilter.value,
      appearanceDtFrom: this.appearanceDateFilter.value,
      appearanceDtThru: this.appearanceDateFilter.value,
      appearanceRoomCode: this.appearanceRoomCodeFilter.value,
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
      if(bind)
      {
        this.dataSource.data = this.tcoDisputes;
      }
      this.appearanceRoomCodes = [...new Set(
        this.tcoDisputes
          .map(item => item.appearanceRoomCode)
          .filter(code => !!code)
      )];
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
    this.saveFilterValues();
    this.tcoDisputeInfo.emit(element);
  }

  saveFilterValues()
  {
    this.hearingInboxFilterService.filters.appearanceDate = this.appearanceDateFilter.value;
    this.hearingInboxFilterService.filters.courthouseLocation = this.courthouseLocationFilter.value ?? '';
    this.hearingInboxFilterService.filters.appearanceRoomCode = this.appearanceRoomCodeFilter.value ?? '';
  }

  sortData(sort: Sort){
    this.sortBy = sort.active;
    this.sortDirection = sort.direction ? sort.direction as SortDirection : SortDirection.Desc;
    this.currentPage = 1;
    this.getTCODisputes(true);
  }

  onPageChange(event: number) {
    this.currentPage = event;
    this.getTCODisputes(true);
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

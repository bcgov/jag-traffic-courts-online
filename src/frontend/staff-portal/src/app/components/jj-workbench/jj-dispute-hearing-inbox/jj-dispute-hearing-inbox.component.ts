import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { SortDirection, YesNo, DisputeCaseFileSummary } from 'app/api';
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

  appearanceDateFilter = new FormControl<Date | null>(null);
  courthouseLocationFilter = new FormControl({ value: '', disabled: true });
  appearanceRoomCodeFilter = new FormControl({ value: '', disabled: true });
  tcoDisputes: DisputeCaseFileSummary[] = [];
  appearanceRoomCodes: string[] = [];
  dataSource = new MatTableDataSource<DisputeCaseFileSummary>([]);
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
  currentPage = 1;
  totalPages = 1;
  sortBy = "toBeHeardAtCourthouseName";
  sortDirection: SortDirection = SortDirection.Asc;
  disputeStatus = DisputeStatus;
  hearingType = HearingType;
  yesNo = YesNo;

  constructor(
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    private readonly changeDetectorRef: ChangeDetectorRef, // Temp fix for DatetimePicker styles
    public lookupsService: LookupsService,
    private hearingInboxFilterService: HearingInboxFilterService
  ) {
    // listen for changes in appearance date
    this.appearanceDateFilter.valueChanges.subscribe((appearanceDate) => {
      this.dataSource.data = [];
      this.appearanceRoomCodeFilter.setValue('', { emitEvent: false });

      if (appearanceDate === null) {
        this.courthouseLocationFilter.setValue('', { emitEvent: false });
      }
      
      this.updateEnabledFilters();

      if (appearanceDate && this.courthouseLocationFilter.value) {
        // fetch disputes for selected courthouse/date
        this.getTCODisputes();
      }
    });

    // listen for changes in courthouse
    this.courthouseLocationFilter.valueChanges.subscribe((courthouse) => {
      this.dataSource.data = [];
      this.appearanceRoomCodeFilter.setValue('', { emitEvent: false });

      this.updateEnabledFilters();

      if (courthouse && this.appearanceDateFilter.value) {
        // fetch disputes for selected courthouse/date
        this.getTCODisputes();
      }
    });

    // listen for changes in courtroom
    this.appearanceRoomCodeFilter.valueChanges.subscribe((_courtroom) => {
      this.updateDataSource();
    });
  }

  ngOnInit(): void {
    const savedFilters = this.hearingInboxFilterService.filters;

    if (savedFilters.appearanceDate) {
      this.appearanceDateFilter.setValue(savedFilters.appearanceDate, { emitEvent: false });
    }

    if (savedFilters.courthouseLocation) {
      this.courthouseLocationFilter.setValue(savedFilters.courthouseLocation, { emitEvent: false });
    }

    if (savedFilters.appearanceRoomCode) {
      this.appearanceRoomCodeFilter.setValue(savedFilters.appearanceRoomCode, { emitEvent: false });
    }

    this.updateEnabledFilters();

    if (this.appearanceDateFilter.value && this.courthouseLocationFilter.value) {
      this.getTCODisputes();
    }
  }

  getTCODisputes() {
    this.logger.log('JJDisputeHearingInboxComponent::getTCODisputes');
    const params = {
      appearances: true,
      multipleOfficersYn: true,
      disputeStatusCodes: [DisputeStatus.HearingScheduled, DisputeStatus.InProgress, DisputeStatus.Review].join(","),
      hearingTypeCd: HearingType.CourtAppearance,
      appearanceCourthouseIds: this.courthouseLocationFilter.value ?? undefined,
      appearanceDtFrom: this.appearanceDateFilter.value?.toLocaleDateString("en-CA"),
      appearanceDtThru: this.appearanceDateFilter.value?.toLocaleDateString("en-CA"),
      appearanceRoomCode: undefined,
      sortBy: this.sortDirection === SortDirection.Asc ? this.sortBy : "-" + this.sortBy,
      pageNumber: this.currentPage,
      pageSize: 25,
      fetchPendingAdjournments: true,
    };
    this.jjDisputeService.getTCODisputes(params).subscribe((response) => {
      this.tcoDisputes = [];
      this.logger.log('JJDisputeHearingInboxComponent::getTCODisputes response');
      this.totalPages = response.totalPages ?? 0;
      this.currentPage = this.totalPages ? response.pageNumber ?? 1 : 1;
      this.tcoDisputes = response.items ?? [];
      
      this.appearanceRoomCodes = [
        ...new Set(
          this.tcoDisputes
            .map((item) => item.appearanceRoomCode)
            .filter((code): code is string => !!code),
        ),
      ];

      this.updateEnabledFilters();
      this.updateDataSource();
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

  saveFilterValues() {
    this.hearingInboxFilterService.filters.appearanceDate = this.appearanceDateFilter.value;
    this.hearingInboxFilterService.filters.courthouseLocation = this.courthouseLocationFilter.value ?? '';
    this.hearingInboxFilterService.filters.appearanceRoomCode = this.appearanceRoomCodeFilter.value ?? '';
  }

  updateEnabledFilters() {
    if (this.appearanceDateFilter.value) {
      this.courthouseLocationFilter.enable({ emitEvent: false });
    } else {
      this.courthouseLocationFilter.disable({ emitEvent: false });
    }

    if (this.appearanceDateFilter.value && this.courthouseLocationFilter.value && this.appearanceRoomCodes.length > 0) {
      this.appearanceRoomCodeFilter.enable({ emitEvent: false });
    } else {
      this.appearanceRoomCodeFilter.disable({ emitEvent: false });
    }
  }

  updateDataSource() {
    if (this.appearanceDateFilter.value && this.courthouseLocationFilter.value && this.appearanceRoomCodeFilter.value) {
      this.dataSource.data = this.tcoDisputes.filter(d => d.appearanceRoomCode === this.appearanceRoomCodeFilter.value);
    }
  }

  sortData(sort: Sort) {
    this.sortBy = sort.active;
    this.sortDirection = sort.direction ? sort.direction as SortDirection : SortDirection.Desc;
    this.currentPage = 1;
    this.getTCODisputes();
  }

  onPageChange(event: number) {
    this.currentPage = event;
    this.getTCODisputes();
  }

  isEditable(element: DisputeCaseFileSummary) {
    const editableStatuses = new Set([DisputeStatus.New, DisputeStatus.Review, DisputeStatus.InProgress, DisputeStatus.HearingScheduled]);
    return editableStatuses.has(element.disputeStatus?.code as DisputeStatus);
  }
}

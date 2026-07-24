import {
  Component,
  OnInit,
  ViewChild,
  Output,
  EventEmitter,
  inject,
} from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, Sort } from '@angular/material/sort';
import {
  SortDirection,
  YesNo,
  DisputeCaseFileSummary,
} from 'app/api';
import { AuthService } from 'app/services/auth.service';
import { DisputeStatus } from '@shared/consts/DisputeStatus.model';
import { HearingType } from '@shared/consts/HearingType.model';
import { LoggerService } from '@core/services/logger.service';
import { ReturnedDecisionStore } from 'app/store';
import { Store } from '@ngrx/store';
import { ReturnedDecisionSelectors } from 'app/store/returned-decision/returned-decision.selectors';
import { UserGroup } from '@shared/enums/user-group.enum';

@Component({
  selector: 'app-jj-dispute-returned-decisions',
  templateUrl: './jj-dispute-returned-decisions.component.html',
  standalone: false,
})
export class JJDisputeReturnedDecisionsComponent implements OnInit {
  @Output() tcoDisputeInfo = new EventEmitter<DisputeCaseFileSummary>();
  @ViewChild(MatSort) sort = new MatSort();

  private authService = inject(AuthService);
  private logger = inject(LoggerService);
  private store = inject(Store);

  pagedCollection$ = this.store.select(
    ReturnedDecisionSelectors.PagedCollection,
  );
  pageNumber = toSignal(this.store.select(ReturnedDecisionSelectors.PageNumber));
  sortBy = toSignal(this.store.select(ReturnedDecisionSelectors.SortBy));
  
  dataSource = new MatTableDataSource();
  totalPages = 1;
  jjIDIR?: string;
  displayedColumns: string[] = [
    'jjAssignedTo',
    'ticketNumber',
    'violationDate',
    'surnameOrOrgName',
    'toBeHeardAtCourthouseName',
    'appearanceTs',
    'appearanceRoomCode',
    'appearanceDuration',
    'accidentYn',
    'multipleOfficersYn',
    'pendingAdjournmentRequestsYn',
  ];
  
  disputeStatus = DisputeStatus;
  hearingType = HearingType;
  yesNo = YesNo;

  ngOnInit(): void {
    this.authService.userProfile$.subscribe((userProfile) => {
      this.jjIDIR = userProfile.idir;
    });

    this.pagedCollection$.subscribe((collection) => {
      this.dataSource.data = collection?.items ?? [];
      this.totalPages = collection?.totalPages ?? 0;
    });
  }

  sortData(sort: Sort) {
    const sortDirection = sort.direction
      ? (sort.direction as SortDirection)
      : SortDirection.Desc;
    const sortBy = sortDirection === SortDirection.Asc ? sort.active : '-' + sort.active;

    this.getReturnedDecisions(1, sortBy);
  }

  getReturnedDecisions(pageNumber: number, sortBy?: string) {
    this.logger.log('JJDisputeReturnedDecisionsComponent::getReturnedDecisionss');
    this.store.dispatch(
      ReturnedDecisionStore.Actions.Get({
        assignedTo: this.authService.checkRole(UserGroup.SUPPORT_STAFF) ? undefined : this.jjIDIR,
        pageNumber: pageNumber,
        sortBy: sortBy,
      }),
    );
  }

  isEditable(element: DisputeCaseFileSummary) {
    return new Set([
      DisputeStatus.New,
      DisputeStatus.Review,
      DisputeStatus.InProgress,
      DisputeStatus.HearingScheduled,
    ]).has(element.disputeStatus?.code as DisputeStatus);
  }
}

import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { DisputeFormMode } from '@shared/enums/dispute-form-mode';
import { DisputeService, FileMetadata } from 'app/services/dispute.service';
import { NoticeOfDispute } from 'app/services/notice-of-dispute.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';
import { DisputeStore } from 'app/store';
import { BehaviorSubject, filter, Observable, of, take } from 'rxjs';

@Component({
  selector: 'app-update-dispute',
  templateUrl: './update-dispute.component.html',
  styleUrls: ['./update-dispute.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class UpdateDisputeComponent implements OnInit {
  isLoaded$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  mode: DisputeFormMode = DisputeFormMode.UPDATE;
  noticeOfDispute: NoticeOfDispute;
  ticketType: string;
  fileData$: Observable<FileMetadata[]>;

  constructor(
    private violationTicketService: ViolationTicketService,
    private disputeService: DisputeService,
    private store: Store,
  ) {
  }

  ngOnInit(): void {
    this.disputeService.checkStoredDispute().pipe(filter(i => !!i), take(1)).subscribe(() => {
      this.store.select(DisputeStore.Selectors.Params).pipe(filter(i => !!i), take(1)).subscribe(params => {
        this.noticeOfDispute = {
          violation_ticket: { counts: [] },
          dispute_counts: [],
          ticket_number: params.ticketNumber
        } as NoticeOfDispute;
        this.ticketType = this.violationTicketService.getTicketType(this.noticeOfDispute);
        this.isLoaded$.next(true);
        this.fileData$ = of([]);
      });
    });
  }

  /**
   * @description
   * Submit the dispute — only send fields the user actually filled in.
   */
  public submitDispute(noticeOfDispute: NoticeOfDispute): void {
    const payload: NoticeOfDispute = {};
    Object.entries(noticeOfDispute).forEach(([key, val]) => {
      if (key.startsWith('__') || key === 'violation_ticket') return;
      if (val !== null && val !== undefined && val !== '') {
        payload[key] = val;
      }
    });
    this.store.dispatch(DisputeStore.Actions.Update({ payload }));
  }
}

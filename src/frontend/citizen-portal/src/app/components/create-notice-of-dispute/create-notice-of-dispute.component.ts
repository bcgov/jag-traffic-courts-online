import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { DisputeFormMode } from '@shared/enums/dispute-form-mode';
import { NoticeOfDisputeFormGroup } from '@shared/models/dispute-form.model';
import { ViolationTicket } from 'app/api';
import { NoticeOfDisputeService } from 'app/services/notice-of-dispute.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-create-notice-of-dispute',
  templateUrl: './create-notice-of-dispute.component.html',
  styleUrls: ['./create-notice-of-dispute.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class CreateNoticeOfDisputeComponent implements OnInit {
  mode: DisputeFormMode = DisputeFormMode.CREATE;
  ticket: ViolationTicket;
  ticketType: string;
  form: NoticeOfDisputeFormGroup;

  constructor(
    private violationTicketService: ViolationTicketService,
    private noticeOfDisputeService: NoticeOfDisputeService,
  ) {
  }

  ngOnInit(): void {
    this.ticket = this.violationTicketService.ticket;
    if (!this.ticket) {
      this.violationTicketService.goToFind();
      return;
    }
    this.ticketType = this.violationTicketService.ticketType;
  }

  /**
   * @description
   * Submit the dispute
   */
  public submitDispute(noticeOfDispute): void {
    this.noticeOfDisputeService.createNoticeOfDispute(noticeOfDispute);
  }
}

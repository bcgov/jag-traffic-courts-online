import { Component, Input, OnInit } from '@angular/core';
import { NoticeOfDispute } from 'app/api';
import { NoticeOfDisputeService } from 'app/services/notice-of-dispute.service';

@Component({
  selector: 'app-dispute-ticket-summary',
  templateUrl: './dispute-ticket-summary.component.html',
  styleUrls: ['./dispute-ticket-summary.component.scss'],
})
export class DisputeTicketSummaryComponent implements OnInit {
  @Input() public noticeOfDispute: NoticeOfDispute;
  public isShowCheckbox: any;

  constructor(
    protected noticeOfDisputeService: NoticeOfDisputeService,
  ) {
  }

  ngOnInit(): void {
    this.isShowCheckbox = this.noticeOfDisputeService.getIsShowCheckBoxes(this.noticeOfDispute);
  }
}

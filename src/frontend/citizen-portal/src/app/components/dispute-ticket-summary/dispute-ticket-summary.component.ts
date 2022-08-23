import { Component, Input, OnInit } from "@angular/core";
import { DisputeRepresentedByLawyer } from "app/api";
import { NoticeOfDisputeService, NoticeOfDisputeExtended } from "app/services/notice-of-dispute.service";

@Component({
  selector: "app-dispute-ticket-summary",
  templateUrl: "./dispute-ticket-summary.component.html",
  styleUrls: ["./dispute-ticket-summary.component.scss"],
})
export class DisputeTicketSummaryComponent implements OnInit {
  @Input() public noticeOfDispute: NoticeOfDisputeExtended;
  public countsActions: any;
  public RepresentedByLawyer = DisputeRepresentedByLawyer;

  constructor(
    protected noticeOfDisputeService: NoticeOfDisputeService,
  ) {
  }

  ngOnInit(): void {
    if (this.noticeOfDispute) {
      this.countsActions = this.noticeOfDisputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
    }
  }
}

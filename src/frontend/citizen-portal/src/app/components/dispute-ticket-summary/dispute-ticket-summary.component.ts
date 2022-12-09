import { Component, Input, OnInit } from "@angular/core";
import { DisputeRepresentedByLawyer, Language } from "app/api";
import { LookupsService } from "app/services/lookups.service";
import { NoticeOfDisputeService, NoticeOfDispute } from "app/services/notice-of-dispute.service";
import { Subscription } from "rxjs";
import { ConfigService } from "@config/config.service";

@Component({
  selector: "app-dispute-ticket-summary",
  templateUrl: "./dispute-ticket-summary.component.html",
  styleUrls: ["./dispute-ticket-summary.component.scss"],
})
export class DisputeTicketSummaryComponent implements OnInit {
  @Input() noticeOfDispute: NoticeOfDispute;
  countsActions: any;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  busy: Subscription;

  constructor(
    public configService: ConfigService,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private lookups: LookupsService
  ) {
  }

  ngOnInit(): void {
    if (this.noticeOfDispute) {
      this.countsActions = this.noticeOfDisputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
    }
  }

  getLanguageDescription(lang): string {
    return this.lookups.getLanguageDescription(lang);
  }
}

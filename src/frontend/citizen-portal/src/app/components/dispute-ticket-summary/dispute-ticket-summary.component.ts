import { Component, Input, OnInit } from "@angular/core";
import { DisputeRepresentedByLawyer, Language } from "app/api";
import { LookupsService } from "app/services/lookups.service";
import { DisputeService, NoticeOfDispute } from "app/services/dispute.service";
import { Subscription } from "rxjs";

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
    private disputeService: DisputeService,
    private lookups: LookupsService
  ) {
  }

  ngOnInit(): void {
    if (this.noticeOfDispute) {
      this.countsActions = this.disputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
    }
  }

  getLanguageDescription(lang): string {
    return this.lookups.getLanguageDescription(lang);
  }
}

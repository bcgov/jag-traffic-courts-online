import { Component, Input, OnInit } from "@angular/core";
import { DisputeRepresentedByLawyer, Language } from "app/api";
import { LookupsService } from "app/services/lookups.service";
import { DisputeService, NoticeOfDispute } from "app/services/dispute.service";
import { Subscription } from "rxjs";
import { ConfigService } from "@config/config.service";

@Component({
  selector: "app-dispute-ticket-summary",
  templateUrl: "./dispute-ticket-summary.component.html",
  styleUrls: ["./dispute-ticket-summary.component.scss"],
})
export class DisputeTicketSummaryComponent implements OnInit {
  @Input() public noticeOfDispute: NoticeOfDispute;
  public countsActions: any;
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public busy: Subscription;

  constructor(
    public configService: ConfigService,
    protected disputeService: DisputeService,
    public lookups: LookupsService
  ) {

    this.busy = this.lookups.getLanguages().subscribe((response: Language[]) => {
      this.lookups.languages$.next(response);
    });
  }

  ngOnInit(): void {
    if (this.noticeOfDispute) {
      this.countsActions = this.disputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
    }
  }
}

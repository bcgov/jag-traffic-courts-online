import { Component, Input, OnInit } from "@angular/core";
import { DisputeRepresentedByLawyer, Language } from "app/api";
import { LookupsService } from "app/services/lookups.service";
import { NoticeOfDisputeService, NoticeOfDisputeExtended } from "app/services/notice-of-dispute.service";
import { Subscription } from "rxjs";
import { ConfigService } from "@config/config.service";

@Component({
  selector: "app-dispute-ticket-summary",
  templateUrl: "./dispute-ticket-summary.component.html",
  styleUrls: ["./dispute-ticket-summary.component.scss"],
})
export class DisputeTicketSummaryComponent implements OnInit {
  @Input() public noticeOfDispute: NoticeOfDisputeExtended;
  public countsActions: any;
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public busy: Subscription;

  constructor(
    protected noticeOfDisputeService: NoticeOfDisputeService,
    public lookups: LookupsService,
    public configService: ConfigService
  ) {

    this.busy = this.lookups.getLanguages().subscribe((response: Language[]) => {
      this.lookups.languages$.next(response);
    });
  }

  ngOnInit(): void {
    if (this.noticeOfDispute) {
      this.countsActions = this.noticeOfDisputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
    }
  }
}

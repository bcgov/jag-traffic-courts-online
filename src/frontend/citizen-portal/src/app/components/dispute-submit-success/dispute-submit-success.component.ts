import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ticketTypes } from "@shared/enums/ticket-type.enum";
import { AppRoutes } from "app/app.routes";
import { DisputeService, NoticeOfDispute } from "app/services/dispute.service";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { Subscription } from "rxjs";
import { DisputeRepresentedByLawyer } from "app/api";

@Component({
  selector: "app-dispute-submit-success",
  templateUrl: "./dispute-submit-success.component.html",
  styleUrls: ["./dispute-submit-success.component.scss"],
})
export class DisputeSubmitSuccessComponent implements OnInit {
  busy: Subscription;
  noticeOfDispute: NoticeOfDispute;
  readonly changeOfAddressURL: string =
    "https://www2.gov.bc.ca/assets/gov/law-crime-and-justice/courthouse-services/court-files-records/court-forms/traffic/ptr805.pdf?forcedownload=true";
  readonly whatToExpectURL: string =
    "https://www.provincialcourt.bc.ca/downloads/Traffic/Traffic%20Court%20Guide.pdf";
  ticketTypes = ticketTypes;
  ticketType;
  countsActions: any;
  RepresentedByLawyer = DisputeRepresentedByLawyer;

  constructor(
    private router: Router,
    private disputeService: DisputeService,
    private violationTicketService: ViolationTicketService,
  ) { }

  ngOnInit(): void {
    this.noticeOfDispute = this.disputeService.noticeOfDispute;
    if (!this.noticeOfDispute) {
      this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      return;
    }
    this.ticketType = this.violationTicketService.ticketType;
    this.countsActions = this.disputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
  }

  onPrint(): void {
    window.print();
  }
}

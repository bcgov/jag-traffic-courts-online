import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { TicketTypes } from "@shared/enums/ticket-type.enum";
import { AppRoutes } from "app/app.routes";
import { NoticeOfDisputeService, NoticeOfDispute } from "app/services/notice-of-dispute.service";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { DisputeRepresentedByLawyer } from "app/api";

@Component({
  selector: "app-dispute-submit-success",
  templateUrl: "./dispute-submit-success.component.html",
  styleUrls: ["./dispute-submit-success.component.scss"],
})
export class DisputeSubmitSuccessComponent implements OnInit {
  noticeOfDispute: NoticeOfDispute;
  readonly changeOfAddressURL: string =
    "https://www2.gov.bc.ca/assets/gov/law-crime-and-justice/courthouse-services/court-files-records/court-forms/traffic/ptr805.pdf?forcedownload=true";
  readonly whatToExpectURL: string =
    "https://www.provincialcourt.bc.ca/downloads/Traffic/Traffic%20Court%20Guide.pdf";
  ticketTypes = TicketTypes ;
  ticketType;
  countsActions: any;
  RepresentedByLawyer = DisputeRepresentedByLawyer;

  constructor(
    private router: Router,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private violationTicketService: ViolationTicketService,
  ) { }

  ngOnInit(): void {
    this.noticeOfDispute = this.noticeOfDisputeService.noticeOfDispute;
    if (!this.noticeOfDispute) {
      this.router.navigate([AppRoutes.ticketPath(AppRoutes.FIND)]);
      return;
    }
    this.ticketType = this.violationTicketService.ticketType;
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
  }

  onPrint(): void {
    window.print();
  }
}

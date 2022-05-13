import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ticketTypes } from "@shared/enums/ticket-type.enum";
import { NoticeOfDispute } from "app/api";
import { AppRoutes } from "app/app.routes";
import { NoticeOfDisputeService } from "app/services/notice-of-dispute.service";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { Subscription } from "rxjs";

@Component({
  selector: "app-dispute-submit-success",
  templateUrl: "./dispute-submit-success.component.html",
  styleUrls: ["./dispute-submit-success.component.scss"],
})
export class DisputeSubmitSuccessComponent implements OnInit {
  public busy: Subscription;
  public noticeOfDispute: NoticeOfDispute;
  public readonly changeOfAddressURL: string =
    "https://www2.gov.bc.ca/assets/gov/law-crime-and-justice/courthouse-services/court-files-records/court-forms/traffic/ptr805.pdf?forcedownload=true";
  public readonly whatToExpectURL: string =
    "https://www.provincialcourt.bc.ca/downloads/Traffic/Traffic%20Court%20Guide.pdf";
  public ticketTypes = ticketTypes;
  public ticketType;
  public countsActions: any;

  constructor(
    private router: Router,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private violationTicketService: ViolationTicketService,
  ) { }

  public ngOnInit(): void {
    this.noticeOfDispute = this.noticeOfDisputeService.noticeOfDispute;
    if (!this.noticeOfDispute) {
      this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      return;
    }
    this.ticketType = this.violationTicketService.ticketType;
    this.countsActions = this.noticeOfDisputeService.getCountsActions(this.noticeOfDispute.disputed_counts);
  }

  public onPrint(): void {
    window.print();
  }
}

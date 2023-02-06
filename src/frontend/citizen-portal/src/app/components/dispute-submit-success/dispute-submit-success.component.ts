import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { TicketTypes } from "@shared/enums/ticket-type.enum";
import { AppRoutes } from "app/app.routes";
import { NoticeOfDisputeService, NoticeOfDispute, CountsActions } from "app/services/notice-of-dispute.service";
import { ViolationTicketService } from "app/services/violation-ticket.service";
import { DisputeRepresentedByLawyer } from "app/api";
import { DisputeFormMode } from "@shared/enums/dispute-form-mode";

@Component({
  selector: "app-dispute-submit-success",
  templateUrl: "./dispute-submit-success.component.html",
  styleUrls: ["./dispute-submit-success.component.scss"],
})
export class DisputeSubmitSuccessComponent implements OnInit {
  private mode: DisputeFormMode;

  readonly changeOfAddressURL: string =
    "https://www2.gov.bc.ca/assets/gov/law-crime-and-justice/courthouse-services/court-files-records/court-forms/traffic/ptr805.pdf?forcedownload=true";
  readonly whatToExpectURL: string =
    "https://www.provincialcourt.bc.ca/downloads/Traffic/Traffic%20Court%20Guide.pdf";

  noticeOfDispute: NoticeOfDispute;
  ticketTypes = TicketTypes;
  ticketType: string;
  countsActions: CountsActions;
  RepresentedByLawyer = DisputeRepresentedByLawyer;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private violationTicketService: ViolationTicketService,
  ) {
    let params = this.route.snapshot.queryParams;
    this.mode = params?.mode;
    if (!this.mode) {
      this.router.navigate([""]);
    }
  }

  ngOnInit(): void {
    if (this.mode === DisputeFormMode.CREATE) {
      this.noticeOfDispute = this.noticeOfDisputeService.noticeOfDispute;
      if (!this.noticeOfDispute) {
        this.router.navigate([AppRoutes.ticketPath(AppRoutes.FIND)]);
        return;
      }
      this.ticketType = this.violationTicketService.ticketType;
      this.countsActions = this.noticeOfDisputeService.getCountsActions(this.noticeOfDispute.dispute_counts);
    }
  }

  get isCreate(): boolean {
    return this.mode === DisputeFormMode.CREATE
  }

  onPrint(): void {
    window.print();
  }
}

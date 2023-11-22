import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AppRoutes } from "app/app.routes";
import { NoticeOfDisputeService } from "app/services/notice-of-dispute.service";
import { DisputeRepresentedByLawyer, DisputeRequestCourtAppearanceYn } from "app/api";
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

  noticeOfDispute: any;
  RepresentedByLawyer = DisputeRepresentedByLawyer;
  RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  DisputeFormMode = DisputeFormMode;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private noticeOfDisputeService: NoticeOfDisputeService,
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
    }
  }

  get isCreate(): boolean {
    return this.mode === DisputeFormMode.CREATE
  }

  onPrint(): void {
    window.print();
  }
}

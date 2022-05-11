import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ticketTypes } from "@shared/enums/ticket-type.enum";
import { NoticeOfDispute } from "app/api";
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
  public countList:any;
  public readonly changeOfAddressURL: string =
    "https://www2.gov.bc.ca/assets/gov/law-crime-and-justice/courthouse-services/court-files-records/court-forms/traffic/ptr805.pdf?forcedownload=true";
  public readonly whatToExpectURL: string =
    "https://www.provincialcourt.bc.ca/downloads/Traffic/Traffic%20Court%20Guide.pdf";
  public ticketTypes = ticketTypes;
  public ticketType;
  
  constructor(
    private router: Router,
    private noticeOfDisputeService: NoticeOfDisputeService,
    private violationTicketService: ViolationTicketService,
  ) {}

  public ngOnInit(): void {
    this.noticeOfDispute = this.noticeOfDisputeService.noticeOfDispute;
    this.ticketType = this.violationTicketService.ticketType;

    // this.disputeService.ticket$.subscribe((ticket) => {
    //   if (!ticket) {
    //     this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
    //     return;
    //   }

    //   this.ticket = ticket;
    //   if(ticket && ticket.countList){
    //       this.countList = ticket.countList;
    //   }
    // });
  }

  public onPrint(): void {
    window.print();
  }

  // private getListOfCountsDisputed(): string {
  //   let countsDisputed = "";
  //   let count = 0;
  //   this.ticket?.offences
  //     ?.filter(
  //       (offence) =>
  //         offence.offenceAgreementStatus === "DISPUTE" ||
  //         offence.offenceAgreementStatus === "REDUCTION"
  //     )
  //     .forEach((offence) => {
  //       if (count > 0) {
  //         countsDisputed += ",";
  //       }
  //       countsDisputed += offence.offenceNumber;
  //       count++;
  //     });
  //   return countsDisputed;
  // }

  // public get isWitnessPresent(): boolean {
  //   const witnessPresent = this.ticket?.additional?.witnessPresent;
  //   return witnessPresent;
  // }
  // public get isLawyerPresent(): boolean {
  //   const lawyerPresent = this.ticket?.additional?.lawyerPresent;
  //   return lawyerPresent;
  // }
  // public get isInterpreterRequired(): boolean {
  //   const interpreterRequired = this.ticket?.additional?.interpreterRequired;
  //   return interpreterRequired;
  // }
  // public get isReductionRequested(): boolean {
  //   const filteredOffences = this.ticket?.offences.filter(
  //     (offence) => offence.offenceAgreementStatus === "REDUCTION"
  //   );
  //   if (filteredOffences?.length > 0) {
  //     return true;
  //   }
  //   return false;
  // }
  // public get willAppearInCourt(): boolean {
  //   const filteredOffences = this.ticket?.offences.filter(
  //     (offence) => offence.reductionAppearInCourt
  //   );
  //   if (filteredOffences.length > 0) {
  //     return true;
  //   }
  //   return false;
  // }
  // public get countsDisputed(): string {
  //   const countsDisputed = this.getListOfCountsDisputed();

  //   if (countsDisputed) {
  //     if (countsDisputed.indexOf(",") > -1) {
  //       return "Counts " + countsDisputed;
  //     } else {
  //       return "Count " + countsDisputed;
  //     }
  //   }
  //   return null;
  // }
}

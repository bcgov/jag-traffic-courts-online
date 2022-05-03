import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { ticketTypes } from '@shared/enums/ticket-type.enum';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
// import { truncate } from 'fs';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dispute-submit-success',
  templateUrl: './dispute-submit-success.component.html',
  styleUrls: ['./dispute-submit-success.component.scss'],
})
export class DisputeSubmitSuccessComponent implements OnInit {
  debugger
  public busy: Subscription;
  public ticket: TicketDisputeView;
  public countList:any;
  public readonly changeOfAddressURL: string =
    'https://www2.gov.bc.ca/assets/gov/law-crime-and-justice/courthouse-services/court-files-records/court-forms/traffic/ptr805.pdf?forcedownload=true';
  public readonly whatToExpectURL: string =
    'https://www.provincialcourt.bc.ca/downloads/Traffic/Traffic%20Court%20Guide.pdf';
  public ticketTypesEnum = ticketTypes;
  constructor(
    private router: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService,
    private ticketTypePipe: TicketTypePipe
  ) {}

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      this.ticket = ticket;
      if(ticket && ticket.countList){
          this.countList = ticket.countList;
      }
    });
  }

  public onMakePayment(): void {
    const { countsToPay, countsToPayAmount } = this.getListOfCountsToPay();
    const formParams = {
      ticketNumber: this.ticket.violationTicketNumber,
      time: this.ticket.violationTime,
      counts: countsToPay,
      amount: countsToPayAmount,
    };

    this.logger.info('onMakePayment', formParams);
  }

  public onPrint(): void {
    window.print();
  }

  private getListOfCountsToPay(): {
    countsToPay: string;
    countsToPayAmount: number;
  } {
    let countsToPay = '';
    let countsToPayAmount = 0;
    let count = 0;

    this.ticket?.offences
      ?.filter((offence) => offence.offenceAgreementStatus === 'PAY')
      .forEach((offence) => {
        if (count > 0) {
          countsToPay += ',';
        }
        countsToPay += offence.offenceNumber;
        countsToPayAmount += offence._amountDue;
        count++;
      });

    return { countsToPay, countsToPayAmount };
  }

  private getListOfCountsDisputed(): string {
    let countsDisputed = '';
    let count = 0;
    this.ticket?.offences
      ?.filter(
        (offence) =>
          offence.offenceAgreementStatus === 'DISPUTE' ||
          offence.offenceAgreementStatus === 'REDUCTION'
      )
      .forEach((offence) => {
        if (count > 0) {
          countsDisputed += ',';
        }
        countsDisputed += offence.offenceNumber;
        count++;
      });
    return countsDisputed;
  }

  public get countsToPay(): string {
    const { countsToPay, countsToPayAmount } = this.getListOfCountsToPay();

    if (countsToPay) {
      if (countsToPay.indexOf(',') > -1) {
        return 'Counts ' + countsToPay;
      } else {
        return 'Count ' + countsToPay;
      }
    }

    return null;
  }

  public get isWitnessPresent(): boolean {
    const witnessPresent = this.ticket?.additional?.witnessPresent;
    return witnessPresent;
  }
  public get isLawyerPresent(): boolean {
    const lawyerPresent = this.ticket?.additional?.lawyerPresent;
    return lawyerPresent;
  }
  public get isInterpreterRequired(): boolean {
    const interpreterRequired = this.ticket?.additional?.interpreterRequired;
    return interpreterRequired;
  }
  public get isReductionRequested(): boolean {
    const filteredOffences = this.ticket?.offences.filter(
      (offence) => offence.offenceAgreementStatus === 'REDUCTION'
    );
    if (filteredOffences?.length > 0) {
      return true;
    }
    return false;
  }
  public get willAppearInCourt(): boolean {
    const filteredOffences = this.ticket?.offences.filter(
      (offence) => offence.reductionAppearInCourt
    );
    if (filteredOffences.length > 0) {
      return true;
    }
    return false;
  }
  public get ticketType(): string {
    const ticketNumber = this.ticket.violationTicketNumber;
    return this.ticketTypePipe.transform(ticketNumber.charAt(0));
  }
  public get countsDisputed(): string {
    const countsDisputed = this.getListOfCountsDisputed();

    if (countsDisputed) {
      if (countsDisputed.indexOf(',') > -1) {
        return 'Counts ' + countsDisputed;
      } else {
        return 'Count ' + countsDisputed;
      }
    }
    return null;
  }
}

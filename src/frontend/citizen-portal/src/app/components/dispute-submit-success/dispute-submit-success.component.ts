import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dispute-submit-success',
  templateUrl: './dispute-submit-success.component.html',
  styleUrls: ['./dispute-submit-success.component.scss'],
})
export class DisputeSubmitSuccessComponent implements OnInit {
  public busy: Subscription;
  public ticket: TicketDisputeView;

  constructor(
    private router: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService
  ) { }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      this.ticket = ticket;
    });
  }

  public onViewYourTicket(): void {
    const ticket = this.disputeService.ticket;
    const params = {
      ticketNumber: ticket.violationTicketNumber,
      time: ticket.violationTime,
    };

    this.disputeService.ticket$.next(null);

    this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
      queryParams: params,
    });
  }

  public onExitTicket(): void {
    this.disputeService.ticket$.next(null);
    this.router.navigate(['/']);
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

    this.busy = this.disputeResource
      .initiateTicketPayment(formParams)
      .subscribe((response) => {
        if (response.redirectUrl) {
          window.location.href = response.redirectUrl;
        }
      });
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
}

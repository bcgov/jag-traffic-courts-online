import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dispute-submit-success',
  templateUrl: './dispute-submit-success.component.html',
  styleUrls: ['./dispute-submit-success.component.scss'],
})
export class DisputeSubmitSuccessComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public ticket: TicketDispute;

  constructor(
    private router: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {}

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }

      this.ticket = ticket;
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.goToTop();
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
    const formParams = {
      ticketNumber: this.ticket.violationTicketNumber,
      time: this.ticket.violationTime,
      counts: this.getListOfCountsToPay(),
    };

    this.logger.info('onMakePayment', formParams);

    this.busy = this.disputeResource
      .initiateTicketPayment(formParams)
      .subscribe((response) => {
        // todo: update later
        if (response.redirectUrl) {
          window.location.href = response.redirectUrl;
        }
      });
  }

  private getListOfCountsToPay(): string {
    let countsToPay = '';
    let count = 0;

    this.ticket?.offences
      ?.filter((offence) => offence.offenceAgreementStatus === 'PAY')
      .forEach((offence) => {
        if (count > 0) {
          countsToPay += ',';
        }
        countsToPay += offence.offenceNumber;
        count++;
      });

    return countsToPay;
  }

  public get countsToPay(): string {
    let countsToPay = this.getListOfCountsToPay();

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

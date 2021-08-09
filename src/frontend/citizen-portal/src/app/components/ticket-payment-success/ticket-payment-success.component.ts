import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-ticket-payment-success',
  templateUrl: './ticket-payment-success.component.html',
  styleUrls: ['./ticket-payment-success.component.scss'],
})
export class TicketPaymentSuccessComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public ticket: TicketDispute;

  constructor(
    private router: Router,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {}

  public ngOnInit(): void {
    // const formParams = { ticketNumber: 'EZ02000460', time: '09:54' };
    // this.disputeResource.getTicket(formParams).subscribe((response) => {
    //   this.disputeService.ticket$.next(response);
    // });

    this.disputeService.ticket$.subscribe((ticket) => {
      this.ticket = ticket;
      this.logger.info('TicketPaymentSuccessComponent current ticket', ticket);

      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.goToTop();
  }
}

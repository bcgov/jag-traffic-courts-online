import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-dispute-submit',
  templateUrl: './dispute-submit.component.html',
  styleUrls: ['./dispute-submit.component.scss'],
})
export class DisputeSubmitComponent implements OnInit {
  public ticket: TicketDispute;

  constructor(private router: Router, private disputeService: DisputeService) {}

  ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      this.ticket = ticket;

      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }
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
}

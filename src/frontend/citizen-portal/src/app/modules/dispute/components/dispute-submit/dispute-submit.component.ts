import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeService } from '@dispute/services/dispute.service';

@Component({
  selector: 'app-dispute-submit',
  templateUrl: './dispute-submit.component.html',
  styleUrls: ['./dispute-submit.component.scss'],
})
export class DisputeSubmitComponent implements OnInit {
  constructor(private router: Router, private disputeService: DisputeService) {}

  ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
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

    this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.SUMMARY)], {
      queryParams: params,
    });
  }
}

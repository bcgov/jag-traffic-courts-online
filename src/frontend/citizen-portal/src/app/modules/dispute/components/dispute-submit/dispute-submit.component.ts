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
    this.disputeService.ticketDispute$.subscribe((ticketDispute) => {
      if (!ticketDispute) {
        this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
      }
    });
  }

  public onViewYourTicket(): void {
    const ticketDispute = this.disputeService.ticketDispute;
    const params = {
      ticketNumber: ticketDispute.violationTicketNumber,
      time: ticketDispute.violationTime,
    };

    this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.SUMMARY)], {
      queryParams: params,
    });
  }
}

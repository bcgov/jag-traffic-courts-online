import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Ticket } from '@shared/models/ticket.model';
import { DisputeService } from '@dispute/services/dispute.service';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent implements OnInit {
  public busy: Subscription;

  public ticket: Ticket;

  constructor(private service: DisputeService) {}

  public ngOnInit(): void {
    this.service.ticket$.subscribe((ticket: Ticket) => {
      this.ticket = ticket;
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { Ticket } from '@shared/models/ticket.model';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent implements OnInit {
  public busy: Subscription;

  public ticket: Ticket;

  constructor(private mockDisputeService: MockDisputeService) {}

  public ngOnInit(): void {
    this.mockDisputeService.ticket$.subscribe((ticket: Ticket) => {
      this.ticket = ticket;
    });
  }

  public onSubmit(): void {
    this.nextRoute();
  }

  public onBack() {}

  public nextRoute() {}
}

import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { Ticket } from '@shared/models/ticket.model';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent implements OnInit {
  public busy: Subscription;

  public ticket: Ticket;

  constructor(private service: DisputeResourceService) {}

  public ngOnInit(): void {
    this.service.ticket$.subscribe((ticket: Ticket) => {
      this.ticket = ticket;
    });
  }

  public onSubmit(): void {
    this.nextRoute();
  }

  public onBack(): void {
    // do nothing for now
  }

  public nextRoute(): void {
    // do nothing for now
  }
}

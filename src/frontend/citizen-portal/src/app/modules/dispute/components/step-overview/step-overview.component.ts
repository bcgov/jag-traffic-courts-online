import { Component, Input, OnInit } from '@angular/core';
import { MatStepper } from '@angular/material/stepper';
import { DisputeService } from '@dispute/services/dispute.service';
import { Ticket } from '@shared/models/ticket.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-step-overview',
  templateUrl: './step-overview.component.html',
  styleUrls: ['./step-overview.component.scss'],
})
export class StepOverviewComponent implements OnInit {
  @Input() public stepper: MatStepper;

  public busy: Subscription;
  public ticket: Ticket;

  constructor(private disputeService: DisputeService) {}

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket: Ticket) => {
      this.ticket = ticket;
    });
  }
}

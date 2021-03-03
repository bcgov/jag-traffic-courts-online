import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
})
export class FindTicketComponent {
  constructor(
    private route: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService
  ) {}

  public onSearch(): void {
    this.disputeResource.getDispute().subscribe((response) => {
      this.disputeService.dispute$.next(response);
      this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.STEPPER)]);
    });
  }
}

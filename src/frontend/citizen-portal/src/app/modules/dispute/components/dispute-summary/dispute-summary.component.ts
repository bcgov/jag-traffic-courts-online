import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Dispute } from '@shared/models/dispute.model';

@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit {
  public dispute: Dispute;

  constructor(
    private route: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService
  ) {}

  ngOnInit(): void {
    this.disputeResource.getDispute().subscribe((response) => {
      this.dispute = response;
    });
  }

  public onDispute(): void {
    this.disputeService.dispute$.next(this.dispute);
    this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.STEPPER)]);
  }
}

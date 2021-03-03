import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Dispute } from '@shared/models/dispute.model';

@Component({
  selector: 'app-dispute-list',
  templateUrl: './dispute-list.component.html',
  styleUrls: ['./dispute-list.component.scss'],
})
export class DisputeListComponent implements OnInit {
  public disputes: Dispute[];

  // public dataSource = new MatTableDataSource([]);

  public columnsToDisplay: string[] = [
    'violationTicketNumber',
    'violationDate',
    'violationTime',
  ];

  constructor(
    private route: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService
  ) {
    // this.dataSource = new MatTableDataSource([]);
  }

  ngOnInit(): void {
    this.disputeResource.getDisputes().subscribe((response) => {
      this.disputes = response;

      // this.dataSource.data = response;
    });
  }

  public onSelect(dispute: Dispute): void {
    this.disputeService.dispute$.next(dispute);
    this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.STEPPER)]);
  }
}

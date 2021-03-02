import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { LoggerService } from '@core/services/logger.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { Dispute } from '@shared/models/dispute.model';

@Component({
  selector: 'app-dispute-list',
  templateUrl: './dispute-list.component.html',
  styleUrls: ['./dispute-list.component.scss'],
})
export class DisputeListComponent implements OnInit {
  public disputes: Dispute[];

  public dataSource = new MatTableDataSource([]);

  public columnsToDisplay: string[] = [
    'violationTicketNumber',
    'violationDate',
    'violationTime',
  ];

  constructor(
    private disputeResource: DisputeResourceService,
    private logger: LoggerService
  ) {
    this.dataSource = new MatTableDataSource([]);
  }

  ngOnInit(): void {
    this.disputeResource.getDisputes().subscribe((response) => {
      this.disputes = response;
      this.logger.info('disputes', this.disputes);

      this.dataSource.data = response;
    });
  }
}

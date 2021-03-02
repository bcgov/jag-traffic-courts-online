import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { LoggerService } from '@core/services/logger.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Ticket } from '@shared/models/ticket.model';

@Component({
  selector: 'app-dispute-list',
  templateUrl: './dispute-list.component.html',
  styleUrls: ['./dispute-list.component.scss'],
})
export class DisputeListComponent implements OnInit {
  public tickets: Ticket[];

  public dataSource = new MatTableDataSource([]);

  public columnsToDisplay: string[] = [
    'violationTicketNumber',
    'violationDate',
    'violationTime',
  ];

  constructor(
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService
  ) {
    this.dataSource = new MatTableDataSource([]);
  }

  ngOnInit(): void {
    this.disputeResource.getTickets().subscribe((response) => {
      this.tickets = response;
      this.logger.info('tickets', this.tickets);

      this.dataSource.data = response;
      this.disputeService.tickets$.next(response);
    });
  }
}

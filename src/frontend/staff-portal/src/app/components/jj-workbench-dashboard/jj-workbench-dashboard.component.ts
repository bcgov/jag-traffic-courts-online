import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JjDisputeService, JjDispute } from 'app/services/jj-dispute.service';
import { DisputeStatus } from 'app/api';
import { LoggerService } from '@core/services/logger.service';
import { of, Subscription } from 'rxjs';

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
})
export class JjWorkbenchDashboardComponent implements OnInit, AfterViewInit {
  busy: Subscription;

  data = [];
  dataSource = new MatTableDataSource();
  @ViewChild(MatSort) sort = new MatSort();
  displayedColumns: string[] = [
    "ticketNumber",
    "violationDate",
    "disputantName",
    "enforcementOfficer",
    "policeDetachment",
    "courthouseLocation",
    "status",
  ];

  constructor(
    public jjDisputeService: JjDisputeService,
    private logger: LoggerService,
  ) { }

  ngOnInit(): void {
    this.getAll();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  getAll(): void {
    this.logger.log('TicketPageComponent::getAllDisputes');

    this.jjDisputeService.getJjDisputes().subscribe((response) => {
      this.data = response;
      this.dataSource.data = this.data;

      // initially sort data by Date Submitted
      this.dataSource.data = this.dataSource.data.sort((a: JjDispute, b: JjDispute) => { if (a.violationDate > b.violationDate) { return -1; } else { return 1 } });

      // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = function (record: JjDispute, filter) {
        return record.ticketNumber.toLocaleLowerCase().indexOf(filter.toLocaleLowerCase()) > -1;
      }
    });
  }

  // called on keyup in filter field
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}
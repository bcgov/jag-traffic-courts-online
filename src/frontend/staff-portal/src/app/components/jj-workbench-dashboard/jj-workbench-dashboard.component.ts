import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { JJDispute } from '../../api/model/jJDispute.model';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { JJDisputeStatus } from 'app/api';

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
})
export class JjWorkbenchDashboardComponent implements OnInit, AfterViewInit {
  busy: Subscription;
  jjDisputeInfo: JJDispute;

  data = [] as JJDispute[];
  showDispute: boolean = false;
  jjPage: string = "Assignments";
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
    public jjDisputeService: JJDisputeService,
    private logger: LoggerService,
  ) { }

  ngOnInit(): void {
    this.getAll();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backTicketList(element) {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage = "Dispute Details";
    else this.jjPage = "Assignments";
    this.jjDisputeInfo = element;
    if (!this.showDispute) this.getAll();  // refresh list
  }

  backTicketpage() {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage = "Dispute Details";
    else this.jjPage = "Assignments";
    if (!this.showDispute) this.getAll(); // refresh list
  }

  getAll(): void {
    this.logger.log('JJWorkbenchDashboardComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputes().subscribe((response: JJDispute[]) => {
      // filter jj disputes only show new or in progress
      this.data = response.filter(x => x.status === JJDisputeStatus.New || x.status === JJDisputeStatus.InProgress);
      this.dataSource.data = this.data;


      // initially sort data by Date Submitted
      this.dataSource.data = this.dataSource.data.sort((a: JJDispute, b: JJDispute) => { if (a.violationDate > b.violationDate) { return -1; } else { return 1 } });

      // this section allows filtering only on ticket number or partial ticket number by setting the filter predicate
      this.dataSource.filterPredicate = function (record: JJDispute, filter) {
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

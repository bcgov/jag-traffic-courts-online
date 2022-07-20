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
export class JjWorkbenchDashboardComponent {
  busy: Subscription;
  jjDisputeInfo: JJDispute;

  jjPage: string = "Assignments";

  constructor(
    public jjDisputeService: JJDisputeService,
    private logger: LoggerService,
  ) { }

  changeHeading(heading: any) {
    console.log(heading);
    this.jjPage = heading;
  }

}

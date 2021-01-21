import { Component, OnInit } from '@angular/core';
import { DisputeRoutes } from '@dispute/dispute.routes';
import {
  DashboardMenuItem,
  DashboardRouteMenuItem,
} from 'app/modules/dashboard/models/dashboard-menu-item.model';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-dispute-dashboard',
  templateUrl: './dispute-dashboard.component.html',
  styleUrls: ['./dispute-dashboard.component.scss'],
})
export class DisputeDashboardComponent implements OnInit {
  public dashboardMenuItems: Observable<DashboardMenuItem[]>;

  public ngOnInit(): void {
    this.dashboardMenuItems = this.getDashboardMenuItems();
  }

  private getDashboardMenuItems(): Observable<DashboardMenuItem[]> {
    return of([
      new DashboardRouteMenuItem('Dispute', DisputeRoutes.HOME, 'pages'),
      new DashboardRouteMenuItem('Review', DisputeRoutes.OVERVIEW, 'preview'),
    ]);
  }
}

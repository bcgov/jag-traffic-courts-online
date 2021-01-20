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

  // constructor() {}

  public ngOnInit(): void {
    this.dashboardMenuItems = this.getDashboardMenuItems();
  }

  private getDashboardMenuItems(): Observable<DashboardMenuItem[]> {
    return of([
      new DashboardRouteMenuItem('Dispute', DisputeRoutes.HOME, 'people'),
      new DashboardRouteMenuItem('Part A', DisputeRoutes.PART_A, 'store'),
      new DashboardRouteMenuItem('Part B', DisputeRoutes.PART_B, 'store'),
      new DashboardRouteMenuItem('Part C', DisputeRoutes.PART_C, 'store'),
      new DashboardRouteMenuItem('Part D', DisputeRoutes.PART_D, 'store'),
      new DashboardRouteMenuItem(
        'Overview',
        DisputeRoutes.OVERVIEW,
        'show_chart'
      ),
    ]);
  }
}

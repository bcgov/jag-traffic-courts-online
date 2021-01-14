import { Observable } from 'rxjs';

import { DashboardMenuItem } from '../models/dashboard-menu-item.model';

export interface IDashboard {
  /**
   * @description
   * Stream of stateful dashboard menu items.
   */
  dashboardMenuItems: Observable<DashboardMenuItem[]>;
}

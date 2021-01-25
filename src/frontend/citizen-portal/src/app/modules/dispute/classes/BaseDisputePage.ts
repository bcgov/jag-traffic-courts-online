import { ActivatedRoute, Router, NavigationExtras } from '@angular/router';

import { Subscription } from 'rxjs';
import { DisputeRoutes } from '../dispute.routes';

export interface IBaseDisputePage {
  busy: Subscription;
  DisputeRoutes: DisputeRoutes;
}

export abstract class BaseDisputePage implements IBaseDisputePage {
  public busy: Subscription;
  public isComplete: boolean;

  // Allow the use of enum in the component template
  public DisputeRoutes = DisputeRoutes;

  constructor(protected route: ActivatedRoute, protected router: Router) {
    this.isComplete = true;
  }

  public routeIndex(currentRoute: string): number {
    const stepRoutes = this.DisputeRoutes.stepRoutes();
    return stepRoutes.findIndex((element) => element === currentRoute);
  }

  public routeNext(currentRoute: string) {
    const stepRoutes = this.DisputeRoutes.stepRoutes();

    const findIndex = this.routeIndex(currentRoute);
    if (findIndex < 0) return;

    this.routeTo(stepRoutes[findIndex + 1]);
  }

  public routeBack(currentRoute: string) {
    const stepRoutes = this.DisputeRoutes.stepRoutes();

    const findIndex = this.routeIndex(currentRoute);
    if (findIndex <= 0) return;

    this.routeTo(stepRoutes[findIndex - 1]);
  }

  public routeTo(
    routePath: DisputeRoutes,
    navigationExtras: NavigationExtras = {}
  ) {
    this.router.navigate([routePath], {
      relativeTo: this.route.parent,
      ...navigationExtras,
    });
  }
}

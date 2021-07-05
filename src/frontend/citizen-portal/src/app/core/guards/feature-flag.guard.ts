import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { AppConfigService } from 'app/services/app-config.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FeatureFlagGuard implements CanActivate {
  constructor(
    private router: Router,
    private appConfigService: AppConfigService
  ) {}

  private isFeatureFlagEnabled(featureFlag: string) {
    return this.appConfigService.isFeatureFlagEnabled(featureFlag);
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    const featureFlag = route.data['featureFlag'];
    const isEnabled = this.isFeatureFlagEnabled(featureFlag);

    if (!isEnabled) {
      this.router.navigate(['/']);
    }

    return isEnabled;
  }
}

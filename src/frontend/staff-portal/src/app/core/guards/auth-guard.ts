import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { createAuthGuard, AuthGuardData } from 'keycloak-angular';

import { AppRoutes } from 'app/app.routes';
import { AuthService } from 'app/services/auth.service';

const isAccessAllowed = async (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
  authData: AuthGuardData,
): Promise<boolean | UrlTree> => {
  const { authenticated } = authData;
  const authService = inject(AuthService);
  const router = inject(Router);

  // Force the user to log in if currently unauthenticated.
  let permission: boolean;
  if (!authenticated) {
    await authService.login();
  }

  // Get the roles required from the route.
  const requiredRoles = route.data.roles;

  // Allow the user to to proceed if no additional roles are required to access the route.
  if (!requiredRoles || requiredRoles.length === 0) {
    permission = true;
  } else {
    // Allow the user to proceed if any of the required role(s) is/are present.
    permission = authService.checkRoles(requiredRoles);
  }

  if (!permission) {
    let application: string;
    if (state.url.indexOf(AppRoutes.JJ) > -1) {
      application = 'JJ';
    } else {
      application = 'Staff';
    }

    router.navigate([AppRoutes.UNAUTHORIZED], {
      queryParams: { application: application },
    });
  }

  return permission;
};

export const authorizationGuard =
  createAuthGuard<CanActivateFn>(isAccessAllowed);

import { Injectable } from '@angular/core';
import { UserRepresentation as UserRepresentationBase } from 'app/api';
import { AppRoutes } from 'app/app.routes';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakService as KeycloakAPIService } from 'app/api'
import { KeycloakProfile as KeycloakProfileJS } from 'keycloak-js';
import { BehaviorSubject, from, Observable, map, catchError, forkJoin } from 'rxjs';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { ConfigService } from '@config/config.service';
import { UserGroup } from '@shared/enums/user-group.enum';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private _isLoggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
  private _userProfile: BehaviorSubject<KeycloakProfile> = new BehaviorSubject<KeycloakProfile>(null);
  private _jjList: BehaviorSubject<UserRepresentation[]> = new BehaviorSubject<UserRepresentation[]>([]);
  private _vtcList: BehaviorSubject<UserRepresentation[]> = new BehaviorSubject<UserRepresentation[]>([]);

  private site: string = "staff-api";
  private roles = [
    { name: UserGroup.JUDICIAL_JUSTICE, redirectUrl: AppRoutes.JJWORKBENCH },
    { name: UserGroup.VTC_STAFF, redirectUrl: AppRoutes.TICKET },
  ]

  constructor(
    private keycloak: KeycloakService,
    private keycloakAPI: KeycloakAPIService,
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
  ) { }

  checkAuth(): Observable<boolean> {
    return from(this.keycloak.isLoggedIn())
      .pipe(
        map((response: boolean) => {
          if (response) {
            this.loadUserProfile().subscribe(() => {
              this._isLoggedIn.next(response);
              return response;
            });
          } else {
            this._isLoggedIn.next(response);
            return response;
          }
        })
      );
  }

  get token(): string {
    return this.keycloak.getKeycloakInstance().token;
  }

  get isLoggedIn$(): Observable<boolean> {
    return this._isLoggedIn.asObservable();
  }

  get isLoggedIn(): boolean {
    return this._isLoggedIn.value;
  }

  loadUserProfile(): Observable<KeycloakProfile> {
    return from(this.keycloak.loadUserProfile())
      .pipe(
        map((userProfile: KeycloakProfile) => {
          userProfile.idir = this.getIDIR(userProfile);
          userProfile.fullName = this.getFullName(userProfile);
          this._userProfile.next(userProfile);
          return userProfile;
        })
      )
  }

  loadUsersLists(): Observable<any> {
    let observables = {
      jjList: this.getUsersInGroup(UserGroup.JUDICIAL_JUSTICE),
      vtcList: this.getUsersInGroup(UserGroup.VTC_STAFF),
    };
    return forkJoin(observables).pipe(
      map(results => {
        this._jjList.next(results.jjList
          .map(u => {
            u.jjDisplayName = u.fullName ? "JJ " + u.fullName : "";
            return u;
          })
          .sort((a, b) => {
            if (a.fullName < b.fullName) { return -1; }
            else { return 1 }
          }));
        this._vtcList.next(results.vtcList);
      }
      ));
  }

  get userProfile$(): Observable<KeycloakProfile> {
    return this._userProfile.asObservable();
  }

  get userProfile(): KeycloakProfile {
    return this._userProfile.value;
  }

  get jjList$(): Observable<UserRepresentation[]> {
    return this._jjList.asObservable();
  }

  get jjList(): UserRepresentation[] {
    return this._jjList.value;
  }

  get vtcList$(): Observable<UserRepresentation[]> {
    return this._vtcList.asObservable();
  }

  get vtcList(): UserRepresentation[] {
    return this._vtcList.value;
  }

  private getIDIR(user: UserRepresentation | KeycloakProfile): string {
    return (user.attributes?.idir_username.length > 0 ? user.attributes?.idir_username[0] : "").toUpperCase();
  }

  private getFullName(user: UserRepresentation | KeycloakProfile): string {
    return user.attributes?.display_name.length > 0 ? user.attributes?.display_name[0] : "";
  }

  login() {
    this.keycloak.login({ redirectUri: window.location.toString() });
  }

  logout() {
    this.keycloak.logout();
    this._isLoggedIn.next(false);
    this._userProfile.next(null);
  }

  getRedirectUrl(): string {
    var result;
    this.roles.forEach(r => {
      if (this.keycloak.isUserInRole(r.name, this.site)) {
        result = r.redirectUrl;
      }
    })
    if (!result) {
      result = AppRoutes.UNAUTHORIZED;
    }
    return result;
  }

  checkRole(role: string): boolean {
    return this.keycloak.isUserInRole(role, this.site);
  }

  getUsersInGroup(group: string): Observable<Array<UserRepresentation>> {
    return this.keycloakAPI.apiKeycloakGroupNameUsersGet(group)
      .pipe(
        map((response: UserRepresentation[]) => {
          this.logger.info('KeycloakService::getUsersInGroup', response)
          response.forEach((user: UserRepresentation) => {
            user.idir = this.getIDIR(user);
            user.fullName = this.getFullName(user);
          })
          return response ? response.filter(u => u.idir) : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail || this.configService.keycloak_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.keycloak_error);
          this.logger.error(
            'KeycloakService::getUsersInGroup Error has occured ',
            error
          );
          throw error;
        })
      );
  }
}

export interface UserRepresentation extends UserRepresentationBase {
  idir?: string;
  fullName?: string;
  jjDisplayName?: string;
}

export interface KeycloakProfile extends KeycloakProfileJS {
  idir?: string;
  fullName?: string;
  attributes?: { [key: string]: Array<string>; } | null;
}

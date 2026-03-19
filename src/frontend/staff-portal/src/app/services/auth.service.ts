import { Injectable } from '@angular/core';
import { UserRepresentation as UserRepresentationBase } from 'app/api';
import { AppRoutes } from 'app/app.routes';
import { KeycloakService as KeycloakAPIService } from 'app/api'
import Keycloak, { KeycloakProfile as KeycloakProfileJS } from 'keycloak-js';
import { BehaviorSubject, from, Observable, map, catchError, forkJoin, first, of } from 'rxjs';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { ConfigService } from '@config/config.service';
import { UserGroup } from '@shared/enums/user-group.enum';
import { AppState, JJDisputeStore } from 'app/store';
import { Store } from '@ngrx/store';
import { LookupsService } from './lookups.service';

const clientId = "staff-api";

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private isInit = true;
  private _isLoggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
  private _userProfile: BehaviorSubject<KeycloakProfile> = new BehaviorSubject<KeycloakProfile>(null);
  private _jjList: BehaviorSubject<UserRepresentation[]> = new BehaviorSubject<UserRepresentation[]>([]);
  private _vtcList: BehaviorSubject<UserRepresentation[]> = new BehaviorSubject<UserRepresentation[]>([]);

  private roles = [
    { name: [UserGroup.JUDICIAL_JUSTICE, UserGroup.ADMIN_JUDICIAL_JUSTICE, UserGroup.SUPPORT_STAFF], redirectUrl: AppRoutes.JJ },
    { name: [UserGroup.VTC_STAFF, UserGroup.SUPPORT_STAFF], redirectUrl: AppRoutes.STAFF },
  ]

  constructor(
    private keycloak: Keycloak,
    private keycloakAPI: KeycloakAPIService,
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private lookupsService: LookupsService,
    private store: Store<AppState>,
  ) {}

  async checkAuth(): Promise<boolean> {
    if (!this.keycloak.authenticated) {
        await this.login();
    }

    const response = this.keycloak.authenticated;
    if (response) {
        this.loadUserProfile().subscribe(() => {
            this._isLoggedIn.next(response);
            if (this.isLoggedIn && this.isInit) {
            this.userProfile$.pipe(first()).subscribe(() => {
                this.isInit = false;
            })
            }
            return response;
        });
    } else {
        this._isLoggedIn.next(response);
        return response;
    }
  }

  loadLookupData(){
    let observables = [
      this.loadUsersLists(),
      this.lookupsService.init()
    ];

    forkJoin(observables).subscribe({
      next: _results => {
        this.store.dispatch(JJDisputeStore.Actions.Get());
      },
      error: _err => {
        this.logger.error("Landing Page Init: Initial data loading failed");
      }
    });
  }

  get token(): string {
    return this.keycloak.token;
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

  async login() {
    this.keycloak.login({ redirectUri: window.location.toString() });
  }

  async logout() {
    await this.keycloak.logout();
    this._isLoggedIn.next(false);
    this._userProfile.next(null);
  }

  getRedirectUrl(): string {
    let result: string;
    this.roles.forEach(r => {
      r.name.forEach(n => {
        if (this.checkRole(n)) {
          result = r.redirectUrl;
        }
      });
    });
    if (!result) {
      result = AppRoutes.UNAUTHORIZED;
    }
    return result;
  }

  checkRole(role: string): boolean {
    return this.keycloak.hasResourceRole(role, clientId);
  }

  /**
   * Check if the user has any of the specified roles.
   *
   * @param {string[]} roles - An array of roles to check.
   * @return {boolean} - true if the user has any of the specified roles, false otherwise.
   */
  checkRoles(roles: string[]): boolean {
    return roles.some(role => this.checkRole(role));
  }

  getUsersInGroup(group: string): Observable<Array<UserRepresentation>> {
    if (group === UserGroup.JUDICIAL_JUSTICE && this._jjList.value?.length > 0) {
      return of(this._jjList.value);
    }

    if (group === UserGroup.VTC_STAFF && this._vtcList.value?.length > 0) {
      return of(this._vtcList.value);
    }

    return this.keycloakAPI.apiKeycloakGroupNameUsersGet(group)
      .pipe(
        map((response: UserRepresentation[]) => {
          this.logger.info('AuthService::getUsersInGroup', response)
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
            'AuthService::getUsersInGroup Error has occured ',
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

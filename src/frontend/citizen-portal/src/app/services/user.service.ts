import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UserService as UserBaseService, UserInfo } from 'app/api';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface IUserService {
  user$: Observable<UserInfo>;
  user: UserInfo;
  getUser(): Observable<UserInfo>;
}

@Injectable({
  providedIn: 'root',
})
export class UserService implements IUserService {
  private _user: BehaviorSubject<UserInfo> = new BehaviorSubject<UserInfo>(null);

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private userService: UserBaseService
  ) {
  }

  /**
     * Get the User Info from BC Services Card
     *
     * @param none
     */
  public getUser(): Observable<UserInfo> {
    return this.userService.apiUserWhoamiGet()
      .pipe(
        map((response: UserInfo) =>
          response ? response : null
        ),
        tap((user) =>
          this.logger.info('UserService::getUser', user)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.user_error);
          this.logger.error(
            'UserService::getUser error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get user$(): Observable<UserInfo> {
    return this._user.asObservable();
  }

  public get user(): UserInfo {
    return this._user.value;
  }
}

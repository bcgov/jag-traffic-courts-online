import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UserService as UserBaseService } from 'app/api';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface IUserService {
  response$: Observable<any>;
  response: any;
  getUser(): Observable<any>;
}

@Injectable({
  providedIn: 'root',
})
export class UserService implements IUserService {
  private _response: BehaviorSubject<any> = new BehaviorSubject<any>(null);

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
  public getUser(): Observable<any> {
    return this.userService.apiUserWhoamiGet()
      .pipe(
        map((response: any) =>
          response ? response : null
        ),
        tap((user) =>
          this.logger.info('UserService::getUser')
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

  public get response$(): Observable<any> {
    return this._response.asObservable();
  }

  public get response(): any {
    return this._response.value;
  }
}

import { Injectable } from '@angular/core';
import { User } from '@shared/models/user.model';
import { from, Observable, of } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class MockAuthService {
  public init(): Promise<boolean> {
    return Promise.resolve(true);
  }

  public isLoggedIn(): Promise<boolean> {
    return Promise.resolve(true);
  }

  public async login(): Promise<void> {
    // empty, just for mock login function.
  }

  public async logout(redirectUrl): Promise<void> {
    // empty, just for mock logout function.
  }

  public async getUser(forceReload?: boolean): Promise<User> {
    return Promise.resolve({
      firstName: 'mockFirstName',
      lastName: 'mockLastName',
    });
  }

  public getUser$(forceReload?: boolean): Observable<User> {
    return of({
      firstName: 'mockFirstName',
      lastName: 'mockLastName',
    });
  }
}

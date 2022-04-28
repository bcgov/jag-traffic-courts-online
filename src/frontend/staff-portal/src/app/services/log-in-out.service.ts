import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LogInOutService {

  private logoutStatus:BehaviorSubject<string> = new BehaviorSubject<string>('');
  private currentStatus:BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  getLogoutStatus = this.logoutStatus.asObservable();
  getCurrentStatus = this.currentStatus.asObservable();

  constructor() { }

  logoutUser(status){
    this.logoutStatus.next(status);
  }

  currentUser(status){
    this.currentStatus.next(status);
  }
}

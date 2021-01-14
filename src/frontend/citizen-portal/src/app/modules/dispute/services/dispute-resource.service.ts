import { Injectable } from '@angular/core';
import { Ticket } from '@shared/models/ticket.model';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  private _ticket: BehaviorSubject<Ticket>;

  constructor() {
    this._ticket = new BehaviorSubject<Ticket>(null);
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  // public getUsers(): Observable<any> {
  // return  http.get<[User]>('https://jsonplaceholder.typicode.com/users').subscribe( res => {
  //     this.users = res;
  // });
  // return of(
  //   new HttpResponse({
  //     status: 200,
  //     body: [
  //       {
  //         id: 1,
  //         name: 'Viktor',
  //       },
  //       {
  //         id: 2,
  //         name: 'John',
  //       },
  //     ],
  //   })
  // );
  // return this.httpClient
  //   .get<any[]>('https://jsonplaceholder.typicode.com/users')
  //   .pipe(
  //     tap((data: any) => this.logger.info('getUsers', data)),
  //     catchError((error: any) => {
  //       this.toastService.openErrorToast('Data could not be retrieved');
  //       this.logger.error(
  //         '[Dispute] DisputeResourceService::getUsers error has occurred: ',
  //         error
  //       );
  //       throw error;
  //     })
  //   );
  // return this.httpClient.get<any[]>(this.baseUrl + `/physical/file/${physId}/lists`)
  // .pipe(
  //   catchError((error: any) => {
  //     throw error;
  //   })
  // );
  // return this.disputMockService.getUsers().pipe(
  //   tap((data: any) => this.logger.info('getUsers', data)),
  //   catchError((error: any) => {
  //     this.toastService.openErrorToast('Data could not be retrieved');
  //     this.logger.error(
  //       '[Dispute] DisputeResourceService::getUsers error has occurred: ',
  //       error
  //     );
  //     throw error;
  //   })
  // );
  // }
}

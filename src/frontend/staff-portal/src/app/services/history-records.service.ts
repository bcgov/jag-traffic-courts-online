import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { EventEmitter, Injectable } from '@angular/core';
import { FileHistoryService, FileHistory, EmailHistory, EmailHistoryService } from 'app/api';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class HistoryRecordService {
  private _FileHistories: BehaviorSubject<FileHistory[]> = new BehaviorSubject<FileHistory[]>(null);
  private _EmailHistories: BehaviorSubject<EmailHistory[]> = new BehaviorSubject<EmailHistory[]>(null);
  public refreshDisputes: EventEmitter<any> = new EventEmitter();

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private fileHistoryService: FileHistoryService,
    private emailHistoryService: EmailHistoryService,
    private authService: AuthService
  ) {
  }

  /**
     * Get the File Histories from RSI
     *
     * @param ticketNumber
     */
  public getFileHistories(ticketNumber: string): Observable<FileHistory[]> {
    return this.fileHistoryService.apiFilehistoryFilehistoryGet(ticketNumber)
      .pipe(
        map((response: FileHistory[]) => {
          this.logger.info('HistoryService::getFileHistories', response);
          this._FileHistories.next(response);
          return response;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.history_error);
          this.logger.error(
            'HistoryService::getFileHistories error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get FileHistories$(): Observable<FileHistory[]> {
    return this._FileHistories.asObservable();
  }

  public get FileHistories(): FileHistory[] {
    return this._FileHistories.value;
  }

  /**
     * Get the Email Histories from RSI
     *
     * @param ticketNumber
     */
   public getEmailHistories(ticketNumber: string): Observable<EmailHistory[]> {
    return this.emailHistoryService.apiEmailhistoryEmailhistoryGet(ticketNumber)
      .pipe(
        map((response: EmailHistory[]) => {
          this.logger.info('HistoryService::getEmailHistories', response);
          this._EmailHistories.next(response);
          return response;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.history_error);
          this.logger.error(
            'HistoryService::getEmailHistories error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get EmailHistories$(): Observable<EmailHistory[]> {
    return this._EmailHistories.asObservable();
  }

  public get EmailHistories(): EmailHistory[] {
    return this._EmailHistories.value;
  }
}

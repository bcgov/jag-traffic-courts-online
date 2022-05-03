import { DatePipe } from '@angular/common';
import { Injectable, KeyValueDiffers } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ImageTicketNotFoundDialogComponent } from '@shared/dialogs/image-ticket-not-found-dialog/image-ticket-not-found-dialog.component';
import { TicketNotFoundDialogComponent } from '@shared/dialogs/ticket-not-found-dialog/ticket-not-found-dialog.component';
import { FileUtilsService } from '@shared/services/file-utils.service';
import { Field, OcrViolationTicket, TicketsService, ViolationTicket } from 'app/api';
import { AppRoutes } from 'app/app.routes';
import { NgProgressRef } from 'ngx-progressbar';
import { BehaviorSubject, catchError, map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ViolationTicketService {
  private _ticket: BehaviorSubject<ViolationTicket> = new BehaviorSubject<ViolationTicket>(null);
  private _ocrTicket: BehaviorSubject<OcrViolationTicket> = new BehaviorSubject<OcrViolationTicket>(null);
  private _inputTicketData: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  public ocrTicketDateKey = "violation_date";
  public ocrTicketTimeKey = "violation_time";

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private logger: LoggerService,
    private ticketService: TicketsService,
    private fileUtilsService: FileUtilsService,
    private datePipe: DatePipe,
  ) {
  }

  public get ticket$(): BehaviorSubject<ViolationTicket> {
    return this._ticket;
  }

  public get ticket(): ViolationTicket {
    return this._ticket.value;
  }

  public get ocrTicket$(): BehaviorSubject<OcrViolationTicket> {
    return this._ocrTicket;
  }

  public get ocrTicket(): OcrViolationTicket {
    return this._ocrTicket.value;
  }

  public get inputTicketData$() {
    return this._inputTicketData;
  }

  public get inputTicketData() {
    return this._inputTicketData.value;
  }

  public searchTicket(params, redirect: boolean = true): Observable<ViolationTicket> {
    this.reset();
    this.logger.info('ViolationTicketService:: Search for ticket');
    return this.ticketService.apiTicketsSearchGet(params.ticketNumber, params.time)
      .pipe(
        map((response: ViolationTicket) => {
          if (response) {
            this.ticket$.next(response);
            if (redirect) {
              this.goToDisputeSummary(params);
            }
          }
          else {
            this.logger.error('ViolationTicketService::searchTicket ticket not found');
            this.onError();
          }
          return response;
        }),
        catchError((error: any) => {
          this.logger.error('ViolationTicketService::searchTicket error has occurred: ', error);
          this.onError();
          throw error;
        })
      );
  }

  public analyseTicket(ticketFile: File, progressRef: NgProgressRef): void {
    this.reset();
    this.logger.info('file target', ticketFile);
    if (!ticketFile) {
      this.logger.info('You must select a file');
      return;
    }
    progressRef.start();
    this.fileUtilsService.readFileAsDataURL(ticketFile).subscribe(ticketImage => {
      const input = {
        filename: ticketFile.name,
        ticketFile,
        ticketImage
      };

      this.ticketService.apiTicketsAnalysePost(ticketFile)
        .subscribe(res => {
          if (res) {
            this.ocrTicket$.next(res);
            this.ticket$.next(this.fromOCR(res));
            this.inputTicketData$.next(input);
            this.router.navigate([AppRoutes.disputePath(AppRoutes.SCAN)]);
          }
          else {
            this.onError();
          }
        }, (err) => {
          this.onError(err);
        })
    })
  }

  private fromOCR(source: OcrViolationTicket): ViolationTicket {
    let result = <ViolationTicket>{};
    let isDateFound = false;
    let isTimeFound = false;

    // Direct convertion
    let keys = Object.keys(source.fields).filter(i => i.toLowerCase().indexOf(".") === -1);
    keys.forEach(key => {
      let value = this.getValue(key, <Field>source.fields[key]);
      result[key] = value;

      if (value && key === this.ocrTicketDateKey) {
        isDateFound = true;
      }
      if (value && key === this.ocrTicketTimeKey) {
        isTimeFound = true;
      }
    })

    // Dynamic convertion for object array
    let arrayKeys = Object.keys(source.fields).filter(i => i.toLowerCase().indexOf(".") > 0 && i.split(".").length === 3);
    if (arrayKeys.length > 0) {
      arrayKeys.forEach(arrayKey => {
        let value = this.getValue(arrayKey, <Field>source.fields[arrayKey]);
        let keySplit = arrayKey.split(".");

        let temp = keySplit[1].split("_"); // e.g. count_1
        let idKey = temp.length > 1 ? temp[0] : "id";
        let id = temp.length > 1 ? parseInt(temp[1]) : parseInt(temp[0]);

        let arrKey = keySplit[0];
        let index = id - 1;
        let key = keySplit[2];

        if (index >= 0) {
          if (!result[arrKey]) {
            result[arrKey] = [];
          }
          if (!result[arrKey][index]) {
            result[arrKey][index] = {};
            result[arrKey][index][idKey] = id;
          }
          result[arrKey][index][key] = value;
        }
      })
    }

    // special handling
    if (isDateFound || isTimeFound) {
      result.issued_date = result[this.ocrTicketDateKey] + " " + result[this.ocrTicketTimeKey];
    }
    if (isDateFound) {
      result[this.ocrTicketDateKey] = this.datePipe.transform(result[this.ocrTicketDateKey], "MMM dd, YYYY");
    }
    return result;
  }

  private getValue(key: string, field: Field): any { // key for logging only
    this.logger.info(`${key}: '${field.value || '<missing>'}', with confidence of ${field.fieldConfidence}`);
    let result;
    let value = field.value;
    if (field.type && value && value.length > 0) {
      switch (field.type.toLowerCase()) {
        case "double":
          result = parseFloat(value.replace(/[^.0-9]/g, '')); // regex replace characters other than numbers
          break;
        case "int64":
          result = parseInt(value.replace(/[^.0-9]/g, '')); // regex replace characters other than numbers
          break;
        case "selectionmark":
          result = value.toLowerCase() === "selected" ? true : false;
          break;
        case "time":
          result = value.replace(' ', ':');
          break;
        case "string":
        default:
          result = value;
          break;
      }
    }
    return result;
  }

  goToDisputeSummary(paramsInput?): void { // start of dispute
    if (paramsInput || this.ticket) {
      let params = paramsInput ?? {
        ticketNumber: this.ticket.ticket_number,
        time: this.datePipe.transform(this.ticket.issued_date, "HH:mm"),
      }
      this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
        queryParams: params,
      });
    } else {
      this.goToFind();
    }
  }

  goToFind(): void {
    this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
  }

  private reset(): void {
    this.inputTicketData$.next(null);
    this.ocrTicket$.next(null);
    this.ticket$.next(null);
  }

  private onError(err?: any): void {
    this.reset();
    if (!err) {
      this.dialog.open(TicketNotFoundDialogComponent);
    } else {
      this.openImageTicketNotFoundDialog(err);
    }
    this.goToFind();
  }

  private openImageTicketNotFoundDialog(err) {
    const data: DialogOptions = {
      titleKey: err.error.title,
      actionType: 'warn',
      messageKey: err.error.errors.file ? err.error.errors.file[0] : err.error.errors[0],
      actionTextKey: 'Ok',
      cancelHide: true,
    };
    return this.dialog.open(ImageTicketNotFoundDialogComponent, { data })
  }
}

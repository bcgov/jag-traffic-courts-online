import { DatePipe } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { Injectable, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { LoggerService } from "@core/services/logger.service";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ImageTicketNotFoundDialogComponent } from "@shared/dialogs/image-ticket-not-found-dialog/image-ticket-not-found-dialog.component";
import { TicketNotFoundDialogComponent } from "@shared/dialogs/ticket-not-found-dialog/ticket-not-found-dialog.component";
import { TicketTypePipe } from "@shared/pipes/ticket-type.pipe";
import { FileUtilsService } from "@shared/services/file-utils.service";
import { Field, OcrViolationTicket, TicketsService, ViolationTicket } from "app/api";
import { AppRoutes } from "app/app.routes";
import { NgProgressRef } from "ngx-progressbar";
import { BehaviorSubject, Observable } from "rxjs";
import { map, catchError } from "rxjs/operators";

@Injectable({
  providedIn: "root",
})
export class ViolationTicketService implements OnInit {
  private _ticket: BehaviorSubject<ViolationTicket> = new BehaviorSubject<ViolationTicket>(null);
  private _ocrTicket: BehaviorSubject<OcrViolationTicket> = new BehaviorSubject<OcrViolationTicket>(null);
  private _inputTicketData: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  private _ticketType: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  public ocrTicketDateKey = "violation_date";
  public ocrTicketTimeKey = "violation_time";

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private logger: LoggerService,
    private ticketService: TicketsService,
    private fileUtilsService: FileUtilsService,
    private datePipe: DatePipe,
    private ticketTypePipe: TicketTypePipe,
  ) {
  }

  ngOnInit(): void {
    // auto update ticket type
    this.ticket$.subscribe(ticket => {
      this._ticketType.next(this.getTicketType(ticket));
    })
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

  public get ticketType() {
    return this._ticketType.value;
  }

  public searchTicket(params): Observable<ViolationTicket> {
    this.reset();
    this.logger.info("ViolationTicketService:: Search for ticket");
    return this.ticketService.apiTicketsSearchGet(params.ticketNumber, params.time)
      .pipe(
        map((response: ViolationTicket) => {
          if (response) {
            this.ticket$.next(response);
            this.goToDisputeSummary(params);
          }
          else {
            this.logger.error("ViolationTicketService::searchTicket ticket not found");
            this.onError();
          }
          return response;
        }),
        catchError((error: any) => {
          this.logger.error("ViolationTicketService::searchTicket error has occurred: ", error);
          this.onError();
          throw error;
        })
      );
  }

  public analyseTicket(ticketFile: File, progressRef: NgProgressRef): void {
    this.reset();
    this.logger.info("file target", ticketFile);
    if (!this.checkSize(ticketFile?.size)) {
      this.logger.info("You must select a file");
      this.openErrorScenarioOneDialog();
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
        .subscribe({
          next: res => {
            if (res) {
              this.ocrTicket$.next(res);
              this.ticket$.next(this.fromOCR(res));
              this.inputTicketData$.next(input);
              this.router.navigate([AppRoutes.disputePath(AppRoutes.SCAN)]);
            }
            else {
              this.onError();
            }
          },
          error: err => {
            this.onError(err);
          }
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
    this.logger.info(`${key}: "${field.value || "<missing>"}", with confidence of ${field.fieldConfidence}`);
    let result;
    let value = field.value;
    if (field.type && value && value.length > 0) {
      switch (field.type.toLowerCase()) {
        case "double":
          result = parseFloat(value.replace(/[^.0-9]/g, "")); // regex replace characters other than numbers
          break;
        case "int64":
          result = parseInt(value.replace(/[^.0-9]/g, "")); // regex replace characters other than numbers
          break;
        case "selectionmark":
          result = value.toLowerCase() === "selected" ? true : false;
          break;
        case "time":
          result = value.replace(" ", ":");
          break;
        case "date":
        case "string":
        default:
          result = value;
          break;
      }
    }
    return result;
  }

  public getTicketType(ticket): string {
    let result: string;
    if (ticket && ticket.ticket_number) {
      result = this.ticketTypePipe.transform(ticket.ticket_number);
    }
    return result;
  }

  goToDisputeSummary(paramsInput?): void { // start of dispute
    if (paramsInput || this.ticket) {
      let params = paramsInput ?? {
        ticketNumber: this.ticket.ticket_number,
        time: this.datePipe.transform(this.ticket.issued_date, "HH:mm"),
      }
      if (this.dateDiff(this.ticket.issued_date) <= 30) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
          queryParams: params,
        });
      } else {
        this.openInValidTicketDateDialog();
      }
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

  private onError(err?: HttpErrorResponse): void {
    this.reset();
    if (!err) {
      this.dialog.open(TicketNotFoundDialogComponent);
    } else {
      if (err.error.errors.file || this.isErrorMatch(err, "Violation Ticket Number is blank")
        || this.isErrorMatch(err, "Violation ticket number must start with an A and be of the form \"AX00000000\".")
        || this.isErrorMatch(err, "low confidence", false)) {
        this.openErrorScenarioOneDialog();
      }
      else if (this.isErrorMatch(err, "more than 30 days ago.", false)) {
        this.openErrorScenarioTwoDialog();
      }
      else if (this.isErrorMatch(err, "MVA must be selected under the \"Did commit the offence(s) indicated\" section.")) {
        this.openErrorScenarioThreeDialog();
      } else { // fall back option
        this.openErrorScenarioOneDialog();
      }
    }
    this.goToFind();
  }

  private isErrorMatch(err: HttpErrorResponse, msg: string, exactMatch: boolean = true) {
    return exactMatch ? err.error.errors?.includes(msg) : err.error.errors?.filter(i => i.indexOf(msg) > -1).length > 0;
  }

  private openImageTicketNotFoundDialog(title: string, key: string) {
    const data: DialogOptions = {
      titleKey: title,
      actionType: "warn",
      messageKey: key,
      actionTextKey: "Ok",
      cancelHide: true,
    };
    return this.dialog.open(ImageTicketNotFoundDialogComponent, { data })
  }

  private openErrorScenarioOneDialog() {
    return this.openImageTicketNotFoundDialog("Your ticket image could not be read", "error1");
  }

  private openErrorScenarioTwoDialog() {
    return this.openImageTicketNotFoundDialog("Your ticket is over 30 days old", "error2");
  }

  private openErrorScenarioThreeDialog() {
    return this.openImageTicketNotFoundDialog("Invalid ticket type", "error3");
  }

  private openInValidTicketDateDialog() {
    return this.openImageTicketNotFoundDialog("more than 30 days old", "error2");
  }

  private dateDiff(givenDate: string) {
    var diffYear = (new Date().getTime() - new Date(givenDate).getTime()) / 1000;
    diffYear /= (60 * 60 * 24);
    return Math.abs(Math.round(diffYear));
  }

  private checkSize(fileSize: number) {
    return fileSize > 0 && fileSize <= (10 * 1024 * 1024); // less or equal to 10MB
  }
}

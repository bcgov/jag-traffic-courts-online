import { DatePipe } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { LoggerService } from "@core/services/logger.service";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ImageTicketNotFoundDialogComponent } from "@shared/dialogs/image-ticket-not-found-dialog/image-ticket-not-found-dialog.component";
import { TicketNotFoundDialogComponent } from "@shared/dialogs/ticket-not-found-dialog/ticket-not-found-dialog.component";
import { TicketTypes } from "@shared/enums/ticket-type.enum";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { TicketTypePipe } from "@shared/pipes/ticket-type.pipe";
import { FileUtilsService } from "@shared/services/file-utils.service";
import { DisputeDisputantDetectedOcrIssues, Field, OcrViolationTicket, TicketsService, ViolationTicket } from "app/api";
import { AppRoutes } from "app/app.routes";
import { NgProgressRef } from "ngx-progressbar";
import { BehaviorSubject, Observable } from "rxjs";
import { map, catchError } from "rxjs/operators";

@Injectable({
  providedIn: "root",
})
export class ViolationTicketService {
  private _ticket: BehaviorSubject<ViolationTicket> = new BehaviorSubject<ViolationTicket>(null);
  private _ocrTicket: BehaviorSubject<OcrViolationTicket> = new BehaviorSubject<OcrViolationTicket>(null);
  private _inputTicketData: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  private _ticketType: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  ocrTicketDateKey = "violation_date";
  ocrTicketTimeKey = "violation_time";
  ocrIssueDetectedKey = "disputant_detected_ocr_issues";
  ocrIssueDescKey = "disputant_ocr_issues";
  DetectedOcrIssues = DisputeDisputantDetectedOcrIssues;
  private queryParams: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    private logger: LoggerService,
    private ticketService: TicketsService,
    private fileUtilsService: FileUtilsService,
    private datePipe: DatePipe,
    private ticketTypePipe: TicketTypePipe,
  ) {
    // auto update ticket type
    this.ticket$.subscribe(ticket => {
      this._ticketType.next(this.getTicketType(ticket));
    })
    this.route.queryParams.subscribe((params) => {
      this.queryParams = params;
    });
  }

  get ticket$(): Observable<ViolationTicket> {
    return this._ticket.asObservable();
  }

  get ticket(): ViolationTicket {
    return this._ticket.value;
  }

  private get ocrTicket(): OcrViolationTicket { // not public for current stage
    return this._ocrTicket.value;
  }

  get inputTicketData$(): Observable<any> {
    return this._inputTicketData.asObservable();
  }

  get inputTicketData() {
    return this._inputTicketData.value;
  }

  get ticketType() {
    return this._ticketType.value;
  }

  searchTicket(params?: QueryParamsForSearch): Observable<ViolationTicket> {
    this.reset();
    this.logger.info("ViolationTicketService:: Search for ticket");
    if (!params) {
      params = this.queryParams;
    }
    return this.ticketService.apiTicketsSearchGet(params.ticketNumber, params.time)
      .pipe(
        map((response: ViolationTicket) => {
          if (response) {
            this._ticket.next(response);
            if (this.validateTicket(params)) {
              this.goToInitiateResolution(params);
            } else {
              this.logger.error("ViolationTicketService::searchTicket ticket info not matched");
              this.onError();
            }
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

  analyseTicket(ticketFile: File, progressRef: NgProgressRef): void {
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
              try {
                this._ocrTicket.next(res);
                this._ticket.next(this.fromOCR(res));
                this._inputTicketData.next(input);
                this.router.navigate([AppRoutes.ticketPath(AppRoutes.SCAN)]);
              }
              catch {
                this.onError();
              }
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

  validateTicket(params?: QueryParamsForSearch): boolean {
    var result = false;
    if (this.ticket && this.ticket.issued_date) {
      var storedTicketTime = this.datePipe.transform(this.ticket.issued_date, "HH:mm");
      if (this.ticket.ticket_number === params.ticketNumber && storedTicketTime === params.time) {
        result = true;
      }
    }
    return result;
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

        let idpos = keySplit[1].lastIndexOf("_");
        let id = keySplit[1].substring(idpos + 1);
        let idKey = keySplit[1].substring(0, idpos) ? keySplit[1].substring(0, idpos) : "id";

        let arrKey = keySplit[0];
        let index = +id - 1;
        let key = keySplit[2];

        if (index >= 0) {
          if (!result[arrKey]) {
            result[arrKey] = [];
          }
          if (!result[arrKey][index]) {
            result[arrKey][index] = {};
            result[arrKey][index][idKey] = +id;
          }
          result[arrKey][index][key] = value;
        }
      })
    }

    // special handling
    if (isDateFound || isTimeFound) {
      result.issued_date = this.datePipe.transform(result[this.ocrTicketDateKey] + " " + result[this.ocrTicketTimeKey], "YYYY-MM-ddTHH:mm:ss");
    }
    if (isDateFound) {
      result[this.ocrTicketDateKey] = this.datePipe.transform(result[this.ocrTicketDateKey], "MMM dd, YYYY");
    }
    result.counts = result.counts.filter(count => count.description || count.section || count.ticketed_amount);

    // set ticket_id to imageFilename returned from Ocr
    result.ticket_id = source.imageFilename;

    // add extra fields for notcie of dispute
    result[this.ocrIssueDetectedKey] = null;
    result[this.ocrIssueDescKey] = null;

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

  getTicketType(ticket): string {
    let result: string;
    if (ticket && ticket.ticket_number) {
      result = this.ticketTypePipe.transform(ticket.ticket_number);
    }
    return result;
  }

  updateOcrIssue(issueDetected, issuseDesc): void {
    let ticket = this.ticket;
    ticket[this.ocrIssueDetectedKey] = (issueDetected === this.DetectedOcrIssues.Y) ? issueDetected : this.DetectedOcrIssues.N;
    ticket[this.ocrIssueDescKey] = issuseDesc;
    this._ticket.next(ticket);
  }

  goToInitiateResolution(paramsInput?): void {
    if (this.ticket) {
      let params = paramsInput ?? {
        ticketNumber: this.ticket.ticket_number,
        time: this.datePipe.transform(this.ticket.issued_date, "HH:mm")
      };
      let dateDiff = this.dateDiff(this.ticket.issued_date); // for electronic or camera tickets
      if (this.ticketType === TicketTypes.HANDWRITTEN_TICKET) { // for handwritten tickets use service date
        dateDiff = this.dateDiff(this.ocrTicket.fields["service_date"].value);
      }
      if ((dateDiff <= 30 && (this.ticketType === TicketTypes.ELECTRONIC_TICKET || this.ticketType === TicketTypes.HANDWRITTEN_TICKET))
        || (dateDiff <= 45 && this.ticketType === TicketTypes.CAMERA_TICKET)) {
        this.router.navigate([AppRoutes.ticketPath(AppRoutes.SUMMARY)], {
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
    this.router.navigate([AppRoutes.ticketPath(AppRoutes.FIND)]);
  }

  private reset(): void {
    this._inputTicketData.next(null);
    this._ocrTicket.next(null);
    this._ticket.next(null);
  }

  private onError(err?: HttpErrorResponse): void {
    this.reset();
    if (!err) {
      this.dialog.open(TicketNotFoundDialogComponent);
    } else {
      if (err.error?.errors?.file || this.isErrorMatch(err, "Violation Ticket Number is blank")
        || this.isErrorMatch(err, "Violation ticket number must start with an A and be of the form \"AX00000000\".")
        || this.isErrorMatch(err, "low confidence", false)) {
        this.openErrorScenarioOneDialog();
      }
      else if (this.isErrorMatch(err, "more than 30 days ago.", false)) {
        this.openErrorScenarioTwoDialog();
      }
      else if (this.isErrorMatch(err, "MVA must be selected under the \"Did commit the offence(s) indicated\" section.")) {
        this.openErrorScenarioThreeDialog();
      }
      else if (this.isErrorMatch(err, "TCO only supports counts with MVA as the ACT/REG at this time. Read 'CTA' for count", false)) {
        this.openErrorScenarioFourDialog();
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

  private openErrorScenarioFourDialog() {
    return this.openImageTicketNotFoundDialog("Non-MVA ticket", "error2");
  }

  private openInValidTicketDateDialog() {
    return this.openImageTicketNotFoundDialog(`Your ticket is over ${this.ticketType === TicketTypes.CAMERA_TICKET ? '45' : '30'} days old`, "error4");
  }

  private dateDiff(givenDate: string) {
    var diffYear = (new Date().getTime() - new Date(givenDate).getTime()) / 1000;
    diffYear /= (60 * 60 * 24);
    return Math.round(diffYear);
  }

  private checkSize(fileSize: number) {
    return fileSize > 0 && fileSize <= (10 * 1024 * 1024); // less or equal to 10MB
  }
}

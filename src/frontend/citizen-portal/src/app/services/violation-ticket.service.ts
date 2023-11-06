import { DatePipe } from "@angular/common";
import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MatLegacyDialog as MatDialog, MatLegacyDialogRef as MatDialogRef } from "@angular/material/legacy-dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { LoggerService } from "@core/services/logger.service";
import { DialogOptions } from "@shared/dialogs/dialog-options.model";
import { ImageTicketNotFoundDialogComponent } from "@shared/dialogs/image-ticket-not-found-dialog/image-ticket-not-found-dialog.component";
import { TicketNotFoundDialogComponent } from "@shared/dialogs/ticket-not-found-dialog/ticket-not-found-dialog.component";
import { WaitForOcrDialogComponent } from "@shared/dialogs/wait-for-ocr-dialog/wait-for-ocr-dialog.component";
import { TicketTypes } from "@shared/enums/ticket-type.enum";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { TicketTypePipe } from "@shared/pipes/ticket-type.pipe";
import { FileUtilsService } from "@shared/services/file-utils.service";
import { DisputeDisputantDetectedOcrIssues, DisputeSystemDetectedOcrIssues, Field, OcrViolationTicket, TicketsService, ViolationTicket } from "app/api";
import { AppRoutes } from "app/app.routes";
import { NgProgressRef } from "ngx-progressbar";
import { BehaviorSubject, Observable } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { NoticeOfDisputeKeys } from "./notice-of-dispute.service";

@Injectable({
  providedIn: "root",
})
export class ViolationTicketService {
  private _ticket: BehaviorSubject<ViolationTicket> = new BehaviorSubject<ViolationTicket>(null);
  private _ocrTicket: BehaviorSubject<OcrViolationTicket> = new BehaviorSubject<OcrViolationTicket>(null);
  private _inputTicketData: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  private _ticketType: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  ocrTicketDateKey = "violation_date"; // scan-ticket page only
  ocrTicketTimeKey = "violation_time"; // scan-ticket page only
  driversLicenceNumberKey: NoticeOfDisputeKeys = "drivers_licence_number";
  ocrIssueDetectedKey: NoticeOfDisputeKeys = "disputant_detected_ocr_issues";
  ocrIssueDescKey: NoticeOfDisputeKeys = "disputant_ocr_issues";
  systemDetectOcrIssueKey: NoticeOfDisputeKeys = "system_detected_ocr_issues";
  systemKeysToCheck = ["violationTicketTitle", "ticket_number", "disputant_surname", "disputant_given_names", "drivers_licence_province", "drivers_licence_number", "violation_time",
    "violation_date", "counts.count_no_1.description", "counts.count_no_1.act_or_regulation_name_code", "counts.count_no_1.is_act", "counts.count_no_1.is_regulation", "counts.count_no_1.section",
    "counts.count_no_1.ticketed_amount", "counts.count_no_2.description", "counts.count_no_2.act_or_regulation_name_code", "counts.count_no_2.is_act", "counts.count_no_2.is_regulation",
    "counts.count_no_2.section", "counts.count_no_2.ticketed_amount", "counts.count_no_3.description", "counts.count_no_3.act_or_regulation_name_code", "counts.count_no_3.is_act",
    "counts.count_no_3.is_regulation", "counts.count_no_3.section", "counts.count_no_3.ticketed_amount", "court_location", "detachment_location"];
  DetectedOcrIssues = DisputeDisputantDetectedOcrIssues;
  SystemDetectedOcrIssues = DisputeSystemDetectedOcrIssues;
  private queryParams: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private datePipe: DatePipe,
    private dialog: MatDialog,
    private logger: LoggerService,
    private ticketService: TicketsService,
    private fileUtilsService: FileUtilsService,
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
            response.issued_date = this.datePipe.transform(response.issued_date, "yyyy-MM-ddTHH:mm:ss'Z'"); // e-tickets need this
            this._ticket.next(response);
            if (this.validateTicket(params)) {
              this.goToInitiateResolution(params);
            } else {
              this.logger.error("ViolationTicketService::searchTicket error has occurred:", "Ticket info not matched");
              this.onError();
            }
          }
          else {
            this.logger.error("ViolationTicketService::searchTicket error has occurred:", "searchTicket ticket not found");
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

  analyseTicket(ticketFile: File, progressRef: NgProgressRef, dialogRef: MatDialogRef<WaitForOcrDialogComponent>): void {
    this.reset();
    this.logger.info("file target", ticketFile);
    // catch if no ticketFile passed in
    if (!ticketFile) {
      this.logger.error("ViolationTicketService::analyseTicket error has occurred: ", "No file selected.");
      this.openErrorScenarioOneDialog();
      dialogRef.close();
      return;
    }
    // Check if file too small (0) or too large (over 10MB)
    var fileSizeError = this.fileUtilsService.checkFileSize(ticketFile?.size);
    if (fileSizeError !== "") {
      this.logger.error("ViolationTicketService::analyseTicket error has occurred: ", fileSizeError);
      this.openErrorScenarioOneDialog();
      dialogRef.close();
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
                this.logger.error("ViolationTicketService::analyseTicket error has occurred:", "Cannot interpret image.");
                this.openErrorScenarioOneDialog();
              }
            }
            else {
              this.logger.error("ViolationTicketService::analyseTicket error has occurred:", "no result from API analyse");
              this.onError();
            }
            progressRef.complete();
            dialogRef.close();
          },
          error: err => {
            this.logger.error("ViolationTicketService::analyseTicket error has occurred:", err);
            this.onError(err);
            dialogRef.close();
          }
        })
    })
  }

  validateTicket(params?: QueryParamsForSearch): boolean {
    var result = false;
    if (this.ticket && this.ticket.issued_date) {
      var storedTicketTime = this.datePipe.transform(this.ticket.issued_date, "HH:mm", "UTC");
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
    let isOcrIssueDetected = false;

    let keys = Object.keys(source.fields);
    keys.forEach(key => {
      let value = "";
      // Direct conversion
      if (key.indexOf(".") === -1) {
        value = this.getValue(key, <Field>source.fields[key]);
        result[key] = value;

        if (value && key === this.ocrTicketDateKey) {
          isDateFound = true;
        }
        if (value && key === this.ocrTicketTimeKey) {
          isTimeFound = true;
        }
      } else if (key.indexOf(".") > 0 && key.split(".").length === 3) {
        value = this.getValue(key, <Field>source.fields[key]);
        let keySplit = key.split(".");

        let idpos = keySplit[1].lastIndexOf("_");
        let id = keySplit[1].substring(idpos + 1);
        let idKey = keySplit[1].substring(0, idpos) ? keySplit[1].substring(0, idpos) : "";

        let arrKey = keySplit[0];
        let index = +id - 1;
        let childkey = keySplit[2];

        if (index >= 0) {
          if (!result[arrKey]) {
            result[arrKey] = [];
          }
          if (!result[arrKey][index]) {
            result[arrKey][index] = {};
            result[arrKey][index][idKey] = +id;
          }
          result[arrKey][index][childkey] = value;
        }
      }

      // check for conf level < 0.8 for selected fields
      if (this.systemKeysToCheck.indexOf(key) >= 0 && !isOcrIssueDetected && source?.fields[key]?.fieldConfidence < 0.8) {
        isOcrIssueDetected = true;
      }
    })

    // special handling
    if (result.drivers_licence_number) {
      result.drivers_licence_number = (<Field>source.fields[this.driversLicenceNumberKey]).value;
    }
    if (isDateFound || isTimeFound) {
      result.issued_date = this.datePipe.transform(result[this.ocrTicketDateKey] + " " + result[this.ocrTicketTimeKey], "yyyy-MM-ddTHH:mm:ss'Z'");
    }
    if (isDateFound) {
      result[this.ocrTicketDateKey] = this.datePipe.transform(result[this.ocrTicketDateKey], "MMM dd, yyyy", "UTC");
    }
    result.counts = result.counts.filter(count => count.description || count.section || count.ticketed_amount);
    result.counts.forEach(count => {
      if (count.ticketed_amount === null) {
        isOcrIssueDetected = true;
      } else {
        count.ticketed_amount = +count.ticketed_amount;
      }
    })

    // set ticket_id to imageFilename returned from Ocr
    result.ticket_id = source.imageFilename;

    // add extra fields for notice of dispute
    result[this.ocrIssueDetectedKey] = null;
    result[this.ocrIssueDescKey] = null;
    result[this.systemDetectOcrIssueKey] = isOcrIssueDetected ? this.SystemDetectedOcrIssues.Y : this.SystemDetectedOcrIssues.N;
    this.logger.info("ViolationTicketService: result of converting to violation ticket", result);
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
        time: this.datePipe.transform(this.ticket.issued_date, "HH:mm", "UTC")
      };
      let dateDiff = this.dateDiff(this.ticket.issued_date); // for electronic or camera tickets
      if (this.ticketType === TicketTypes.HANDWRITTEN_TICKET) { // for handwritten tickets use service date
        dateDiff = this.dateDiff(this.ocrTicket?.fields["service_date"].value);
      }
      // handwritten tickets are additionally checked in the analyze ticket API but cheked again here for <=30 days
      // e-tickets can be <=30 days and camera tickets <=45
      // TODO:  temporarily turned off checking dates for e-tickets for testing needs to be turned bacxk on by uncommenting next two lines
      // and deleting following two lines AND uncommenting lines for electronic ticket erorr check and camera ticket error check below
      // if ((dateDiff <= 30 && (this.ticketType === TicketTypes.ELECTRONIC_TICKET || this.ticketType === TicketTypes.HANDWRITTEN_TICKET))
      // || (dateDiff <= 45 && this.ticketType === TicketTypes.CAMERA_TICKET)) {
      if ((dateDiff <= 30 && this.ticketType === TicketTypes.HANDWRITTEN_TICKET)
        || this.ticketType === TicketTypes.ELECTRONIC_TICKET || this.ticketType === TicketTypes.CAMERA_TICKET) {
        this.router.navigate([AppRoutes.ticketPath(AppRoutes.SUMMARY)], {
          queryParams: params,
        });
      } else if (dateDiff > 30 && this.ticketType === TicketTypes.HANDWRITTEN_TICKET) {
        let errMsg = "Issued " + dateDiff.toString() + "days ago on " + this.ocrTicket?.fields["service_date"].value.toString();
        this.logger.error("ViolationTicketService::goToInitiateResolution ticket too old has occurred", dateDiff);
        this.openInValidHandwrittenTicketDateDialog();
      // } else if (dateDiff > 30 && this.ticketType === TicketTypes.ELECTRONIC_TICKET) {
      //   let errMsg = "Issued " + dateDiff.toString() + "days ago on " + this.ticket.issued_date.toString();
      //   this.logger.error("ViolationTicketService::goToInitiateResolution ticket too old has occurred", dateDiff);
      //   this.openInValidETicketDateDialog();
      // } else if (dateDiff > 45 && this.ticketType === TicketTypes.CAMERA_TICKET) {
      //   let errMsg = "Issued " + dateDiff.toString() + "days ago on " + this.ticket.issued_date.toString();
      //   this.logger.error("ViolationTicketService::goToInitiateResolution ticket too old has occurred", dateDiff);
      //   this.openInValidISCTicketDateDialog();
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
        var errorMessages = "";
        if (err.error?.errors) {
          err.error.errors.forEach(error => { errorMessages += ". \n" + error });
        }
        this.logger.error("ViolationTicketService:onError critical validation error has occurred", errorMessages);
        this.openErrorScenarioOneDialog();
      } else if (this.isErrorMatch(err, "more than 30 days ago.", false)) {  // more than 30 days old
        this.logger.error("ViolationTicketService:onError validation error has occurred", "More than 30 days old");
        this.openErrorScenarioTwoDialog();
      }
      else if (this.isErrorMatch(err, "TCO only supports counts with MVA as the ACT/REG at this time. Read 'CTA' for count", false)) {
        this.openErrorScenarioFourDialog();
      } else { // fall back option
        var errorMessages = "";
        if (err.error?.errors) {
          err.error.errors.forEach(error => { errorMessages += ". \n" + error });
        }
        this.logger.error("ViolationTicketService:onError validation error has occurred", errorMessages);
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

  private openErrorScenarioFourDialog() {
    return this.openImageTicketNotFoundDialog("Non-MVA ticket", "error2");
  }

  private openInValidHandwrittenTicketDateDialog() {
    return this.openImageTicketNotFoundDialog(`Your ticket is over 30 days old`, "error4");
  }

  private openInValidETicketDateDialog() {
    return this.openImageTicketNotFoundDialog(`Your ticket is over 30 days old`, "error5");
  }

  private openInValidISCTicketDateDialog() {
    return this.openImageTicketNotFoundDialog(`Your ticket is over 45 days old`, "error6");
  }

  private dateDiff(givenDate: string) {
    var diffYear = (new Date().getTime() - new Date(givenDate).getTime()) / 1000;
    diffYear /= (60 * 60 * 24);
    return Math.round(diffYear);
  }
}

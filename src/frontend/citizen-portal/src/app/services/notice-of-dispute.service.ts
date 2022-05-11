import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { NoticeOfDispute, Plea } from 'app/api';
import { AppRoutes } from 'app/app.routes';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NoticeOfDisputeService {
  private _noticOfDispute: BehaviorSubject<NoticeOfDispute> = new BehaviorSubject<NoticeOfDispute>(null);

  public countFormFields = {
    plea: null,
    request_time_to_pay: false,
    request_reduction: false,
    appear_in_court: null,
  }

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private logger: LoggerService,
    private datePipe: DatePipe,
  ) {
  }

  public get noticOfDispute$(): BehaviorSubject<NoticeOfDispute> {
    return this._noticOfDispute;
  }

  public get noticOfDispute(): NoticeOfDispute {
    return this._noticOfDispute.value;
  }

  public createNoticeOfDispute(input: NoticeOfDispute): void {
    const data: DialogOptions = {
      titleKey: "Submit request",
      messageKey:
        "When your request is submitted for adjudication, it can no longer be updated. Are you ready to submit your request?",
      actionTextKey: "Submit request",
      cancelTextKey: "Cancel",
      icon: null,
    };
    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((action: boolean) => {
        if (action) {
          // return this.disputesService.apiDisputesCreatePost(input).subscribe(res => {
          this.noticOfDispute$.next(input);
          this.router.navigate([AppRoutes.disputePath(AppRoutes.SUBMIT_SUCCESS)], {
            queryParams: {
              ticketNumber: input.ticket_number,
              time: this.datePipe.transform(input.issued_date, "HH:mm"),
            },
          });
          // })
        }
      });
  }

  public getIsShowCheckBoxes(ticket: NoticeOfDispute): any {
    let isShowCheckbox: any = {};
    let fields = Object.keys(this.countFormFields);
    fields.forEach(field => {
      if (ticket.disputed_counts && ticket.disputed_counts.length > -1) {
        isShowCheckbox[field] = ticket.disputed_counts.filter(i => i[field]).map(i => i.count);
      } else {
        isShowCheckbox[field] = [];
      }
    });
    isShowCheckbox.request_counts =
      [...isShowCheckbox.request_time_to_pay, ...isShowCheckbox.request_reduction]
        .filter((value, index, self) => { return self.indexOf(value) === index; }).sort();
    isShowCheckbox.not_guilty = ticket.disputed_counts.filter(i => i.plea === Plea.NotGuilty).map(i => i.count);
    return isShowCheckbox;
  }
}

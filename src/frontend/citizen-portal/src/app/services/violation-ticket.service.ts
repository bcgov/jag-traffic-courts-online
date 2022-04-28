import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ImageTicketNotFoundDialogComponent } from '@shared/dialogs/image-ticket-not-found-dialog/image-ticket-not-found-dialog.component';
import { ViolationTicket } from 'app/api';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ViolationTicketService {
  private _ticket: BehaviorSubject<ViolationTicket> = new BehaviorSubject<ViolationTicket>(null);
  //   private _scannedTicket: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private dialog: MatDialog,
  ) {
  }

  public get ticket$(): BehaviorSubject<ViolationTicket> {
    return this._ticket;
  }

  public get ticket(): ViolationTicket {
    return this._ticket.value;
  }

  //   public get scannedTicket$(): BehaviorSubject<any> {
  //     return this._shellTicketData;
  //   }

  //   public get scannedTicket(): any {
  //     return this._shellTicketData.value;
  //   }

  openImageTicketNotFoundDialog(err) {
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

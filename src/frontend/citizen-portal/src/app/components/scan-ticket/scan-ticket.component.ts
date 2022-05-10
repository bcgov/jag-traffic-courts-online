import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
} from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ViolationTicket } from 'app/api';
import { AppRoutes } from 'app/app.routes';
import { ViolationTicketService } from 'app/services/violation-ticket.service';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-scan-ticket',
  templateUrl: './scan-ticket.component.html',
  styleUrls: ['./scan-ticket.component.scss'],
})
export class ScanTicketComponent implements OnInit {
  public busy: Subscription | Promise<any>;
  public ticketImageSrc: string;
  public ticketFilename: string;
  public form: FormGroup;

  private progressRef: NgProgressRef;
  private ticket: ViolationTicket;

  constructor(
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    private router: Router,
    private ngProgress: NgProgress,
    private logger: LoggerService,
    private violationTicketService: ViolationTicketService,
  ) {
    this.router.routeReuseStrategy.shouldReuseRoute = () => { return false; };
    this.progressRef = this.ngProgress.ref();
  }

  public ngOnInit(): void {
    let inputTicketData = this.violationTicketService.inputTicketData;
    this.ticket = this.violationTicketService.ticket;
    if (!inputTicketData || !this.ticket) {
      this.violationTicketService.goToFind();
      return;
    }
    this.ticketImageSrc = inputTicketData.ticketImage;
    this.ticketFilename = inputTicketData.filename;
    this.form = this.formBuilder.group(this.ticket); // can add control
    this.form.disable();
  }

  public onSubmit(): void {
    const data: DialogOptions = {
      titleKey: 'Are you sure all ticket information is correct?',
      messageKey: `Please ensure that all entered fields match the paper ticket copy exactly.
        If you are not sure, please go back and update any fields as needed before submitting ticket information.`,
      actionTextKey: 'Yes I am sure, create online ticket',
      cancelTextKey: 'Go back and edit',
    };

    this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          this.violationTicketService.ticket$.next(this.ticket);
          this.violationTicketService.goToDisputeSummary({
            ticketNumber: this.ticket.ticket_number,
            time: (<any>this.ticket)[this.violationTicketService.ocrTicketTimeKey], // special handling
          });
        }
      });
  }

  public onFileChange(event: any) {
    this.violationTicketService.analyseTicket(event.target.files[0], this.progressRef);
    event.target.value = null; // reset file input
  }

  public onStatuteSelected(event$: MatAutocompleteSelectedEvent): void {
    this.logger.log('onStatuteSelected', event$.option.value);
  }
}

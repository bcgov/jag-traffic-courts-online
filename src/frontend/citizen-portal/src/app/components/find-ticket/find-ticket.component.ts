import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ImageRequirementsDialogComponent } from '@shared/dialogs/image-requirements-dialog/image-requirements-dialog.component';
import { TicketExampleDialogComponent } from '@shared/dialogs/ticket-example-dialog/ticket-example-dialog.component';
import { WaitForOcrDialogComponent } from '@shared/dialogs/wait-for-ocr-dialog/wait-for-ocr-dialog.component';
import { Configuration } from 'app/api';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';
import { ViolationTicketService } from 'app/services/violation-ticket.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';


@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindTicketComponent implements OnInit {
  private progressRef: NgProgressRef;
  form: FormGroup;
  dialogRef: MatDialogRef<WaitForOcrDialogComponent>;
  analyzingTicket: boolean = false;
  notFound = false;
  toolTipData = 'If you cannot upload a copy of your handwritten ticket, you can initiate a dispute by contacting the Violation Ticket Centre: violationticketcentre@gov.bc.ca.  You must include your Given Name, Surname, Driver\'s Licence Number, Violation Ticket Number, and Violation Time. Do not include any evidence, such as videos or pictures, with your email. Evidence can only be presented at the hearing.';
  configuration = new Configuration();

  constructor(
    private formBuilder: FormBuilder,
    private formUtilsService: FormUtilsService,
    private dialog: MatDialog,
    private ngProgress: NgProgress,
    private logger: LoggerService,
    private violationTicketService: ViolationTicketService,
  ) {

    this.progressRef?.state?.subscribe(value => {
      if (value.active === false) this.dialogRef?.close();
    });
  }

  ngOnInit(): void {
    this.progressRef = this.ngProgress.ref();

    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      time: [null, [Validators.required]],
    });
  }

  onSearch(): void {
    this.logger.log('FindTicketComponent::onSearch');

    this.notFound = false;
    const validity = this.formUtilsService.checkValidity(this.form);
    const errors = this.formUtilsService.getFormErrors(this.form);

    this.logger.log('validity', validity);
    this.logger.log('errors', errors);
    this.logger.log('form.value', this.form.value);

    if (!validity) {
      return;
    }
    this.violationTicketService.searchTicket(this.form.value).subscribe(res => res);
  }

  onFileChange(event: any) {
    this.logger.log('FindTicketComponent::onFileChange');
    this.viewWaitForOcr();
    this.violationTicketService.analyseTicket(event.target.files[0], this.progressRef, this.dialogRef);
    event.target.value = null; // reset file input
  }

  viewWaitForOcr(): void {
    this.dialogRef = this.dialog.open(WaitForOcrDialogComponent, {
      width: '600px',
    });
  }

  onViewTicketExample(): void {
    this.dialog.open(TicketExampleDialogComponent, {
      width: '600px',
    });
  }

  onViewTicketUploadManual(): void {
      const data: DialogOptions = {
        titleKey: "Upload Ticket",
        actionType: "primary",
        messageKey: `If you cannot upload a copy of your handwritten ticket, you can initiate a dispute by contacting the Violation Ticket Centre: violationticketcentre@gov.bc.ca.  You must include your Given Name, Surname, Driver's Licence Number, Violation Ticket Number, and Violation Time. Do not include any evidence, such as videos or pictures, with your email. Evidence can only be presented at the hearing. `,
        actionTextKey: "Close",
        cancelHide: true
      };
      this.dialog.open(ConfirmDialogComponent, { data });
  }

  onViewImageRequirements(): void {
    this.dialog.open(ImageRequirementsDialogComponent, {
      width: '600px',
    });
  }
}

import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ImageRequirementsDialogComponent } from '@shared/dialogs/image-requirements-dialog/image-requirements-dialog.component';
import { TicketExampleDialogComponent } from '@shared/dialogs/ticket-example-dialog/ticket-example-dialog.component';
import { Configuration } from 'app/api';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindTicketComponent implements OnInit {
  private progressRef: NgProgressRef;
  form: FormGroup;

  notFound = false;
  toolTipData = 'It is preferred that you include an image of your blue violation ticket. If you are not able to upload an image or take a photo of your ticket on your mobile device. You will need:  1. Ticket number and violation date 2. Driver\'s licence number and loation 3. Count Act / Section / Description 4. Fine amount';
  configuration = new Configuration();

  constructor(
    private formBuilder: FormBuilder,
    private formUtilsService: FormUtilsService,
    private dialog: MatDialog,
    private ngProgress: NgProgress,
    private logger: LoggerService,
    private violationTicketService: ViolationTicketService,
  ) {
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
    this.violationTicketService.analyseTicket(event.target.files[0], this.progressRef);
    event.target.value = null; // reset file input
  }

  onViewTicketExample(): void {
    this.dialog.open(TicketExampleDialogComponent, {
      width: '600px',
    });
  }

  onViewImageRequirements(): void {
    this.dialog.open(ImageRequirementsDialogComponent, {
      width: '600px',
    });
  }
}

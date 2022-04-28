import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators
} from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ImageRequirementsDialogComponent } from '@shared/dialogs/image-requirements-dialog/image-requirements-dialog.component';
import { TicketExampleDialogComponent } from '@shared/dialogs/ticket-example-dialog/ticket-example-dialog.component';
import { TicketNotFoundDialogComponent } from '@shared/dialogs/ticket-not-found-dialog/ticket-not-found-dialog.component';
import { ShellTicketData } from '@shared/models/shellTicketData.model';
import { Configuration, TicketsService } from 'app/api';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';
import { Subscription } from 'rxjs';
import { FileUtilsService } from '@shared/services/file-utils.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindTicketComponent implements OnInit {
  public busy: Subscription;
  public form: FormGroup;
  private progressRef: NgProgressRef;

  public notFound = false;
  public toolTipData = 'It is preferred that you include an image of your blue violation ticket. If you are not able to upload an image or take a photo of your ticket on your mobile device. You will need:  1. Ticket number and violation date 2. Driver\'s license number and loation 3. Count Act / Section / Description 4. Fine amount';
  public configuration = new Configuration();

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private formUtilsService: FormUtilsService,
    private disputeService: DisputeService,
    private disputeResource: DisputeResourceService,
    private fileUtilsService: FileUtilsService,
    private dialog: MatDialog,
    private ngProgress: NgProgress,
    private logger: LoggerService,
    private violationTicketService: ViolationTicketService,
    private ticketService: TicketsService
  ) {
  }

  public ngOnInit(): void {
    this.progressRef = this.ngProgress.ref();

    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      time: [null, [Validators.required]],
    });
  }

  public onSearch(): void {
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

    const formParams = { ...this.form.value };
    this.busy = this.disputeResource
      .getTicket(formParams)
      .subscribe((response) => {
        this.disputeService.ticket$.next(response);
        if (response) {
          this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
            queryParams: formParams,
          });
        } else {
          this.onTicketNotFound();
        }
      });
    // this.busy = this.ticketService.apiTicketsSearchGet(formParams.ticketNumber, formParams.time)
    //   .subscribe((response) => {
    //     this.violationTicketService.ticket$.next(response);
    //     if (response) {
    //       this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
    //         queryParams: formParams,
    //       });
    //     } else {
    //       this.onTicketNotFound();
    //     }
    //   });
  }

  public onFileChange(event: any) {
    // reset
    this.disputeService.shellTicketData$.next(null);
    const ticketFile: File = event.target.files[0];
    this.logger.info('file target', ticketFile);
    if (!ticketFile) {
      this.logger.info('You must select a file');
      return;
    }
    this.progressRef.start();
    this.fileUtilsService.readFileAsDataURL(ticketFile).subscribe(ticketImage => {
      const shellTicketData: ShellTicketData = {
        filename: ticketFile.name,
        ticketFile,
        ticketImage
      };

      this.ticketService.apiTicketsAnalysePost(ticketFile)
        .subscribe(res => {
          if (res) {
            this.ticketService.setImageData(res);
            this.disputeService.shellTicketData$.next(shellTicketData);
            this.router.navigate([AppRoutes.disputePath(AppRoutes.SHELL)]);
          }
          else {
            this.notFound = true;
            this.onTicketNotFound();
          }
        }, (err) => {
          this.violationTicketService.openImageTicketNotFoundDialog(err);
        })
    })
  }

  public onTicketNotFound(): void {
    this.notFound = true;
    this.dialog.open(TicketNotFoundDialogComponent);
  }

  public onViewTicketExample(): void {
    this.dialog.open(TicketExampleDialogComponent, {
      width: '600px',
    });
  }

  public onViewImageRequirements(): void {
    this.dialog.open(ImageRequirementsDialogComponent, {
      width: '600px',
    });
  }
}

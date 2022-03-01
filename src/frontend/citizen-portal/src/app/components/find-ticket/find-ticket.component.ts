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
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { Configuration, TicketsService } from 'app/api';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';
import { FileDetector } from 'protractor';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindTicketComponent implements OnInit {
  public busy: Subscription;
  public form: FormGroup;

  public notFound = false;
  // protected basePath = 'http://localhost:5000'; 
  protected basePath = ''; 
  public configuration = new Configuration();
  // public encoder: HttpParameterCodec;

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private disputeResource: DisputeResourceService,
    private formUtilsService: FormUtilsService,
    private disputeService: DisputeService,
    private dialog: MatDialog,
    private ngProgress: NgProgress,
    private logger: LoggerService,
    private http: HttpClient,
    public ticketService:TicketsService
  ) {
    //
  
  if (typeof this.configuration.basePath !== 'string') {
      this.configuration.basePath = this.basePath;
  }
  }

  public ngOnInit(): void {
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      time: [null, [Validators.required]],
    });
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

  public onSearch(): void {
    this.logger.log('FindTicketComponent::onSearch');

    const validity = this.formUtilsService.checkValidity(this.form);
    const errors = this.formUtilsService.getFormErrors(this.form);

    this.logger.log('validity', validity);
    this.logger.log('errors', errors);
    this.logger.log('form.value', this.form.value);

    if (!validity) {
      return;
    }

    this.notFound = false;

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
          this.notFound = true;
          this.onTicketNotFound();
        }
      });
  }
 public onTicketNotFound(): void {
    this.dialog.open(TicketNotFoundDialogComponent);
  }
  public onFileChange(event: any) {
    let filename: string;
    let ticketImage: string;

    // reset
    this.disputeService.shellTicketData$.next(null);

    if (!event.target.files[0] || event.target.files[0].length === 0) {
      this.logger.info('You must select an image');
      return;
    }

    const mimeType = event.target.files[0].type;

    if (mimeType.match(/image\/*/) == null) {
      this.logger.info('Only images are supported');
      return;
    }

    const progressRef: NgProgressRef = this.ngProgress.ref();
    progressRef.start();

    const reader = new FileReader();
    const ticketFile: File = event.target.files[0];
    this.logger.info('file target', event.target.files[0]);

    filename = ticketFile.name;
    reader.readAsDataURL(ticketFile);
    this.logger.info('file', ticketFile.name, ticketFile.lastModified);

    reader.onload = () => {
      ticketImage = reader.result as string;

      const shellTicketData: ShellTicketData = {
        filename,
        ticketFile,
        ticketImage
      };

      const formParams = { image:ticketFile};
      const fd = new FormData();
      fd.append('file',ticketFile);

      // this.busy = this.disputeResource
      // .postTicket(ticketFile)
      // .subscribe((response) => {
      //   console.log('image data',response);
      //   this.disputeService.shellTicketData$.next(shellTicketData);
      //   if (response) {
      //     this.router.navigate([AppRoutes.disputePath(AppRoutes.SHELL)], {
      //       queryParams: shellTicketData
      //     });
      //   } else {
      //     this.notFound = true;
      //     this.onTicketNotFound();
      //   }
      // });
      
      this.http.post(`http://localhost:5000/api/tickets/analyse`,fd)
      .subscribe(res=>{
        this.ticketService.setImageData(res);
        this.disputeService.shellTicketData$.next(shellTicketData);
      this.router.navigate([AppRoutes.disputePath(AppRoutes.SHELL)]);
      })
        
      
    };
  }

  public get ticketNumber(): FormControl {
    return this.form.get('ticketNumber') as FormControl;
  }
}

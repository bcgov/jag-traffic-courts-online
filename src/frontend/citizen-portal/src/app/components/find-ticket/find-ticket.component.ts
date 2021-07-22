import {
  AfterViewInit,
  Component,
  OnInit,
  ViewEncapsulation,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { TicketExampleDialogComponent } from '@shared/dialogs/ticket-example-dialog/ticket-example-dialog.component';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindTicketComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public form: FormGroup;

  public notFound = false;

  constructor(
    private route: Router,
    private formBuilder: FormBuilder,
    private disputeResource: DisputeResourceService,
    private formUtilsService: FormUtilsService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private dialog: MatDialog,
    private logger: LoggerService
  ) {
    //
  }

  public ngOnInit(): void {
    this.form = this.formBuilder.group({
      ticketNumber: ['EZ02000460', [Validators.required]],
      time: ['09:54', [Validators.required]],
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }

  public onViewTicketExample(): void {
    this.dialog.open(TicketExampleDialogComponent, {
      width: '600px',
    });
  }

  public onSearch(): void {
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
          this.route.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
            queryParams: formParams,
          });
        } else {
          this.notFound = true;
        }
      });
  }

  public get ticketNumber(): FormControl {
    return this.form.get('ticketNumber') as FormControl;
  }
}

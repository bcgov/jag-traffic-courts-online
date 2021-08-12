import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CountSummaryComponent } from '@components/count-summary/count-summary.component';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { TicketPaymentDialogComponent } from '@shared/dialogs/ticket-payment-dialog/ticket-payment-dialog.component';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-ticket-payment',
  templateUrl: './ticket-payment.component.html',
  styleUrls: ['./ticket-payment.component.scss'],
})
export class TicketPaymentComponent implements OnInit, AfterViewInit {
  @ViewChild(CountSummaryComponent, { static: false }) countSummary;
  public busy: Subscription;
  public ticket: TicketDispute;

  constructor(
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private router: Router,
    private toastService: ToastService,
    private dialog: MatDialog,
    private logger: LoggerService
  ) {}

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }

      this.logger.info('TicketPaymentComponent current ticket', ticket);
      this.ticket = ticket;
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }

  public onPayTicket(): void {
    this.logger.info('onPayTicket', this.ticket);

    this.countSummary.countComponents.forEach((child) => {
      console.log('isSelected', child.isSelected);
    });

    /*
    const data: DialogOptions = {
      titleKey: 'submit_confirmation.heading',
      messageKey: 'submit_confirmation.message',
      actionTextKey: 'submit_confirmation.confirm',
      cancelTextKey: 'submit_confirmation.cancel',
    };

    this.dialog
      .open(TicketPaymentDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          this.toastService.openSuccessToast('Ticket payment is successful');

          this.router.navigate([
            AppRoutes.disputePath(AppRoutes.PAYMENT_SUCCESS),
          ]);
        }
      });
      */
  }
}

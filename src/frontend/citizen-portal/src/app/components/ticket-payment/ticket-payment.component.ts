import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CountSummaryComponent } from '@components/count-summary/count-summary.component';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
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

  public onMakePayment(): void {
    this.logger.info('onMakePayment', this.ticket);

    let numberSelected = 0;
    this.countSummary.countComponents.forEach((child) => {
      if (child.isSelected.selected) {
        numberSelected++;
      }
    });

    if (numberSelected === 0) {
      const data: DialogOptions = {
        titleKey: 'Make payment',
        actionType: 'warn',
        messageKey: 'You must select at least one Count to pay',
        actionTextKey: 'Ok',
        cancelHide: true,
      };

      this.dialog.open(ConfirmDialogComponent, { data });
      return;
    }

    // const data: DialogOptions = {
    //   titleKey: 'Ticket payment',
    //   messageKey: 'Enter your credit card information...',
    //   actionTextKey: 'Proceed with payment',
    //   cancelTextKey: 'Cancel',
    // };

    this.dialog
      .open(TicketPaymentDialogComponent)
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          this.toastService.openSuccessToast('Ticket payment is successful');

          this.router.navigate([
            AppRoutes.disputePath(AppRoutes.PAYMENT_COMPLETE),
          ]);
        }
      });
  }
}

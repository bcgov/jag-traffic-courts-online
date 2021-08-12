import { AfterViewInit, Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { TicketPaymentDialogComponent } from '@shared/dialogs/ticket-payment-dialog/ticket-payment-dialog.component';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-dispute-submit-success',
  templateUrl: './dispute-submit-success.component.html',
  styleUrls: ['./dispute-submit-success.component.scss'],
})
export class DisputeSubmitSuccessComponent implements OnInit, AfterViewInit {
  public ticket: TicketDispute;

  constructor(
    private router: Router,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private toastService: ToastService,
    private dialog: MatDialog,
    private logger: LoggerService
  ) {}

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }

      this.ticket = ticket;
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.goToTop();
  }

  public onViewYourTicket(): void {
    const ticket = this.disputeService.ticket;
    const params = {
      ticketNumber: ticket.violationTicketNumber,
      time: ticket.violationTime,
    };

    this.disputeService.ticket$.next(null);

    this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
      queryParams: params,
    });
  }

  public onExitTicket(): void {
    this.disputeService.ticket$.next(null);
    this.router.navigate(['/']);
  }

  public onMakePayment(): void {
    this.logger.info('onMakePayment', this.ticket);

    const data: DialogOptions = {
      titleKey: 'Ticket payment',
      messageKey: 'Enter your credit card information...',
      actionTextKey: 'Proceed with payment',
      cancelTextKey: 'Cancel',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          this.toastService.openSuccessToast('Ticket payment is successful');

          this.router.navigate([
            AppRoutes.disputePath(AppRoutes.PAYMENT_SUCCESS),
          ]);
        }
      });
  }

  public get countsToPay(): string {
    let countsToPay = '';
    let count = 0;

    this.ticket?.offences
      ?.filter((offence) => offence.offenceAgreementStatus === 'PAY')
      .forEach((offence) => {
        if (count > 0) {
          countsToPay += ', ';
        }
        countsToPay += offence.offenceNumber;
        count++;
      });

    if (count > 1) {
      return 'Counts ' + countsToPay;
    } else if (count === 1) {
      return 'Count ' + countsToPay;
    }

    return null;
  }
}

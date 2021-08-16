import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CountSummaryComponent } from '@components/count-summary/count-summary.component';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
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
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private router: Router,
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
    let countsToPay = '';
    let countsToPayAmount = 0;
    let numberSelected = 0;
    this.countSummary.countComponents.forEach((child) => {
      if (child.isSelected.selected) {
        if (numberSelected > 0) {
          countsToPay += ',';
        }
        countsToPay += child.isSelected.offenceNumber;
        countsToPayAmount += child.isSelected.amount;
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

    const formParams = {
      ticketNumber: this.ticket.violationTicketNumber,
      time: this.ticket.violationTime,
      counts: countsToPay,
      amount: countsToPayAmount,
    };

    this.logger.info('onMakePayment', formParams);

    this.busy = this.disputeResource
      .initiateTicketPayment(formParams)
      .subscribe((response) => {
        // todo: update later
        if (response.redirectUrl) {
          window.location.href = response.redirectUrl;
        }
      });
  }
}

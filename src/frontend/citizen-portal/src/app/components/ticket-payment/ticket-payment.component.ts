import { Component, OnInit, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CountSummaryComponent } from '@components/count-summary/count-summary.component';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { AppRoutes } from 'app/app.routes';
import { AppConfigService } from 'app/services/app-config.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-ticket-payment',
  templateUrl: './ticket-payment.component.html',
  styleUrls: ['./ticket-payment.component.scss'],
})
export class TicketPaymentComponent implements OnInit {
  @ViewChild(CountSummaryComponent, { static: false }) countSummary;
  public busy: Subscription;
  public ticket: TicketDisputeView;
  public form: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private formUtilsService: FormUtilsService,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private appConfigService: AppConfigService,
    private router: Router,
    private dialog: MatDialog,
    private logger: LoggerService
  ) {
    this.form = this.formBuilder.group({
      emailAddress: [null, [Validators.required, Validators.email]],
    });
  }

  public ngOnInit(): void {
    this.disputeService.ticket$.subscribe((ticket) => {
      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      this.logger.info('TicketPaymentComponent current ticket', ticket);
      this.ticket = ticket;
    });
  }

  public onMakePayment(): void {
    if (this.appConfigService.useMockServices) {
      this.router.navigate([AppRoutes.disputePath(AppRoutes.PAYMENT_COMPLETE)]);
      return;
    }

    const validity = this.formUtilsService.checkValidity(this.form);
    const errors = this.formUtilsService.getFormErrors(this.form);

    this.logger.log('validity', validity);
    this.logger.log('errors', errors);
    this.logger.log('form.value', this.form.value);

    if (!validity) {
      this.utilsService.scrollToErrorSection();
      return;
    }

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

  public get emailAddress(): FormControl {
    return this.form.get('emailAddress') as FormControl;
  }
}

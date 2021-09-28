import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { AppRoutes } from 'app/app.routes';
import { AppConfigService } from 'app/services/app-config.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-ticket-payment-complete',
  templateUrl: './ticket-payment-complete.component.html',
  styleUrls: ['./ticket-payment-complete.component.scss'],
})
export class TicketPaymentCompleteComponent implements OnInit {
  public busy: Subscription;
  public ticket: TicketDisputeView;
  public paymentStatus: string;
  public paymentConfNo: string;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private disputeService: DisputeService,
    private disputeResource: DisputeResourceService,
    private appConfigService: AppConfigService,
    private logger: LoggerService
  ) { }

  public ngOnInit(): void {
    if (this.appConfigService.useMockServices) {
      this.paymentStatus = 'paid';
      this.paymentConfNo = '123-mock';
      this.ticket = this.disputeService.ticket;
      return;
    }

    this.activatedRoute.queryParams.subscribe((params) => {
      const idParam = 'id';
      const id = params[idParam];

      const amountParam = 'amount';
      const amount = params[amountParam];

      const transIdParam = 'transId';
      const transId = params[transIdParam];

      const statusParam = 'status';
      const status = params[statusParam];

      const confNoParam = 'confNo';
      const confNo = params[confNoParam];

      this.logger.info(
        'TicketPaymentCompleteComponent',
        idParam,
        id,
        statusParam,
        status,
        amountParam,
        amount,
        confNoParam,
        confNo,
        transIdParam,
        transId
      );

      const paramsApi = {
        id,
        status,
        amount,
        confNo,
        transId,
      };

      this.busy = this.disputeResource.makeTicketPayment(paramsApi).subscribe({
        next: (res) => {
          this.paymentStatus = status;
          this.paymentConfNo = confNo;

          this.disputeService.ticket$.next(res);
          this.ticket = res;
        },
        error: (err) => {
          this.paymentStatus = 'error';
          console.log('HTTP Error', err);
        },
      });
    });
  }

  public onPrint(): void {
    window.print();
  }

  public onInitiateResolution(): void {
    const { countsToResolve } = this.getListOfCountsToResolve();
    const formParams = {
      ticketNumber: this.ticket.violationTicketNumber,
      time: this.ticket.violationTime
    };

    this.logger.info('onInitiateResolution', formParams);
    if (countsToResolve) {
      this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
        queryParams: formParams,
      });
    }
  }

  private getListOfCountsToResolve(): {
    countsToResolve: string;
  } {
    let countsToResolve = '';
    let count = 0;

    this.ticket?.offences
      ?.filter((offence) => offence._offenceStatus === 'Owe')
      .forEach((offence) => {
        if (count > 0) {
          countsToResolve += ',';
        }
        countsToResolve += offence.offenceNumber;
        count++;
      });

    return { countsToResolve };
  }

  public get isPaymentSuccess(): boolean {
    return this.paymentStatus === 'paid';
  }

  public get isPaymentCancelled(): boolean {
    return this.paymentStatus === 'cancelled';
  }

  public get countsToResolve(): string {
    const { countsToResolve } = this.getListOfCountsToResolve();

    if (countsToResolve) {
      if (countsToResolve.indexOf(',') > -1) {
        return 'Counts ' + countsToResolve;
      } else {
        return 'Count ' + countsToResolve;
      }
    }

    return null;
  }
}

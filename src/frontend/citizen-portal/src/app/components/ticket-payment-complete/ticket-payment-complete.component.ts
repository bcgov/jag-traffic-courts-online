import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-ticket-payment-complete',
  templateUrl: './ticket-payment-complete.component.html',
  styleUrls: ['./ticket-payment-complete.component.scss'],
})
export class TicketPaymentCompleteComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public ticket: TicketDispute;
  public paymentStatus: string;
  public paymentConfNo: string;

  constructor(
    private activatedRoute: ActivatedRoute,
    private disputeService: DisputeService,
    private disputeResource: DisputeResourceService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {}

  public ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      const idParam = 'id';
      const id = params[idParam];

      const amountParam = 'amount';
      const amount = params[amountParam];

      const transIdParam = 'transId';
      const transId = params[transIdParam];

      const statusParam = 'status';
      const status = params[statusParam];
      this.paymentStatus = status;

      const confNoParam = 'confNo';
      const confNo = params[confNoParam];
      this.paymentConfNo = confNo;

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

      this.busy = this.disputeResource.makeTicketPayment(paramsApi).subscribe(
        (res) => {
          this.disputeService.ticket$.next(res);
          this.ticket = res;
        },
        (err) => {
          this.paymentStatus = 'error';
          console.log('HTTP Error', err);
        }
      );

      // const paramsApi = {
      //   ticketNumber: 'EZ02000460',
      //   time: '09:54',
      // };
      // this.busy = this.disputeResource
      //   .getTicket(paramsApi)
      //   .subscribe((response) => {
      //     this.logger.info(
      //       'TicketPaymentCompleteComponent::makeTicketPayment response',
      //       response
      //     );
      //     this.disputeService.ticket$.next(response);
      //     this.ticket = response;
      //   });
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.goToTop();
  }

  public get isPaymentSuccess(): boolean {
    return this.paymentStatus === 'paid';
  }

  public get isPaymentCancelled(): boolean {
    return this.paymentStatus === 'cancelled';
  }
}

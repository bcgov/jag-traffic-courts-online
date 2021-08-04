import { AfterViewInit, Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { TranslateService } from '@ngx-translate/core';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public ticket: TicketDispute;
  public defaultLanguage: string;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private logger: LoggerService,
    private translateService: TranslateService,
    private toastService: ToastService,
    private dialog: MatDialog
  ) {}

  public ngOnInit(): void {
    this.defaultLanguage = this.translateService.getDefaultLang();

    this.route.queryParams.subscribe((params) => {
      if (Object.keys(params).length === 0) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }

      const ticketNumber = params.ticketNumber;
      const ticketTime = params.time;

      if (!ticketNumber || !ticketTime) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }

      const ticket = this.disputeService.ticket;
      if (ticket) {
        this.ticket = ticket;
      } else {
        this.performSearch(params);
      }
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }

  private performSearch(params): void {
    this.busy = this.disputeResource.getTicket(params).subscribe((response) => {
      this.disputeService.ticket$.next(response);
      this.ticket = response;
    });
  }

  public onDisputeTicket(): void {
    this.logger.info('onDisputeTicket', this.disputeService.ticket);
    // const source = timer(1000);
    // this.busy = source.subscribe((val) => {
    this.router.navigate([AppRoutes.disputePath(AppRoutes.STEPPER)]);
    // });
  }

  public onPayTicket(): void {
    this.logger.info('onPayTicket', this.disputeService.ticket);
    // const source = timer(1000);
    // this.busy = source.subscribe((val) => {
    this.router.navigate([AppRoutes.disputePath(AppRoutes.PAYMENT)]);
    // });
    // const data: DialogOptions = {
    //   titleKey: 'submit_confirmation.heading',
    //   messageKey: 'submit_confirmation.message',
    //   actionTextKey: 'submit_confirmation.confirm',
    //   cancelTextKey: 'submit_confirmation.cancel',
    // };

    // this.dialog
    //   .open(TicketPaymentDialogComponent, { data })
    //   .afterClosed()
    //   .subscribe((response: boolean) => {
    //     console.log('response', response);
    //     if (response) {
    //       this.toastService.openSuccessToast('Ticket payment is successful');
    //       // this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)]);
    //     }
    //   });
  }
}

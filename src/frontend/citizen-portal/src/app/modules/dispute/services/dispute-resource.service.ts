import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Dispute } from '@shared/models/dispute.model';
import { Ticket } from '@shared/models/ticket.model';
import { Offence } from '@shared/models/offence.model';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  constructor(
    private apiResource: ApiResource,
    private toastService: ToastService,
    private logger: LoggerService
  ) {}

  /**
   * Get the ticket from RSI.
   *
   * @param params containing the ticketNumber and time
   */
  public getTicket(params: {
    ticketNumber: string;
    time: string;
  }): Observable<Ticket> {
    const httpParams = new HttpParams({ fromObject: params });

    return this.apiResource.get<Ticket>('tickets', httpParams).pipe(
      map((response: ApiHttpResponse<Ticket>) =>
        response ? response.result : null
      ),
      tap((ticket: Ticket) =>
        this.logger.info('DisputeResourceService::getTicket', ticket)
      ),
      map((ticket) => {
        if (ticket) {
          this.setOffenceInfo(ticket);
        }
        return ticket;
      }),
      catchError((error: any) => {
        this.toastService.openErrorToast('Ticket could not be retrieved');
        this.logger.error(
          'DisputeResourceService::getTicket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  /**
   * Create the dispute
   *
   * @param dispute The dispute to be created
   */
  public createDispute(dispute: Dispute): Observable<Dispute> {
    this.logger.info('createDispute', dispute);

    return this.apiResource.post<Dispute>('disputes', dispute).pipe(
      map((response: ApiHttpResponse<Dispute>) => null),
      catchError((error: any) => {
        this.toastService.openErrorToast('Dispute could not be created');
        this.logger.error(
          'DisputeResourceService::createDispute error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  private getOffenceInfo(
    row: Offence
  ): { status: number; desc: string; notes: string } {
    const disputeStatus = row.dispute ? row.dispute.status : null;

    const status = disputeStatus ? disputeStatus : row.amountDue > 0 ? -1 : -2;

    let desc: string;
    if (disputeStatus) {
      if (disputeStatus === 0) {
        desc = 'Created';
      } else if (disputeStatus === 1) {
        desc = 'Submitted';
      } else if (disputeStatus === 2) {
        desc = 'In Progress';
      } else if (disputeStatus === 3) {
        desc = 'Resolved';
      } else if (disputeStatus === 4) {
        desc = 'Rejected';
      }
    } else if (row.amountDue > 0) {
      desc = 'Outstanding Balance';
    } else {
      desc = 'Paid';
    }

    let notes: string;
    if (disputeStatus) {
      if (disputeStatus === 1) {
        notes =
          'The dispute has been filed. An email with the court information will be sent soon.';
      } else if (disputeStatus === 2) {
        notes =
          'A court date has been set for this dispute. Check your email for more information.';
      }
    }

    return {
      status,
      desc,
      notes,
    };
  }

  /**
   * populate the offence object with the calculated information
   */
  private setOffenceInfo(ticket: Ticket): void {
    let balance = 0;
    let disputesExist = false;
    ticket.offences.forEach((offence) => {
      const { status, desc, notes } = this.getOffenceInfo(offence);
      offence.offenceStatus = status;
      offence.offenceStatusDesc = desc;
      offence.notes = notes;

      if (offence.dispute) {
        disputesExist = true;
      }

      balance += offence.amountDue;
    });

    // ------------------------------------
    ticket.outstandingBalance = balance;
    ticket.disputesExist = disputesExist;
  }
}

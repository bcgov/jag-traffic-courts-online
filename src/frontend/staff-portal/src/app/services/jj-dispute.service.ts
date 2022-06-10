import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { DisputeService, Dispute } from './dispute.service';
import { cloneDeep } from 'lodash';
import { DatePipe } from '@angular/common';
import { DisputeStatus } from 'app/api';

@Injectable({
  providedIn: 'root',
})
export class JjDisputeService {
  private _jjDisputes: BehaviorSubject<JjDispute[]> = new BehaviorSubject<JjDispute[]>(null);
  private _jjDispute: BehaviorSubject<JjDispute> = new BehaviorSubject<JjDispute>(null);

  private mockData: JjDispute[] = [
    {
      "createdBy": "System",
      "createdTs": "2022-06-09T21:45:35.781+00:00",
      "modifiedBy": null,
      "modifiedTs": null,
      "ticketNumber": "1000001",
      "violationDate": "2022-06-09T21:45:35.781+00:00",
      "disputantName": "John Doe",
      "enforcementOfficer": "Steven Allan",
      "policeDetachment": "West Shore",
      "courthouseLocation": "Victoria",
      "jjAssignedTo": "JJ1",
      "jjGroupAssignedTo": "JJGroup1"
    },
    {
      "createdBy": "System",
      "createdTs": "2022-06-09T21:45:35.781+00:00",
      "modifiedBy": null,
      "modifiedTs": null,
      "ticketNumber": "1000002",
      "violationDate": "2022-06-08T00:00:00.000+00:00",
      "disputantName": "Jane Doe",
      "enforcementOfficer": "Alison Kerr",
      "policeDetachment": "Valemount",
      "courthouseLocation": "Vancouver",
      "jjAssignedTo": "JJ2",
      "jjGroupAssignedTo": "JJGroup2"
    },
    {
      "createdBy": "System",
      "createdTs": "2022-06-09T21:45:35.781+00:00",
      "modifiedBy": null,
      "modifiedTs": null,
      "ticketNumber": "1000003",
      "violationDate": "2022-06-07T00:00:00.000+00:00",
      "disputantName": "Simon Young",
      "enforcementOfficer": "Adrian Peake",
      "policeDetachment": "University",
      "courthouseLocation": "Vancouver",
      "jjAssignedTo": null,
      "jjGroupAssignedTo": "JJGroup2"
    },
    {
      "createdBy": "System",
      "createdTs": "2022-06-09T21:45:35.781+00:00",
      "modifiedBy": null,
      "modifiedTs": null,
      "ticketNumber": "1000004",
      "violationDate": "2022-06-06T00:00:00.000+00:00",
      "disputantName": "Matt Vaughan",
      "enforcementOfficer": "Steven Allan",
      "policeDetachment": "Whistler",
      "courthouseLocation": "Whistler",
      "jjAssignedTo": "JJ3",
      "jjGroupAssignedTo": "JJGroup1"
    },
    {
      "createdBy": "System",
      "createdTs": "2022-06-09T21:45:35.781+00:00",
      "modifiedBy": null,
      "modifiedTs": null,
      "ticketNumber": "1000005",
      "violationDate": "2022-06-05T00:00:00.000+00:00",
      "disputantName": "Gavin Glover",
      "enforcementOfficer": "Harry Reid",
      "policeDetachment": "Ladysmith",
      "courthouseLocation": "Squamish",
      "jjAssignedTo": "JJ3",
      "jjGroupAssignedTo": "JJGroup3"
    },
    {
      "createdBy": "System",
      "createdTs": "2022-06-09T21:45:35.781+00:00",
      "modifiedBy": null,
      "modifiedTs": null,
      "ticketNumber": "1000006",
      "violationDate": "2022-06-04T00:00:00.000+00:00",
      "disputantName": "Gavin Glover",
      "enforcementOfficer": "Harry Reid",
      "policeDetachment": "Ladysmith",
      "courthouseLocation": "Squamish",
      "jjAssignedTo": null,
      "jjGroupAssignedTo": null
    }
  ]
  private tempData: JjDispute[] = [];

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private disputeService: DisputeService,
    private datePipe: DatePipe
  ) {
    this.disputeService.getDisputes().subscribe(disputes => {
      if (disputes) {
        let i = 0;
        this.tempData = [];
        disputes.filter(dispute => dispute.assignedTo).forEach(dispute => {
          let data = cloneDeep(this.mockData[i % (this.mockData.length - 1)]);
          data.dispute = dispute;
          data.jjAssignedTo = dispute.assignedTo;
          data.disputantName = dispute.givenNames + " " + dispute.surname;
          data.modifiedBy = dispute.modifiedBy;
          data.modifiedTs = dispute.modifiedTs;
          data.violationDate = this.datePipe.transform(data.violationDate, "yyyy-MM-dd");
          data.__status = DisputeStatus.New;
          this.tempData.push(data);
          i++;
        })
        this._jjDisputes.next(this.tempData);
      }
    });
  }

  /**
     * Get the disputes from RSI excluding CANCELLED
     *
     * @param none
     */
  public getJjDisputes(): Observable<JjDispute[]> {
    return this.jjDisputes$;
  }

  public get jjDisputes$(): Observable<JjDispute[]> {
    return this._jjDisputes.asObservable();
  }

  public get jjDisputes(): JjDispute[] {
    return this._jjDisputes.value;
  }

  /**
   * Get the dispute from RSI by Id.
   *
   * @param disputeId
   */
  public getJjDispute(disputeId: string): Observable<JjDispute> {
    let data = this.tempData.filter(i => i.dispute.id === disputeId);
    let result = data.length > 0 ? data[0] : null;
    this._jjDispute.next(result);
    return this.jjDispute$;
  }

  public get jjDispute$(): Observable<JjDispute> {
    return this._jjDispute.asObservable();
  }

  public get jjDispute(): JjDispute {
    return this._jjDispute.value;
  }
}

export interface JjDispute {
  createdBy?: string | null;
  createdTs?: string;
  modifiedBy?: string | null;
  modifiedTs?: string;
  id?: string;
  ticketNumber?: string | null;
  violationDate?: string;
  disputantName?: string | null;
  enforcementOfficer?: string | null;
  policeDetachment?: string | null;
  courthouseLocation?: string | null;
  jjAssignedTo?: string | null;
  jjGroupAssignedTo?: string | null;
  dispute?: Dispute | null;
  __status?: string | null;
}
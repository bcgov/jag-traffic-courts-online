import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { Dispute } from '../../../../services/dispute.service';
import { DisputantUpdateRequest } from '../../../../services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { DisputeContactTypeCd, DisputeUpdateRequestStatus2 } from 'app/api';

@Component({
  selector: 'app-name-update-request-info',
  templateUrl: './name-update-request-info.component.html',
  styleUrls: ['./name-update-request-info.component.scss', '../../../../app.component.scss']
})
export class NameUpdateRequestInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Input() public disputantUpdateRequest!: DisputantUpdateRequest;
  @Output() public disputantUpdateRequestStatusChange: EventEmitter<DisputantUpdateRequest> = new EventEmitter<DisputantUpdateRequest>();
  public updateRequested: nameUpdateJSON;
  public requestReadable: boolean = null;
  public UpdateRequestStatus = DisputeUpdateRequestStatus2;
  public ContactTypeCd = DisputeContactTypeCd;

  constructor(
    private logger: LoggerService,

  ) {
  }

  ngOnInit() {
    this.logger.log('NameUpdateRequestInfoComponent::Init', this.disputantUpdateRequest);

    try {
      this.updateRequested = JSON.parse(this.disputantUpdateRequest.updateJson);
      this.requestReadable = true;
      // check if current and update requested values are equal, if so change status to accepted.
      if (this.disputeInfo.contactTypeCd === this.updateRequested.ContactType &&
        this.disputeInfo.disputantSurname === this.updateRequested.DisputantSurname &&
        this.disputeInfo.disputantGivenName1 === this.updateRequested.DisputantGivenName1 &&
        this.disputeInfo.disputantGivenName2 === this.updateRequested.DisputantGivenName2 &&
        this.disputeInfo.disputantGivenName3 === this.updateRequested.DisputantGivenName3 &&
        this.disputeInfo.contactSurnameNm === this.updateRequested.ContactSurnameNm &&
        this.disputeInfo.contactGiven1Nm === this.updateRequested.ContactGiven1Nm &&
        this.disputeInfo.contactGiven2Nm === this.updateRequested.ContactGiven2Nm &&
        this.disputeInfo.contactGiven3Nm === this.updateRequested.ContactGiven3Nm &&
        this.disputeInfo.contactLawFirmNm === this.updateRequested.ContactLawFirmNm &&
        this.disputantUpdateRequest.status === DisputeUpdateRequestStatus2.Pending)  {
        this.disputantUpdateRequest.status = DisputeUpdateRequestStatus2.Accepted;
        this.disputantUpdateRequestStatusChange.emit(this.disputantUpdateRequest);
      }
    }
    catch (ex) {
      // Just dont crash, fail gracefully
      this.requestReadable = false;
    }
  }

  // emit status change to parent control
  statusChange(event) {
    this.disputantUpdateRequestStatusChange.emit(this.disputantUpdateRequest);
  }

}

export interface nameUpdateJSON {
  DisputantSurname?: string;
  DisputantGivenName1?: string;
  DisputantGivenName2?: string;
  DisputantGivenName3?: string;
  ContactSurnameNm?: string;
  ContactGiven1Nm?: string;
  ContactGiven2Nm?: string;
  ContactGiven3Nm?: string;
  ContactLawFirmNm?: string;
  ContactType?: DisputeContactTypeCd;
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { ConfigService } from '@config/config.service';
import { Dispute, DisputeService } from '../../../services/dispute.service';
import { DisputantUpdateRequestUpdateType, DisputantUpdateRequestStatus2 } from 'app/api';
import { DisputantUpdateRequest } from '../../../services/dispute.service';

@Component({
  selector: 'app-update-request-info',
  templateUrl: './update-request-info.component.html',
  styleUrls: ['./update-request-info.component.scss', '../../../app.component.scss']
})
export class UpdateRequestInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Output() public backInbox: EventEmitter<any> = new EventEmitter();
  public initialDisputeValues: Dispute;
  public retrieving: boolean = true;
  public violationDate: string = "";
  public violationTime: string = "";
  public conflict: boolean = false;
  public collapseObj: any = {
    contactInformation: true
  }
  public disputeUpdateRequests: DisputantUpdateRequest[] = [];
  public RequestUpdateType = DisputantUpdateRequestUpdateType;
  public RequestUpdateStatus = DisputantUpdateRequestStatus2;

  constructor(
    public config: ConfigService,
    private disputeService: DisputeService,
    private logger: LoggerService,
  ) {

  }

  public ngOnInit() {
    this.getDispute();
  }

  public onSubmit(): void {
    // process accepts and rejects
    this.disputeUpdateRequests.forEach(disputeUpdateRequest => {
      if (disputeUpdateRequest.status === this.RequestUpdateStatus.Pending && disputeUpdateRequest.newStatus === this.RequestUpdateStatus.Accepted) {
        this.disputeService.acceptDisputeUpdateRequest(disputeUpdateRequest.disputantUpdateRequestId).subscribe({
          next: response => {
            disputeUpdateRequest.status = this.RequestUpdateStatus.Accepted;
          },
          error: err => { },
          complete: () => { }
        });
      } else if (disputeUpdateRequest.status === this.RequestUpdateStatus.Pending && disputeUpdateRequest.newStatus === this.RequestUpdateStatus.Rejected) {
        this.disputeService.rejectDisputeUpdateRequest(disputeUpdateRequest.disputantUpdateRequestId).subscribe({
          next: response => {
            disputeUpdateRequest.status = this.RequestUpdateStatus.Rejected;
          },
          error: err => { },
          complete: () => { }
        });      }
    })
    this.getDispute();
  }

  // get dispute
  getDispute(): void {
    this.logger.log('UpdateRequestInfoComponent::getDispute');

    this.disputeService.getDispute(this.disputeInfo.disputeId).subscribe((response: Dispute) => {
      this.logger.info(
        'UpdateRequestInfoComponent::getDispute response',
        response
      );

      this.initialDisputeValues = response;

      // set violation date and time
      let tempViolationDate = this.initialDisputeValues?.issuedTs?.split("T");
      if (tempViolationDate) {
        this.violationDate = tempViolationDate[0];
        this.violationTime = tempViolationDate[1].split(":")[0] + ":" + tempViolationDate[1].split(":")[1];
      }

      if (this.retrieving === true) this.getDisputeUpdateRequests(this.initialDisputeValues.disputeId);

    });
  }

  getDisputeUpdateRequests(disputeId: number) {
    this.logger.log('UpdateRequestInfoComponent::getDisputeUpdateRequests');

    this.disputeService.getDisputeUpdateRequests(disputeId).subscribe((response) => {
      this.retrieving = false;
      this.logger.info(
        'UpdateRequestInfoComponent::getDisputeUpdateRequests response',
        response
      );

      this.disputeUpdateRequests = response;

      // sort oldest to newest
      this.disputeUpdateRequests = this.disputeUpdateRequests.sort((a,b) => {if (a.createdTs < b.createdTs) return -1});

    });
  }

  // respond to status changes from child component
  onDisputantUpdateRequestStatusChange(disputantUpdateRequest: DisputantUpdateRequest) {
    this.disputeUpdateRequests.forEach(disputeUpdateRequest => {
      if (disputeUpdateRequest.disputantUpdateRequestId === disputantUpdateRequest.disputantUpdateRequestId) disputeUpdateRequest.status = disputantUpdateRequest.status;
    });
  }

  getFormattedAddress(): string {
    let addresString = `${this.initialDisputeValues.addressLine1}`;

    if (this.initialDisputeValues.addressLine2) addresString += `, ${this.initialDisputeValues.addressLine2}`;
    if (this.initialDisputeValues.addressLine3) addresString += `, ${this.initialDisputeValues.addressLine3}`;
    if (this.initialDisputeValues.addressCity) addresString += ` ${this.initialDisputeValues.addressCity}`;

    if (this.initialDisputeValues.addressProvinceCountryId) {
      let prov = this.config.provincesAndStates.filter(x => x.provId === this.initialDisputeValues.addressProvinceCountryId && x.provSeqNo === this.initialDisputeValues.addressProvinceSeqNo);
      if (prov.length > 0) addresString += ` ${prov[0].provAbbreviationCd}`;
    } else if (this.initialDisputeValues.addressProvince) {
      addresString  += ` ${this.initialDisputeValues.addressProvince}`;
    }

    if (this.initialDisputeValues.addressCountryId) {
      let country = this.config.countries.filter(x => x.ctryId === this.initialDisputeValues.addressCountryId);
      if (country.length > 0) addresString += ` ${country[0].ctryLongNm}`;
    }

    if (this.initialDisputeValues.postalCode) addresString += ` ${this.initialDisputeValues.postalCode}`;

    return addresString;
  }

  public onBack() {
    this.backInbox.emit();
  }
}

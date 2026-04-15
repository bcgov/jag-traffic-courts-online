import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { Dispute } from '../../../../services/dispute.service';
import { DisputantUpdateRequest } from '../../../../services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { ConfigService } from '@config/config.service';
import { DisputeCountPleaCode, DisputeCountRequestReduction, DisputeCountRequestTimeToPay, DisputeInterpreterRequired, DisputeRepresentedByLawyer, DisputeRequestCourtAppearanceYn } from 'app/api';
import { LookupsService } from 'app/services/lookups.service';

@Component({
  selector: 'app-count-update-request-info',
  templateUrl: './count-update-request-info.component.html',
  styleUrls: ['./count-update-request-info.component.scss'],
  standalone: false,
})
export class CountUpdateRequestInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Input() public disputantUpdateRequest!: DisputantUpdateRequest;
  @Output() public disputantUpdateRequestStatusChange: EventEmitter<DisputantUpdateRequest> = new EventEmitter<DisputantUpdateRequest>();
  public updateRequested: CountsUpdateJSON;
  public requestReadable: boolean = null;

  constructor(
    private logger: LoggerService,
    public config: ConfigService

  ) {
  }

  ngOnInit() {
    this.logger.log('CountUpdateRequestInfoComponent::Init', this.disputantUpdateRequest);

    try {
      this.updateRequested = JSON.parse(this.disputantUpdateRequest.updateJson);
      this.requestReadable = true;

      //update for display
      this.updateRequested.DisputeCounts = this.updateRequested.DisputeCounts.filter(count => {
        const oldCount = this.disputeInfo.disputeCounts.find(x => x.countNo === count.CountNo);
        if (oldCount) {
          // Assign old values for potential use or display
          count.OldPleaCode = oldCount.pleaCode;
          count.OldRequestReduction = oldCount.requestReduction;
          count.OldRequestTimeToPay = oldCount.requestTimeToPay;
          
          // If all new values equal the old values, filter this count out
          return (
            count.PleaCode !== oldCount.pleaCode ||
            count.RequestReduction !== oldCount.requestReduction ||
            count.RequestTimeToPay !== oldCount.requestTimeToPay
          );
        }
        // If there's no matching old record, keep the count
        return true;
      });
    }
    catch (ex) {
      // Just dont crash, fail gracefully
      this.requestReadable = false;
      console.error("Could not parse", this.disputantUpdateRequest.updateJson);
    }
  }

  // emit status change to parent control
  statusChange(event) {
    this.disputantUpdateRequestStatusChange.emit(this.disputantUpdateRequest);
  }

}

export interface CountsUpdateJSON {
  DisputeCounts?: CountUpdateJSON[];
}

export interface CountUpdateJSON {
  CountNo: number;
  PleaCode?: DisputeCountPleaCode;
  RequestTimeToPay?: DisputeCountRequestTimeToPay;
  RequestReduction?: DisputeCountRequestReduction;
  OldPleaCode?: DisputeCountPleaCode;
  OldRequestTimeToPay?: DisputeCountRequestTimeToPay;
  OldRequestReduction?: DisputeCountRequestReduction;
}

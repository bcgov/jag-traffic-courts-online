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
  styleUrls: ['./count-update-request-info.component.scss', '../../../../app.component.scss']
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

      // update for display
      this.updateRequested.DisputeCounts.forEach(count => {
        let oldCount = this.disputeInfo.disputeCounts.filter(x => x.countNo === count.CountNo);
        if (oldCount.length > 0) {
          count.OldPleaCode = oldCount[0].pleaCode;
          count.OldRequestReduction = oldCount[0].requestReduction;
          count.OldRequestTimeToPay = oldCount[0].requestTimeToPay;
        }
      })
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

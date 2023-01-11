import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { Dispute } from '../../../../services/dispute.service';
import { DisputantUpdateRequest } from '../../../../services/dispute.service';
import { LoggerService } from '@core/services/logger.service';

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

  constructor(
    private logger: LoggerService,

  ) {
  }

  ngOnInit() {
    this.logger.log('NameUpdateRequestInfoComponent::Init', this.disputantUpdateRequest);

    try {
      this.updateRequested = JSON.parse(this.disputantUpdateRequest.updateJson);
      this.requestReadable = true;
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
}

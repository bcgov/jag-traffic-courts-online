import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { Dispute } from '../../../../services/dispute.service';
import { DisputantUpdateRequest } from '../../../../services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { ConfigService } from '@config/config.service';
import { DisputeInterpreterRequired, DisputeRepresentedByLawyer, DisputeRequestCourtAppearanceYn } from 'app/api';
import { LookupsService } from 'app/services/lookups.service';

@Component({
  selector: 'app-court-options-update-request-info',
  templateUrl: './court-options-update-request-info.component.html',
  styleUrls: ['./court-options-update-request-info.component.scss', '../../../../app.component.scss']
})
export class CourtOptionsUpdateRequestInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Input() public disputantUpdateRequest!: DisputantUpdateRequest;
  @Output() public disputantUpdateRequestStatusChange: EventEmitter<DisputantUpdateRequest> = new EventEmitter<DisputantUpdateRequest>();
  public updateRequested: courtOptionsUpdateJSON;
  public requestReadable: boolean = null;
  public oldLanguageName: string = "";
  public newLanguageName: string = "";
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public InterpreterRequired = DisputeInterpreterRequired;
  public RequestCourtAppearance = DisputeRequestCourtAppearanceYn;

  constructor(
    private logger: LoggerService,
    public config: ConfigService,
    private lookups: LookupsService

  ) {
  }

  ngOnInit() {
    this.logger.log('CourtOptionsUpdateRequestInfoComponent::Init', this.disputantUpdateRequest);

    try {
      this.updateRequested = JSON.parse(this.disputantUpdateRequest.updateJson);
      this.requestReadable = true;

      // Get old language name
      if (this.disputeInfo.interpreterLanguageCd !== null) {
        this.oldLanguageName = this.lookups.getLanguageDescription(this.disputeInfo.interpreterLanguageCd);
      }

      // Get new language name if needed
      if (this.updateRequested.InterpreterLanguageCd !== null) {
        this.newLanguageName = this.lookups.getLanguageDescription(this.updateRequested.InterpreterLanguageCd);
      }

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

export interface courtOptionsUpdateJSON {
  RepresentedByLawyer?: boolean;
  LawFirmName?: string;
  LawyerSurname?: string;
  LawyerGivenName1?: string;
  LawyerGivenName2?: string;
  LawyerGivenName3?: string;
  LawyerAddress?: string;
  LawyerPhoneNumber?: string;
  LawyerEmail?: string;
  RequestCourtAppearance?: DisputeRequestCourtAppearanceYn;
  InterpreterLanguageCd?: string;
  InterpreterRequired?: boolean;
  WitnessNo?: number;
  FineReductionReason?: string;
  TimeToPayReason?: string;
}

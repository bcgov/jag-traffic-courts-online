import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { Dispute } from '../../../../services/dispute.service';
import { DisputantUpdateRequest } from '../../../../services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { ConfigService } from '@config/config.service';

@Component({
  selector: 'app-address-update-request-info',
  templateUrl: './address-update-request-info.component.html',
  styleUrls: ['./address-update-request-info.component.scss', '../../../../app.component.scss']
})
export class AddressUpdateRequestInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Input() public disputantUpdateRequest!: DisputantUpdateRequest;
  @Output() public disputantUpdateRequestStatusChange: EventEmitter<DisputantUpdateRequest> = new EventEmitter<DisputantUpdateRequest>();
  public updateRequested: addressUpdateJSON;
  public requestReadable: boolean = null;
  public oldProvince: string = "";
  public newProvince: string = "";
  public oldCountry: string = "";
  public newCountry: string = "";

  constructor(
    private logger: LoggerService,
    public config: ConfigService,

  ) {
  }

  ngOnInit() {
    this.logger.log('AddressUpdateRequestInfoComponent::Constructor', this.disputantUpdateRequest);

    try {
      this.updateRequested = JSON.parse(this.disputantUpdateRequest.updateJson);
      this.requestReadable = true;

      // Get old province name
      if (this.disputeInfo.addressProvinceCountryId !== null) {
        let oldProv = this.config.provincesAndStates.filter(x => x.ctryId === this.updateRequested.address_province_country_id && x.provSeqNo === this.updateRequested.address_province_seq_no);
        if (oldProv.length >0) this.oldProvince = oldProv[0].provNm;
      } else if (this.disputeInfo.addressProvince !== null) this.oldProvince = this.disputeInfo.addressProvince;

      // Get new province name if needed
      if (this.updateRequested.address_province_country_id !== null) {
        let newProv = this.config.provincesAndStates.filter(x => x.ctryId === this.updateRequested.address_province_country_id && x.provSeqNo === this.updateRequested.address_province_seq_no);
        if (newProv.length > 0) this.newProvince = newProv[0].provNm;
      } else if (this.updateRequested.address_province !== null) this.newProvince = this.updateRequested.address_province;

      let oldCountry = this.config.countries.filter(x => x.ctryId == this.disputeInfo.addressCountryId);
      if (oldCountry.length > 0) this.oldCountry = oldCountry[0].ctryLongNm;

      // Get new country name if needed
      if (this.updateRequested.address_country_id !== null) {
        let newCountry = this.config.countries.filter(x => x.ctryId === this.updateRequested.address_country_id);
        if (newCountry.length > 0) this.newCountry = newCountry[0].ctryLongNm;
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

export interface addressUpdateJSON {
  address_line1?: string;
  address_line2?: string;
  address_line3?: string;
  address_city?: string;
  address_province?: string;
  address_province_country_id?: number;
  address_province_seq_no?: number;
  address_country_id?: number;
  postal_code?: string;
}

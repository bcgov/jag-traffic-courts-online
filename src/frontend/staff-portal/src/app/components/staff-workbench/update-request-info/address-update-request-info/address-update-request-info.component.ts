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
  public oldDLProvince: string = "";
  public newDLProvince: string = "";

  constructor(
    private logger: LoggerService,
    public config: ConfigService,

  ) {
  }

  ngOnInit() {
    this.logger.log('AddressUpdateRequestInfoComponent::Init', this.disputantUpdateRequest);

    try {
      this.updateRequested = JSON.parse(this.disputantUpdateRequest.updateJson);
      this.requestReadable = true;

      // Get old province name
      if (this.disputeInfo.addressProvinceCountryId !== null) {
        let oldProv = this.config.provincesAndStates.filter(x => x.ctryId === this.disputeInfo.addressProvinceCountryId && x.provSeqNo === this.disputeInfo.addressProvinceSeqNo);
        if (oldProv.length >0) this.oldProvince = oldProv[0].provNm;
      } else if (this.disputeInfo.addressProvince !== null) this.oldProvince = this.disputeInfo.addressProvince;

      // Get new province name if needed
      if (this.updateRequested.AddressProvinceCountryId !== null) {
        let newProv = this.config.provincesAndStates.filter(x => x.ctryId === this.updateRequested.AddressProvinceCountryId && x.provSeqNo === this.updateRequested.AddressProvinceSeqNo);
        if (newProv.length > 0) this.newProvince = newProv[0].provNm;
      } else if (this.updateRequested.AddressProvince !== null) this.newProvince = this.updateRequested.AddressProvince;

      let oldCountry = this.config.countries.filter(x => x.ctryId == this.disputeInfo.addressCountryId);
      if (oldCountry.length > 0) this.oldCountry = oldCountry[0].ctryLongNm;

      // Get new country name if needed
      if (this.updateRequested.AddressCountryId !== null) {
        let newCountry = this.config.countries.filter(x => x.ctryId === this.updateRequested.AddressCountryId);
        if (newCountry.length > 0) this.newCountry = newCountry[0].ctryLongNm;
      }

      // Get old province name for DL
      if (this.disputeInfo.driversLicenceIssuedProvinceSeqNo !== null) {
        let oldDLProv = this.config.provincesAndStates.filter(x => x.ctryId === this.disputeInfo.driversLicenceIssuedCountryId && x.provSeqNo === this.disputeInfo.driversLicenceIssuedProvinceSeqNo);
        if (oldDLProv.length >0) this.oldDLProvince = oldDLProv[0].provNm;
      } else if (this.disputeInfo.driversLicenceProvince !== null) this.oldDLProvince = this.disputeInfo.driversLicenceProvince;

      // Get new province name for DL if needed
      if (this.updateRequested.DriversLicenceIssuedProvSeqNo !== null) {
        let newDLProv = this.config.provincesAndStates.filter(x => x.ctryId === this.updateRequested.DriversLicenceIssuedCountryId && x.provSeqNo === this.updateRequested.DriversLicenceIssuedProvSeqNo);
        if (newDLProv.length > 0) this.newDLProvince = newDLProv[0].provNm;
      } else if (this.updateRequested.DriversLicenceProvince !== null) this.newDLProvince = this.updateRequested.DriversLicenceProvince;

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

export interface addressUpdateJSON {
  AddressLine1?: string;
  AddressLine2?: string;
  AddressLine3?: string;
  AddressCity?: string;
  AddressProvince?: string;
  AddressProvinceCountryId?: number;
  AddressProvinceSeqNo?: number;
  AddressCountryId?: number;
  PostalCode?: string;
  DriversLicenceNumber?: string;
  DriversLicenceProvince?: string;
  DriversLicenceIssuedCountryId?: number;
  DriversLicenceIssuedProvSeqNo?: number;
}

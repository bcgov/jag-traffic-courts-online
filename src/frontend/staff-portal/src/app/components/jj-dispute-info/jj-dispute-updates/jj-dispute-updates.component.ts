import { Component, Input } from '@angular/core';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';
import { DisputantUpdateRequestExpanded } from '@shared/models/disputant-update-request-expanded';
import { DisputeUpdateRequestUpdateType, Province } from 'app/api';
import { DisputantUpdateRequest, DisputeService } from 'app/services/dispute.service';
import { LookupsService } from 'app/services/lookups.service';

@Component({
  selector: 'app-jj-dispute-updates',
  templateUrl: './jj-dispute-updates.component.html',
  styleUrls: ['./jj-dispute-updates.component.scss']
})
export class JjDisputeUpdatesComponent {
  @Input() disputeId: number;

  dataSource = new MatTableDataSource<DisputantUpdateRequestExpanded>();
  displayedColumns: string[] = [
    "requestedTs",
    "fieldTitle",
    "oldValue",
    "newValue",
    "status",
    "statusTs"
  ];
  DISPUTANT_ADDRESS_FIELDS = {
    "AddressLine1":"Address Line 1", 
    "AddressLine2":"Address Line 2", 
    "AddressLine3":"Address Line 3", 
    "AddressCity":"City", 
    "AddressProvince":"Province", 
    "AddressCountry":"Country",
    "PostalCode":"Postal Code"
  };
  DISPUTANT_PHONE_FIELDS = {
    "HomePhoneNumber":"Phone Number"
  };
  DISPUTANT_NAME_FIELDS = {
    "DisputantGivenName1":"Disputant Given Name 1", 
    "DisputantGivenName2":"Disputant Given Name 2", 
    "DisputantGivenName3":"Disputant Given Name 3", 
    "DisputantSurname":"Surname"
  };

  constructor(private disputeService: DisputeService, private lookupsService: LookupsService) {
  }

  ngOnInit(): void {
    this.disputeService.getDisputeUpdateRequests(this.disputeId).subscribe(request => {
      let data : DisputantUpdateRequestExpanded[] = [];
      // expand the DisputeUpdateRequest object so that each entry in updateJson is a separate row in the MAT table
      request.forEach(request => {
        // The updateJson structure contains ALL Disputant fields, not just the ones pertaining to the specific updateType
        // So, we first need to extract the fields pertaining to the specific updateType and then use only those fields
        // in the MAT table.
        let updateJson = JSON.parse(request.updateJson);
        let currentJson = request.currentJson ? JSON.parse(request.currentJson) : {};

        // Province fields are handled differently. They are 2 fields, an id and sequence number. 
        // We need to replace these values using the lookups service to get the province name. Likewise for country.
        let ctryId = updateJson["AddressCountryId"];
        let provCtryId = updateJson["AddressProvinceCountryId"];
        let provSeqNo = updateJson["AddressProvinceSeqNo"];
        let province : Province = this.lookupsService.provinces.find(p => p.ctryId == provCtryId && p.provSeqNo == provSeqNo);
        let country = this.lookupsService.countries.find(p => p.ctryId == ctryId);
        delete updateJson["AddressCountryId"];
        delete updateJson["AddressProvinceCountryId"];
        delete updateJson["AddressProvinceSeqNo"];
        updateJson["AddressProvince"] = province ? province.provNm : "";
        updateJson["AddressCountry"] = country ? country.ctryLongNm : "";

        ctryId = currentJson["AddressCountryId"];
        provCtryId = currentJson["AddressProvinceCountryId"];
        provSeqNo = currentJson["AddressProvinceSeqNo"];
        province = this.lookupsService.provinces.find(p => p.ctryId == provCtryId && p.provSeqNo == provSeqNo);
        country = this.lookupsService.countries.find(p => p.ctryId == ctryId);
        delete currentJson["AddressCountryId"];
        delete currentJson["AddressProvinceCountryId"];
        delete currentJson["AddressProvinceSeqNo"];
        currentJson["AddressProvince"] = province ? province.provNm : "";
        currentJson["AddressCountry"] = country ? country.ctryLongNm : "";

        // Identify the specific fields based on the updateType
        let fields = {};
        if (request.updateType === DisputeUpdateRequestUpdateType.DisputantAddress) {
          fields = this.DISPUTANT_ADDRESS_FIELDS;
        }
        else if (request.updateType === DisputeUpdateRequestUpdateType.DisputantPhone) {
          fields = this.DISPUTANT_PHONE_FIELDS;
        }
        else if (request.updateType === DisputeUpdateRequestUpdateType.DisputantName) {
          fields = this.DISPUTANT_NAME_FIELDS;
        }

        // For each field in the updateType specific fields, extract the value from the updateJson and add it to the data array
        Object.entries(fields).forEach(([fieldName, fieldTitle]: [string, string]) => {
          if (updateJson.hasOwnProperty(fieldName)) {
            const newValue = updateJson[fieldName];
            const oldValue = currentJson[fieldName];

            if (oldValue !== newValue) {
              data.push({
                requestedTs: request.createdTs,
                fieldTitle: fieldTitle,
                oldValue: oldValue,
                newValue: newValue,
                status: request.status,
                statusTs: request.statusUpdateTs
              });
            }
          }
        });
      });

      // make the data unique
      data = data.filter((entry, index, self) => index === self.findIndex(function (t) {
        return t.requestedTs === entry.requestedTs 
            // && t.fieldTitle === entry.fieldTitle
            && t.oldValue === entry.oldValue 
            && t.newValue === entry.newValue 
            && t.status === entry.status 
            && t.statusTs === entry.statusTs;
      }));

      this.dataSource.data = data;
    });
  }

}

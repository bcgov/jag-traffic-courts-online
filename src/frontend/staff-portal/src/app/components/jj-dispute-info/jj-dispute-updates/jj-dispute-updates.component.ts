import { Component, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { DisputantUpdateRequestExpanded } from '@shared/models/disputant-update-request-expanded';
import { DisputeUpdateRequestUpdateType, Province } from 'app/api';
import { DisputantUpdateRequest, DisputeService } from 'app/services/dispute.service';
import { LookupsService } from 'app/services/lookups.service';

@Component({
  selector: 'app-jj-dispute-updates',
  templateUrl: './jj-dispute-updates.component.html',
  styleUrls: ['./jj-dispute-updates.component.scss'],
  standalone: false,
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
    "PostalCode":"Postal Code",
    "DriversLicenceNumber": "Drivers Licence Number",
    "DriversLicenceProvince": "Drivers Licence Province",
    "DriversLicenceIssuedCountry": "Drivers Licence Issued Country",
    "DriversLicenceIssuedProvince": "Drivers Licence Issued Province"
  };
  DISPUTANT_PHONE_FIELDS = {
    "HomePhoneNumber":"Phone Number"
  };
  DISPUTANT_NAME_FIELDS = {
    "DisputantGivenName1":"Disputant Given Name 1", 
    "DisputantGivenName2":"Disputant Given Name 2", 
    "DisputantGivenName3":"Disputant Given Name 3", 
    "DisputantSurname":"Surname",
    "ContactLawFirmName": "Contact Law Firm Name",
    "ContactGiven1Nm": "Contact Given Name 1",
    "ContactGiven2Nm": "Contact Given Name 2",
    "ContactGiven3Nm": "Contact Given Name 3",
    "ContactSurnameNm": "Contact Surname"
  };
  COUNT_FIELDS = {
    "PleaCode": "Plea Code",
    "CountNo": "Count No",
    "RequestTimeToPay": "Request Time To Pay",
    "RequestReduction": "Request Reduction"
  };
  COURT_OPTIONS_FIELDS = {
    "LawFirmName": "Law Firm Name",
    "LawyerSurname": "Lawyer Surname",
    "LawyerGivenName1": "Lawyer Given Name 1",
    "LawyerGivenName2": "Lawyer Given Name 2",
    "LawyerGivenName3": "Lawyer Given Name 3",
    "LawyerAddress": "Lawyer Address",
    "LawyerPhoneNumber": "Lawyer Phone Number",
    "LawyerEmail": "Lawyer Email",
    "InterpreterLanguageCd": "Interpreter Language",
    "InterpreterRequired": "Interpreter Required",
    "WitnessNo": "Witness No",
    "FineReductionReason": "Fine Reduction Reason",
    "TimeToPayReason": "Time To Pay Reason",
    "RequestCourtAppearance": "Request Court Appearance",
    "SignatoryName": "Signatory Name",
    "SignatoryType": "Signatory Type"
  };
  DISPUTANT_EMAIL_FIELDS = {
    "EmailAddress": "Email Address"
  }

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
        let dlCtryId = updateJson["DriversLicenceIssuedCountryId"];        
        let dlProvSeqNo = updateJson["DriversLicenceIssuedProvinceSeqNo"];
        let dlCountry = this.lookupsService.countries.find(p => p.ctryId == dlCtryId);
        let dlProvince = this.lookupsService.provinces.find(p => p.ctryId == dlCtryId && p.provSeqNo == dlProvSeqNo);        
        delete updateJson["AddressCountryId"];
        delete updateJson["AddressProvinceCountryId"];
        delete updateJson["AddressProvinceSeqNo"];
        delete updateJson["DriversLicenceIssuedCountryId"];
        delete updateJson["DriversLicenceIssuedProvinceSeqNo"];
        updateJson["AddressProvince"] = province ? province.provNm : "";
        updateJson["AddressCountry"] = country ? country.ctryLongNm : "";
        updateJson["DriversLicenceIssuedCountry"] = dlCountry ? dlCountry.ctryLongNm : "";
        updateJson["DriversLicenceIssuedProvince"] = dlProvince ? dlProvince.provNm : "";

        ctryId = currentJson["AddressCountryId"];
        provCtryId = currentJson["AddressProvinceCountryId"];
        provSeqNo = currentJson["AddressProvinceSeqNo"];
        province = this.lookupsService.provinces.find(p => p.ctryId == provCtryId && p.provSeqNo == provSeqNo);
        country = this.lookupsService.countries.find(p => p.ctryId == ctryId);
        dlCtryId = currentJson["DriversLicenceIssuedCountryId"];        
        dlProvSeqNo = currentJson["DriversLicenceIssuedProvinceSeqNo"];
        dlCountry = this.lookupsService.countries.find(p => p.ctryId == dlCtryId);
        dlProvince = this.lookupsService.provinces.find(p => p.ctryId == dlCtryId && p.provSeqNo == dlProvSeqNo);
        delete currentJson["AddressCountryId"];
        delete currentJson["AddressProvinceCountryId"];
        delete currentJson["AddressProvinceSeqNo"];
        delete currentJson["DriversLicenceIssuedCountryId"];
        delete currentJson["DriversLicenceIssuedProvinceSeqNo"];
        currentJson["AddressProvince"] = province ? province.provNm : "";
        currentJson["AddressCountry"] = country ? country.ctryLongNm : "";
        currentJson["DriversLicenceIssuedCountry"] = dlCountry ? dlCountry.ctryLongNm : "";
        currentJson["DriversLicenceIssuedProvince"] = dlProvince ? dlProvince.provNm : "";

        // Identify the specific fields based on the updateType
        let fields = {};
        if (request.updateType === DisputeUpdateRequestUpdateType.DisputantAddress) {
          fields = this.DISPUTANT_ADDRESS_FIELDS;
        } else if (request.updateType === DisputeUpdateRequestUpdateType.DisputantPhone) {
          fields = this.DISPUTANT_PHONE_FIELDS;
        } else if (request.updateType === DisputeUpdateRequestUpdateType.DisputantName) {
          fields = this.DISPUTANT_NAME_FIELDS;
        } else if (request.updateType == DisputeUpdateRequestUpdateType.DisputantEmail) {
          fields = this.DISPUTANT_EMAIL_FIELDS;
        } else if (request.updateType == DisputeUpdateRequestUpdateType.Count) {
          [0, 1, 2].forEach (i => {            
            Object.entries(this.COUNT_FIELDS).forEach(([fieldKey, fieldTitle]) => {
              const newValue = updateJson.DisputeCounts[i][fieldKey];
              const oldValue = currentJson.DisputeCounts[i][fieldKey];
              if (oldValue !== newValue) {
                data.push({
                  requestedTs: request.createdTs,
                  fieldTitle: `Count ${i + 1} ${fieldTitle}`,
                  oldValue: oldValue,
                  newValue: newValue,
                  status: request.status,
                  statusTs: request.statusUpdateTs
                });
              }
            });
          });
        } else if (request.updateType == DisputeUpdateRequestUpdateType.CourtOptions) {
          fields = this.COURT_OPTIONS_FIELDS;
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

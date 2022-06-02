import { LoggerService } from '@core/services/logger.service';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ViolationTicket, OcrViolationTicket, Field, ViolationTicketCount } from 'app/api';
import { DatePipe } from '@angular/common';

export interface IViolationTicketService {
  getAllOCRMessages(ocrViolationTicket: OcrViolationTicket);
  setViolationTicketFromJSON(ocrViolationTicket: OcrViolationTicket, violationTicket: ViolationTicket): ViolationTicket;
  getLegalParagraphing(violationTicketCount: ViolationTicketCount);
}

@Injectable({
  providedIn: 'root',
})
export class ViolationTicketService implements IViolationTicketService {
  public ocrMapping: mappingOCR[] = [
    { key: "ticket_number", heading: "Ticket Number" },
    { key: "violation_date", heading: "Date" },
    { key: "violation_time", heading: "Time" },
    { key: "surname", heading: "Surname" },
    { key: "given_names", heading: "Given name(s)" },
    { key: "drivers_licence_province", heading: "Prov/State of DL" },
    { key: "drivers_licence_number", heading: "DL Number" },
  ];
  public count1OcrMapping: mappingOCR[] = [
    { key: "counts.count_1.section", heading: "Act/Sect/Desc" },
    { key: "counts.count_1.ticketed_amount", heading: "Fine" }
  ];
  public count2OcrMapping: mappingOCR[] = [
    { key: "counts.count_2.section", heading: "Act/Sect/Desc" },
    { key: "counts.count_2.ticketed_amount", heading: "Fine" }
  ];
  public count3OcrMapping: mappingOCR[] = [
    { key: "counts.count_3.section", heading: "Act/Sect/Desc" },
    { key: "counts.count_3.ticketed_amount", heading: "Fine" },
  ];
  public ocrMessages: OCRMessageToDisplay[] = [];
  public count1OcrMessages: OCRMessageToDisplay[] = [];
  public count2OcrMessages: OCRMessageToDisplay[] = [];
  public count3OcrMessages: OCRMessageToDisplay[] = [];

  constructor(
    private datePipe: DatePipe,
    private logger: LoggerService,
  ) {
  }

  // act fullsection(section)(subsection)(paragraph)
  public getLegalParagraphing(violationTicketCount: ViolationTicketCount): string {
    if (!violationTicketCount || !violationTicketCount.description) return "";
    let ticketDesc = (violationTicketCount.actRegulation ? violationTicketCount.actRegulation : "") + " ";
    if (violationTicketCount.section && violationTicketCount.section.length > 0) ticketDesc = ticketDesc + violationTicketCount.section;
    if (violationTicketCount.subsection && violationTicketCount.subsection.length > 0) ticketDesc = ticketDesc + "(" + violationTicketCount.subsection + ")";
    if (violationTicketCount.paragraph && violationTicketCount.paragraph.length > 0) ticketDesc = ticketDesc + "(" + violationTicketCount.paragraph + ")";
    return ticketDesc;
  }

  // return all OCR messages for a ticket
  public getAllOCRMessages(ocrViolationTicket: OcrViolationTicket) {
    this.ocrMessages = [];

    // build list of messages
    this.ocrMapping.forEach(ocrField => {
      this.ocrMessages = this.ocrMessages.concat(this.getFieldOCRMessages(ocrViolationTicket.fields[ocrField.key], ocrField.key, this.ocrMapping));
    });

    // build list of count 1 messages
    this.count1OcrMapping.forEach(ocrField => {
      this.count1OcrMessages = this.count1OcrMessages.concat(this.getFieldOCRMessages(ocrViolationTicket.fields[ocrField.key], ocrField.key, this.count1OcrMapping));
    });

    // build list of count 2 messages
    this.count2OcrMapping.forEach(ocrField => {
      this.count2OcrMessages = this.count2OcrMessages.concat(this.getFieldOCRMessages(ocrViolationTicket.fields[ocrField.key], ocrField.key, this.count2OcrMapping));
    });

    // build list of count 3 messages
    this.count3OcrMapping.forEach(ocrField => {
      this.count3OcrMessages = this.count3OcrMessages.concat(this.getFieldOCRMessages(ocrViolationTicket.fields[ocrField.key], ocrField.key, this.count3OcrMapping));
    });
  }

  // return OCR error for a single field
  private getFieldOCRMessages(field: Field, key: string, ocrMapping: mappingOCR[]): OCRMessageToDisplay {
    // set heading message
    let heading = ocrMapping.filter(x => x.key == key)[0].heading;
    if (!heading) heading = key;
    return ({ heading: heading, key: key, fieldConfidence: field.fieldConfidence });
  }

  public setViolationTicketFromJSON(ocrViolationTicket: OcrViolationTicket, violationTicket: ViolationTicket): ViolationTicket {

    if (ocrViolationTicket) {

      if (!violationTicket.ticketNumber) violationTicket.ticketNumber = ocrViolationTicket.fields["ticket_number"].value;
      if (!violationTicket.surname) violationTicket.surname = ocrViolationTicket.fields["surname"].value;
      if (!violationTicket.givenNames) violationTicket.givenNames = ocrViolationTicket.fields["given_names"].value;
      if (!violationTicket.driversLicenceProvince) violationTicket.driversLicenceProvince = ocrViolationTicket.fields["drivers_licence_province"].value;
      if (!violationTicket.driversLicenceNumber) violationTicket.driversLicenceNumber = ocrViolationTicket.fields["drivers_licence_number"].value;
      if (!violationTicket.isMvaOffence) violationTicket.isMvaOffence = ocrViolationTicket.fields["is_mva_offence"].value == "selected" ? true : false;
      if (!violationTicket.isLcaOffence) violationTicket.isLcaOffence = ocrViolationTicket.fields["is_lca_offence"].value == "selected" ? true : false;
      if (!violationTicket.isMcaOffence) violationTicket.isMcaOffence = ocrViolationTicket.fields["is_mca_offence"].value == "selected" ? true : false;
      if (!violationTicket.isFaaOffence) violationTicket.isFaaOffence = ocrViolationTicket.fields["is_faa_offence"].value == "selected" ? true : false;
      if (!violationTicket.isTcrOffence) violationTicket.isTcrOffence = ocrViolationTicket.fields["is_tcr_offence"].value == "selected" ? true : false;
      if (!violationTicket.isCtaOffence) violationTicket.isCtaOffence = ocrViolationTicket.fields["is_cta_offence"].value == "selected" ? true : false;
      if (!violationTicket.isWlaOffence) violationTicket.isWlaOffence = ocrViolationTicket.fields["is_wla_offence"].value == "selected" ? true : false;
      if (!violationTicket.isOtherOffence) violationTicket.isOtherOffence = ocrViolationTicket.fields["is_other_offence"].value == "selected" ? true : false;;
      if (!violationTicket.organizationLocation) violationTicket.organizationLocation = ocrViolationTicket.fields["organization_location"].value;

      // set up ticket count 1
      if (ocrViolationTicket.fields["counts.count_1.description"]) {
        const foundViolationTicketCount1 = violationTicket.violationTicketCounts.filter(x => x.count == 1);
        if (foundViolationTicketCount1.length > 0) {
          if (!foundViolationTicketCount1[0].description) foundViolationTicketCount1[0].description = ocrViolationTicket.fields["counts.count_1.description"].value;
          if (!foundViolationTicketCount1[0].actRegulation) foundViolationTicketCount1[0].actRegulation = ocrViolationTicket.fields["counts.count_1.act_or_regulation"].value;
          if (!foundViolationTicketCount1[0].section) foundViolationTicketCount1[0].section = ocrViolationTicket.fields["counts.count_1.section"].value;
          if (!foundViolationTicketCount1[0].ticketedAmount) foundViolationTicketCount1[0].ticketedAmount = +ocrViolationTicket.fields["counts.count_1.ticketed_amount"].value?.substring(1);
          if (!foundViolationTicketCount1[0].isAct) foundViolationTicketCount1[0].isAct = ocrViolationTicket.fields["counts.count_1.is_act"].value == "selected" ? true : false;
          if (!foundViolationTicketCount1[0].isRegulation) foundViolationTicketCount1[0].isRegulation = ocrViolationTicket.fields["counts.count_1.is_regulation"].value == "selected" ? true : false;
        } else {
          let violationTicketCount = {
            count: 1,
            description: ocrViolationTicket.fields["counts.count_1.description"].value,
            actRegulation: ocrViolationTicket.fields["counts.count_1.act_or_regulation"].value,
            section: ocrViolationTicket.fields["counts.count_1.section"].value,
            ticketedAmount: ocrViolationTicket.fields["counts.count_1.ticketed_amount"].value ? +ocrViolationTicket.fields["counts.count_1.ticketed_amount"].value?.substring(1) : undefined,
            isAct: ocrViolationTicket.fields["counts.count_1.is_act"].value == "selected" ? true : false,
            isRegulation: ocrViolationTicket.fields["counts.count_1.is_regulation"].value == "selected" ? true : false
          } as ViolationTicketCount;
          violationTicket.violationTicketCounts = violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      }

      // // set up ticket count 2
      if (ocrViolationTicket.fields["counts.count_2.description"]) {
        const foundViolationTicketCount2 = violationTicket.violationTicketCounts.filter(x => x.count == 2);
        if (foundViolationTicketCount2.length > 0) {
          if (!foundViolationTicketCount2[0].description) foundViolationTicketCount2[0].description = ocrViolationTicket.fields["counts.count_2.description"].value;
          if (!foundViolationTicketCount2[0].actRegulation) foundViolationTicketCount2[0].actRegulation = ocrViolationTicket.fields["counts.count_2.act_or_regulation"].value;
          if (!foundViolationTicketCount2[0].section) foundViolationTicketCount2[0].section = ocrViolationTicket.fields["counts.count_2.section"].value;
          if (!foundViolationTicketCount2[0].ticketedAmount) foundViolationTicketCount2[0].ticketedAmount = +ocrViolationTicket.fields["counts.count_2.ticketed_amount"].value?.substring(1);
          if (!foundViolationTicketCount2[0].isAct) foundViolationTicketCount2[0].isAct = ocrViolationTicket.fields["counts.count_2.is_act"].value == "selected" ? true : false;
          if (!foundViolationTicketCount2[0].isRegulation) foundViolationTicketCount2[0].isRegulation = ocrViolationTicket.fields["counts.count_2.is_regulation"].value == "selected" ? true : false;
        } else {
          let violationTicketCount = {
            count: 2,
            description: ocrViolationTicket.fields["counts.count_2.description"].value,
            actRegulation: ocrViolationTicket.fields["counts.count_2.act_or_regulation"].value,
            section: ocrViolationTicket.fields["counts.count_2.section"].value,
            ticketedAmount: ocrViolationTicket.fields["counts.count_2.ticketed_amount"].value ? +ocrViolationTicket.fields["counts.count_2.ticketed_amount"].value?.substring(1) : undefined,
            isAct: ocrViolationTicket.fields["counts.count_2.is_act"].value == "selected" ? true : false,
            isRegulation: ocrViolationTicket.fields["counts.count_2.is_regulation"].value == "selected" ? true : false
          } as ViolationTicketCount;
          violationTicket.violationTicketCounts = violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      }

      // // set up ticket count 3
      if (ocrViolationTicket.fields["counts.count_3.description"]) {
        const foundViolationTicketCount3 = violationTicket.violationTicketCounts.filter(x => x.count == 3);
        if (foundViolationTicketCount3.length > 0) {
          if (!foundViolationTicketCount3[0].description) foundViolationTicketCount3[0].description = ocrViolationTicket.fields["counts.count_3.description"].value;
          if (!foundViolationTicketCount3[0].actRegulation) foundViolationTicketCount3[0].actRegulation = ocrViolationTicket.fields["counts.count_3.act_or_regulation"].value;
          if (!foundViolationTicketCount3[0].section) foundViolationTicketCount3[0].section = ocrViolationTicket.fields["counts.count_3.section"].value;
          if (!foundViolationTicketCount3[0].ticketedAmount) foundViolationTicketCount3[0].ticketedAmount = +ocrViolationTicket.fields["counts.count_3.ticketed_amount"].value?.substring(1);
          if (!foundViolationTicketCount3[0].isAct) foundViolationTicketCount3[0].isAct = ocrViolationTicket.fields["counts.count_3.is_act"].value == "selected" ? true : false;
          if (!foundViolationTicketCount3[0].isRegulation) foundViolationTicketCount3[0].isRegulation = ocrViolationTicket.fields["counts.count_3.is_regulation"].value == "selected" ? true : false;
        } else {
          let violationTicketCount = {
            count: 3,
            description: ocrViolationTicket.fields["counts.count_3.description"].value,
            actRegulation: ocrViolationTicket.fields["counts.count_3.act_or_regulation"].value,
            section: ocrViolationTicket.fields["counts.count_3.section"].value,
            ticketedAmount: ocrViolationTicket.fields["counts.count_3.ticketed_amount"].value ? +ocrViolationTicket.fields["counts.count_3.ticketed_amount"].value?.substring(1) : undefined,
            isAct: ocrViolationTicket.fields["counts.count_3.is_act"].value == "selected" ? true : false,
            isRegulation: ocrViolationTicket.fields["counts.count_3.is_regulation"].value == "selected" ? true : false
          } as ViolationTicketCount;
          violationTicket.violationTicketCounts = violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      }
    }

    return violationTicket;
  }

  // Copied from citizen portal
  // TODO : use this in function - setViolationTicketFromJSON
  private getValue(field: Field): any {
    let result;
    let value = field.value;
    if (field.type && value && value.length > 0) {
      switch (field.type.toLowerCase()) {
        case "double":
          result = parseFloat(value.replace(/[^.0-9]/g, "")); // regex replace characters other than numbers
          break;
        case "int64":
          result = parseInt(value.replace(/[^.0-9]/g, "")); // regex replace characters other than numbers
          break;
        case "selectionmark":
          result = value.toLowerCase() === "selected" ? true : false;
          break;
        case "time":
          result = value.replace(" ", ":");
          break;
        case "date":
        case "string":
        default:
          result = value;
          break;
      }
    }
    return result;
  }
}

export interface mappingOCR {
  key: string;
  heading: string;
}

export interface OCRMessageToDisplay {
  heading: string;
  key: string;
  fieldConfidence: number;
}
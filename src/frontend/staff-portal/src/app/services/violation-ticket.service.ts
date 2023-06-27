import { LoggerService } from '@core/services/logger.service';
import { Injectable } from '@angular/core';
import { ViolationTicket, OcrViolationTicket, Field, ViolationTicketCount, ViolationTicketCountIsAct, ViolationTicketCountIsRegulation } from 'app/api';
import { ConfigService } from '@config/config.service';

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
    { key: "disputant_surname", heading: "Surname" },
    { key: "disputant_given_names", heading: "Given name(s)" },
    { key: "drivers_licence_province", heading: "Prov/State of DL" },
    { key: "drivers_licence_number", heading: "DL Number" },
  ];
  public count1OcrMapping: mappingOCR[] = [
    { key: "counts.count_no_1.section", heading: "Act/Sect/Desc" },
    { key: "counts.count_no_1.ticketed_amount", heading: "Fine" }
  ];
  public count2OcrMapping: mappingOCR[] = [
    { key: "counts.count_no_2.section", heading: "Act/Sect/Desc" },
    { key: "counts.count_no_2.ticketed_amount", heading: "Fine" }
  ];
  public count3OcrMapping: mappingOCR[] = [
    { key: "counts.count_no_3.section", heading: "Act/Sect/Desc" },
    { key: "counts.count_no_3.ticketed_amount", heading: "Fine" },
  ];
  public ocrMessages: OCRMessageToDisplay[] = [];
  public count1OcrMessages: OCRMessageToDisplay[] = [];
  public count2OcrMessages: OCRMessageToDisplay[] = [];
  public count3OcrMessages: OCRMessageToDisplay[] = []
  public IsAct = ViolationTicketCountIsAct;
  public IsRegulation = ViolationTicketCountIsRegulation;

  constructor(
    private logger: LoggerService,
    private config: ConfigService
  ) {
  }

  // act section(subsection)(paragraph)(subparagraph)
  public getLegalParagraphing(violationTicketCount: ViolationTicketCount): string {
    if (!violationTicketCount) return "";
    let ticketDesc = (violationTicketCount.actOrRegulationNameCode ? violationTicketCount.actOrRegulationNameCode : "") + " ";
    if (violationTicketCount.section && violationTicketCount.section.length > 0) ticketDesc = ticketDesc + violationTicketCount.section;
    if (violationTicketCount.subsection && violationTicketCount.subsection.length > 0) ticketDesc = ticketDesc + "(" + violationTicketCount.subsection + ")";
    if (violationTicketCount.paragraph && violationTicketCount.paragraph.length > 0) ticketDesc = ticketDesc + "(" + violationTicketCount.paragraph + ")";
    if (violationTicketCount.subparagraph && violationTicketCount.subparagraph.length > 0) ticketDesc = ticketDesc + "(" + violationTicketCount.subparagraph + ")";
    return ticketDesc;
  }

  // return all OCR messages for a ticket
  public getAllOCRMessages(ocrViolationTicket: OcrViolationTicket) {
    // build list of messages
    this.ocrMessages = this.ocrMapping.map(ocrField =>  this.getFieldOCRMessages(ocrViolationTicket?.fields[ocrField.key], ocrField.key, this.ocrMapping));

    // build list of count 1 messages
    this.count1OcrMessages = this.count1OcrMapping.map(ocrField =>  this.getFieldOCRMessages(ocrViolationTicket?.fields[ocrField.key], ocrField.key, this.count1OcrMapping));

    // build list of count 2 messages
    this.count2OcrMessages = this.count2OcrMapping.map(ocrField =>  this.getFieldOCRMessages(ocrViolationTicket?.fields[ocrField.key], ocrField.key, this.count2OcrMapping));

    // build list of count 3 messages
    this.count3OcrMessages = this.count3OcrMapping.map(ocrField =>  this.getFieldOCRMessages(ocrViolationTicket?.fields[ocrField.key], ocrField.key, this.count3OcrMapping));
  }

  // return OCR error for a single field
  private getFieldOCRMessages(field: Field, key: string, ocrMapping: mappingOCR[]): OCRMessageToDisplay {
    // set heading message
    let heading = ocrMapping?.filter(x => x.key == key).shift().heading;
    if (!heading) heading = key;
    return ({ heading: heading, key: key, fieldConfidence: field?.fieldConfidence });
  }

  public setViolationTicketFromJSON(ocrViolationTicket: OcrViolationTicket, violationTicket: ViolationTicket): ViolationTicket {
    // when violation ticket record is initially created it has three counts
    // so there must always be three violation ticket count records sent through update or update will fail
    if (ocrViolationTicket) {
      if (!violationTicket.ticketNumber) violationTicket.ticketNumber = ocrViolationTicket.fields["ticket_number"].value;
      if (!violationTicket.disputantSurname) violationTicket.disputantSurname = ocrViolationTicket.fields["disputant_surname"].value;
      if (!violationTicket.disputantGivenNames) violationTicket.disputantGivenNames = ocrViolationTicket.fields["disputant_given_names"].value;
      if (!violationTicket.driversLicenceProvince) {
        violationTicket.driversLicenceProvince = ocrViolationTicket.fields["drivers_licence_province"].value;
        let provFound = this.config.provincesAndStates.filter(x => x.provNm === violationTicket.driversLicenceProvince).shift();
        if (provFound) {
          let ctryFound = this.config.countries.filter(x => x.ctryId === provFound.ctryId).shift();
          if (ctryFound) violationTicket.driversLicenceCountry = ctryFound.ctryLongNm;
        }
      }
      if (!violationTicket.disputantDriversLicenceNumber) violationTicket.disputantDriversLicenceNumber = ocrViolationTicket.fields["drivers_licence_number"].value;
      if (!violationTicket.detachmentLocation) violationTicket.detachmentLocation = ocrViolationTicket.fields["detachment_location"].value;
      if (!violationTicket.courtLocation) violationTicket.courtLocation = ocrViolationTicket.fields["court_location"].value;

      // set up ticket count 1
      if (ocrViolationTicket.fields["counts.count_no_1.description"]) {
        const foundViolationTicketCount1 : ViolationTicketCount[] = violationTicket.violationTicketCounts.filter(x => x.countNo == 1);
        if (foundViolationTicketCount1.length > 0) {
          if (!foundViolationTicketCount1[0].description) foundViolationTicketCount1[0].description = ocrViolationTicket.fields["counts.count_no_1.description"].value;
          if (!foundViolationTicketCount1[0].actOrRegulationNameCode) foundViolationTicketCount1[0].actOrRegulationNameCode = ocrViolationTicket.fields["counts.count_no_1.act_or_regulation_name_code"].value;
          if (!foundViolationTicketCount1[0].section) foundViolationTicketCount1[0].section = ocrViolationTicket.fields["counts.count_no_1.section"].value;
          if (!foundViolationTicketCount1[0].ticketedAmount) foundViolationTicketCount1[0].ticketedAmount = +ocrViolationTicket.fields["counts.count_no_1.ticketed_amount"].value;
          if (!foundViolationTicketCount1[0].isAct) foundViolationTicketCount1[0].isAct = ocrViolationTicket.fields["counts.count_no_1.is_act"].value == "selected" ? this.IsAct.Y : this.IsAct.N;
          if (!foundViolationTicketCount1[0].isRegulation) foundViolationTicketCount1[0].isRegulation = ocrViolationTicket.fields["counts.count_no_1.is_regulation"].value == "selected" ? this.IsRegulation.Y : this.IsRegulation.N;
        } else {
          let violationTicketCount = {
            countNo: 1,
            description: ocrViolationTicket.fields["counts.count_no_1.description"].value,
            actOrRegulationNameCode: ocrViolationTicket.fields["counts.count_no_1.act_or_regulation_name_code"].value,
            section: ocrViolationTicket.fields["counts.count_no_1.section"].value,
            ticketedAmount: ocrViolationTicket.fields["counts.count_no_1.ticketed_amount"].value ? +ocrViolationTicket.fields["counts.count_no_1.ticketed_amount"].value : undefined,
            isAct: ocrViolationTicket.fields["counts.count_no_1.is_act"].value == "selected" ? this.IsAct.Y : this.IsAct.N,
            isRegulation: ocrViolationTicket.fields["counts.count_no_1.is_regulation"].value == "selected" ? this.IsRegulation.Y : this.IsRegulation.N
          } as ViolationTicketCount;
          violationTicket.violationTicketCounts = violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      }

      // // set up ticket count 2
      if (ocrViolationTicket.fields["counts.count_no_2.description"]) {
        const foundViolationTicketCount2 = violationTicket.violationTicketCounts.filter(x => x.countNo == 2);
        if (foundViolationTicketCount2.length > 0) {
          if (!foundViolationTicketCount2[0].description) foundViolationTicketCount2[0].description = ocrViolationTicket.fields["counts.count_no_2.description"].value;
          if (!foundViolationTicketCount2[0].actOrRegulationNameCode) foundViolationTicketCount2[0].actOrRegulationNameCode = ocrViolationTicket.fields["counts.count_no_2.act_or_regulation_name_code"].value;
          if (!foundViolationTicketCount2[0].section) foundViolationTicketCount2[0].section = ocrViolationTicket.fields["counts.count_no_2.section"].value;
          if (!foundViolationTicketCount2[0].ticketedAmount) foundViolationTicketCount2[0].ticketedAmount = +ocrViolationTicket.fields["counts.count_no_2.ticketed_amount"].value;
          if (!foundViolationTicketCount2[0].isAct) foundViolationTicketCount2[0].isAct = ocrViolationTicket.fields["counts.count_no_2.is_act"].value == "selected" ? this.IsAct.Y : this.IsAct.N;
          if (!foundViolationTicketCount2[0].isRegulation) foundViolationTicketCount2[0].isRegulation = ocrViolationTicket.fields["counts.count_no_2.is_regulation"].value == "selected" ? this.IsRegulation.Y : this.IsRegulation.N;
        } else {
          let violationTicketCount = {
            countNo: 2,
            description: ocrViolationTicket.fields["counts.count_no_2.description"].value,
            actOrRegulationNameCode: ocrViolationTicket.fields["counts.count_no_2.act_or_regulation_name_code"].value,
            section: ocrViolationTicket.fields["counts.count_no_2.section"].value,
            ticketedAmount: ocrViolationTicket.fields["counts.count_no_2.ticketed_amount"].value ? +ocrViolationTicket.fields["counts.count_no_2.ticketed_amount"].value : undefined,
            isAct: ocrViolationTicket.fields["counts.count_no_2.is_act"].value == "selected" ? this.IsAct.Y : this.IsAct.N,
            isRegulation: ocrViolationTicket.fields["counts.count_no_2.is_regulation"].value == "selected" ? this.IsRegulation.Y : this.IsRegulation.N
          } as ViolationTicketCount;
          violationTicket.violationTicketCounts = violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      } else {
        let violationTicketCount = {
          countNo: 2,
            description: null,
            actOrRegulationNameCode: null,
            section: null,
            ticketedAmount: 0,
            isAct: this.IsAct.N,
            isRegulation: this.IsRegulation.N
        }
        violationTicket.violationTicketCounts = violationTicket.violationTicketCounts.concat(violationTicketCount);
      }

      // // set up ticket count 3
      if (ocrViolationTicket.fields["counts.count_no_3.description"]) {
        const foundViolationTicketCount3 = violationTicket.violationTicketCounts.filter(x => x.countNo == 3);
        if (foundViolationTicketCount3.length > 0) {
          if (!foundViolationTicketCount3[0].description) foundViolationTicketCount3[0].description = ocrViolationTicket.fields["counts.count_no_3.description"].value;
          if (!foundViolationTicketCount3[0].actOrRegulationNameCode) foundViolationTicketCount3[0].actOrRegulationNameCode = ocrViolationTicket.fields["counts.count_no_3.act_or_regulation_name_code"].value;
          if (!foundViolationTicketCount3[0].section) foundViolationTicketCount3[0].section = ocrViolationTicket.fields["counts.count_no_3.section"].value;
          if (!foundViolationTicketCount3[0].ticketedAmount) foundViolationTicketCount3[0].ticketedAmount = +ocrViolationTicket.fields["counts.count_no_3.ticketed_amount"].value;
          if (!foundViolationTicketCount3[0].isAct) foundViolationTicketCount3[0].isAct = ocrViolationTicket.fields["counts.count_no_3.is_act"].value == "selected" ? this.IsAct.Y : this.IsAct.N;
          if (!foundViolationTicketCount3[0].isRegulation) foundViolationTicketCount3[0].isRegulation = ocrViolationTicket.fields["counts.count_no_3.is_regulation"].value == "selected" ? this.IsRegulation.Y : this.IsRegulation.N;
        } else {
          let violationTicketCount = {
            countNo: 3,
            description: ocrViolationTicket.fields["counts.count_no_3.description"].value,
            actOrRegulationNameCode: ocrViolationTicket.fields["counts.count_no_3.act_or_regulation_name_code"].value,
            section: ocrViolationTicket.fields["counts.count_no_3.section"].value,
            ticketedAmount: ocrViolationTicket.fields["counts.count_no_3.ticketed_amount"].value ? +ocrViolationTicket.fields["counts.count_no_3.ticketed_amount"].value : undefined,
            isAct: ocrViolationTicket.fields["counts.count_no_3.is_act"].value == "selected" ? this.IsAct.Y : this.IsAct.N,
            isRegulation: ocrViolationTicket.fields["counts.count_no_3.is_regulation"].value == "selected" ? this.IsAct.Y : this.IsAct.N
          } as ViolationTicketCount;
          violationTicket.violationTicketCounts = violationTicket.violationTicketCounts.concat(violationTicketCount);
        }
      } else {
        let violationTicketCount = {
          countNo: 3,
            description: null,
            actOrRegulationNameCode: null,
            section: null,
            ticketedAmount: 0,
            isAct: this.IsAct.N,
            isRegulation: this.IsRegulation.N
        }
        violationTicket.violationTicketCounts = violationTicket.violationTicketCounts.concat(violationTicketCount);
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

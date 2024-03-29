/**
 * VTC Staff API
 * Violation Ticket Centre Staff API
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */
import { ViolationTicketCountIsAct } from './violationTicketCountIsAct.model';
import { ViolationTicketCountIsRegulation } from './violationTicketCountIsRegulation.model';


export interface ViolationTicketCount { 
    createdBy?: string | null;
    createdTs?: string;
    modifiedBy?: string | null;
    modifiedTs?: string | null;
    violationTicketCountId?: number;
    countNo?: number;
    description?: string | null;
    actOrRegulationNameCode?: string | null;
    isAct?: ViolationTicketCountIsAct;
    isRegulation?: ViolationTicketCountIsRegulation;
    section?: string | null;
    subsection?: string | null;
    paragraph?: string | null;
    subparagraph?: string | null;
    ticketedAmount?: number | null;
    additionalProperties?: { [key: string]: any; } | null;
}


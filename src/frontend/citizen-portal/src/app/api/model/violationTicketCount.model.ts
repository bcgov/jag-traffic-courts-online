/**
 * Traffic Court Online Citizen Api
 * An API for creating violation ticket disputes
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


/**
 * Represents a violation ticket count.
 */
export interface ViolationTicketCount { 
    /**
     * The count number. Must be unique within an individual violation ticket.
     */
    count_no?: number;
    /**
     * The description of the offence.
     */
    description?: string | null;
    /**
     * The act or regulation code the violation occurred against. For example, MVA, WLA, TCR, etc
     */
    act_or_regulation_name_code?: string | null;
    /**
     * The full section designation of the act or regulation. For example, \"147(1)\" which means \"Speed in school zone\"
     */
    full_section?: string | null;
    /**
     * The ticketed amount.
     */
    ticketed_amount?: number | null;
    is_act?: ViolationTicketCountIsAct;
    is_regulation?: ViolationTicketCountIsRegulation;
}


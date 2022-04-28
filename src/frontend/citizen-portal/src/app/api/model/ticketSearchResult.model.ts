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
import { TicketOffence } from './ticketOffence.model';


/**
 * Obsolete: Represents a violation ticket that is returned from search requests
 */
export interface TicketSearchResult { 
    /**
     * The violation ticket number. This will match the ticket number searched for.
     */
    violationTicketNumber?: string | null;
    /**
     * The date the violation ticket was issued.
     */
    violationDate?: string;
    /**
     * The time of day the violation ticket was issued. This will match the time searched for.
     */
    violationTime?: string | null;
    /**
     * The list of offences on this violation ticket.
     */
    offences?: Array<TicketOffence> | null;
}


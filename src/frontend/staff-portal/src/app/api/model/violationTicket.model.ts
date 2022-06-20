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
import { OcrViolationTicket } from './ocrViolationTicket.model';
import { ViolationTicketImage } from './violationTicketImage.model';
import { ViolationTicketCount } from './violationTicketCount.model';


export interface ViolationTicket { 
    violationTicketImage?: ViolationTicketImage;
    ocrViolationTicket?: OcrViolationTicket;
    createdBy?: string | null;
    createdTs?: string;
    modifiedBy?: string | null;
    modifiedTs?: string;
    id?: number;
    ticketNumber?: string | null;
    surname?: string | null;
    givenNames?: string | null;
    isYoungPerson?: boolean | null;
    driversLicenceNumber?: string | null;
    driversLicenceProvince?: string | null;
    driversLicenceProducedYear?: number | null;
    driversLicenceExpiryYear?: number | null;
    birthdate?: string | null;
    address?: string | null;
    city?: string | null;
    province?: string | null;
    postalCode?: string | null;
    isChangeOfAddress?: boolean | null;
    isDriver?: boolean | null;
    isCyclist?: boolean | null;
    isOwner?: boolean | null;
    isPedestrian?: boolean | null;
    isPassenger?: boolean | null;
    isOther?: boolean | null;
    otherDescription?: string | null;
    issuedDate?: string | null;
    issuedOnRoadOrHighway?: string | null;
    issuedAtOrNearCity?: string | null;
    isMvaOffence?: boolean | null;
    isWlaOffence?: boolean | null;
    isLcaOffence?: boolean | null;
    isMcaOffence?: boolean | null;
    isFaaOffence?: boolean | null;
    isTcrOffence?: boolean | null;
    isCtaOffence?: boolean | null;
    isOtherOffence?: boolean | null;
    otherOffenceDescription?: string | null;
    organizationLocation?: string | null;
    violationTicketCounts?: Array<ViolationTicketCount> | null;
    additionalProperties?: { [key: string]: any; } | null;
}


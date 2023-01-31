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
import { DisputeRepresentedByLawyer } from './disputeRepresentedByLawyer.model';
import { DisputeInterpreterRequired } from './disputeInterpreterRequired.model';
import { DisputeCount } from './disputeCount.model';


/**
 * Represents a violation ticket notice of dispute.
 */
export interface Dispute { 
    /**
     * The disputant\'s email address.
     */
    email_address?: string | null;
    /**
     * The first given name or corporate name continued.
     */
    disputant_given_name1?: string | null;
    /**
     * The second given name
     */
    disputant_given_name2?: string | null;
    /**
     * The third given name
     */
    disputant_given_name3?: string | null;
    /**
     * The surname or corporate name.
     */
    disputant_surname?: string | null;
    /**
     * The mailing address of the disputant.
     */
    address_line1?: string | null;
    /**
     * The mailing address of the disputant.
     */
    address_line2?: string | null;
    /**
     * The mailing address of the disputant.
     */
    address_line3?: string | null;
    /**
     * The mailing address city of the disputant.
     */
    address_city?: string | null;
    /**
     * The mailing address province of the disputant.
     */
    address_province?: string | null;
    /**
     * The mailing address province\'s country code of the disputant.
     */
    address_province_country_id?: number | null;
    /**
     * The mailing address province\'s sequence number of the disputant.
     */
    address_province_seq_no?: number | null;
    /**
     * The mailing address country id of the disputant.
     */
    address_country_id?: number | null;
    /**
     * The mailing address postal code or zip code of the disputant.
     */
    postal_code?: string | null;
    /**
     * The disputant\'s home phone number.
     */
    home_phone_number?: string | null;
    /**
     * The violation ticket number.
     */
    ticket_number?: string | null;
    /**
     * The date and time the violation ticket was issue. Time must only be hours and minutes.
     */
    issued_date?: string;
    /**
     * The disputant\'s birthdate.
     */
    disputant_birthdate?: string;
    /**
     * The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
     */
    drivers_licence_number?: string | null;
    /**
     * The province or state the drivers licence was issued by.
     */
    drivers_licence_province?: string | null;
    /**
     * The province sequence number of the drivers licence was issued by.
     */
    drivers_licence_province_seq_no?: number | null;
    /**
     * The country code of the drivers licence was issued by.
     */
    drivers_licence_country_id?: number | null;
    /**
     * The disputant\'s work phone number.
     */
    work_phone_number?: string | null;
    represented_by_lawyer?: DisputeRepresentedByLawyer;
    /**
     * Name of the law firm that will represent the disputant at the hearing.
     */
    law_firm_name?: string | null;
    /**
     * Surname of the lawyer who will represent the disputant at the hearing.
     */
    lawyer_surname?: string | null;
    /**
     * Given Name 1 of the lawyer who will represent the disputant at the hearing.
     */
    lawyer_given_name1?: string | null;
    /**
     * Given Name 2 of the lawyer who will represent the disputant at the hearing.
     */
    lawyer_given_name2?: string | null;
    /**
     * Given Name 3 of the lawyer who will represent the disputant at the hearing.
     */
    lawyer_given_name3?: string | null;
    /**
     * Email address of the lawyer who will represent the disputant at the hearing.
     */
    lawyer_email?: string | null;
    /**
     * Address of the lawyer who will represent the disputant at the hearing.
     */
    lawyer_address?: string | null;
    /**
     * Address of the lawyer who will represent the disputant at the hearing.
     */
    lawyer_phone_number?: string | null;
    /**
     * The disputant requires spoken language interpreter. The language name is indicated in this field.
     */
    interpreter_language_cd?: string | null;
    interprer_required?: DisputeInterpreterRequired;
    /**
     * The number of witnesses that the disputant intends to call.
     */
    witness_no?: number;
    /**
     * The reason that disputant declares for requesting a fine reduction.
     */
    fine_reduction_reason?: string | null;
    /**
     * The reason that disputant declares for requesting more time to pay the amount on the violation ticket.
     */
    time_to_pay_reason?: string | null;
    /**
     * Dispute Counts
     */
    dispute_counts?: Array<DisputeCount> | null;
}

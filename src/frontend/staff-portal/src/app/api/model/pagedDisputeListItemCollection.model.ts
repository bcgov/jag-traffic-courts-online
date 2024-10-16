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
import { DisputeListItem } from './disputeListItem.model';


export interface PagedDisputeListItemCollection { 
    items?: Array<DisputeListItem> | null;
    readonly pageNumber?: number;
    readonly pageSize?: number;
    readonly pageCount?: number;
    readonly totalItemCount?: number;
    readonly hasPreviousPage?: boolean;
    readonly hasNextPage?: boolean;
    readonly isFirstPage?: boolean;
    readonly isLastPage?: boolean;
}


import { Pipe, PipeTransform } from '@angular/core';
import { DisputeDisputeSourceType } from 'app/api';

/**
 * The database user that citizen-portal disputes were created with. Only used as a fallback when
 * the API response has no dispute source (for example an API deployed before TCVP-3480).
 */
const LEGACY_CITIZEN_USER = 'OCCAMORDS';

/**
 * True if the dispute was submitted through the Citizen Portal.
 * @param sourceType DisputeSourceType from the Dispute or JJDispute (CITZ, STAF, JSTN).
 * @param createdBy The user that created the dispute (Dispute.createdBy or JJDispute.occamDisputeCreatedBy).
 */
export function isCitizenSubmitted(sourceType: string | null | undefined, createdBy: string | null | undefined): boolean {
  if (sourceType && sourceType !== DisputeDisputeSourceType.Unknown) {
    return sourceType === DisputeDisputeSourceType.Citz;
  }
  return createdBy === LEGACY_CITIZEN_USER;
}

/**
 * Displays who submitted a dispute, e.g. "Disputant", "STAFF (DAVEKAY)" or "JUSTIN".
 *
 * Usage: {{ dispute.disputeSourceType | disputeSource: dispute.createdBy }}
 */
@Pipe({
  name: 'disputeSource',
  standalone: false,
})
export class DisputeSourcePipe implements PipeTransform {
  transform(sourceType: string | null | undefined, createdBy?: string | null): string {
    switch (sourceType) {
      case DisputeDisputeSourceType.Citz:
        return 'Disputant';
      case DisputeDisputeSourceType.Staf:
        return createdBy ? `STAFF (${createdBy})` : 'STAFF';
      case DisputeDisputeSourceType.Jstn:
        return 'JUSTIN';
      default:
        // No source in the response; fall back to the old created-by check
        return isCitizenSubmitted(sourceType, createdBy) ? 'Disputant' : (createdBy ?? '');
    }
  }
}

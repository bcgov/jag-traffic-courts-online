export interface Offence {
  id?: string;
  offenceNumber: number;
  invoiceType?: string;
  offenceDescription?: string;
  violationDateTime?: string;
  vehicleDescription?: string;

  ticketedAmount?: number;
  amountDue?: number;
  discountAmount?: number;

  status: number;
  offenceAgreementStatus?: string;
  reductionAppearInCourt: boolean;
  requestReduction: boolean;
  requestMoreTime: boolean;
  reductionReason?: string;
  moreTimeReason?: string;

  /*
  status (for disputes):
    0 - New,
    1 - Submitted,
    2 - InProgress, // ticket already verified
    3 - Complete,
    4 - Rejected

  offenceAgreementStatus:
    NOTHING
                    I do not wish to take any action on this count at this time.

    PAY
                     I agree I committed this offence and I would like to pay for this count.

    REDUCTION
                    I agree I committed this offence and I would like to request a
                    fine reduction and/or more time to pay for this count.

    DISPUTE
                    I do not agree that I committed this offence and I would like to
                    dispute this count.

  */

  // derived later on
  offenceStatusDesc?: string;
  // offenceAgreementStatusDesc?: string;
}

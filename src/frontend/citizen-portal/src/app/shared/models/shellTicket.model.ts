export interface ShellTicket {
  violationTicketNumber: string;
  violationTime: string;
  violationDate: string;

  lastName: string;
  givenNames: string;
  birthdate: string;
  gender: string;
  address: string;
  city: string;
  province: string;
  postalCode: string;
  driverLicenseNumber: string;
  driverLicenseProvince: string;

  courtHearingLocation: string;
  detachmentLocation: string;

  count1Charge: number;
  count1FineAmount: string;
  count2Charge: number;
  count2FineAmount: string;
  count3Charge: number;
  count3FineAmount: string;

  photo?: string;

  _chargeCount: number;
  _amountOwing: number;
  _count1ChargeDesc: string;
  _count2ChargeDesc: string;
  _count3ChargeDesc: string;
  _count1ChargeSection: string;
  _count2ChargeSection: string;
  _count3ChargeSection: string;
}

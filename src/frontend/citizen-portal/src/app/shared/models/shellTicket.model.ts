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
}

export interface ShellTicket {
  violationTicketNumber: string;
  violationTime: string;
  violationDate: string;

  lastName: string;
  givenNames: string;
  driverLicenseNumber: string;
  birthdate: string;
  gender: string;
  courtHearingLocation: string;
  detachmentLocation: string;

  count1Charge: string;
  count1FineAmount: string;
  count2Charge: string;
  count2FineAmount: string;
  count3Charge: string;
  count3FineAmount: string;

  photo: string;
}

import { Address } from './address.model';

// eslint-disable-next-line
export class DisputantView {
  // TODO remove once backend has added this to their side
  lastName?: string | null;
  givenNames?: string | null;
  mailingAddress?: string | null;
  city?: string | null;
  province?: string | null;
  postalCode?: string | null;
  birthdate?: string | null;
  emailAddress?: string | null;
  driverLicenseNumber?: string | null;
  driverLicenseProvince?: string | null;
  phoneNumber?: string | null;
  _mailingAddress: Address;
}

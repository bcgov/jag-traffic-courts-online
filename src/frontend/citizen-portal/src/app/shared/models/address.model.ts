export type AddressLine = Exclude<keyof Address, 'id'>;

/**
 * @description
 * List of optional address line items.
 */
export const optionalAddressLineItems: (keyof Address)[] = ['id', 'street2'];

export class Address {
  id?: number = null;
  address: string = null;
  street2?: string = null;
  city: string = null;
  provinceCode: string = null;
  countryCode: string = null;
  postalCode: string = null;

  constructor(
    countryCode: string = null,
    provinceCode: string = null,
    address: string = null,
    street2: string = null,
    city: string = null,
    postalCode: string = null,
    id: number = null
  ) {
    this.address = address;
    this.street2 = street2;
    this.city = city;
    this.provinceCode = provinceCode;
    this.countryCode = countryCode;
    this.postalCode = postalCode;
  }

  /**
   * @description
   * Create an new instance of an Address.
   *
   * NOTE: Useful for converting object literals into an instance, as well as,
   * creating an empty Address.
   */
  public static instanceOf(addressInfo: Address) {
    const { id = 0, address, street2 = null, city, provinceCode, countryCode, postalCode } = addressInfo;
    return new Address(countryCode, provinceCode, address, street2, city, postalCode, id);
  }

  /**
   * @description
   * Check for an empty address.
   *
   * NOTE: Most use cases don't require `street2`, and therefore it has
   * been excluded by default as optional.
   */
  public static isEmpty(addressInfo: Address, omitList: (keyof Address)[] = optionalAddressLineItems): boolean {
    if (!addressInfo) {
      return true;
    }

    return Object.keys(addressInfo)
      .filter((key: keyof Address) => !omitList.includes(key))
      .every(k => !addressInfo[k]);
  }

  /**
   * @description
   * Checks for a partial address.
   */
  public static isNotEmpty(addressInfo: Address, omitList?: (keyof Address)[]): boolean {
    if (!addressInfo || addressInfo.id === 0) {
      return false;
    }

    return !this.isEmpty(addressInfo, omitList);
  }
}

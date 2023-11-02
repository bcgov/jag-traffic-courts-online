export class DateUtil {

    /**
     * Returns true if the first date is on or after the second date (ignores Time)
     * @param utcString a Date in string UTC format (ie, yyyy-MM-ddTHH:mm:ssZ)
     * @param localDate a Date in string local, no time (ie, yyyy-MM-dd)
     * @returns true if the first date is on or after the second date (ignores Time)
     */
    static isDateOnOrAfter(utcString: string, localDate: string) {
        if (!this.isValid(utcString) || !this.isValid(localDate)) {
            return false;
        }
        
        let dscDt = new Date(utcString);
        dscDt.setUTCHours(0, 0, 0, 0);

        let localDt = new Date(localDate);
        let utcDt = new Date(localDt.getTime() + (localDt.getTimezoneOffset() * 60000)); // set localDt to be in UTC
        utcDt.setUTCHours(0, 0, 0, 0);

        return dscDt >= utcDt;
    }
    
    /**
     * Returns true if the first date is on or before the second date (ignores Time)
     * @param utcString a Date in string UTC format (ie, yyyy-MM-ddTHH:mm:ssZ)
     * @param localDate a Date in string local, no time (ie, yyyy-MM-dd)
     * @returns true if the first date is on or before the second date (ignores Time)
     */
    static isDateOnOrBefore(utcString: string, localDate: string) {
        if (!this.isValid(utcString) || !this.isValid(localDate)) {
            return false;
        }

        let dscDt = new Date(utcString);
        dscDt.setUTCHours(0, 0, 0, 0);

        let localDt = new Date(localDate);
        let utcDt = new Date(localDt.getTime() + (localDt.getTimezoneOffset() * 60000)); // set localDt to be in UTC
        utcDt.setUTCHours(0, 0, 0, 0);

        return dscDt <= utcDt;
    }

    /**
     * Returns true if the given string value properly represents a Date object
     * @param value a date or datetime string representation of a Date object
     * @returns true if the given string is valid.
     */
    static isValid(value: string) : boolean {
        return !isNaN(Date.parse(value));
    }

}
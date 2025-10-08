import {
  AbstractControl, ValidationErrors, ValidatorFn,
  Validators
} from '@angular/forms';
import { StringNormalizer } from '@core/utils/string-normalizer.util';

export class FormControlValidators {
  /**
   * @description
   * Checks the form control value is letters.
   */
  public static alpha(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    const regExp = /^[a-z]+$/i;
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { alpha: true };
  }

  /**
   * @description
   * Checks the form control value is letters or numerals.
   */
  public static alphanumeric(
    control: AbstractControl
  ): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    const regExp = new RegExp(/^[a-zA-Z0-9]*$/);
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { alphanumeric: true };
  }

  /**
   * @description
   * Checks the form control value is a currency.
   *
   * 0?               The string can start with a zero if the value is zero, OR
   * (?![0,])         The string can NOT start with zero or a comma, AND
   * (,?[\d]{1,3})+   The string must contain numbers and decimals only, commas
   *                  are prevented from being side-by-side, or coming before
   *                  a decimal
   * (\.[\d]{2})?     The string will either have a fraction with a precision
   *                  of 2, or no fraction
   */
  public static currency(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    // Doesn't allow . or .# only .## or no decimal
    const regExp = /^(0?|(?![0,])(,?[\d]{1,3})+)(\.[\d]{2})?$/;
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { currency: true };
  }

  /**
   * @description
   * Checks the form control value is an email address.
   */
  public static email(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    const regExp = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/i;
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { email: true };
  }

  public static multipleEmails(
    control: AbstractControl
  ): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    const regExp = /^([a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,})(,(\s)?[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,})*$/i;
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { emails: true };
  }

  /**
  * @description
  * Checks the form control value contains an email-like pattern.
  *
  * It gives an error if the value contains:
  * 1. The "@" symbol anywhere.
  * 2. A valid domain suffix (e.g., ".com", ".net", ".xyz", etc.).
  */
  public static containsEmailPattern(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }

    // Regular expression to check if the value contains the "@" symbol anywhere
    const atSymbolRegExp = /@/;

    // Regular expression to check if the value contains a valid domain suffix (e.g., .com, .net, .org)
    const domainSuffixRegExp = /\.[a-zA-Z]{2,}$/;

    // Check for either condition:
    const containsAtSymbol = atSymbolRegExp.test(control.value);
    const containsDomainSuffix = domainSuffixRegExp.test(control.value);

    // If either condition is true, return an error.
    if (containsAtSymbol || containsDomainSuffix) {
      return { containsEmailPattern: true };
    }

    return null;
  }

  /**
   * @description
   * Checks the form control value is an phone number.
   */
  public static phone(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    // Allows for () or not on area code
    // const regExp = /^((\([2-9]{1}[0-9]{2}\))|(([2-9]{1}[0-9]{2})))[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/;
    const regExp = /^([2-9]{1}[0-9]{2})[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/;
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { phone: true };
  }

  /**
   * @description
   * Checks the form control value is a float.
   */
  public static float(
    control: AbstractControl,
    precision: number = 2
  ): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    // Doesn't allow . or .# only .##+ or no decimal
    const regExp = /^[-+]?(0?|(?![0,])(,?[\d]{1,3})+)(\.[\d]{2,})?$/;
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { float: true };
  }

  /**
   * @description
   * Checks the form control value is numeric.
   */
  public static numeric(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    const regExp = /^[0-9]+$/;
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { numeric: true };
  }

  /**
   * @description
   * Checks the form control value is a percentage.
   */
  public static percent(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    const regExp = /^([0-9]|([1-9][0-9])|100)(\.[\d]{0,2})?$/;
    const valid = control.valid && regExp.test(control.value);
    return valid ? null : { percent: true };
  }

  /**
   * @description
   * Checks a form control is non-empty or false.
   */
  public static requiredTruthful(
    control: AbstractControl
  ): ValidationErrors | null {
    // Not checking the control value on purpose!
    return typeof control.value === 'boolean'
      ? Validators.requiredTrue(control)
      : Validators.required(control);
  }

  /**
   * @description
   * Checks a form control is a boolean.
   */
  public static requiredBoolean(
    control: AbstractControl
  ): ValidationErrors | null {
    // Not checking the control value on purpose!
    return typeof control.value === 'boolean' ? null : { boolean: true };
  }

  /**
   * @description
   * Checks a form control is within a valid length,
   * if there is no maxLength, it will be assumed to be the same as minLength.
   */
  public static requiredLength(
    minLength: number,
    maxLength?: number
  ): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }
      if (!maxLength) {
        maxLength = minLength;
      }
      const currentLength = control.value.length;
      const valid =
        control.valid &&
        currentLength >= minLength &&
        currentLength <= maxLength;
      return valid ? null : { length: true };
    };
  }

  /**
   * @description
   * Validates that the form control value contains only ASCII characters.
   * Automatically normalizes common problematic Unicode characters before validation.
   * 
   * This validator implements a two-phase approach:
   * Phase 1: Normalize common problematic characters (Unicode quotes, dashes, etc.)
   * Phase 2: Validate that no non-ASCII characters remain after normalization
   * 
   * @returns ValidationErrors with 'asciiOnly' property if validation fails, null otherwise
   */
  public static asciiOnly(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }

    const result = StringNormalizer.normalizeAndValidate(control.value);
    
    // Update the control value with the normalized version if it changed
    if (result.normalized !== control.value) {
      // Use setTimeout to avoid ExpressionChangedAfterItHasBeenCheckedError
      setTimeout(() => {
        control.setValue(result.normalized, { emitEvent: false });
      });
    }

    if (!result.isValid) {
      const errorMessage = StringNormalizer.getValidationErrorMessage(result.nonASCIICharacters);
      return { 
        asciiOnly: {
          message: errorMessage,
          invalidCharacters: result.nonASCIICharacters
        }
      };
    }

    return null;
  }

  /**
   * @description
   * Validates ASCII characters without automatic normalization.
   * Use this when you want to strictly validate without modifying the input.
   * 
   * @returns ValidationErrors with 'asciiOnlyStrict' property if validation fails, null otherwise
   */
  public static asciiOnlyStrict(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }

    const isValid = StringNormalizer.isASCII(control.value);
    
    if (!isValid) {
      const nonASCIIChars = StringNormalizer.getNonASCIICharacters(control.value);
      const errorMessage = StringNormalizer.getValidationErrorMessage(nonASCIIChars);
      return { 
        asciiOnlyStrict: {
          message: errorMessage,
          invalidCharacters: nonASCIIChars
        }
      };
    }

    return null;
  }

}

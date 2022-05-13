import { FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';

export class FormGroupValidators {
  /**
   * @description
   * Checks two form control values are equal within a form group.
   */
  public static match(inputKey: string, confirmInputKey: string): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const input = group.controls[inputKey];
      const confirmInput = group.controls[confirmInputKey];
      if (!input || !confirmInput) {
        return null;
      }
      const valid = input.value === confirmInput.value;
      return valid ? null : { nomatch: true };
    };
  }

  /**
   * @description
   * Checks that at least one field has been chosen within a form group.
   */
  public static atLeastOne(
    validator: ValidatorFn,
    whitelist: string[] = []
  ): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const atLeastOne =
        group &&
        group.controls &&
        Object.keys(group.controls)
          .filter((key) => whitelist.indexOf(key) !== -1)
          .some((key) => validator(group.controls[key]) === null);
      return atLeastOne ? null : { atleastone: true };
    };
  }

  /**
   * @description
   * Checks that the start key value is less than end key value.
   */
  public static lessThan(startKey: string, endKey: string): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const start = +group.controls[startKey].value;
      const end = +group.controls[endKey].value;
      if (!start || !end) {
        return null;
      }
      const valid = start < end;
      return valid ? null : { lessthan: true };
    };
  }

  /**
   * @description
   * If the start key is true then the check key is required
   */
  public static requiredIfTrue(firstKey: string, checkKey: string): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const funcKey = "requiredIfTrue";
      const first = group.controls[firstKey].value;
      const check = group.controls[checkKey].value;
      let errors = group.controls[checkKey].errors;
      if (errors) {
        delete errors[funcKey];
      }

      if (!first) {
        group.controls[checkKey].setErrors(errors);
        return null;
      }

      const valid = !!(check);
      if (valid) {
        group.controls[checkKey].setErrors(errors);
        return null;

      } else {
        let funcError = {};
        funcError[funcKey] = true;
        group.controls[checkKey].setErrors({ ...errors, ...funcError });
        return funcError;

      }
    };
  }

  /**
   * @description
   * If the start key is equal to valueText then the end key is required
   */
  public static requiredIfValue(firstKey: string, valueText: string, checkKey: string): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const funcKey = "requiredIfValue";
      const first = group.controls[firstKey].value;
      const check = group.controls[checkKey].value;
      let errors = group.controls[checkKey].errors;
      if (errors) {
        delete errors[funcKey];
      }

      if (!first || valueText !== first) {
        group.controls[checkKey].setErrors(errors);
        return null;
      }

      const valid = ((valueText === first) && (check != null));

      if (valid) {
        group.controls[checkKey].setErrors(errors);
        return null;

      } else {
        let funcError = {};
        funcError[funcKey] = true;
        group.controls[checkKey].setErrors({ ...errors, ...funcError });
        return funcError;
      }
    };
  }

  /**
   * @description
   * If the start key is equal to valueText then the end key is required
   */
  public static requiredIfFlags(firstKey: string, secondKey: string, checkKey: string): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const funcKey = "requiredIfFlags";
      const first = group.controls[firstKey].value;
      const second = group.controls[secondKey].value;
      const check = group.controls[checkKey].value;
      let errors = group.controls[checkKey].errors;
      if (errors) {
        delete errors[funcKey];
      }

      if (!first || !second) {
        group.controls[checkKey].setErrors(errors);
        return null;
      }

      const valid = (first && second && (check != null));
      if (valid) {
        group.controls[checkKey].setErrors(errors);
        return null;

      } else {
        let funcError = {};
        funcError[funcKey] = true;
        group.controls[checkKey].setErrors({ ...errors, ...funcError });
        return funcError;
      }
    };
  }

  /**
   * @description
   * If the property is true, then one of the checkboxes must be selected
   */
  public static atLeastOneCheckedIf(ifKey: string, check1Key: string, check2Key: string): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const funcKey = "atLeastOneCheckedIf";
      const ifKeyVal = group.controls[ifKey].value;
      const check1Val = group.controls[check1Key].value;
      const check2Val = group.controls[check2Key].value;
      let errors = group.controls[check2Val].errors;
      if (errors) {
        delete errors[funcKey];
      }

      if (!ifKeyVal) {
        group.controls[check2Key].setErrors(errors);
        return null;
      }

      const valid = !!(check1Val) || !!(check2Val);
      if (valid) {
        group.controls[check2Key].setErrors(errors);
        return null;
      } else {
        let funcError = {};
        funcError[funcKey] = true;
        group.controls[check2Key].setErrors({ ...errors, ...funcError });
        return funcError;
      }
    };
  }

  /**
   * @description
   * Compares date range start and end.
   */
  public static dateRange(
    rangeStartKey: string,
    rangeEndKey: string,
    rangeName: string
  ): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const start = group.controls[rangeStartKey];
      const end = group.controls[rangeEndKey];

      if (!start.value || !end.value) {
        return null;
      }

      const rangeStart = new Date(Date.parse(start.value));
      const rangeEnd = new Date(Date.parse(end.value));

      const valid = rangeEnd.getTime() >= rangeStart.getTime();
      return valid ? null : { [rangeName]: true };
    };
  }
}

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
   * If the start key is true then the end key is required
   */
  public static requiredIfTrue(startKey: string, endKey: string): ValidatorFn {
    return (group: FormGroup): ValidationErrors | null => {
      const start = group.controls[startKey].value;
      const end = group.controls[endKey].value;

      if (!start) {
        group.controls[endKey].setErrors(null);
        return null;
      }

      const valid = !!(end);
      if (valid) {
        group.controls[endKey].setErrors(null);
        return null;

      } else {
        group.controls[endKey].setErrors({ requiredIfTrue: true });
        return { requiredIfTrue: true };

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

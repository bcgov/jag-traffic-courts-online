import { Injectable } from '@angular/core';
import {
  AbstractControl,
  FormGroup,
  FormControl,
  ValidatorFn,
  FormArray,
  FormBuilder,
} from '@angular/forms';

import { LoggerService } from './logger.service';

@Injectable({
  providedIn: 'root',
})
export class FormUtilsService {
  constructor(private fb: FormBuilder, private logger: LoggerService) {}

  /**
   * @description
   * Checks the validity of a form, and triggers validation messages when invalid.
   */
  public checkValidity(form: FormGroup | FormArray): boolean {
    if (form.valid) {
      return true;
    } else {
      this.logger.info('FORM_INVALID', this.getFormErrors(form));

      form.markAllAsTouched();
      return false;
    }
  }

  /**
   * @description
   * Sets FormControl validators.
   */
  public setValidators(
    control: FormControl | FormGroup,
    validators: ValidatorFn | ValidatorFn[],
    blocklist: string[] = []
  ): void {
    if (control instanceof FormGroup) {
      // Assumes that FormGroups will not be deeply nested
      Object.keys(control.controls).forEach((key: string) => {
        // Skip blocklisted keys from having validators updated
        if (!blocklist.includes(key)) {
          this.setValidators(
            control.controls[key] as FormControl,
            validators,
            blocklist
          );
        }
      });
    } else {
      control.setValidators(validators);
      control.updateValueAndValidity();
    }
  }

  /**
   * @description
   * Resets FormControl value(s) and clears associated validators.
   */
  public resetAndClearValidators(
    control: FormControl | FormGroup,
    blocklist: string[] = []
  ): void {
    if (control instanceof FormGroup) {
      // Assumes that FormGroups will not be deeply nested
      Object.keys(control.controls).forEach((key: string) => {
        if (!blocklist.includes(key)) {
          this.resetAndClearValidators(control.controls[key] as FormControl);
        }
      });
    } else {
      control.reset();
      control.clearValidators();
      control.updateValueAndValidity();
    }
  }

  /**
   * @description
   * Check for the required validator applied to a FormControl,
   * FormGroup, or FormArray.
   *
   * @example
   * isRequired('controlName')
   * isRequired('groupName')
   * isRequired('groupName.controlName')
   * isRequired('arrayName')
   * isRequired('arrayName[#].groupName.controlName')
   */
  public isRequired(form: FormGroup, path: string): boolean {
    const control = form.get(path);

    if (control.validator) {
      const validator = control.validator({} as AbstractControl);
      if (validator && validator.required) {
        return true;
      }
    }
    return false;
  }

  /**
   * @description
   * Get all the errors contained within a form.
   */
  public getFormErrors(
    form: FormGroup | FormArray
  ): { [key: string]: any } | null {
    if (!form) {
      return null;
    }

    let hasError = false;
    const result = Object.keys(form?.controls).reduce((acc, key) => {
      const control = form.get(key);
      const errors =
        control instanceof FormGroup || control instanceof FormArray
          ? this.getFormErrors(control)
          : control.errors;
      if (errors) {
        acc[key] = errors;
        hasError = true;
      }
      return acc;
    }, {}); // as { [key: string]: any });
    return hasError ? result : null;
  }
}

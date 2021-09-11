import { Injectable } from '@angular/core';
import {
  AbstractControl,
  FormGroup,
  FormControl,
  ValidatorFn,
  FormArray,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { Country } from '@shared/enums/country.enum';
import { Province } from '@shared/enums/province.enum';
import { AddressLine } from '@shared/models/address.model';

import { LoggerService } from './logger.service';

@Injectable({
  providedIn: 'root',
})
export class FormUtilsService {
  constructor(private fb: FormBuilder, private logger: LoggerService) { }

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



  /**
   * @description
   * Provide an address form group.
   *
   * @param options available for manipulating the form group
   *  areRequired control names that are required
   *  areDisabled control names that are disabled
   *  useDefaults for province and country, otherwise empty
   *  exclude control names that are not needed
   */
  public buildAddressForm(options: {
    areRequired?: AddressLine[],
    areDisabled?: AddressLine[],
    useDefaults?: Extract<AddressLine, 'provinceCode' | 'countryCode'>[],
    exclude?: AddressLine[];
  } = null): FormGroup {
    const controlsConfig = {
      id: [
        0,
        []
      ],
      street: [
        { value: null, disabled: false },
        []
      ],
      street2: [
        { value: null, disabled: false },
        []
      ],
      city: [
        { value: null, disabled: false },
        []
      ],
      provinceCode: [
        { value: null, disabled: false },
        []
      ],
      countryCode: [
        { value: null, disabled: false },
        []
      ],
      postal: [
        { value: null, disabled: false },
        []
      ]
    };

    Object.keys(controlsConfig)
      .filter((key: AddressLine) => !options?.exclude?.includes(key))
      .forEach((key: AddressLine, index: number) => {
        const control = controlsConfig[key];
        const controlProps = control[0] as { value: any, disabled: boolean; };
        const controlValidators = control[1] as Array<ValidatorFn>;

        if (options?.areDisabled?.includes(key)) {
          controlProps.disabled = true;
        }

        const useDefaults = options?.useDefaults;
        if (useDefaults) {
          if (key === 'provinceCode') {
            controlProps.value = Province.BRITISH_COLUMBIA;
          } else if (key === 'countryCode') {
            controlProps.value = Country.CANADA;
          }
        }

        if (options?.areRequired?.includes(key)) {
          controlValidators.push(Validators.required);
        }
      });

    return this.fb.group(controlsConfig);
  }
}

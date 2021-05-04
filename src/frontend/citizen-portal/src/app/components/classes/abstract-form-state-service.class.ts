import { AbstractControl, FormBuilder } from '@angular/forms';
import { LoggerService } from '@core/services/logger.service';

export abstract class AbstractFormStateService<T> {
  protected patched: boolean;

  constructor(protected fb: FormBuilder, protected logger: LoggerService) {}

  /**
   * @description
   * Check whether the form has been patched.
   */
  public get isPatched() {
    return this.patched;
  }

  /**
   * @description
   * Convert JSON into reactive form abstract controls, which can
   * only be set more than once when explicitly forced.
   *
   * NOTE: Executed by views to populate their form models, which
   * allows for it to be used for setting required values that
   * can't be loaded during instantiation.
   */
  public setForm(model: T, forcePatch: boolean = false): void {
    if (this.patched && !forcePatch) {
      return;
    }

    // Indicate that the form is patched, and may contain unsaved information
    this.patched = true;

    this.patchForm(model);
  }

  /**
   * @description
   * Convert reactive form abstract controls into JSON.
   */
  public abstract get json(): T;

  /**
   * @description
   * List of constituent model forms, which is used at minimum to
   * drive internal form helper methods.
   */
  public abstract get forms(): AbstractControl[];

  /**
   * @description
   * Check that all constituent forms are valid.
   */
  public get isValid(): boolean {
    return this.forms.reduce(
      (valid: boolean, form: AbstractControl) => valid && form.valid,
      true
    );
  }

  /**
   * @description
   * Check that at least one constituent form is dirty.
   */
  public get isDirty(): boolean {
    return this.forms.reduce(
      (dirty: boolean, form: AbstractControl) => dirty || form.dirty,
      false
    );
  }

  /**
   * @description
   * Mark all constituent forms as pristine.
   */
  public markAsPristine(): void {
    this.forms.forEach((form: AbstractControl) => form.markAsPristine());
  }

  /**
   * @description
   * Reset all the forms.
   */
  public reset(): void {
    this.patched = false;

    // Recreate the forms to enforce that the expected
    // defaults are maintained, which doesn't occur
    // if AbstractControl::reset() is used
    this.buildForms();
  }

  /**
   * @description
   * Build and configure the forms for patching.
   */
  protected abstract buildForms(): void;

  /**
   * @description
   * Manage the conversion of JSON to reactive forms.
   */
  protected abstract patchForm(model: T): void;

  /**
   * @description
   * Initialize the form state service for use by building the required
   * forms and setting up the route state listener.
   *
   * NOTE: Must be invoked by the inheriting class!
   */
  protected initialize() {
    // Initial state of the form is unpatched and ready for
    // dispute information to be populated
    this.patched = false;

    this.buildForms();
  }
}

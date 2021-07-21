import { Component, Input, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidatorFn,
} from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { Config } from '@config/config.model';
import { ConfigService } from '@config/config.service';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

export function autocompleteObjectValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    if (typeof control.value === 'string') {
      return { invalidAutocompleteObject: { value: control.value } };
    }
    return null; /* valid option selected */
  };
}

@Component({
  selector: 'app-count-form-field',
  templateUrl: './count-form-field.component.html',
  styleUrls: ['./count-form-field.component.scss'],
})
export class CountFormFieldComponent implements OnInit {
  @Input() parentForm: FormGroup;
  @Input() parentFormControlName: string;
  @Input() fieldLabel: string;

  public statutes: Config<number>[];
  public filteredStatutes: Observable<Config<number>[]>;

  constructor(private configService: ConfigService) {
    this.statutes = this.configService.statutes;
  }

  ngOnInit(): void {
    console.log('aaa', this.parentFormControlName);
    this.filteredStatutes = this.countFormControlName.valueChanges.pipe(
      startWith(''),
      map((value) => (typeof value === 'string' ? value : value.name)),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );
  }

  private filterStatutes(value: string): Config<number>[] {
    console.log('filterStatutes', value, this.statutes.length);
    const trimValue = value.toLowerCase().replace(/\s+/g, ''); // Get rid of whitespace
    const noBracketValue = trimValue.replace(/[\(\)']+/g, ''); // Get rid of brackets

    if (trimValue === noBracketValue) {
      const filtered = this.statutes.filter((option) =>
        option.name
          .toLowerCase()
          .replace(/\s+/g, '') // Get rid of whitespace
          .replace(/[\(\)']+/g, '') // Get rid of brackets
          .includes(noBracketValue)
      );
      console.log('   1filtered', filtered.length);
      return filtered;
    }

    const filtered = this.statutes.filter((option) =>
      option.name.toLowerCase().replace(/\s+/g, '').includes(trimValue)
    );
    console.log('   2filtered', filtered.length);
    return filtered;
  }

  public onDisplayWithStatute(statute?: Config<number>): string | undefined {
    return statute ? statute.name : undefined;
  }

  public onStatuteSelected(event$: MatAutocompleteSelectedEvent): void {
    console.log('onStatuteSelected', event$.option.value);
  }

  public get countFormControlName(): FormControl {
    // console.log('yyy', this.parentFormControlName);
    // console.log('xxx', this.parentForm);
    // console.log('xxx', this.parentForm.get(this.parentFormControlName));
    return this.parentForm.get(this.parentFormControlName) as FormControl;
    // return this.parentFormControlName as FormControl;
  }
}

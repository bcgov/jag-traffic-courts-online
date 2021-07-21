import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { Config } from '@config/config.model';
import { ConfigService } from '@config/config.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { Observable, Subscription } from 'rxjs';
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
  selector: 'app-shell-ticket',
  templateUrl: './shell-ticket.component.html',
  styleUrls: ['./shell-ticket.component.scss'],
})
export class ShellTicketComponent implements OnInit {
  public busy: Subscription;
  public form: FormGroup;
  public todayDate: Date = new Date();
  public maxDateOfBirth: Date;
  public isMobile: boolean;

  public statutes: Config<number>[];
  public courtLocations: Config<string>[];
  public policeLocations: Config<string>[];
  public filteredStatutes1: Observable<Config<number>[]>;
  public filteredStatutes2: Observable<Config<number>[]>;
  public filteredStatutes3: Observable<Config<number>[]>;

  constructor(
    private formBuilder: FormBuilder,
    private formUtilsService: FormUtilsService,
    private configService: ConfigService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {
    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      offenceDate: [null, [Validators.required]],
      offenceTime: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      birthdate: [null], // Optional
      gender: [null, [Validators.required]],
      count1: [null, [Validators.required, autocompleteObjectValidator()]],
      count1FineAmount: [null, [Validators.required]],
      count2: [null, [autocompleteObjectValidator()]],
      count2FineAmount: [null],
      count3: [null, [autocompleteObjectValidator()]],
      count3FineAmount: [null],
      courtHearingLocation: [null, [Validators.required]],
      orgDetachmentLocation: [null, [Validators.required]],
      driverLicense: [null, [Validators.required]],
      chargeCount: [1],
    });

    this.statutes = this.configService.statutes;
    this.courtLocations = this.configService.courtLocations;
    this.policeLocations = this.configService.policeLocations;

    this.maxDateOfBirth = new Date();
    this.maxDateOfBirth.setFullYear(this.todayDate.getFullYear() - 16); // TODO 16 or 18?
    this.isMobile = this.utilsService.isMobile();
  }

  public ngOnInit(): void {
    this.filteredStatutes1 = this.count1.valueChanges.pipe(
      startWith(''),
      map((value) => (typeof value === 'string' ? value : value.name)),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );
    this.filteredStatutes2 = this.count2.valueChanges.pipe(
      startWith(''),
      map((value) => (typeof value === 'string' ? value : value.name)),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );
    this.filteredStatutes3 = this.count3.valueChanges.pipe(
      startWith(''),
      map((value) => (typeof value === 'string' ? value : value.name)),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );
  }

  public onSubmit(): void {
    const validity = this.formUtilsService.checkValidity(this.form);
    const errors = this.formUtilsService.getFormErrors(this.form);

    this.logger.log('validity', validity);
    this.logger.log('errors', errors);
    this.logger.log('form.value', this.form.value);

    if (!validity) {
      return;
    }
  }

  public get emailAddress(): FormControl {
    return this.form.get('emailAddress') as FormControl;
  }

  private filterStatutes(value: string): Config<number>[] {
    const trimValue = value.toLowerCase().replace(/\s+/g, ''); // Get rid of whitespace
    const noBracketValue = trimValue.replace(/[\(\)']+/g, ''); // Get rid of brackets

    if (trimValue === noBracketValue) {
      return this.statutes.filter((option) =>
        option.name
          .toLowerCase()
          .replace(/\s+/g, '') // Get rid of whitespace
          .replace(/[\(\)']+/g, '') // Get rid of brackets
          .includes(noBracketValue)
      );
    }

    return this.statutes.filter((option) =>
      option.name.toLowerCase().replace(/\s+/g, '').includes(trimValue)
    );
  }

  public onDisplayWithStatute(statute?: Config<number>): string | undefined {
    return statute ? statute.name : undefined;
  }

  public onStatuteSelected(event$: MatAutocompleteSelectedEvent): void {
    console.log('onStatuteSelected', event$.option.value);
  }

  public get ticketNumber(): FormControl {
    return this.form.get('ticketNumber') as FormControl;
  }

  public get chargeCount(): FormControl {
    return this.form.get('chargeCount') as FormControl;
  }

  public get count1(): FormControl {
    return this.form.get('count1') as FormControl;
  }

  public get count2(): FormControl {
    return this.form.get('count2') as FormControl;
  }

  public get count3(): FormControl {
    return this.form.get('count3') as FormControl;
  }
}

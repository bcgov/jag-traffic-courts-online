import { AfterViewInit, Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Config } from '@config/config.model';
import { ConfigService } from '@config/config.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { Observable, Subscription } from 'rxjs';

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
export class ShellTicketComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public form: FormGroup;
  public todayDate: Date = new Date();
  public maxDateOfBirth: Date;
  public isMobile: boolean;

  // public statutes: Config<number>[];
  public courtLocations: Config<string>[];
  public policeLocations: Config<string>[];
  public filteredStatutes: Observable<Config<number>[]>;

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
      courtHearingLocation: [null, [Validators.required]],
      orgDetachmentLocation: [null, [Validators.required]],
      driverLicense: [null, [Validators.required]],
      chargeCount: [1],
      amountOwing: ['$125'],
      test: [null],
      tonk: [null],
      tink: [null],
    });

    // this.statutes = this.configService.statutes;
    this.courtLocations = this.configService.courtLocations;
    this.policeLocations = this.configService.policeLocations;

    this.maxDateOfBirth = new Date();
    this.maxDateOfBirth.setFullYear(this.todayDate.getFullYear() - 16); // TODO 16 or 18?
    this.isMobile = this.utilsService.isMobile();
  }

  public ngOnInit(): void {
    // this.filteredStatutes = this.count1.valueChanges.pipe(
    //   startWith(''),
    //   map((value) => (typeof value === 'string' ? value : value.name)),
    //   map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    // );
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
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

  // private filterStatutes(value: string): Config<number>[] {
  //   const trimValue = value.toLowerCase().replace(/\s+/g, ''); // Get rid of whitespace
  //   const noBracketValue = trimValue.replace(/[\(\)']+/g, ''); // Get rid of brackets

  //   if (trimValue === noBracketValue) {
  //     return this.statutes.filter((option) =>
  //       option.name
  //         .toLowerCase()
  //         .replace(/\s+/g, '') // Get rid of whitespace
  //         .replace(/[\(\)']+/g, '') // Get rid of brackets
  //         .includes(noBracketValue)
  //     );
  //   }

  //   return this.statutes.filter((option) =>
  //     option.name.toLowerCase().replace(/\s+/g, '').includes(trimValue)
  //   );
  // }

  // public onDisplayWithStatute(statute?: Config<number>): string | undefined {
  //   return statute ? statute.name : undefined;
  // }

  // public onStatuteSelected(event$: MatAutocompleteSelectedEvent): void {
  //   console.log('onStatuteSelected', event$.option.value);
  // }

  public get ticketNumber(): FormControl {
    return this.form.get('ticketNumber') as FormControl;
  }

  public get chargeCount(): FormControl {
    return this.form.get('chargeCount') as FormControl;
  }

  // public get count1(): FormControl {
  //   return this.form.get('count1') as FormControl;
  // }

  // public get count2(): FormControl {
  //   return this.form.get('count2') as FormControl;
  // }

  // public get count3(): FormControl {
  //   return this.form.get('count3') as FormControl;
  // }

  public get test(): FormControl {
    return this.form.get('test') as FormControl;
  }

  public get tonk(): FormControl {
    return this.form.get('tonk') as FormControl;
  }

  public get tink(): FormControl {
    return this.form.get('tink') as FormControl;
  }
}

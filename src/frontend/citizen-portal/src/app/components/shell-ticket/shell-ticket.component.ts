import { CurrencyPipe } from '@angular/common';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Config } from '@config/config.model';
import { ConfigService } from '@config/config.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { UtilsService } from '@core/services/utils.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { AppRoutes } from 'app/app.routes';
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
export class ShellTicketComponent implements OnInit, AfterViewInit {
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
    private currencyPipe: CurrencyPipe,
    private dialog: MatDialog,
    private router: Router,
    private toastService: ToastService,
    private logger: LoggerService
  ) {
    this.form = this.formBuilder.group({
      violationTicketNumber: [null, [Validators.required]],
      violationDate: [null, [Validators.required]],
      violationTime: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      birthdate: [null], // Optional
      gender: [null, [Validators.required]],
      count1: [null, [Validators.required, autocompleteObjectValidator()]],
      count1FineAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      count2: [null, [Validators.required, autocompleteObjectValidator()]],
      count2FineAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      count3: [null, [Validators.required, autocompleteObjectValidator()]],
      count3FineAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      courtHearingLocation: [null, [Validators.required]],
      orgDetachmentLocation: [null, [Validators.required]],
      driverLicense: [null, [Validators.required]],
      chargeCount: [1],
      amountOwing: [null],
    });

    this.statutes = this.configService.statutes;
    this.courtLocations = this.configService.courtLocations;
    this.policeLocations = this.configService.policeLocations;

    this.maxDateOfBirth = new Date();
    this.maxDateOfBirth.setFullYear(this.todayDate.getFullYear() - 16); // TODO 16 or 18?
    this.isMobile = this.utilsService.isMobile();
  }

  public ngOnInit(): void {
    // Listen for typeahead changes in the statute fields
    this.filteredStatutes1 = this.count1.valueChanges.pipe(
      startWith(''),
      map((value) =>
        value ? (typeof value === 'string' ? value : value.name) : null
      ),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );

    this.filteredStatutes2 = this.count2.valueChanges.pipe(
      startWith(''),
      map((value) =>
        value ? (typeof value === 'string' ? value : value.name) : null
      ),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );

    this.filteredStatutes3 = this.count3.valueChanges.pipe(
      startWith(''),
      map((value) =>
        value ? (typeof value === 'string' ? value : value.name) : null
      ),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );

    // Calculate the amount owing
    this.count1FineAmount.valueChanges.subscribe(() => {
      this.onCalculateAmountOwing();
    });
    this.count2FineAmount.valueChanges.subscribe(() => {
      this.onCalculateAmountOwing();
    });
    this.count3FineAmount.valueChanges.subscribe(() => {
      this.onCalculateAmountOwing();
    });
    this.onCalculateAmountOwing();

    // Set the enabled/disabled of the count fields depending upon visibility
    this.chargeCount.valueChanges.subscribe((selectedValue) => {
      this.onChargeCountChange(selectedValue);
    });
    this.onChargeCountChange(this.chargeCount.value);
  }

  private onChargeCountChange(selectedValue): void {
    this.logger.info('chargeCount.valueChanges', selectedValue);

    if (selectedValue < 3) {
      this.count3.disable();
      this.count3FineAmount.disable();
    } else {
      this.count3.enable();
      this.count3FineAmount.enable();
    }

    if (selectedValue < 2) {
      this.count2.disable();
      this.count2FineAmount.disable();
    } else {
      this.count2.enable();
      this.count2FineAmount.enable();
    }
  }

  private onCalculateAmountOwing(): void {
    let total = 0;
    total += this.count1FineAmount.value;
    total += this.count2FineAmount.value;
    total += this.count3FineAmount.value;
    this.amountOwing.setValue(this.currencyPipe.transform(total));
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

    const data: DialogOptions = {
      titleKey: 'shell_ticket_confirmation.heading',
      messageKey: 'shell_ticket_confirmation.message',
      actionTextKey: 'shell_ticket_confirmation.confirm',
      cancelTextKey: 'shell_ticket_confirmation.cancel',
    };
    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          this.toastService.openSuccessToast(
            'The ticket has successfully been created'
          );
          this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
            queryParams: { violationTicketNumber: 'EZ02000460', time: '09:54' },
          });
        }
      });
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

  public get violationTicketNumber(): FormControl {
    return this.form.get('violationTicketNumber') as FormControl;
  }

  public get violationDate(): FormControl {
    return this.form.get('violationDate') as FormControl;
  }

  public get birthdate(): FormControl {
    return this.form.get('birthdate') as FormControl;
  }

  public get chargeCount(): FormControl {
    return this.form.get('chargeCount') as FormControl;
  }

  public get count1(): FormControl {
    return this.form.get('count1') as FormControl;
  }

  public get count1FineAmount(): FormControl {
    return this.form.get('count1FineAmount') as FormControl;
  }

  public get count2(): FormControl {
    return this.form.get('count2') as FormControl;
  }

  public get count2FineAmount(): FormControl {
    return this.form.get('count2FineAmount') as FormControl;
  }

  public get count3(): FormControl {
    return this.form.get('count3') as FormControl;
  }

  public get count3FineAmount(): FormControl {
    return this.form.get('count3FineAmount') as FormControl;
  }

  public get amountOwing(): FormControl {
    return this.form.get('amountOwing') as FormControl;
  }
}

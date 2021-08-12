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
import { ShellTicket } from '@shared/models/shellTicket.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { AppConfigService } from 'app/services/app-config.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
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
    private disputeService: DisputeService,
    private disputeResource: DisputeResourceService,
    private configService: ConfigService,
    private utilsService: UtilsService,
    private currencyPipe: CurrencyPipe,
    private appConfigService: AppConfigService,
    private dialog: MatDialog,
    private router: Router,
    private logger: LoggerService
  ) {
    this.statutes = this.configService.statutes;
    this.courtLocations = this.configService.courtLocations;
    this.policeLocations = this.configService.policeLocations;

    this.maxDateOfBirth = new Date();
    this.maxDateOfBirth.setFullYear(this.todayDate.getFullYear() - 16); // TODO 16 or 18?
    this.isMobile = this.utilsService.isMobile();

    this.form = this.formBuilder.group({
      violationTicketNumber: [null, [Validators.required]],
      violationDate: [null, [Validators.required]],
      violationTime: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      birthdate: [null], // Optional
      gender: [null, [Validators.required]],
      count1Charge: [
        null,
        [Validators.required, autocompleteObjectValidator()],
      ],
      count1FineAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      count2Charge: [
        null,
        [Validators.required, autocompleteObjectValidator()],
      ],
      count2FineAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      count3Charge: [
        null,
        [Validators.required, autocompleteObjectValidator()],
      ],
      count3FineAmount: [
        null,
        [Validators.required, FormControlValidators.currency],
      ],
      courtHearingLocation: [null, [Validators.required]],
      detachmentLocation: [null, [Validators.required]],
      driverLicenseNumber: [null, [Validators.required]],
      chargeCount: [1],
      amountOwing: [null],
    });

    if (this.appConfigService.useMockServices) {
      // Default values during testing
      this.form.get('violationTicketNumber').setValue('EZ02000455');
      this.form.get('violationDate').setValue('2008-07-03T07:00:00.000Z');
      this.form.get('violationTime').setValue('09:54');
      this.form.get('lastName').setValue('test');
      this.form.get('givenNames').setValue('test');
      this.form.get('birthdate').setValue('1988-03-03T08:00:00.000Z');
      this.form.get('gender').setValue('M');
      this.form.get('count1Charge').setValue(19023);
      this.form.get('count1FineAmount').setValue(234);
      this.form.get('courtHearingLocation').setValue('82.0001');
      this.form.get('detachmentLocation').setValue('9393.0001');
      this.form.get('driverLicenseNumber').setValue(2342343);
      this.form.get('chargeCount').setValue(1);
    }
  }

  public ngOnInit(): void {
    // Listen for typeahead changes in the statute fields
    this.filteredStatutes1 = this.count1Charge.valueChanges.pipe(
      startWith(''),
      map((value) =>
        value ? (typeof value === 'string' ? value : value.name) : null
      ),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );

    this.filteredStatutes2 = this.count2Charge.valueChanges.pipe(
      startWith(''),
      map((value) =>
        value ? (typeof value === 'string' ? value : value.name) : null
      ),
      map((name) => (name ? this.filterStatutes(name) : this.statutes.slice()))
    );

    this.filteredStatutes3 = this.count3Charge.valueChanges.pipe(
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
      this.count3Charge.disable();
      this.count3FineAmount.disable();
    } else {
      this.count3Charge.enable();
      this.count3FineAmount.enable();
    }

    if (selectedValue < 2) {
      this.count2Charge.disable();
      this.count2FineAmount.disable();
    } else {
      this.count2Charge.enable();
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

  public onClearBirthdate(): void {
    this.birthdate.setValue(null);
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
          const payload: ShellTicket = this.form.getRawValue();

          this.busy = this.disputeResource
            .createShellTicket(payload)
            .subscribe((newShellTicket: TicketDispute) => {
              this.disputeService.ticket$.next(newShellTicket);

              this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
                queryParams: {
                  ticketNumber: payload.violationTicketNumber,
                  time: payload.violationTime,
                },
              });
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

  public onDisplayWithStatute(code?: number): string | undefined {
    return code
      ? this.statutes.find((statute) => statute.code === code)?.name
      : undefined;
  }

  public onStatuteSelected(event$: MatAutocompleteSelectedEvent): void {
    this.logger.log('onStatuteSelected', event$.option.value);
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

  public get count1Charge(): FormControl {
    return this.form.get('count1Charge') as FormControl;
  }

  public get count1FineAmount(): FormControl {
    return this.form.get('count1FineAmount') as FormControl;
  }

  public get count2Charge(): FormControl {
    return this.form.get('count2Charge') as FormControl;
  }

  public get count2FineAmount(): FormControl {
    return this.form.get('count2FineAmount') as FormControl;
  }

  public get count3Charge(): FormControl {
    return this.form.get('count3Charge') as FormControl;
  }

  public get count3FineAmount(): FormControl {
    return this.form.get('count3FineAmount') as FormControl;
  }

  public get amountOwing(): FormControl {
    return this.form.get('amountOwing') as FormControl;
  }
}

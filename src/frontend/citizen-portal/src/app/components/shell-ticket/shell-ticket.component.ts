import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import {
  AzureKeyCredential,
  FormPollerLike,
  FormRecognizerClient
} from '@azure/ai-form-recognizer';
import { Config } from '@config/config.model';
import { ConfigService } from '@config/config.service';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { FormControlValidators } from '@core/validators/form-control.validators';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ShellTicketData } from '@shared/models/shellTicketData.model';
import { ShellTicketView } from '@shared/models/shellTicketView.model';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { AppRoutes } from 'app/app.routes';
import { AppConfigService } from 'app/services/app-config.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';
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
  public busy: Subscription | Promise<any>;
  public ticketImageSrc: string;
  public ticketFilename: string;
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

  private progressRef: NgProgressRef;
  private MINIMUM_AGE = 18;

  constructor(
    private formBuilder: FormBuilder,
    private formUtilsService: FormUtilsService,
    private disputeService: DisputeService,
    private disputeResource: DisputeResourceService,
    private configService: ConfigService,
    private utilsService: UtilsService,
    private dialog: MatDialog,
    private router: Router,
    private ngProgress: NgProgress,
    private appConfigService: AppConfigService,
    private logger: LoggerService
  ) {
    this.progressRef = this.ngProgress.ref();
    this.statutes = this.configService.statutes;
    this.courtLocations = this.configService.courtLocations;
    this.policeLocations = this.configService.policeLocations;

    this.maxDateOfBirth = new Date();
    this.maxDateOfBirth.setFullYear(
      this.todayDate.getFullYear() - this.MINIMUM_AGE
    );
    this.isMobile = this.utilsService.isMobile();

    this.form = this.formBuilder.group({
      violationTicketNumber: [null, [Validators.required]],
      violationDate: [null, [Validators.required]],
      violationTime: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      birthdate: [null], // Optional
      gender: [null, [Validators.required]],
      address: [null, [Validators.required]],
      city: [null, [Validators.required]],
      province: [null, [Validators.required]],
      postalCode: [null, [Validators.required]],
      driverLicenseNumber: [null, [Validators.required]],
      driverLicenseProvince: [null, [Validators.required]],

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
      _chargeCount: [1],
      _amountOwing: [null],
    });

    this.disputeService.shellTicketData$.subscribe((shellTicketData) => {
      if (!shellTicketData) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      this.ticketImageSrc = shellTicketData.ticketImage;
      this.ticketFilename = shellTicketData.filename;
      if (this.appConfigService.useMockServices) {
        this.progressRef.complete();
      } else {
        this.recognizeContent(shellTicketData.ticketFile);
      }
    });
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

    // Set the enabled/disabled of the count fields depending upon visibility
    this._chargeCount.valueChanges.subscribe((selectedValue) => {
      this.onChargeCountChange(selectedValue);
    });
    this.onChargeCountChange(this._chargeCount.value);
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
      this.utilsService.scrollToErrorSection();
      return;
    }

    const data: DialogOptions = {
      titleKey: 'Are you sure all ticket information is correct?',
      messageKey: `Please ensure that all entered fields match the paper ticket copy exactly.
        If you do not ensure all fields match it could cause issues during reconciliation.
        If you are not sure, please go back and update any fields as needed before submitting ticket information.`,
      actionTextKey: 'Yes, continue to resolution options',
      cancelTextKey: 'Go back and edit',
    };

    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          const payload: ShellTicketView = this.form.getRawValue();

          this.busy = this.disputeResource
            .createShellTicket(payload)
            .subscribe((newShellTicket: TicketDisputeView) => {
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

  public onDisplayWithStatute(code?: number): string | undefined {
    return code
      ? this.statutes.find((statute) => statute.code === code)?.name
      : undefined;
  }

  public onFileChange(event: any) {
    let filename: string;
    let ticketImage: string;

    this.ticketImageSrc = null;
    this.ticketFilename = null;
    this.form.reset();

    if (!event.target.files[0] || event.target.files[0].length === 0) {
      this.logger.info('You must select an image');
      return;
    }

    const mimeType = event.target.files[0].type;

    if (mimeType.match(/image\/*/) == null) {
      this.logger.info('Only images are supported');
      return;
    }

    this.progressRef.start();

    const reader = new FileReader();
    const ticketFile: File = event.target.files[0];
    this.logger.info('file target', event.target.files[0]);

    filename = ticketFile.name;
    reader.readAsDataURL(ticketFile);
    this.logger.info('file', ticketFile.name, ticketFile.lastModified);

    reader.onload = () => {
      ticketImage = reader.result as string;

      const shellTicketData: ShellTicketData = {
        filename,
        ticketImage,
        ticketFile,
      };
      this.disputeService.shellTicketData$.next(shellTicketData);
    };
  }

  private findMatchingCharge(
    chargeDesc: string,
    chargeStatute: string
  ): number {
    let chargeId = null;

    if (chargeDesc) {
      chargeId = this.statutes.find((statute) =>
        statute.name
          .trim()
          .toUpperCase()
          .includes(chargeDesc.trim().toUpperCase())
      )?.code;

      if (!chargeId) {
        chargeId = this.statutes.find((statute) =>
          statute.name
            .trim()
            .toUpperCase()
            .includes(chargeStatute.trim().toUpperCase())
        )?.code;
      }
    }

    return chargeId ? chargeId : null;
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

  private recognizeContent(imageSource: File): void {
    const endpoint = 'https://canadacentral.api.cognitive.microsoft.com';
    const apiKey = '65440468b684497988679b8857f33a36';

    const client = new FormRecognizerClient(
      endpoint,
      new AzureKeyCredential(apiKey)
    );

    const poller = client.beginRecognizeCustomForms(
      '8b16e3eb-b797-4e1c-a2f6-5891105222dc',
      imageSource,
      {
        onProgress: (state) => {
          this.logger.info(`analyzing status: ${state.status}`);
        },
      }
    );

    // const poller = client.beginRecognizeInvoices(imageSource, {
    //   onProgress: (state) => {
    //     this.logger.info(`analyzing status: ${state.status}`);
    //   },
    // });

    this.busy = poller;

    poller.then(this.onFulfilled(), this.onRejected);
  }

  private onFulfilled(): (poller: FormPollerLike) => void {
    return (poller: FormPollerLike) => {
      this.busy = poller.pollUntilDone().then(() => {
        // this.logger.info('result', poller.getResult());
        const invoices = poller.getResult();

        if (!invoices || invoices.length <= 0) {
          throw new Error('Expecting at least one invoice in analysis result');
        }

        const invoice = invoices[0];
        this.logger.info('First invoice:', invoice);

        const invoiceIdFieldIndex = 'violation ticket number';
        const invoiceIdField = invoice.fields[invoiceIdFieldIndex];
        if (invoiceIdField.valueType === 'string') {
          this.logger.info(
            `  violation ticket number: '${invoiceIdField.value || '<missing>'
            }', with confidence of ${invoiceIdField.confidence}`
          );
        }
        const surnameFieldIndex = 'surname';
        const surnameField = invoice.fields[surnameFieldIndex];
        if (surnameField.valueType === 'string') {
          this.logger.info(
            `  surname: '${surnameField.valueData?.text || '<missing>'
            }', with confidence of ${surnameField.confidence}`
          );
        }
        const givenNameFieldIndex = 'given name';
        const givenNameField = invoice.fields[givenNameFieldIndex];
        if (givenNameField.valueType === 'string') {
          this.logger.info(
            `  given name: '${givenNameField.valueData?.text || '<missing>'
            }', with confidence of ${givenNameField.confidence}`
          );
        }
        const count1DescFieldIndex = 'count 1 description';
        const count1DescField = invoice.fields[count1DescFieldIndex];
        if (count1DescField.valueType === 'string') {
          this.logger.info(
            `  count 1 description: '${count1DescField.valueData?.text || '<missing>'
            }', with confidence of ${count1DescField.confidence}`
          );
        }
        const count1SectionFieldIndex = 'count 1 section';
        const count1SectionField = invoice.fields[count1SectionFieldIndex];
        if (count1SectionField.valueType === 'string') {
          this.logger.info(
            `  count 1 section: '${count1SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count1SectionField.confidence}`
          );
        }
        const count1TicketAmountFieldIndex = 'count 1 ticket amount';
        const count1TicketAmountField =
          invoice.fields[count1TicketAmountFieldIndex];
        if (count1TicketAmountField.valueType === 'number') {
          this.logger.info(
            `  count 1 ticket amount: '${count1TicketAmountField.value || '<missing>'
            }', with confidence of ${count1TicketAmountField.confidence}`
          );
        }
        const count2DescFieldIndex = 'count 2 description';
        const count2DescField = invoice.fields[count2DescFieldIndex];
        if (count2DescField.valueType === 'string') {
          this.logger.info(
            `  count 2 description: '${count2DescField.valueData?.text || '<missing>'
            }', with confidence of ${count2DescField.confidence}`
          );
        }
        const count2SectionFieldIndex = 'count 2 section';
        const count2SectionField = invoice.fields[count2SectionFieldIndex];
        if (count2SectionField.valueType === 'string') {
          this.logger.info(
            `  count 2 section: '${count2SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count2SectionField.confidence}`
          );
        }
        const count2TicketAmountFieldIndex = 'count 2 ticket amount';
        const count2TicketAmountField =
          invoice.fields[count2TicketAmountFieldIndex];
        if (count2TicketAmountField.valueType === 'number') {
          this.logger.info(
            `  count 2 ticket amount: '${count2TicketAmountField.value || '<missing>'
            }', with confidence of ${count2TicketAmountField.confidence}`
          );
        }
        const count3DescFieldIndex = 'count 3 description';
        const count3DescField = invoice.fields[count3DescFieldIndex];
        if (count3DescField.valueType === 'string') {
          this.logger.info(
            `  count 3 description: '${count3DescField.valueData?.text || '<missing>'
            }', with confidence of ${count3DescField.confidence}`
          );
        }
        const count3SectionFieldIndex = 'count 3 section';
        const count3SectionField = invoice.fields[count3SectionFieldIndex];
        if (count3SectionField.valueType === 'string') {
          this.logger.info(
            `  count 3 section: '${count3SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count3SectionField.confidence}`
          );
        }
        const count3TicketAmountFieldIndex = 'count 3 ticket amount';
        const count3TicketAmountField =
          invoice.fields[count3TicketAmountFieldIndex];
        if (count3TicketAmountField.valueType === 'number') {
          this.logger.info(
            `  count 3 ticket amount: '${count3TicketAmountField.value || '<missing>'
            }', with confidence of ${count3TicketAmountField.confidence}`
          );
        }

        let chargeCount = 0;
        if (
          count1DescField.valueData?.text ||
          count1SectionField.valueData?.text ||
          count1TicketAmountField.value
        ) {
          chargeCount++;
        }
        if (
          count2DescField.valueData?.text ||
          count2SectionField.valueData?.text ||
          count2TicketAmountField.value
        ) {
          chargeCount++;
        }
        if (
          count3DescField.valueData?.text ||
          count3SectionField.valueData?.text ||
          count3TicketAmountField.value
        ) {
          chargeCount++;
        }
        this.logger.info('chargeCount', chargeCount);

        const shellTicket: ShellTicketView = {
          violationTicketNumber: String(invoiceIdField.value),
          violationTime: '',
          violationDate: '',

          lastName: surnameField.valueData?.text
            ? surnameField.valueData?.text
            : '',
          givenNames: givenNameField.valueData?.text
            ? givenNameField.valueData?.text
            : '',
          birthdate: '',
          gender: '',
          address: '',
          city: '',
          province: '',
          postalCode: '',
          driverLicenseNumber: '',
          driverLicenseProvince: '',
          courtHearingLocation: '',
          detachmentLocation: '',

          count1Charge: null,
          _count1ChargeDesc: count1DescField.valueData?.text
            ? count1DescField.valueData?.text
            : '',
          _count1ChargeSection: count1SectionField.valueData?.text
            ? count1SectionField.valueData?.text.replace(/\s/g, '')
            : '',
          count1FineAmount: count1TicketAmountField.value
            ? Number(count1TicketAmountField.value)
            : 0,
          count2Charge: null,
          _count2ChargeDesc: count2DescField.valueData?.text
            ? count2DescField.valueData?.text
            : '',
          _count2ChargeSection: count2SectionField.valueData?.text
            ? count2SectionField.valueData?.text.replace(/\s/g, '')
            : '',
          count2FineAmount: count2TicketAmountField.value
            ? Number(count2TicketAmountField.value)
            : 0,
          count3Charge: null,
          _count3ChargeDesc: count3DescField.valueData?.text
            ? count3DescField.valueData?.text
            : '',
          _count3ChargeSection: count3SectionField.valueData?.text
            ? count3SectionField.valueData?.text.replace(/\s/g, '')
            : '',
          count3FineAmount: count3TicketAmountField.value
            ? Number(count3TicketAmountField.value)
            : 0,
          _chargeCount: chargeCount,
          _amountOwing: 0,
        };

        this.logger.info('before', { ...shellTicket });

        shellTicket.count1Charge = this.findMatchingCharge(
          shellTicket._count1ChargeDesc,
          shellTicket._count1ChargeSection
        );
        shellTicket.count2Charge = this.findMatchingCharge(
          shellTicket._count2ChargeDesc,
          shellTicket._count2ChargeSection
        );
        shellTicket.count3Charge = this.findMatchingCharge(
          shellTicket._count3ChargeDesc,
          shellTicket._count3ChargeSection
        );

        // console.log('after', { ...shellTicket });

        delete shellTicket._count1ChargeDesc;
        delete shellTicket._count2ChargeDesc;
        delete shellTicket._count3ChargeDesc;
        delete shellTicket._count1ChargeSection;
        delete shellTicket._count2ChargeSection;
        delete shellTicket._count3ChargeSection;

        this.form.setValue(shellTicket);
        // this.disputeService.shellTicket$.next(shellTicket);

        this.progressRef.complete();
      });
    };
  }

  private onRejected(info) {
    this.logger.info('onRejected', info);
    this.progressRef.complete();
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

  public get _chargeCount(): FormControl {
    return this.form.get('_chargeCount') as FormControl;
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
}

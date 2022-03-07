import { HttpClient } from '@angular/common/http';
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
import { Configuration, TicketSearchResult, TicketsService } from 'app/api';
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
  protected basePath = ''; 
  public configuration = new Configuration();
  isHidden= true;
  isLinear = false;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;

  public statutes: Config<number>[];
  public courtLocations: Config<string>[];
  public policeLocations: Config<string>[];
  public filteredStatutes1: Observable<Config<number>[]>;
  public filteredStatutes2: Observable<Config<number>[]>;
  public filteredStatutes3: Observable<Config<number>[]>;

  private progressRef: NgProgressRef;
  private MINIMUM_AGE = 18;
  public fieldsData;
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
    private logger: LoggerService,
    public ticketService:TicketsService,
    private http: HttpClient,
  ) {
    if (typeof this.configuration.basePath !== 'string') {
      this.configuration.basePath = this.basePath;
  }
    this.progressRef = this.ngProgress.ref();
    this.statutes = this.configService.statutes;
    this.courtLocations = this.configService.courtLocations;
    this.policeLocations = this.configService.policeLocations;

    this.maxDateOfBirth = new Date();
    this.maxDateOfBirth.setFullYear(
      this.todayDate.getFullYear() - this.MINIMUM_AGE
    );
    this.isMobile = this.utilsService.isMobile();
    console.log("ticket service",this.ticketService.getImageData())
    this.fieldsData = this.ticketService.getImageData()
    this.form = this.formBuilder.group({
      violationTicketNumber: [null, [Validators.required]],
      violationDate: [null, [Validators.required]],
      violationTime: [null, [Validators.required]],
      lastName: [null, [Validators.required]],
      givenNames: [null, [Validators.required]],
      birthdate: [null], // Optional
      gender: [null],
      address: [null],
      city: [null],
      province: [null],
      postalCode: [null],
      driverLicenseNumber: [null],
      driverLicenseProvince: [null],
      count1Charge: [
        null
      ],
      count1FineAmount: [
        null
      ],
      count2Charge: [
        null
      ],
      count2FineAmount: [
        null
      ],
      count3Charge: [
        null,
      ],
      count3FineAmount: [
        null,
      ],
      courtHearingLocation: [null],
      detachmentLocation: [null],
      _chargeCount: [1],
      _amountOwing: [null],
      _count1ChargeDesc: [null],
      _count2ChargeDesc: [null],
      _count3ChargeDesc: [null]

    });
    this.onFulfilled()
    this.disputeService.shellTicketData$.subscribe((shellTicketData) => {
      if (!shellTicketData) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      this.ticketImageSrc = shellTicketData.ticketImage;
      this.ticketFilename = shellTicketData.filename;
      // if (this.appConfigService.useMockServices) {
      //   this.progressRef.complete();
      // } else {
      //   this.recognizeContent(shellTicketData.ticketFile);
      // }
    });
  }

  public ngOnInit(): void {
    this.form.disable();

    // this.firstFormGroup = this.formBuilder.group({
    //   firstCtrl: ['', Validators.required],
    // });
    // this.secondFormGroup = this.formBuilder.group({
    //   secondCtrl: ['', Validators.required],
    // });
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
  public toggle(){
    this.isHidden = !this.isHidden;
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
        If you are not sure, please go back and update any fields as needed before submitting ticket information.`,
      actionTextKey: 'Yes I am sure, create online ticket',
      cancelTextKey: 'Go back and edit',
    };
    const payload: ShellTicketView = this.form.getRawValue();

const data2: TicketDisputeView
 = 
  {
    violationTicketNumber: payload.violationTicketNumber,
    violationDate: new Date("2020-09-18T00:00:00"),
    violationTime: payload.violationTime? payload.violationTime.replace(' ',':'): '',
    offences: [
      {
        offenceNumber: 1,
        ticketedAmount:payload.count1FineAmount,
        amountDue: payload.count1FineAmount,
        violationDateTime: '2020-09-18T00:00:00',
        offenceDescription:payload._count1ChargeDesc,
        invoiceType: 'Traffic Violation Ticket',
        vehicleDescription: null,
        discountAmount: 25,
        status: 'New',
        offenceAgreementStatus: null,
        requestReduction: false,
        requestMoreTime: false,
        reductionAppearInCourt: false,
        _applyToAllCounts: false,
        _allowApplyToAllCounts: false,
        _firstOffence: false,
        _within30days: false,
        _amountDue:  payload.count1FineAmount,
        _offenceStatusDesc:'Active',
        _offenceStatus:'New'
      },
      {
        offenceNumber: 2,
        ticketedAmount: payload.count2FineAmount,
        amountDue: payload.count2FineAmount,
        violationDateTime: '2020-09-18T00:00:00',
        offenceDescription:payload._count2ChargeDesc,
        invoiceType: 'Traffic Violation Ticket',
        vehicleDescription: null,
        discountAmount: null,
        status: 'New',
        offenceAgreementStatus: null,
        requestReduction: false,
        requestMoreTime: false,
        reductionAppearInCourt: false,
        _applyToAllCounts: false,
        _allowApplyToAllCounts: false,
        _firstOffence: false,
        _within30days: false,
        _amountDue: payload.count2FineAmount,
        _offenceStatusDesc:'Active',
        _offenceStatus:'New'
      },
      {
        offenceNumber: 3,
        ticketedAmount: payload.count3FineAmount,
        amountDue: payload.count3FineAmount,
        violationDateTime: '2020-09-18T00:00:00',
        offenceDescription:
          payload._count3ChargeDesc,
        invoiceType: 'Traffic Violation Ticket',
        vehicleDescription: null,
        discountAmount: 25,
        status: 'New',
        offenceAgreementStatus: null,
        requestReduction: false,
        requestMoreTime: false,
        reductionAppearInCourt: false,
        _applyToAllCounts: false,
        _allowApplyToAllCounts: false,
        _firstOffence: false,
        _within30days: false,
        _amountDue: payload.count3FineAmount,
        _offenceStatusDesc:'Active',
        _offenceStatus:'New'
      }
    ]
}
// this.disputeService.setTicketData({value:data2});

// this.disputeResource.updateTicketViewModel(data2)
    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          const payload: ShellTicketView = this.form.getRawValue();

          this.disputeService.ticket$.next(data2);

          this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
            queryParams: {
              ticketNumber: payload.violationTicketNumber,
              time: payload.violationTime? payload.violationTime.replace(' ',':'): '' ,
            },
          });

          // this.busy = this.disputeResource
          //   .createShellTicket(payload)
          //   .subscribe((newShellTicket: TicketDisputeView) => {
          //     this.disputeService.ticket$.next(newShellTicket);

          //     this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
          //       queryParams: {
          //         ticketNumber: payload.violationTicketNumber,
          //         time: payload.violationTime,
          //       },
          //     });
          //   });
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
      const fd = new FormData();
      fd.append('file',ticketFile);

      this.http.post(`${this.configuration.basePath }/api/tickets/analyse`,fd)
      .subscribe(res=>{
        console.log('image data 2',res);
        this.fieldsData = res;
        this.ticketService.setImageData(res);
        this.onFulfilled()
        this.disputeService.shellTicketData$.next(shellTicketData);
        this.router.navigate([AppRoutes.disputePath(AppRoutes.SHELL)]);

      })
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

    // poller.then(this.onFulfilled(), this.onRejected);
  }

  private onFulfilled() {
    // return (poller: FormPollerLike) => {
      // this.busy = poller.pollUntilDone().then(() => {
        // this.logger.info('result', poller.getResult());
        const invoices = this.fieldsData

        if (!invoices || invoices.length <= 0) {
          throw new Error('Expecting at least one invoice in analysis result');
        }

        const invoice = invoices;
        this.logger.info('First invoice:', invoice);
        if(!invoice.fields){
          return  true
        }

        const invoiceIdFieldIndex = 'violationTicketNumber';
        const invoiceIdField = invoice.fields[invoiceIdFieldIndex];
        if (invoiceIdField.type === 'String') {
          this.logger.info(
            `  violationTicketNumber: '${invoiceIdField.value || '<missing>'
            }', with confidence of ${invoiceIdField.confidence}`
          );
        }
        const invoiceDateFieldIndex = 'violationDate';
        const invoiceDateField = invoice.fields[invoiceDateFieldIndex];
        if (invoiceIdField.type === 'Date') {
          this.logger.info(
            `  violationTicketNumber: '${invoiceDateField.value || '<missing>'
            }', with confidence of ${invoiceDateField.confidence}`
          );
        }
        const invoiceTimeFieldIndex = 'violationTime';
        const invoiceTimeField = invoice.fields[invoiceTimeFieldIndex];
        if (invoiceTimeField.type === 'Time') {
          this.logger.info(
            `  violationTicketNumber: '${invoiceTimeField.value || '<missing>'
            }', with confidence of ${invoiceTimeField.confidence}`
          );
        }
        const surnameFieldIndex = 'surname';
        const surnameField = invoice.fields[surnameFieldIndex];
        if (surnameField.type === 'String') {
          this.logger.info(
            `  surname: '${surnameField.value || '<missing>'
            }', with confidence of ${surnameField.fieldConfidence}`
          );
        }
        const givenNameFieldIndex = 'givenName';
        const givenNameField = invoice.fields[givenNameFieldIndex];
        if (givenNameField.type === 'String') {
          this.logger.info(
            `  given name: '${givenNameField.value || '<missing>'
            }', with confidence of ${givenNameField.confidence}`
          );
        }
        const driverLicenceFieldIndex = 'driverLicenceNumber';
        const driverLicenceField = invoice.fields[driverLicenceFieldIndex];
        if (driverLicenceField.type === 'String') {
          this.logger.info(
            `  given name: '${driverLicenceField.value || '<missing>'
            }', with confidence of ${driverLicenceField.confidence}`
          );
        }
        const detachmentLocationFieldIndex = 'detachmentLocation';
        const detachmentLocationField = invoice.fields[detachmentLocationFieldIndex];
        if (detachmentLocationField.type === 'String') {
          this.logger.info(
            `  given name: '${detachmentLocationField.value || '<missing>'
            }', with confidence of ${detachmentLocationField.confidence}`
          );
        }
        const count1DescFieldIndex = 'count1Description';
        const count1DescField = invoice.fields[count1DescFieldIndex];
        if (count1DescField.type === 'String') {
          this.logger.info(
            `  count 1 description: '${count1DescField.value || '<missing>'
            }', with confidence of ${count1DescField.confidence}`
          );
        }
        const count1SectionFieldIndex = 'count1Section';
        const count1SectionField = invoice.fields[count1SectionFieldIndex];
        if (count1SectionField.valueType === 'String') {
          this.logger.info(
            `  count 1 section: '${count1SectionField.value || '<missing>'
            }', with confidence of ${count1SectionField.confidence}`
          );
        }
        const count1TicketAmountFieldIndex = 'count1TicketAmount';
        const count1TicketAmountField =
          invoice.fields[count1TicketAmountFieldIndex];
        if (count1TicketAmountField.type === 'Double') {
          this.logger.info(
            `  count 1 ticket amount: '${count1TicketAmountField.value || '<missing>'
            }', with confidence of ${count1TicketAmountField.confidence}`
          );
        }
        debugger
        const count1ActRegsFieldIndex = 'count1ActRegs';
        const count1ActRegsField = invoice.fields[count1ActRegsFieldIndex];
        if (count1ActRegsField.type === 'String') {
          this.logger.info(
            `  count 1 ticket amount: '${count1ActRegsField.value || '<missing>'
            }', with confidence of ${count1ActRegsField.confidence}`
          );
        }
        const count2DescFieldIndex = 'count2Description';
        const count2DescField = invoice.fields[count2DescFieldIndex];
        if (count2DescField.type === 'String') {
          this.logger.info(
            `  count 2 description: '${count2DescField.value || '<missing>'
            }', with confidence of ${count2DescField.confidence}`
          );
        }
        const count2SectionFieldIndex = 'count2Section';
        const count2SectionField = invoice.fields[count2SectionFieldIndex];
        if (count2SectionField.type === 'String') {
          this.logger.info(
            `  count 2 section: '${count2SectionField.value || '<missing>'
            }', with confidence of ${count2SectionField.confidence}`
          );
        }
        const count2TicketAmountFieldIndex = 'count2TicketAmount';
        const count2TicketAmountField =
          invoice.fields[count2TicketAmountFieldIndex];
        if (count2TicketAmountField.type === 'Double') {
          this.logger.info(
            `  count 2 ticket amount: '${count2TicketAmountField.value || '<missing>'
            }', with confidence of ${count2TicketAmountField.confidence}`
          );
        }
        const count2ActRegsFieldIndex = 'count2ActRegs';
        const count2ActRegsField = invoice.fields[count2ActRegsFieldIndex];
        if (count2ActRegsField.type === 'String') {
          this.logger.info(
            `  count 2 ticket amount: '${count2ActRegsField.value || '<missing>'
            }', with confidence of ${count2ActRegsField.confidence}`
          );
        }
        const count3DescFieldIndex = 'count3Description';
        const count3DescField = invoice.fields[count3DescFieldIndex];
        if (count3DescField.type === 'String') {
          this.logger.info(
            `  count 3 description: '${count3DescField.value || '<missing>'
            }', with confidence of ${count3DescField.confidence}`
          );
        }
        const count3SectionFieldIndex = 'count3Section';
        const count3SectionField = invoice.fields[count3SectionFieldIndex];
        if (count3SectionField.type === 'String') {
          this.logger.info(
            `  count 3 section: '${count3SectionField.value || '<missing>'
            }', with confidence of ${count3SectionField.confidence}`
          );
        }
        const count3TicketAmountFieldIndex = 'count3TicketAmount';
        const count3TicketAmountField =
          invoice.fields[count3TicketAmountFieldIndex];
        if (count3TicketAmountField.type === 'Double') {
          this.logger.info(
            `  count 3 ticket amount: '${count3TicketAmountField.value || '<missing>'
            }', with confidence of ${count3TicketAmountField.confidence}`
          );
        }
        const count3ActRegsFieldIndex = 'count3ActRegs';
        const count3ActRegsField = invoice.fields[count3ActRegsFieldIndex];
        if (count3ActRegsField.type === 'String') {
          this.logger.info(
            `  count 3 ticket amount: '${count3ActRegsField.value || '<missing>'
            }', with confidence of ${count3ActRegsField.confidence}`
          );
        }

        let chargeCount = 0;
        if (
          count1DescField.value ||
          count1SectionField.value ||
          count1TicketAmountField.value
        ) {
          chargeCount++;
        }
        if (
          count2DescField.value ||
          count2SectionField.value ||
          count2TicketAmountField.value
        ) {
          chargeCount++;
        }
        if (
          count3DescField.value ||
          count3SectionField.valuet ||
          count3TicketAmountField.value
        ) {
          chargeCount++;
        }
        this.logger.info('chargeCount', chargeCount);

    const shellTicket: ShellTicketView = {
      violationTicketNumber: invoiceIdField.value
        ? String(invoiceIdField.value)
        : '',
      violationTime: invoiceTimeField.value
        ? invoiceTimeField.value.replace('.', ':')
        : '',
      violationDate: new Date(invoiceDateField.value),

      lastName: surnameField.value
        ? surnameField.value
        : '',
      givenNames: givenNameField.value
        ? givenNameField.value
        : '',
      birthdate: '',
      gender: '',
      address: '',
      city: '',
      province: '',
      postalCode: '',
      driverLicenseNumber: driverLicenceField.value
        ? driverLicenceField.value
        : '',
      driverLicenseProvince: '',
      courtHearingLocation: '',
      detachmentLocation: detachmentLocationField.value
        ? detachmentLocationField.value
        : '',

      count1Charge: count1ActRegsField.value
        ? count1ActRegsField.value
        : '',
      _count1ChargeDesc: count1DescField.value
        ? count1DescField.value
        : '',
      _count1ChargeSection: count1SectionField.value
        ? count1SectionField.value.replace(/\s/g, '')
        : '',
      count1FineAmount: count1TicketAmountField.value
        ? Number(count1TicketAmountField.value)
        : 0,
      count2Charge: count2ActRegsField.value
        ? count2ActRegsField.value
        : '',
      _count2ChargeDesc: count2DescField.value
        ? count2DescField.value
        : '',
      _count2ChargeSection: count2SectionField.value
        ? count2SectionField.value.replace(/\s/g, '')
        : '',
      count2FineAmount: count2TicketAmountField.value
        ? Number(count2TicketAmountField.value)
        : 0,
      count3Charge: count3ActRegsField.value
        ? count3ActRegsField.value
        : '',
      _count3ChargeDesc: count3DescField.value
        ? count3DescField.value
        : '',
      _count3ChargeSection: count3SectionField.value
        ? count3SectionField.value.replace(/\s/g, '')
        : '',
      count3FineAmount: count3TicketAmountField.value
        ? Number(count3TicketAmountField.value)
        : 0,
      _chargeCount: chargeCount,
      _amountOwing: 0,
    };

        this.logger.info('before', { ...shellTicket });

        // shellTicket.count1Charge = this.findMatchingCharge(
        //   shellTicket._count1ChargeDesc,
        //   shellTicket._count1ChargeSection
        // );
        // shellTicket.count2Charge = this.findMatchingCharge(
        //   shellTicket._count2ChargeDesc,
        //   shellTicket._count2ChargeSection
        // );
        // shellTicket.count3Charge = this.findMatchingCharge(
        //   shellTicket._count3ChargeDesc,
        //   shellTicket._count3ChargeSection
        // );

        // console.log('after', { ...shellTicket });

        // delete shellTicket._count1ChargeDesc;
        // delete shellTicket._count2ChargeDesc;
        // delete shellTicket._count3ChargeDesc;
        delete shellTicket._count1ChargeSection;
        delete shellTicket._count2ChargeSection;
        delete shellTicket._count3ChargeSection;

        this.form.setValue(shellTicket);
        // this.disputeService.shellTicket$.next(shellTicket);

        this.progressRef.complete();
      // });
    // };
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

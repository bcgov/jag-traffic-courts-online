import { formatCurrency } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {
  AzureKeyCredential,
  FormPollerLike,
  FormRecognizerClient,
} from '@azure/ai-form-recognizer';
import { LoggerService } from '@core/services/logger.service';
import { ShellTicket } from '@shared/models/shellTicket.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-ticket-image',
  templateUrl: './ticket-image.component.html',
  styleUrls: ['./ticket-image.component.scss'],
})
export class TicketImageComponent implements OnInit {
  // public busy: Subscription;
  public busy: Promise<any>;
  public formInfo: any;

  public imageSrc: string;
  public myForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    file: new FormControl('', [Validators.required]),
    fileSource: new FormControl('', [Validators.required]),
  });

  @Input()
  requiredFileType: string;

  public fileName = '';
  public uploadProgress: number;
  public uploadSub: Subscription;

  constructor(
    private disputeService: DisputeService,
    private router: Router,
    private logger: LoggerService
  ) {}

  ngOnInit(): void {}

  public onFileChange(event: any) {
    if (!event.target.files[0] || event.target.files[0].length === 0) {
      this.logger.info('You must select an image');
      return;
    }

    const mimeType = event.target.files[0].type;

    if (mimeType.match(/image\/*/) == null) {
      this.logger.info('Only images are supported');
      return;
    }

    const reader = new FileReader();
    const file: File = event.target.files[0];
    this.logger.info('file target', event.target.files[0]);
    this.fileName = file.name;
    reader.readAsDataURL(file);
    this.logger.info('file', file.name, file.lastModified);

    reader.onload = () => {
      this.imageSrc = reader.result as string;

      this.formInfo = null;
      this.recognizeContent(file);

      this.myForm.patchValue({
        fileSource: reader.result,
      });
    };
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
        // this.formInfo = invoice;

        const invoiceIdFieldIndex = 'violation ticket number';
        const invoiceIdField = invoice.fields[invoiceIdFieldIndex];
        if (invoiceIdField.valueType === 'string') {
          this.logger.info(
            `  violation ticket number: '${
              invoiceIdField.value || '<missing>'
            }', with confidence of ${invoiceIdField.confidence}`
          );
        }
        const surnameFieldIndex = 'surname';
        const surnameField = invoice.fields[surnameFieldIndex];
        if (surnameField.valueType === 'string') {
          this.logger.info(
            `  surname: '${
              surnameField.valueData?.text || '<missing>'
            }', with confidence of ${surnameField.confidence}`
          );
        }
        const givenNameFieldIndex = 'given name';
        const givenNameField = invoice.fields[givenNameFieldIndex];
        if (givenNameField.valueType === 'string') {
          this.logger.info(
            `  given name: '${
              givenNameField.valueData?.text || '<missing>'
            }', with confidence of ${givenNameField.confidence}`
          );
        }
        const count1DescFieldIndex = 'count 1 description';
        const count1DescField = invoice.fields[count1DescFieldIndex];
        if (count1DescField.valueType === 'string') {
          this.logger.info(
            `  count 1 description: '${
              count1DescField.valueData?.text || '<missing>'
            }', with confidence of ${count1DescField.confidence}`
          );
        }
        const count1SectionFieldIndex = 'count 1 section';
        const count1SectionField = invoice.fields[count1SectionFieldIndex];
        if (count1SectionField.valueType === 'string') {
          this.logger.info(
            `  count 1 section: '${
              count1SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count1SectionField.confidence}`
          );
        }
        const count1TicketAmountFieldIndex = 'count 1 ticket amount';
        const count1TicketAmountField =
          invoice.fields[count1TicketAmountFieldIndex];
        if (count1TicketAmountField.valueType === 'number') {
          this.logger.info(
            `  count 1 ticket amount: '${
              count1TicketAmountField.value || '<missing>'
            }', with confidence of ${count1TicketAmountField.confidence}`
          );
        }
        const count2DescFieldIndex = 'count 2 description';
        const count2DescField = invoice.fields[count2DescFieldIndex];
        if (count2DescField.valueType === 'string') {
          this.logger.info(
            `  count 2 description: '${
              count2DescField.valueData?.text || '<missing>'
            }', with confidence of ${count2DescField.confidence}`
          );
        }
        const count2SectionFieldIndex = 'count 2 section';
        const count2SectionField = invoice.fields[count2SectionFieldIndex];
        if (count2SectionField.valueType === 'string') {
          this.logger.info(
            `  count 2 section: '${
              count2SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count2SectionField.confidence}`
          );
        }
        const count2TicketAmountFieldIndex = 'count 2 ticket amount';
        const count2TicketAmountField =
          invoice.fields[count2TicketAmountFieldIndex];
        if (count2TicketAmountField.valueType === 'number') {
          this.logger.info(
            `  count 2 ticket amount: '${
              count2TicketAmountField.value || '<missing>'
            }', with confidence of ${count2TicketAmountField.confidence}`
          );
        }
        const count3DescFieldIndex = 'count 3 description';
        const count3DescField = invoice.fields[count3DescFieldIndex];
        if (count3DescField.valueType === 'string') {
          this.logger.info(
            `  count 3 description: '${
              count3DescField.valueData?.text || '<missing>'
            }', with confidence of ${count3DescField.confidence}`
          );
        }
        const count3SectionFieldIndex = 'count 3 section';
        const count3SectionField = invoice.fields[count3SectionFieldIndex];
        if (count3SectionField.valueType === 'string') {
          this.logger.info(
            `  count 3 section: '${
              count3SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count3SectionField.confidence}`
          );
        }
        const count3TicketAmountFieldIndex = 'count 3 ticket amount';
        const count3TicketAmountField =
          invoice.fields[count3TicketAmountFieldIndex];
        if (count3TicketAmountField.valueType === 'number') {
          this.logger.info(
            `  count 3 ticket amount: '${
              count3TicketAmountField.value || '<missing>'
            }', with confidence of ${count3TicketAmountField.confidence}`
          );
        }

        this.formInfo = [
          {
            label: 'Ticket Number',
            data: invoiceIdField.value,
            confidence: invoiceIdField.confidence,
          },
          {
            label: 'Surname',
            data: surnameField.valueData?.text,
            confidence: surnameField.confidence,
          },
          {
            label: 'Given Name',
            data: givenNameField.valueData?.text,
            confidence: givenNameField.confidence,
          },
          {
            label: 'Count 1 Description',
            data: count1DescField.valueData?.text,
            confidence: count1DescField.confidence,
          },
          {
            label: 'Count 1 Section',
            data: count1SectionField.valueData?.text
              ? count1SectionField.valueData?.text?.replace(/\s/g, '')
              : '',
            confidence: count1SectionField.confidence,
          },
          {
            label: 'Count 1 Ticket Amount',
            data: count1TicketAmountField.value
              ? formatCurrency(
                  Number(count1TicketAmountField.value),
                  'en-US',
                  '$'
                )
              : '',
            confidence: count1TicketAmountField.confidence,
          },
          {
            label: 'Count 2 Description',
            data: count2DescField.valueData?.text,
            confidence: count2DescField.confidence,
          },
          {
            label: 'Count 2 Section',
            data: count2SectionField.valueData?.text
              ? count2SectionField.valueData?.text?.replace(/\s/g, '')
              : '',
            confidence: count2SectionField.confidence,
          },
          {
            label: 'Count 2 Ticket Amount',
            data: count2TicketAmountField.value
              ? formatCurrency(
                  Number(count2TicketAmountField.value),
                  'en-US',
                  '$'
                )
              : '',
            confidence: count2TicketAmountField.confidence,
          },
          {
            label: 'Count 3 Description',
            data: count3DescField.valueData?.text,
            confidence: count3DescField.confidence,
          },
          {
            label: 'Count 3 Section',
            data: count3SectionField.valueData?.text
              ? count3SectionField.valueData?.text?.replace(/\s/g, '')
              : '',
            confidence: count3SectionField.confidence,
          },
          {
            label: 'Count 3 Ticket Amount',
            data: count3TicketAmountField.value
              ? formatCurrency(
                  Number(count3TicketAmountField.value),
                  'en-US',
                  '$'
                )
              : '',
            confidence: count3TicketAmountField.confidence,
          },
        ];

        let chargeCount = 0;
        if (
          count1DescField.valueData?.text ||
          count1SectionField.valueData?.text ||
          count1TicketAmountField.value
        ) {
          // this.logger.info('1', count1DescField.valueData?.text);
          // this.logger.info('1', count1SectionField.valueData?.text);
          // this.logger.info('1', count1TicketAmountField.value);
          chargeCount++;
        }
        if (
          count2DescField.valueData?.text ||
          count2SectionField.valueData?.text ||
          count2TicketAmountField.value
        ) {
          // this.logger.info('2', count2DescField.valueData?.text);
          // this.logger.info('2', count2SectionField.valueData?.text);
          // this.logger.info('2', count2TicketAmountField.value);
          chargeCount++;
        }
        if (
          count3DescField.valueData?.text ||
          count3SectionField.valueData?.text ||
          count3TicketAmountField.value
        ) {
          // this.logger.info('3', count3DescField.valueData?.text);
          // this.logger.info('3', count3SectionField.valueData?.text);
          // this.logger.info('3', count3TicketAmountField.value);
          chargeCount++;
        }
        this.logger.info('chargeCount', chargeCount);

        const shellTicket: ShellTicket = {
          violationTicketNumber: String(invoiceIdField.value),
          violationTime: '',
          violationDate: '',

          lastName: surnameField.valueData?.text
            ? surnameField.valueData?.text
            : '',
          givenNames: givenNameField.valueData?.text
            ? givenNameField.valueData?.text
            : '',
          driverLicenseNumber: '',
          birthdate: '',
          gender: '',
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
            ? String(count1TicketAmountField.value)
            : '',
          count2Charge: null,
          _count2ChargeDesc: count2DescField.valueData?.text
            ? count2DescField.valueData?.text
            : '',
          _count2ChargeSection: count2SectionField.valueData?.text
            ? count2SectionField.valueData?.text.replace(/\s/g, '')
            : '',
          count2FineAmount: count2TicketAmountField.value
            ? String(count2TicketAmountField.value)
            : '',
          count3Charge: null,
          _count3ChargeDesc: count3DescField.valueData?.text
            ? count3DescField.valueData?.text
            : '',
          _count3ChargeSection: count3SectionField.valueData?.text
            ? count3SectionField.valueData?.text.replace(/\s/g, '')
            : '',
          count3FineAmount: count3TicketAmountField.value
            ? String(count3TicketAmountField.value)
            : '',

          chargeCount,
          amountOwing: 0,
        };

        this.logger.info('before', { ...shellTicket });
        this.disputeService.shellTicket$.next(shellTicket);
      });
    };
  }

  private onRejected(info) {
    this.logger.info('onRejected', info);
  }

  public onCancel(): void {
    const ticket = this.disputeService.ticket;
    const params = {
      ticketNumber: ticket.violationTicketNumber,
      time: ticket.violationTime,
    };

    this.router.navigate([AppRoutes.disputePath(AppRoutes.SUMMARY)], {
      queryParams: params,
    });
  }
}

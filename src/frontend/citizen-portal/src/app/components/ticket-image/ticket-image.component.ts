import { formatCurrency } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {
  AzureKeyCredential,
  FormPollerLike,
  FormRecognizerClient,
} from '@azure/ai-form-recognizer';
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
    protected router: Router
  ) {}

  ngOnInit(): void {}

  public onFileChange(event: any) {
    if (!event.target.files[0] || event.target.files[0].length === 0) {
      console.log('You must select an image');
      return;
    }

    const mimeType = event.target.files[0].type;

    if (mimeType.match(/image\/*/) == null) {
      console.log('Only images are supported');
      return;
    }

    const reader = new FileReader();
    const file: File = event.target.files[0];
    console.log('file target', event.target.files[0]);
    this.fileName = file.name;
    reader.readAsDataURL(file);
    console.log('file', file.name, file.lastModified);

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
          console.log(`analyzing status: ${state.status}`);
        },
      }
    );

    // const poller = client.beginRecognizeInvoices(imageSource, {
    //   onProgress: (state) => {
    //     console.log(`analyzing status: ${state.status}`);
    //   },
    // });

    this.busy = poller;

    poller.then(this.onFulfilled(), this.onRejected);
  }

  private onFulfilled(): (poller: FormPollerLike) => void {
    return (poller: FormPollerLike) => {
      this.busy = poller.pollUntilDone().then(() => {
        // console.log('result', poller.getResult());
        const invoices = poller.getResult();

        if (!invoices || invoices.length <= 0) {
          throw new Error('Expecting at least one invoice in analysis result');
        }

        const invoice = invoices[0];
        console.log('First invoice:', invoice);
        // this.formInfo = invoice;

        const invoiceIdField = invoice.fields['violation ticket number'];
        if (invoiceIdField.valueType === 'string') {
          console.log(
            `  violation ticket number: '${
              invoiceIdField.value || '<missing>'
            }', with confidence of ${invoiceIdField.confidence}`
          );
        }
        const surnameField = invoice.fields['surname'];
        if (surnameField.valueType === 'string') {
          console.log(
            `  surname: '${
              surnameField.valueData?.text || '<missing>'
            }', with confidence of ${surnameField.confidence}`
          );
        }
        const givenNameField = invoice.fields['given name'];
        if (givenNameField.valueType === 'string') {
          console.log(
            `  given name: '${
              givenNameField.valueData?.text || '<missing>'
            }', with confidence of ${givenNameField.confidence}`
          );
        }
        const count1DescField = invoice.fields['count 1 description'];
        if (count1DescField.valueType === 'string') {
          console.log(
            `  count 1 description: '${
              count1DescField.valueData?.text || '<missing>'
            }', with confidence of ${count1DescField.confidence}`
          );
        }
        const count1SectionField = invoice.fields['count 1 section'];
        if (count1SectionField.valueType === 'string') {
          console.log(
            `  count 1 section: '${
              count1SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count1SectionField.confidence}`
          );
        }
        const count1TicketAmountField = invoice.fields['count 1 ticket amount'];
        if (count1TicketAmountField.valueType === 'number') {
          console.log(
            `  count 1 ticket amount: '${
              count1TicketAmountField.value || '<missing>'
            }', with confidence of ${count1TicketAmountField.confidence}`
          );
        }
        const count2DescField = invoice.fields['count 2 description'];
        if (count2DescField.valueType === 'string') {
          console.log(
            `  count 2 description: '${
              count2DescField.valueData?.text || '<missing>'
            }', with confidence of ${count2DescField.confidence}`
          );
        }
        const count2SectionField = invoice.fields['count 2 section'];
        if (count2SectionField.valueType === 'string') {
          console.log(
            `  count 2 section: '${
              count2SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count2SectionField.confidence}`
          );
        }
        const count2TicketAmountField = invoice.fields['count 2 ticket amount'];
        if (count2TicketAmountField.valueType === 'number') {
          console.log(
            `  count 2 ticket amount: '${
              count2TicketAmountField.value || '<missing>'
            }', with confidence of ${count2TicketAmountField.confidence}`
          );
        }
        const count3DescField = invoice.fields['count 3 description'];
        if (count3DescField.valueType === 'string') {
          console.log(
            `  count 3 description: '${
              count3DescField.valueData?.text || '<missing>'
            }', with confidence of ${count3DescField.confidence}`
          );
        }
        const count3SectionField = invoice.fields['count 3 section'];
        if (count3SectionField.valueType === 'string') {
          console.log(
            `  count 3 section: '${
              count3SectionField.valueData?.text || '<missing>'
            }', with confidence of ${count3SectionField.confidence}`
          );
        }
        const count3TicketAmountField = invoice.fields['count 3 ticket amount'];
        if (count3TicketAmountField.valueType === 'number') {
          console.log(
            `  count 3 ticket amount: '${
              count3TicketAmountField.value || '<missing>'
            }', with confidence of ${count3TicketAmountField.confidence}`
          );
        }

        console.log('111');
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
        console.log('222');

        let chargeCount = 0;
        if (
          count1DescField.valueData?.text ||
          count1SectionField.valueData?.text ||
          count1TicketAmountField.value
        ) {
          console.log('aaa', count1DescField.valueData?.text);
          console.log('aaa', count1SectionField.valueData?.text);
          console.log('aaa', count1TicketAmountField.value);
          chargeCount++;
        }
        if (
          count2DescField.valueData?.text ||
          count2SectionField.valueData?.text ||
          count2TicketAmountField.value
        ) {
          console.log('bbb', count2DescField.valueData?.text);
          console.log('bbb', count2SectionField.valueData?.text);
          console.log('bbb', count2TicketAmountField.value);
          chargeCount++;
        }
        if (
          count3DescField.valueData?.text ||
          count3SectionField.valueData?.text ||
          count3TicketAmountField.value
        ) {
          console.log('ccc', count3DescField.valueData?.text);
          console.log('ccc', count3SectionField.valueData?.text);
          console.log('ccc', count3TicketAmountField.value);
          chargeCount++;
        }
        console.log('333', 'chargeCount', chargeCount);

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

          chargeCount: chargeCount,
          amountOwing: 0,
        };
        console.log('444');

        console.log('before', { ...shellTicket });
        this.disputeService.shellTicket$.next(shellTicket);
      });
    };
  }

  private onRejected(info) {
    console.log('onRejected', info);
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

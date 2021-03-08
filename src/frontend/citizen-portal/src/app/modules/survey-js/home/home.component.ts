import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { SurveyResourceService } from '@survey/services/survey-resource.service';
import { SurveyJson } from 'tests/survey';
import * as Survey from 'survey-angular';
import * as widgets from 'surveyjs-widgets';
import { PhonePipe } from '@shared/pipes/phone.pipe';
import { Dispute } from '@shared/models/dispute.model';
import { Ticket } from '@shared/models/ticket.model';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { Subscription, timer } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ToastService } from '@core/services/toast.service';
import { DisputeRoutes } from '@dispute/dispute.routes';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  providers: [FormatDatePipe, PhonePipe],
})
export class HomeComponent implements OnInit {
  public busy: Subscription;
  public dispute: Dispute;

  constructor(
    private logger: LoggerService,
    private dialog: MatDialog,
    private router: Router,
    private toastService: ToastService,
    private datePipe: FormatDatePipe,
    private phonePipe: PhonePipe,
    private surveyResource: SurveyResourceService
  ) {}

  ngOnInit() {
    Survey.StylesManager.applyTheme('bootstrap');
    widgets.inputmask(Survey);

    Survey.defaultBootstrapCss.signaturepad.clearButton =
      'btn btn-outline-secondary';

    const tcoSurvey = new Survey.Model(SurveyJson);
    tcoSurvey.showCompletedPage = false;
    tcoSurvey.completeText = 'Submit Dispute';

    this.surveyResource.getDispute().subscribe((dispute) => {
      this.dispute = dispute;
      const ticket = dispute.ticket;

      tcoSurvey.setValue(
        'info_violationTicketNumber',
        ticket.violationTicketNumber
      );

      tcoSurvey.setValue(
        'info_violationDate',
        this.datePipe.transform(ticket.violationDate?.toString()) +
          ' ' +
          ticket.violationTime
      );
      tcoSurvey.setValue('info_surname', ticket.surname);
      tcoSurvey.setValue('info_givenNames', ticket.givenNames);
      tcoSurvey.setValue('info_mailing', ticket.mailing);
      tcoSurvey.setValue('info_postal', ticket.postal);
      tcoSurvey.setValue('info_city', ticket.city);
      tcoSurvey.setValue('info_province', ticket.province);
      tcoSurvey.setValue('info_license', ticket.license);
      tcoSurvey.setValue('info_provLicense', ticket.provLicense);
      tcoSurvey.setValue('info_homePhone', ticket.homePhone);
      tcoSurvey.setValue('info_workPhone', ticket.workPhone);
      tcoSurvey.setValue(
        'info_birthdate',
        this.datePipe.transform(ticket.birthdate?.toString())
      );

      tcoSurvey.setValue(
        'info_party',
        ticket.givenNames +
          ' ' +
          ticket.surname +
          '\nBirthdate: ' +
          this.datePipe.transform(ticket.birthdate?.toString()) +
          '\n\nDriver License: ' +
          ticket.license +
          ' ' +
          ticket.provLicense
      );

      tcoSurvey.setValue(
        'info_address',
        ticket.mailing +
          '\n' +
          ticket.city +
          ' ' +
          ticket.province +
          ' ' +
          ticket.postal +
          '\nHome: ' +
          this.phonePipe.transform(ticket.homePhone) +
          '\nWork: ' +
          this.phonePipe.transform(ticket.workPhone)
      );

      const numberOfCounts = ticket.counts?.length;
      tcoSurvey.setValue('numberOfCounts', ticket.counts?.length);

      ticket?.counts.forEach((cnt) => {
        const question = tcoSurvey.getQuestionByName(
          'alert_info_count' + cnt.countNo
        );
        question.html =
          `<div>
          <h2 class="mb-1">Offence #` +
          cnt.countNo +
          `<small class="ml-2">` +
          cnt.description +
          `</small>
          </h2>
          <hr class="m-0" style="border: 1px solid #ffb200;" />
        </div>`;
      });

      Survey.SurveyNG.render('surveyContainer', {
        model: tcoSurvey,
      });
    });

    // let ticketYn = survey.getValue('ticketYn');
    // survey.setValue('disputeYn', false);
    // survey.focusQuestion("disputeYn");

    tcoSurvey.onCompleting.add((survey, options) => {
      this.logger.info(
        'onCompleting Survey JSON:',
        JSON.stringify(survey.data, null, 3)
      );

      if (!!tcoSurvey.isConfirming) {
        return;
      }
      tcoSurvey.isConfirming = true;
      options.allowComplete = false;

      const data: DialogOptions = {
        title: 'Submit Dispute',
        message:
          'When your dispute is submitted for adjudication, it can no longer be updated. Are you ready to submit your dispute?',
        actionText: 'Submit Dispute',
      };

      this.dialog
        .open(ConfirmDialogComponent, { data })
        .afterClosed()
        .subscribe((response: boolean) => {
          if (response) {
            const source = timer(1000);
            this.busy = source.subscribe((val) => {
              tcoSurvey.doComplete();
              this.toastService.openSuccessToast('Dispute has been submitted');
              this.router.navigate([
                DisputeRoutes.routePath(DisputeRoutes.LIST),
              ]);
            });
          } else {
            tcoSurvey.isConfirming = false;
          }
        });
    });

    tcoSurvey.onComplete.add((survey) => {
      this.logger.info(
        'onComplete Survey JSON:',
        JSON.stringify(survey.data, null, 3)
      );
    });
  }
}

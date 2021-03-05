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

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  providers: [FormatDatePipe, PhonePipe],
})
export class HomeComponent implements OnInit {
  public isComplete = false;

  constructor(
    private logger: LoggerService,
    private route: Router,
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

    this.surveyResource.getDispute().subscribe((dispute) => {
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
        if (cnt.countNo === 1) {
          question.html =
            '<div class="alert alert-primary"><h1 class="alert-heading">Violation Ticket Offences</h1>' +
            '<p class="mt-2 mb-0">Look at each of the offences on your ticket and please answer the following questions.</p></div>' +
            '<br/><br/><div class="alert alert-primary"><h1 class="alert-heading">Offence #' +
            cnt.countNo +
            '<small>' +
            cnt.description +
            '</small></h1>' +
            '</div>';
        } else {
          question.html =
            '<div class="alert alert-primary"><h1 class="alert-heading">Offence #' +
            cnt.countNo +
            '<small>' +
            cnt.description +
            '</small></h1></div>';
        }
      });

      Survey.SurveyNG.render('surveyContainer', {
        model: tcoSurvey,
      });
    });

    // let ticketYn = survey.getValue('ticketYn');
    // survey.setValue('disputeYn', false);
    // survey.focusQuestion("disputeYn");

    tcoSurvey.onComplete.add((survey) => {
      this.logger.info('Survey JSON:', JSON.stringify(survey.data, null, 3));
      this.isComplete = true;
      this.route.navigate(['dispute/list']);
    });
  }
}

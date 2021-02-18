import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { SurveyResourceService } from '@survey/services/survey-resource.service';
import { SurveyJson } from 'tests/survey';
import * as Survey from 'survey-angular';
import * as widgets from 'surveyjs-widgets';
import { PhonePipe } from '@shared/pipes/phone.pipe';

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

    const tcoSurvey = new Survey.Model(SurveyJson);
    Survey.SurveyNG.render('surveyContainer', {
      model: tcoSurvey,
    });

    // let ticketYn = survey.getValue('ticketYn');
    // survey.setValue('disputeYn', false);
    // survey.focusQuestion("disputeYn");

    tcoSurvey.onComplete.add((survey) => {
      this.logger.info('Survey JSON:', JSON.stringify(survey.data, null, 3));

      const payYn = survey.getValue('payYn');
      if (payYn) {
        console.log('go', payYn);
        this.route.navigate(['/']);
        return;
      }

      this.isComplete = true;
    });

    // tcoSurvey.onValidateQuestion.add((survey, options) => {
    //   this.surveyValidateQuestion(survey, options);
    // });

    tcoSurvey.onCurrentPageChanged.add((survey, options) => {
      this.surveyCurrentPageChanged(survey);
    });
  }

  public surveyCurrentPageChanged(survey: Survey.SurveyModel): void {
    this.logger.info(
      'surveyCurrentPageChanged',
      survey.currentPage.name,
      JSON.stringify(survey.data, null, 3)
    );

    const pageName = survey.currentPage.name;
    if (pageName === 'page2') {
      this.surveyResource.getTicket().subscribe((response) => {
        survey.setValue(
          'info_violationTicketNumber',
          response.violationTicketNumber
        );

        survey.setValue(
          'info_violationDate',
          this.datePipe.transform(response.violationDate?.toString())
        );
        survey.setValue('info_courtLocation', response.courtLocation);
        survey.setValue('info_surname', response.surname);
        survey.setValue('info_givenNames', response.givenNames);
        survey.setValue('info_mailing', response.mailing);
        survey.setValue('info_postal', response.postal);
        survey.setValue('info_city', response.city);
        survey.setValue('info_province', response.province);
        survey.setValue('info_license', response.license);
        survey.setValue('info_provLicense', response.provLicense);
        survey.setValue('info_homePhone', response.homePhone);
        survey.setValue('info_workPhone', response.workPhone);
        survey.setValue(
          'info_birthdate',
          this.datePipe.transform(response.birthdate?.toString())
        );

        survey.setValue(
          'info_party',
          response.givenNames +
            ' ' +
            response.surname +
            '\nBirthdate: ' +
            this.datePipe.transform(response.birthdate?.toString()) +
            '\n\nDriver License: ' +
            response.license +
            ' ' +
            response.provLicense
        );

        survey.setValue(
          'info_address',
          response.mailing +
            '\n' +
            response.city +
            ' ' +
            response.province +
            ' ' +
            response.postal +
            '\nHome: ' +
            this.phonePipe.transform(response.homePhone) +
            '\nWork: ' +
            this.phonePipe.transform(response.workPhone)
        );

        let numberOfCounts = response.counts?.length;
        survey.setValue('numberOfCounts', response.counts?.length);

        response.counts.forEach((cnt) => {
          let question = survey.getQuestionByName(
            'alert_info_count' + cnt.countNo
          );
          question.html =
            '<div class="alert alert-primary"><h1>Count #' +
            cnt.countNo +
            '<small>' +
            cnt.description +
            '</small>';
          '</h1>' + '</div>';
        });
      });
    }
  }

  // public surveyValidateQuestion(
  //   survey: Survey.SurveyModel,
  //   options: any
  // ): void {
  // if (options.name === 'violationTicketNumber') {
  //   this.logger.info('surveyValidateQuestion', options);
  //   // const violationTicketNumber = options.value;
  //   this.surveyResource.getTicket().subscribe((response) => {
  //     if (!response) {
  //       options.error = 'There was a problem finding your ticket';
  //       this.logger.info('surveyValidateQuestion after', options);
  //       return;
  //     }
  //   });
  // }
  // }
}

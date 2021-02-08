import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { SurveyResourceService } from '@survey/services/survey-resource.service';
import { SurveyJson } from 'tests/survey';
import * as Survey from 'survey-angular';
import * as widgets from 'surveyjs-widgets';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  providers: [FormatDatePipe],
})
export class HomeComponent implements OnInit {
  public isComplete: boolean = false;

  constructor(
    private logger: LoggerService,
    private route: Router,
    private formatDatePipe: FormatDatePipe,
    private surveyResource: SurveyResourceService
  ) {}

  ngOnInit() {
    Survey.StylesManager.applyTheme('bootstrap');

    widgets.inputmask(Survey);

    const survey = new Survey.Model(SurveyJson);
    Survey.SurveyNG.render('surveyContainer', {
      model: survey,
    });

    // let ticketYn = survey.getValue('ticketYn');
    // survey.setValue('disputeYn', false);
    // survey.focusQuestion("disputeYn");

    survey.onComplete.add((survey) => {
      this.logger.info('Survey JSON:', JSON.stringify(survey.data, null, 3));

      let payYn = survey.getValue('payYn');
      if (payYn) {
        console.log('go', payYn);
        this.route.navigate(['/']);
        return;
      }

      this.isComplete = true;
    });

    survey.onValidateQuestion.add((survey, options) => {
      this.surveyValidateQuestion(survey, options);
    });

    survey.onCurrentPageChanged.add((survey, options) => {
      this.logger.info(
        'onCurrentPageChanged',
        survey.currentPage.name,
        JSON.stringify(survey.data, null, 3)
      );

      let pageName = survey.currentPage.name;
      if (pageName === 'page2') {
        this.surveyResource.ticket().subscribe((response) => {
          survey.setValue(
            'info_violationTicketNumber',
            response.violationTicketNumber
          );

          survey.setValue(
            'info_violationDate',
            this.formatDatePipe.transform(response.violationDate)
          );
          survey.setValue('info_courtLocation', response.courtLocation);
          survey.setValue('info_surname', response.surname);
          survey.setValue('info_givenNames', response.givenNames);
        });
      }
    });
  }

  public surveyValidateQuestion(
    survey: Survey.SurveyModel,
    options: any
  ): void {
    if (options.name == 'violationTicketNumber') {
      this.logger.info('surveyValidateQuestion', options);

      var violationTicketNumber = options.value;
      this.surveyResource.test(violationTicketNumber).subscribe((response) => {
        const key = 'success';
        if (!response[key]) {
          options.error = 'There was a problem finding your ticket';
          this.logger.info('surveyValidateQuestion after', options);

          return;
        }
      });
    }
  }
}

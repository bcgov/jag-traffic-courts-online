import { Component, OnInit } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import * as Survey from 'survey-angular';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  public isComplete = false;
  public json: any = {
    showProgressBar: 'top',
    pages: [
      {
        questions: [
          {
            name: 'fullname',
            type: 'text',
            title: 'What is your name',
            isRequired: true,
          },
          {
            name: 'birthdate',
            type: 'text',
            inputType: 'date',
            title: 'What is your birth date?',
            isRequired: false,
          },
          {
            name: 'emailaddr',
            type: 'text',
            inputType: 'email',
            title: 'What is your email address?',
            placeHolder: '',
            isRequired: false,
            validators: [
              {
                type: 'email',
              },
            ],
          },
        ],
      },
      {
        title: 'Page 2 Set of Questions',
        questions: [
          {
            type: 'checkbox',
            name: 'opSystem',
            title: 'What operating system do you use?',
            hasOther: true,
            isRequired: false,
            choices: ['Windows', 'Linux', 'Macintosh OSX'],
          },
          {
            type: 'dropdown',
            name: 'cars',
            title: 'What type of car do you drive?',
            isRequired: false,
            hasNone: true,
            hasOther: true,
            colCount: 4,
            choices: [
              'Ford',
              'Volkswagen',
              'Nissan',
              'Audi',
              'Mercedes-Benz',
              'BMW',
              'Toyota',
            ],
          },
          {
            type: 'checkbox',
            name: 'car',
            title: 'Car checkbox',
            hasSelectAll: true,
            isRequired: false,
            hasNone: true,
            colCount: 4,
            choices: [
              'Ford',
              'Volkswagen',
              'Nissan',
              'Audi',
              'Mercedes-Benz',
              'BMW',
              'Toyota',
            ],
          },
          {
            type: 'radiogroup',
            name: 'carss',
            title: 'Car radiogroup',
            isRequired: false,
            colCount: 4,
            choices: [
              'None',
              'Ford',
              'Volkswagen',
              'Nissan',
              'Audi',
              'Mercedes-Benz',
              'BMW',
              'Toyota',
            ],
          },
        ],
      },
      {
        questions: [
          {
            type: 'boolean',
            name: 'bool',
            title: 'Boolean',
            label: 'Are you 18 or older?',
            isRequired: false,
          },
          {
            type: 'matrix',
            name: 'Quality',
            title: 'The product',
            columns: [
              {
                value: 1,
                text: 'Strongly Disagree',
              },
              {
                value: 2,
                text: 'Disagree',
              },
              {
                value: 3,
                text: 'Neutral',
              },
              {
                value: 4,
                text: 'Agree',
              },
              {
                value: 5,
                text: 'Strongly Agree',
              },
            ],
            rows: [
              {
                value: 'affordable',
                text: 'Product is affordable',
              },
              {
                value: 'does what it claims',
                text: 'Product does what it claims',
              },
              {
                value: 'better than others',
                text: 'Product is better than other products on the market',
              },
              {
                value: 'easy to use',
                text: 'Product is easy to use',
              },
            ],
          },
        ],
      },
      {
        name: 'page1',
        elements: [
          {
            type: 'rating',
            name: 'satisfaction',
            title: 'How satisfied are you with the Product?',
            mininumRateDescription: 'Not Satisfied',
            maximumRateDescription: 'Completely satisfied',
          },
          {
            type: 'panel',
            innerIndent: 1,
            name: 'panel1',
            title: 'Please, help us improve our product',
            visibleIf: '{satisfaction} < 3',
            elements: [
              {
                type: 'checkbox',
                choices: [
                  {
                    value: '1',
                    text: 'Customer relationship',
                  },
                  {
                    value: '2',
                    text: 'Service quality',
                  },
                  {
                    value: '3',
                    text: 'Support response time',
                  },
                ],
                name: 'What should be improved?',
              },
              {
                type: 'comment',
                name: 'suggestions',
                title: 'What would make you more satisfied with the Product?',
              },
              {
                type: 'panel',
                innerIndent: 1,
                name: 'panel2',
                title: 'Send us your contact information (optionally)',
                state: 'collapsed',
                elements: [
                  {
                    type: 'text',
                    name: 'name',
                    title: 'Name:',
                  },
                  {
                    type: 'text',
                    inputType: 'email',
                    name: 'email',
                    title: 'E-mail',
                  },
                ],
              },
            ],
          },
        ], // questions
      }, // page
    ], // pages
    completedHtml: '<p><h3>Thank you for completing the survey!</h3></p>',
  };

  constructor(private logger: LoggerService) {}

  ngOnInit() {
    Survey.StylesManager.applyTheme('bootstrap');

    const survey = new Survey.Model(this.json);
    Survey.SurveyNG.render('surveyContainer', { model: survey });

    survey.onComplete.add((result) => {
      this.isComplete = true;
      this.logger.info('Result JSON:', JSON.stringify(result.data, null, 3));
    });
  }
}

import { Component, OnInit } from '@angular/core';
import * as Survey from 'survey-angular';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  constructor() {}

  ngOnInit() {
    Survey.StylesManager.applyTheme('bootstrap');
    var model = new Survey.Model(this.json);
    Survey.SurveyNG.render('surveyContainer', { model: model });
  }

  public json: any = {
    showProgressBar: 'both',
    title: 'Sample Survey',
    logo: 'https://surveyjs.io/Content/Images/examples/image-picker/lion.jpg',
    logoPosition: 'left',
    questions: [
      {
        name: 'name',
        type: 'text',
        title: 'Text',
        placeHolder: 'Jon Snow',
        isRequired: true,
      },
      {
        name: 'birthdate',
        type: 'text',
        inputType: 'date',
        title: 'Text Date',
        isRequired: true,
      },
      {
        name: 'color',
        type: 'text',
        inputType: 'color',
        title: 'Text Color',
      },
      {
        name: 'email',
        type: 'text',
        inputType: 'email',
        title: 'Text Email',
        placeHolder: 'jon.snow@nightwatch.org',
        isRequired: true,
        validators: [
          {
            type: 'email',
          },
        ],
      },
      {
        type: 'dropdown',
        name: 'cars',
        title: 'Dropdown',
        isRequired: true,
        hasNone: true,
        colCount: 4,
        choices: [
          'Ford',
          'Vauxhall',
          'Volkswagen',
          'Nissan',
          'Audi',
          'Mercedes-Benz',
          'BMW',
          'Peugeot',
          'Toyota',
          'Citroen',
        ],
      },
      {
        type: 'checkbox',
        name: 'car',
        title: 'Checkbox',
        hasSelectAll: true,
        isRequired: true,
        hasNone: true,
        colCount: 4,
        choices: [
          'Ford',
          'Vauxhall',
          'Volkswagen',
          'Nissan',
          'Audi',
          'Mercedes-Benz',
          'BMW',
          'Peugeot',
          'Toyota',
          'Citroen',
        ],
      },
      {
        type: 'radiogroup',
        name: 'carss',
        title: 'Radiogroup',
        isRequired: true,
        colCount: 4,
        choices: [
          'None',
          'Ford',
          'Vauxhall',
          'Volkswagen',
          'Nissan',
          'Audi',
          'Mercedes-Benz',
          'BMW',
          'Peugeot',
          'Toyota',
          'Citroen',
        ],
      },
      {
        type: 'image',
        name: 'banner',
        imageHeight: '300px',
        imageWidth: '450px',
        imageLink:
          'https://surveyjs.io/Content/Images/examples/image-picker/lion.jpg',
      },
      {
        type: 'imagepicker',
        name: 'choosepicture',
        title: 'Imagepicker',
        imageHeight: '150px',
        imageWidth: '225px',
        choices: [
          {
            value: 'lion',
            imageLink:
              'https://surveyjs.io/Content/Images/examples/image-picker/lion.jpg',
          },
          {
            value: 'giraffe',
            imageLink:
              'https://surveyjs.io/Content/Images/examples/image-picker/giraffe.jpg',
          },
          {
            value: 'panda',
            imageLink:
              'https://surveyjs.io/Content/Images/examples/image-picker/panda.jpg',
          },
          {
            value: 'camel',
            imageLink:
              'https://surveyjs.io/Content/Images/examples/image-picker/camel.jpg',
          },
        ],
      },
      {
        type: 'boolean',
        name: 'bool',
        title: 'Boolean',
        label: 'Are you 21 or older?',
        isRequired: true,
      },
      {
        type: 'matrix',
        name: 'Quality',
        title: 'Matrix',
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
      {
        type: 'matrixdynamic',
        name: 'teachersRate',
        title: 'Matrix Dynamic',
        addRowText: 'Add Subject',
        horizontalScroll: true,
        columnMinWidth: '130px',
        columnColCount: 1,
        cellType: 'radiogroup',
        choices: [
          {
            value: 1,
            text: 'Yes',
          },
          {
            value: 0,
            text: 'Sometimes',
          },
          {
            value: -1,
            text: 'No',
          },
        ],
        columns: [
          {
            name: 'subject',
            cellType: 'dropdown',
            title: 'Select a subject',
            isRequired: true,
            minWidth: '300px',
            choices: [
              'English: American Literature',
              'English: British and World Literature',
              'Math: Consumer Math',
              'Math: Practical Math',
              'Math: Developmental Algebra',
              'Math: Continuing Algebra',
              'Math: Pre-Algebra',
              'Math: Algebra',
              'Math: Geometry',
              'Math: Integrated Mathematics',
              'Science: Physical Science',
              'Science: Earth Science',
              'Science: Biology',
              'Science: Chemistry',
              'History: World History',
              'History: Modern World Studies',
              'History: U.S. History',
              'History: Modern U.S. History',
              'Social Sciences: U.S. Government and Politics',
              'Social Sciences: U.S. and Global Economics',
              'World Languages: Spanish',
              'World Languages: French',
              'World Languages: German',
              'World Languages: Latin',
              'World Languages: Chinese',
              'World Languages: Japanese',
            ],
          },
          {
            name: 'explains',
            title: 'Clearly explains the objectives',
          },
          {
            name: 'interesting',
            title: 'Makes class interesting',
          },
          {
            name: 'effective',
            title: 'Uses class time effectively',
          },
          {
            name: 'knowledge',
            title: 'Knows the subject matter',
          },
          {
            name: 'recognition',
            title: 'Recognizes and acknowledges effort',
          },
          {
            name: 'inform',
            title: 'Keeps me informed of my progress',
          },
          {
            name: 'opinion',
            title: 'Encourages and accepts different opinions',
          },
          {
            name: 'respect',
            title: 'Has the respect of the student',
          },
          {
            name: 'cooperation',
            title: 'Encourages cooperation and participation',
          },
          {
            name: 'parents',
            title: 'Communicates with my parents',
          },
          {
            name: 'selfthinking',
            title: 'Encourages me to think for myself',
          },
          {
            name: 'frusturation',
            cellType: 'comment',
            title: 'Is there anything about this class that frustrates you?',
            minWidth: '250px',
          },
          {
            name: 'likeTheBest',
            cellType: 'comment',
            title: 'What do you like best about this class and/or teacher?',
            minWidth: '250px',
          },
          {
            name: 'improvements',
            cellType: 'comment',
            title:
              'What do you wish this teacher would do differently that would improve this class?',
            minWidth: '250px',
          },
        ],
        rowCount: 2,
      },
      {
        type: 'matrixdynamic',
        name: 'Current Level of Function',
        title: 'Matrix Dynamic (vertical columns)',
        columnLayout: 'vertical',
        minRowCount: 1,
        maxRowCount: 5,
        columns: [
          {
            name: 'Date',
            title: 'Date',
            cellType: 'text',
            inputType: 'date',
          },
          {
            name: 'AmbDistance',
            title: 'Amb Distance',
            cellType: 'text',
          },
          {
            name: 'Amb Assistance',
            cellType: 'dropdown',
            choices: ['D', 'MAX', 'MOD', 'MIN'],
          },
          {
            name: 'Standing Tolerance',
            cellType: 'text',
          },
          {
            name: 'UE Strength',
            cellType: 'text',
          },
          {
            name: 'Cognitive Function',
            cellType: 'comment',
          },
        ],
        choices: [1],
        cellType: 'comment',
        confirmDelete: true,
        addRowText: 'Add Date +',
        removeRowText: 'Remove',
      },
      {
        type: 'matrixdynamic',
        name: 'orderList',
        rowCount: 1,
        minRowCount: 1,
        title: 'Matrix Dynamic (totals)',
        addRowText: 'Add new item',
        columns: [
          {
            name: 'id',
            title: 'Id',
            cellType: 'expression',
            expression: '{rowIndex}',
          },
          {
            name: 'phone_model',
            title: 'Phone model',
            isRequired: true,
            totalType: 'count',
            totalFormat: 'Items count: {0}',
            choices: [
              {
                value: 'iPhone7-32',
                text: 'iPhone 7, 32GB',
                price: 449,
              },
              {
                value: 'iPhone7-128',
                text: 'iPhone 7, 128GB',
                price: 549,
              },
              {
                value: 'iPhone7Plus-32',
                text: 'iPhone 7 Plus, 32GB',
                price: 569,
              },
              {
                value: 'iPhone7Plus-128',
                text: 'iPhone 7 Plus, 128GB',
                price: 669,
              },
              {
                value: 'iPhone8-64',
                text: 'iPhone 8, 64GB',
                price: 599,
              },
              {
                value: 'iPhone8-256',
                text: 'iPhone 8, 256GB',
                price: 749,
              },
              {
                value: 'iPhone8Plus-64',
                text: 'iPhone 8 Plus, 64GB',
                price: 699,
              },
              {
                value: 'iPhone8Plus-256',
                text: 'iPhone 8 Plus, 256GB',
                price: 849,
              },
              {
                value: 'iPhoneXR-64',
                text: 'iPhone XR, 64GB',
                price: 749,
              },
              {
                value: 'iPhoneXR-128',
                text: 'iPhone XR, 128GB',
                price: 799,
              },
              {
                value: 'iPhoneXR-256',
                text: 'iPhone XR, 256GB',
                price: 899,
              },
              {
                value: 'iPhoneXS-64',
                text: 'iPhone XS, 64GB',
                price: 999,
              },
              {
                value: 'iPhoneXS-256',
                text: 'iPhone XS, 256GB',
                price: 1149,
              },
              {
                value: 'iPhoneXS-512',
                text: 'iPhone XS, 512GB',
                price: 1349,
              },
              {
                value: 'iPhoneXSMAX-64',
                text: 'iPhone XS Max, 64GB',
                price: 1099,
              },
              {
                value: 'iPhoneXSMAX-256',
                text: 'iPhone XS Max, 256GB',
                price: 1249,
              },
              {
                value: 'iPhoneXSMAX-512',
                text: 'iPhone XS, 512GB',
                price: 1449,
              },
            ],
          },
          {
            name: 'price',
            title: 'Price',
            cellType: 'expression',
            expression: "getItemPrice('phone_model')",
            displayStyle: 'currency',
          },
          {
            name: 'quantity',
            title: 'Quantity',
            isRequired: true,
            cellType: 'text',
            inputType: 'number',
            totalType: 'sum',
            totalFormat: 'Total phones: {0}',
            validators: [
              {
                type: 'numeric',
                minValue: 1,
                maxValue: 100,
              },
            ],
          },
          {
            name: 'total',
            title: 'Total',
            cellType: 'expression',
            expression: '{row.quantity} * {row.price}',
            displayStyle: 'currency',
            totalType: 'sum',
            totalDisplayStyle: 'currency',
            totalFormat: 'Total: {0}',
          },
        ],
      },
      {
        name: 'vatProcents',
        type: 'text',
        title: 'VAT (in %)',
        defaultValue: 20,
        inputType: 'number',
        validators: [
          {
            type: 'numeric',
            minValue: 0,
            maxValue: 40,
          },
        ],
      },
      {
        name: 'vatTotal',
        type: 'expression',
        title: 'VAT',
        expression: '{orderList-total.total} * {vatProcents} / 100',
        displayStyle: 'currency',
        startWithNewLine: false,
      },
      {
        name: 'total',
        type: 'expression',
        title: 'Total',
        expression: '{orderList-total.total} + {vatTotal}',
        displayStyle: 'currency',
        startWithNewLine: false,
      },
      {
        type: 'multipletext',
        name: 'pricelimit',
        title: 'Multipletext',
        colCount: 2,
        items: [
          {
            name: 'mostamount',
            title: 'Most amount you would every pay for a product like ours',
          },
          {
            name: 'leastamount',
            title: 'The least amount you would feel comfortable paying',
          },
        ],
      },
      {
        type: 'rating',
        name: 'satisfaction',
        title: 'Rating',
        minRateDescription: 'Not Satisfied',
        maxRateDescription: 'Completely satisfied',
      },
      {
        type: 'comment',
        name: 'suggestions',
        title: 'Comment',
      },
      {
        type: 'file',
        title: 'File',
        name: 'image',
        storeDataAsText: false,
        showPreview: true,
        imageWidth: 150,
        maxSize: 102400,
      },
      {
        type: 'panel',
        title: 'Panel',
        innerIndent: 1,
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
        ],
      },
      {
        type: 'paneldynamic',
        name: 'relatives',
        title: 'Panel Dynamic',
        renderMode: 'progressTop',
        templateTitle: 'Information about: {panel.relativeType}',
        templateElements: [
          {
            name: 'relativeType',
            type: 'dropdown',
            title: 'Relative',
            choices: [
              'father',
              'mother',
              'brother',
              'sister',
              'son',
              'daughter',
            ],
            isRequired: true,
          },
          {
            name: 'isalive',
            type: 'radiogroup',
            title: 'Alive?',
            startWithNewLine: false,
            isRequired: true,
            colCount: 0,
            choices: ['Yes', 'No'],
          },
          {
            name: 'liveage',
            type: 'dropdown',
            title: 'Age',
            isRequired: true,
            startWithNewLine: false,
            visibleIf: "{panel.isalive} = 'Yes'",
            choicesMin: 1,
            choicesMax: 115,
          },
          {
            name: 'deceasedage',
            type: 'dropdown',
            title: 'Deceased Age',
            isRequired: true,
            startWithNewLine: false,
            visibleIf: "{panel.isalive} = 'No'",
            choices: [
              {
                value: -1,
                text: 'Unknown',
              },
            ],
            choicesMin: 1,
            choicesMax: 115,
          },
          {
            name: 'causeofdeathknown',
            type: 'radiogroup',
            title: 'Cause of Death Known?',
            isRequired: true,
            colCount: 0,
            startWithNewLine: false,
            visibleIf: "{panel.isalive} = 'No'",
            choices: ['Yes', 'No'],
          },
          {
            name: 'causeofdeath',
            type: 'text',
            title: 'Cause of Death',
            isRequired: true,
            startWithNewLine: false,
            visibleIf:
              "{panel.isalive} = 'No' and {panel.causeofdeathknown} = 'Yes'",
          },
          {
            type: 'panel',
            name: 'moreInfo',
            state: 'expanded',
            title: 'Detail Information about: {panel.relativeType}',
            elements: [
              {
                type: 'matrixdynamic',
                name: 'relativeillness',
                title: 'Describe the illness or condition.',
                rowCount: 0,
                columns: [
                  {
                    name: 'illness',
                    cellType: 'dropdown',
                    title: 'Illness/Condition',
                    choices: [
                      'Cancer',
                      'Heart Disease',
                      'Diabetes',
                      'Stroke/TIA',
                      'High Blood Pressure',
                      'High Cholesterol or Triglycerides',
                      'Liver Disease',
                      'Alcohol or Drug Abuse',
                      'Anxiety, Depression or Psychiatric Illness',
                      'Tuberculosis',
                      'Anesthesia Complications',
                      'Genetic Disorder',
                      'Other – describe',
                    ],
                    isRequired: true,
                  },
                  {
                    name: 'description',
                    cellType: 'text',
                    title: 'Describe',
                    isRequired: true,
                  },
                ],
              },
            ],
          },
        ],
        panelCount: 2,
        panelAddText: 'Add a blood relative',
        panelRemoveText: 'Remove the relative',
      },
      {
        type: 'panel',
        title: 'Expression Example Panel',
        innerIndent: 1,
        elements: [
          {
            type: 'paneldynamic',
            name: 'items',
            title: 'Items',
            keyName: 'name',
            showQuestionNumbers: 'none',
            templateTitle: 'item #{panelIndex}',
            templateElements: [
              {
                type: 'text',
                name: 'name',
                title: 'Name:',
                isRequired: true,
              },
              {
                type: 'text',
                name: 'cost',
                inputType: 'number',
                title: 'Item Cost:',
                isRequired: true,
                startWithNewLine: false,
              },
              {
                type: 'text',
                name: 'vendor',
                title: 'Vendor:',
                isRequired: true,
              },
              {
                type: 'text',
                name: 'quantity',
                inputType: 'number',
                title: 'Quantity:',
                isRequired: true,
                startWithNewLine: false,
              },
              {
                type: 'text',
                name: 'link',
                title: 'Link:',
                isRequired: true,
              },
              {
                type: 'expression',
                name: 'total',
                title: 'Total Item Cost:',
                expression: '{panel.cost} * {panel.quantity}',
                displayStyle: 'currency',
                currency: 'US',
                startWithNewLine: false,
              },
            ],
            minPanelCount: 1,
            panelAddText: 'Add another  item',
            panelRemoveText: 'Remove item',
          },
          {
            type: 'panel',
            title: 'Totals',
            elements: [
              {
                type: 'expression',
                name: 'totalQuantity',
                title: 'Total  Quantity:',
                expression: "sumInArray({items}, 'quantity'",
              },
              {
                type: 'expression',
                name: 'totalCost',
                title: 'Total Cost:',
                expression: "sumInArray({items}, 'total'",
                displayStyle: 'currency',
                currency: 'US',
                startWithNewLine: false,
              },
            ],
          },
        ],
      },
      {
        type: 'radiogroup',
        choices: ['Yes', 'No'],
        isRequired: true,
        name: 'frameworkUsing',
        title: 'Do you use any front-end framework like Bootstrap?',
      },
      {
        type: 'checkbox',
        choices: ['Bootstrap', 'Foundation'],
        hasOther: true,
        isRequired: true,
        name: 'framework',
        title: 'What front-end framework do you use?',
        visibleIf: "{frameworkUsing} = 'Yes'",
      },
      {
        name: 'page2',
        questions: [
          {
            type: 'radiogroup',
            choices: ['Yes', 'No'],
            isRequired: true,
            name: 'mvvmUsing',
            title: 'Do you use any MVVM framework?',
          },
          {
            type: 'checkbox',
            choices: ['AngularJS', 'KnockoutJS', 'React'],
            hasOther: true,
            isRequired: true,
            name: 'mvvm',
            title: 'What MVVM framework do you use?',
            visibleIf: "{mvvmUsing} = 'Yes'",
          },
        ],
      },
      {
        name: 'page3',
        questions: [
          {
            type: 'comment',
            name: 'about',
            title:
              'Please tell us about your main requirements for Survey library',
          },
        ],
      },
      {
        name: 'page4',
        questions: [
          {
            isRequired: true,
            type: 'matrix',
            name: 'Quality',
            title:
              'Please indicate if you agree or disagree with the following statements',
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
                value: 'better then others',
                text: 'Product is better than other products on the market',
              },
              {
                value: 'easy to use',
                text: 'Product is easy to use',
              },
            ],
          },
          {
            type: 'rating',
            name: 'satisfaction',
            title: 'How satisfied are you with the Product?',
            mininumRateDescription: 'Not Satisfied',
            maximumRateDescription: 'Completely satisfied',
          },
          {
            type: 'rating',
            name: 'recommend friends',
            visibleIf: '{satisfaction} > 3',
            title:
              'How likely are you to recommend the Product to a friend or co-worker?',
            mininumRateDescription: 'Will not recommend',
            maximumRateDescription: 'I will recommend',
          },
          {
            type: 'comment',
            name: 'suggestions',
            title: 'What would make you more satisfied with the Product?',
          },
        ],
      },
      {
        name: 'page5',
        questions: [
          {
            type: 'radiogroup',
            isRequired: true,
            name: 'price to competitors',
            title: 'Compared to our competitors, do you feel the Product is',
            choices: [
              'Less expensive',
              'Priced about the same',
              'More expensive',
              'Not sure',
            ],
          },
          {
            isRequired: true,
            type: 'radiogroup',
            name: 'price',
            title: 'Do you feel our current price is merited by our product?',
            choices: [
              'correct|Yes, the price is about right',
              'low|No, the price is too low for your product',
              'high|No, the price is too high for your product',
            ],
          },
          {
            isRequired: true,
            type: 'multipletext',
            name: 'pricelimit',
            title: 'What is the... ',
            items: [
              {
                name: 'mostamount',
                title:
                  'Most amount you would every pay for a product like ours',
              },
              {
                name: 'leastamount',
                title: 'The least amount you would feel comfortable paying',
              },
            ],
          },
        ],
      },
      {
        name: 'page6',
        questions: [
          {
            type: 'text',
            name: 'email',
            title:
              "Thank you for taking our survey. Your survey is almost complete, please enter your email address and then press the 'Submit' button.",
            isRequired: true,
          },
        ],
      },
      {
        name: 'page7',
        questions: [
          {
            isRequired: true,
            type: 'matrix',
            name: 'Quality',
            title:
              'Please indicate if you agree or disagree with the following statements',
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
                value: 'better then others',
                text: 'Product is better than other products on the market',
              },
              {
                value: 'easy to use',
                text: 'Product is easy to use',
              },
            ],
          },
          {
            type: 'rating',
            name: 'satisfaction',
            title: 'How satisfied are you with the Product?',
            mininumRateDescription: 'Not Satisfied',
            maximumRateDescription: 'Completely satisfied',
          },
          {
            type: 'rating',
            name: 'recommend friends',
            visibleIf: '{satisfaction} > 3',
            title:
              'How likely are you to recommend the Product to a friend or co-worker?',
            mininumRateDescription: 'Will not recommend',
            maximumRateDescription: 'I will recommend',
            isRequired: true,
          },
          {
            type: 'comment',
            name: 'suggestions',
            title: 'What would make you more satisfied with the Product?',
            isRequired: true,
          },
        ],
      },
    ],
  };
}

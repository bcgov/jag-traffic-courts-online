export const SurveyJson = {
  showProgressBar: 'top',
  progressBarType: 'buttons',
  showQuestionNumbers: 'off',
  completeText: 'Submit Dispute',
  showCompletedPage: false,
  pages: [
    {
      name: 'page1',
      navigationTitle: 'Violation Ticket',
      navigationDescription: 'Summary',
      elements: [
        {
          name: 'panel1',
          type: 'panel',
          elements: [
            {
              type: 'html',
              name: 'alert_info',
              html: `
                <div>
                  <h2 class="mb-1">Violation Ticket Information</h2>
                  <div class="text-muted m-2">
                  Here is a summary of your ticket information.
                  </div>
                  <hr class="m-0" style="border: 1px solid #ffb200;" />
                </div>
             `,
            },
            {
              name: 'info_violationTicketNumber',
              type: 'text',
              title: 'Violation Ticket Number',
              hideNumber: true,
              readOnly: true,
            },
            {
              name: 'info_violationDate',
              type: 'text',
              title: 'Violation Date and Time',
              startWithNewLine: false,
              hideNumber: true,
              readOnly: true,
            },
            {
              name: 'info_party',
              type: 'comment',
              title: 'Personal Information',
              hideNumber: true,
              readOnly: true,
            },
            {
              name: 'info_address',
              type: 'comment',
              title: 'Address Information',
              startWithNewLine: false,
              hideNumber: true,
              readOnly: true,
            },
          ],
        },
      ],
    },
    {
      name: 'pageCount1',
      navigationTitle: 'Offence #1',
      elements: [
        {
          type: 'panel',
          name: 'panel_count1',
          elements: [
            {
              type: 'html',
              name: 'alert_info_count1',
              html: '',
            },
            {
              type: 'radiogroup',
              name: 'count1',
              title: 'For this offence, which do you agree?',
              isRequired: false,
              choices: [
                {
                  value: 'A',
                  text:
                    'I agree that I committed this offence, and I do not want to appear in court',
                },
                {
                  value: 'B',
                  text:
                    'I agree that I committed this offence, and I want to appear in court',
                },
                {
                  value: 'C',
                  text:
                    'I do not agree that I committed this offence (allegation)',
                },
              ],
            },
            {
              type: 'checkbox',
              name: 'count1A',
              visibleIf: '{count1} = "A"',
              title: 'Which requests would you like to make?',
              isRequired: false,
              choices: [
                {
                  value: 'A1',
                  text: 'I request a reduction of the ticketed amount: and/or',
                },
                {
                  value: 'A2',
                  text: 'I request time to pay the ticketed amount',
                },
              ],
            },
            {
              type: 'comment',
              name: 'count1_reduction_reason',
              title:
                'What are your reasons for a reduction in the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: false,
              visibleIf: '{count1} = "A" and {count1A} contains "A1"',
            },
            {
              type: 'comment',
              name: 'count1_time_reason',
              title:
                'What are your reasons for requiring more time to pay the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: false,
              visibleIf: '{count1} = "A" and {count1A} contains "A2"',
            },
            {
              type: 'checkbox',
              name: 'count1B',
              visibleIf: '{count1} = "B"',
              title: 'Why do you want to appear?',
              isRequired: false,
              choices: [
                {
                  value: 'B1',
                  text: 'To request a reduction of the ticketed amount: and/or',
                },
                {
                  value: 'B2',
                  text: 'To request time to pay the ticketed amount',
                },
              ],
            },
          ],
        },
      ],
    },
    {
      name: 'pageCount2',
      navigationTitle: 'Offence #2',
      visibleIf: '{numberOfCounts} > 1',
      elements: [
        {
          type: 'panel',
          name: 'panel_count2',
          elements: [
            {
              type: 'html',
              name: 'alert_info_count2',
              html: '',
            },
            {
              type: 'radiogroup',
              name: 'count2',
              title: 'For this offence, which do you agree?',
              isRequired: false,
              choices: [
                {
                  value: 'A',
                  text:
                    'I agree that I committed this offence, and I do not want to appear in court',
                },
                {
                  value: 'B',
                  text:
                    'I agree that I committed this offence, and I want to appear in court',
                },
                {
                  value: 'C',
                  text:
                    'I do not agree that I committed this offence (allegation)',
                },
              ],
            },
            {
              type: 'checkbox',
              name: 'count2A',
              visibleIf: '{count2} = "A"',
              title: 'Which requests would you like to make?',
              isRequired: false,
              choices: [
                {
                  value: 'A1',
                  text: 'I request a reduction of the ticketed amount: and/or',
                },
                {
                  value: 'A2',
                  text: 'I request time to pay the ticketed amount',
                },
              ],
            },
            {
              type: 'comment',
              name: 'count2_reduction_reason',
              title:
                'What are your reasons for a reduction in the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: false,
              visibleIf: '{count2} = "A" and {count2A} contains "A1"',
            },
            {
              type: 'comment',
              name: 'count2_time_reason',
              title:
                'What are your reasons for requiring more time to pay the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: false,
              visibleIf: '{count2} = "A" and {count2A} contains "A2"',
            },
            {
              type: 'checkbox',
              name: 'count2B',
              visibleIf: '{count2} = "B"',
              title: 'Why do you want to appear?',
              isRequired: false,
              choices: [
                {
                  value: 'B1',
                  text: 'To request a reduction of the ticketed amount: and/or',
                },
                {
                  value: 'B2',
                  text: 'To request time to pay the ticketed amount',
                },
              ],
            },
          ],
        },
      ],
    },
    {
      name: 'pageCount3',
      navigationTitle: 'Offence #3',
      visibleIf: '{numberOfCounts} > 2',
      elements: [
        {
          type: 'panel',
          name: 'panel_count3',
          elements: [
            {
              type: 'html',
              name: 'alert_info_count3',
              html: '',
            },
            {
              type: 'radiogroup',
              name: 'count3',
              title: 'For this offence, which do you agree?',
              isRequired: false,
              choices: [
                {
                  value: 'A',
                  text:
                    'I agree that I committed this offence, and I do not want to appear in court',
                },
                {
                  value: 'B',
                  text:
                    'I agree that I committed this offence, and I want to appear in court',
                },
                {
                  value: 'C',
                  text:
                    'I do not agree that I committed this offence (allegation)',
                },
              ],
            },
            {
              type: 'checkbox',
              name: 'count3A',
              visibleIf: '{count3} = "A"',
              title: 'Which requests would you like to make?',
              isRequired: false,
              choices: [
                {
                  value: 'A1',
                  text: 'I request a reduction of the ticketed amount: and/or',
                },
                {
                  value: 'A2',
                  text: 'I request time to pay the ticketed amount',
                },
              ],
            },
            {
              type: 'comment',
              name: 'count3_reduction_reason',
              title:
                'What are your reasons for a reduction in the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: false,
              visibleIf: '{count3} = "A" and {count3A} contains "A1"',
            },
            {
              type: 'comment',
              name: 'count3_time_reason',
              title:
                'What are your reasons for requiring more time to pay the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: false,
              visibleIf: '{count3} = "A" and {count3A} contains "A2"',
            },
            {
              type: 'checkbox',
              name: 'count3B',
              visibleIf: '{count3} = "B"',
              title: 'Why do you want to appear?',
              isRequired: false,
              choices: [
                {
                  value: 'B1',
                  text: 'To request a reduction of the ticketed amount: and/or',
                },
                {
                  value: 'B2',
                  text: 'To request time to pay the ticketed amount',
                },
              ],
            },
          ],
        },
      ],
    },
    {
      name: 'pageCourtInfo',
      navigationTitle: 'Court Information',
      visibleIf:
        '({count1} and {count1} != "A") or ({count2} and {count2} != "A") or ({count3} and {count3} != "A")',
      elements: [
        {
          type: 'html',
          name: 'alert_info',
          html: `
            <div>
              <h2 class="mb-1">Disputing your Ticket in Court</h2>
              <div class="text-muted m-2">
              Based upon your answers to the previous questions, you have decided to dispute your ticket in Court.
            Please answer the following questions.
              </div>
              <hr class="m-0" style="border: 1px solid #ffb200;" />
            </div>
          `,
        },
        {
          type: 'radiogroup',
          name: 'lawyerYn',
          title: 'Do you intend to be represented at the hearing by a lawyer?',
          isRequired: false,
          choices: [
            {
              value: 'Y',
              text: 'Yes',
            },
            {
              value: 'N',
              text: 'No',
            },
          ],
          colCount: 0,
        },
        {
          type: 'radiogroup',
          name: 'interpreterYn',
          title: 'Do you require an interpreter at the hearing?',
          isRequired: false,
          choices: [
            {
              value: 'Y',
              text: 'Yes',
            },
            {
              value: 'N',
              text: 'No',
            },
          ],
          colCount: 0,
        },
        {
          type: 'dropdown',
          name: 'interpreterLang',
          title: 'Language',
          visibleIf: '{interpreterYn} = "Y"',
          choices: [
            {
              value: 'L1',
              text: 'Spanish',
            },
            {
              value: 'L2',
              text: 'French',
            },
          ],
        },
        {
          type: 'radiogroup',
          name: 'witnessYn',
          title: 'Do you intend to call a witness at the hearing?',
          isRequired: false,
          choices: [
            {
              value: 'Y',
              text: 'Yes',
            },
            {
              value: 'N',
              text: 'No',
            },
          ],
          colCount: 0,
        },
      ],
    },
    {
      name: 'pageConfirmation',
      navigationTitle: 'Overview',
      navigationDescription: 'Summary',
      elements: [
        {
          type: 'html',
          name: 'alert_info',
          html: `
          <div>
          <h2 class="mb-1">Dispute Overview</h2>
          <div class="text-muted m-2">
          Please review the following information. You can go back to any step to
          update the information
          </div>
          <hr class="m-0" style="border: 1px solid #ffb200;" />
          </div>`,
        },
        {
          name: 'info_violationTicketNumber',
          type: 'text',
          title: 'Violation Ticket Number',
          readOnly: true,
        },
        {
          name: 'info_violationDate',
          type: 'text',
          title: 'Violation Date and Time',
          startWithNewLine: false,
          readOnly: true,
        },
        {
          type: 'checkbox',
          name: 'certifyCorrect',
          title: 'Certification',
          isRequired: true,
          requiredErrorText:
            'The information must be certified before you can submit the dispute',
          choices: [
            {
              value: 'true',
              text:
                'I certify that all information provided is true and complete. I understand it is an offence under the law to knowingly provide false or misleading information.',
            },
          ],
        },
      ],
    },
  ],
  completedHtml:
    '<p><h3>Your Violation Ticket information has been submitted.</h3></p>',
};

/*

  showPreviewBeforeComplete: 'showAnsweredQuestions',
    {
      name: 'page1',
      elements: [
        {
          type: 'image',
          name: 'first_page_image',
          imageLink: '/assets/traffic_light.jpg',
          imageFit: 'none',
          imageHeight: 426,
          imageWidth: 400,
          width: '500px',
        },
        {
          type: 'panel',
          elements: [
            {
              type: 'html',
              name: 'alert_info',
              html: `<div class="alert alert-primary">
                <h1 class="alert-heading">Violation Ticket Lookup</h1>
                <p class="mt-2 mb-0">Find your Violation Ticket by entering the Violation Ticket Number and Time.</p>
                </div>`,
            },
            {
              type: 'text',
              inputFormat: 'AA99999999',
              name: 'violationTicketNumber',
              title: 'What is the Violation Ticket Number',
              autoComplete: 'off',
              description:
                'A Violation Ticket Number is 2 letters and 8 numbers. For example, AB12345678.',
              isRequired: false,
              hideNumber: true,
            },
            {
              type: 'text',
              name: 'ticketTime',
              title: 'What is the Time of the Ticket',
              inputType: 'time',
              isRequired: false,
              hideNumber: true,
            },
          ],
          startWithNewLine: false,
        },
      ],
    },
    {
      name: 'page2',
      elements: [
        {
          name: 'panel1',
          type: 'panel',
          elements: [
            {
              type: 'html',
              name: 'alert_info',
              html: `<div class="alert alert-primary">
                <h1 class="alert-heading">Violation Ticket Information</h1>
                <p class="mt-2 mb-0">Here is a summary of your ticket information.
                Ensure that the specifics of the ticket are correct.</p>
                </div>`,
            },
            {
              name: 'info_violationTicketNumber',
              type: 'text',
              title: 'Violation Ticket Number',
              hideNumber: true,
              readOnly: true,
            },
            {
              name: 'info_violationDate',
              type: 'text',
              title: 'Violation Date and Time',
              startWithNewLine: false,
              hideNumber: true,
              readOnly: true,
            },
            {
              name: 'info_party',
              type: 'comment',
              title: 'Personal Information',
              hideNumber: true,
              readOnly: true,
            },
            {
              name: 'info_address',
              type: 'comment',
              title: 'Address Information',
              hideNumber: true,
              readOnly: true,
            },
          ],
        },
      ],
    },*/

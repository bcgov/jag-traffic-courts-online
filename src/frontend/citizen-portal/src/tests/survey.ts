export const SurveyJson = {
  showProgressBar: 'top',
  pages: [
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
              title: 'Violation Date',
              startWithNewLine: false,
              hideNumber: true,
              readOnly: true,
            },
            {
              name: 'info_courtLocation',
              type: 'text',
              title: 'Court Location',
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
            {
              type: 'radiogroup',
              name: 'correctYn',
              title: 'Is the information displayed above correct?',
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
      ],
    },
    {
      name: 'pageCount1',
      visibleIf: '{numberOfCounts} > 0',
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
              title: 'For this count, which do you agree?',
              isRequired: true,
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
              isRequired: true,
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
              isRequired: true,
              visibleIf: '{count1} = "A" and {count1A} contains "A1"',
            },
            {
              type: 'comment',
              name: 'count1_time_reason',
              title:
                'What are your reasons for requiring more time to pay the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: true,
              visibleIf: '{count1} = "A" and {count1A} contains "A2"',
            },
            {
              type: 'checkbox',
              name: 'count1B',
              visibleIf: '{count1} = "B"',
              title: 'Why do you want to appear?',
              isRequired: true,
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
              title: 'For this count, which do you agree?',
              isRequired: true,
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
              isRequired: true,
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
              isRequired: true,
              visibleIf: '{count2} = "A" and {count2A} contains "A1"',
            },
            {
              type: 'comment',
              name: 'count2_time_reason',
              title:
                'What are your reasons for requiring more time to pay the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: true,
              visibleIf: '{count2} = "A" and {count2A} contains "A2"',
            },
            {
              type: 'checkbox',
              name: 'count2B',
              visibleIf: '{count2} = "B"',
              title: 'Why do you want to appear?',
              isRequired: true,
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
              title: 'For this count, which do you agree?',
              isRequired: true,
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
              isRequired: true,
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
              isRequired: true,
              visibleIf: '{count3} = "A" and {count3A} contains "A1"',
            },
            {
              type: 'comment',
              name: 'count3_time_reason',
              title:
                'What are your reasons for requiring more time to pay the ticketed amount(s)?',
              description:
                'This reason must not contain a defence of the allegation',
              isRequired: true,
              visibleIf: '{count3} = "A" and {count3A} contains "A2"',
            },
            {
              type: 'checkbox',
              name: 'count3B',
              visibleIf: '{count3} = "B"',
              title: 'Why do you want to appear?',
              isRequired: true,
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
      visibleIf:
        '({count1} and {count1} != "A") or ({count2} and {count2} != "A") or ({count3} and {count3} != "A")',
      elements: [
        {
          type: 'html',
          name: 'alert_info',
          html: `<div class="alert alert-primary">
          <h1 class="alert-heading">Disputing your Ticket in Court</h1>
          <p class="mt-2 mb-0">Based upon your answers to the previous questions, you have decided to dispute your ticket in Court.
          Please answer the following questions.</p>
          </div>`,
        },
        {
          type: 'radiogroup',
          name: 'lawyerYn',
          title: 'Do you intend to be represented at the hearing by a lawyer?',
          isRequired: true,
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
          isRequired: true,
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
          isRequired: true,
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
      name: 'pageNoCourt',
      visibleIf:
        '({count1} == "A" or !{count1}) and ({count2} == "A" or !{count2}) and ({count3} == "A" or !{count3})',
      elements: [
        {
          type: 'radiogroup',
          name: 'xxYn',
          title: 'What is next?',
          isRequired: true,
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
      elements: [
        {
          type: 'signaturepad',
          name: 'signature',
          width: 500,
          title:
            'Please sign below declaring that the previous information is correct.',
          description: 'Signature of Disputant/Agent',
        },
      ],
    },
  ],
  completedHtml:
    '<p><h3>Your Violation Ticket information has been submitted.</h3></p>',
};

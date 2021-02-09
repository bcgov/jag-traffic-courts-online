export const SurveyJson = {
  showProgressBar: 'top',
  pages: [
    {
      name: 'page1',
      elements: [
        {
          type: 'image',
          name: 'first_page_image',
          imageLink: '/assets/traffic-light.jpg',
          imageFit: 'none',
          imageHeight: 426,
          imageWidth: 400,
          width: '500px',
        },
        {
          type: 'panel',
          elements: [
            {
              type: 'boolean',
              name: 'ticketYn',
              title: 'Boolean',
              label: 'Do you have a violation ticket?',
              isRequired: true,
            },
            {
              type: 'text',
              inputFormat: '9999999',
              name: 'driverLicenseNumber',
              visibleIf: '{ticketYn} = false',
              title: 'What is your Drivers License Number',
              isRequired: true,
            },
            {
              type: 'text',
              inputFormat: 'AA99999999',
              name: 'violationTicketNumber',
              visibleIf: '{ticketYn} = true',
              title: 'What is the Violation Ticket Number',
              autoComplete: 'off',
              description:
                'A Violation Ticket Number is 2 letters and 8 numbers. For example, AB12345678.',
              isRequired: true,
            },
            {
              type: 'text',
              name: 'ticketTime',
              title: 'What is the Time of the Ticket',
              visibleIf: '{ticketYn} = true',
              isRequired: true,
              inputType: 'time',
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
              name: 'ticket_info',
              html:
                '<article class="intro">    <h1 class="intro__heading intro__heading--income title"> Violation Ticket Information </h1>    <div class=intro__body wysiwyg">       <p>Here is a summary of your ticket information.         </div> </article>',
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
              name: 'info_surname',
              type: 'text',
              title: 'Surname',
              hideNumber: true,
              readOnly: true,
            },
            {
              name: 'info_givenNames',
              type: 'text',
              title: 'Given',
              startWithNewLine: false,
              hideNumber: true,
              readOnly: true,
            },
          ],
        },
        {
          type: 'boolean',
          name: 'disputeYn',
          title: 'Would you like to dispute this violation ticket?',
          isRequired: true,
        },
      ],
    },
    {
      name: 'page3',
      elements: [
        {
          type: 'boolean',
          name: 'financialYn',
          title: 'Boolean',
          visibleIf: '{disputeYn} = true',
          label: 'Are you disputing the ticket for financial reasons?',
          isRequired: true,
        },
        {
          type: 'checkbox',
          name: 'disputeReason',
          visibleIf: '{disputeYn} = true and {financialYn} = false',
          title: 'Why do you want to dispute the ticket?',
          isRequired: true,
          hasOther: true,
          colCount: 4,
          choices: [
            {
              value: 'choice1',
              text: 'I am innocent',
            },
            {
              value: 'choice2',
              text: 'Not my fault, I was distracted',
            },
            {
              value: 'choice3',
              text: 'The weather impaired my vision',
            },
            {
              value: 'choice4',
              text: 'I forgot to wear my glasses',
            },
            {
              value: 'choice5',
              text: 'I have witnesses that can help my case',
            },
          ],
        },
        {
          type: 'boolean',
          name: 'createCaseYn',
          title: 'Boolean',
          visibleIf:
            '{disputeYn} = true and {financialYn} = false and {disputeReason.length} > 0',
          label: 'Would you like to create a court date?',
          isRequired: true,
        },
        {
          type: 'boolean',
          name: 'moreTimeYn',
          title: 'Boolean',
          visibleIf: '{disputeYn} = true and {financialYn} = true',
          label: 'Do you need more time to pay the fine?',
          isRequired: true,
        },
        {
          type: 'boolean',
          name: 'installmentsYn',
          title: 'Boolean',
          visibleIf:
            '{disputeYn} = true and {financialYn} = true and {moreTimeYn} = true',
          label: 'Would you like to pay in installments?',
          isRequired: true,
        },
        {
          type: 'boolean',
          name: 'payYn',
          title: 'Boolean',
          visibleIf:
            '{disputeYn} = true and {financialYn} = true and {installmentsYn} = true',
          label: 'Would you like to pay the first installment now?',
          isRequired: true,
        },
      ],
    },
  ],
  completedHtml: '<p><h3>Thank you for completing the survey!</h3></p>',
};

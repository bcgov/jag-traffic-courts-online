import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { SurveyResourceService } from '@survey/services/survey-resource.service';
import { SurveyJson } from 'tests/survey';
import * as Survey from 'survey-angular';
import * as widgets from 'surveyjs-widgets';
import { PhonePipe } from '@shared/pipes/phone.pipe';
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { ToastService } from '@core/services/toast.service';
import { AppRoutes } from 'app/app.routes';
import { TicketDispute } from '@shared/models/ticketDispute.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  providers: [FormatDatePipe, PhonePipe],
})
export class HomeComponent implements OnInit {
  public busy: Subscription;
  public ticket: TicketDispute;

  private tcoSurvey: Survey.Survey;

  constructor(
    private logger: LoggerService,
    private dialog: MatDialog,
    private router: Router,
    private toastService: ToastService,
    private surveyResource: SurveyResourceService
  ) {}

  ngOnInit() {
    Survey.StylesManager.applyTheme('bootstrap');
    widgets.inputmask(Survey);

    Survey.defaultBootstrapCss.signaturepad.clearButton =
      'btn btn-outline-secondary';

    this.setupSurvey();
  }

  private setupSurvey(): void {
    this.tcoSurvey = new Survey.Model(SurveyJson);

    this.surveyResource.getTicket().subscribe((ticket) => {
      this.ticket = ticket;
      this.handleSurveySetup(ticket);

      Survey.SurveyNG.render('surveyContainer', {
        model: this.tcoSurvey,
      });
    });

    // let ticketYn = survey.getValue('ticketYn');
    // survey.setValue('disputeYn', false);
    // survey.focusQuestion("disputeYn");

    this.tcoSurvey.onCompleting.add((survey, options) => {
      this.logger.info(
        'onCompleting Survey JSON:',
        JSON.stringify(survey.data, null, 3)
      );

      options.allowComplete = false;
      this.handleOnCompleting();
    });

    this.tcoSurvey.onComplete.add((survey) => {
      this.logger.info(
        'onComplete Survey JSON:',
        JSON.stringify(survey.data, null, 3)
      );
    });
  }

  private handleSurveySetup(ticket: TicketDispute): void {
    this.tcoSurvey.setValue(
      'info_violationTicketNumber',
      ticket.violationTicketNumber
    );

    this.tcoSurvey.setValue('info_violationDate', ticket.violationTime);
    this.tcoSurvey.setValue('info_surname', '');
    this.tcoSurvey.setValue('info_givenNames', '');
    this.tcoSurvey.setValue('info_mailing', '');
    this.tcoSurvey.setValue('info_postal', '');
    this.tcoSurvey.setValue('info_city', '');
    this.tcoSurvey.setValue('info_province', '');
    this.tcoSurvey.setValue('info_license', '');
    this.tcoSurvey.setValue('info_provLicense', '');
    this.tcoSurvey.setValue('info_homePhone', '');
    this.tcoSurvey.setValue('info_workPhone', '');
    this.tcoSurvey.setValue('info_birthdate', '');

    this.tcoSurvey.setValue('info_party', '');

    this.tcoSurvey.setValue('info_address', '');

    // const numberOfCounts = ticket.counts?.length;
    this.tcoSurvey.setValue('numberOfCounts', 0); // ticket.counts?.length);

    // ticket?.counts.forEach((cnt) => {
    //   const question = this.tcoSurvey.getQuestionByName(
    //     'alert_info_count' + cnt.countNo
    //   );
    //   question.html =
    //     `<div>
    //     <h2 class="mb-1">Offence #` +
    //     cnt.countNo +
    //     `<small class="ml-2">` +
    //     cnt.description +
    //     `</small>
    //     </h2>
    //     <hr class="m-0" style="border: 1px solid #ffb200;" />
    //   </div>`;
    // });
  }

  private handleOnCompleting(): void {
    if (!!this.tcoSurvey.isConfirming) {
      return;
    }
    // this.tcoSurvey.isConfirming = true;

    // const data: DialogOptions = {
    //   title: 'Submit Dispute',
    //   message:
    //     'When your dispute is submitted for adjudication, it can no longer be updated. Are you ready to submit your dispute?',
    //   actionText: 'Submit Dispute',
    // };

    // this.dialog
    //   .open(ConfirmDialogComponent, { data })
    //   .afterClosed()
    //   .subscribe((response: boolean) => {
    //     if (response) {
    //       const source = timer(1000);
    //       this.busy = source.subscribe((val) => {
    this.tcoSurvey.doComplete();
    this.toastService.openSuccessToast('Dispute has been submitted');
    this.router.navigate([AppRoutes.disputePath(AppRoutes.SUCCESS)]);
    //     });
    //   } else {
    //     this.tcoSurvey.isConfirming = false;
    //   }
    // });
  }
}

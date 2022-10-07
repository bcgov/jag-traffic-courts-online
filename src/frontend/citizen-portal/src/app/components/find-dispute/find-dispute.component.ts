import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';
import { Subscription } from 'rxjs';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-find-dispute',
  templateUrl: './find-dispute.component.html',
  styleUrls: ['./find-dispute.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindDisputeComponent implements OnInit {
  public busy: Subscription;
  public form: FormGroup;
  private progressRef: NgProgressRef;

  public notFound = false;
  public toolTipData = 'It is preferred that you include an image of your blue violation ticket. If you are not able to upload an image or take a photo of your ticket on your mobile device. You will need:  1. Ticket number and violation date 2. Driver\'s license number and loation 3. Count Act / Section / Description 4. Fine amount';

  constructor(
    private formBuilder: FormBuilder,
    private formUtilsService: FormUtilsService,
    private ngProgress: NgProgress,
    private logger: LoggerService,
    private violationTicketService: ViolationTicketService,
  ) {
  }

  public ngOnInit(): void {
    this.progressRef = this.ngProgress.ref();

    this.form = this.formBuilder.group({
      ticketNumber: [null, [Validators.required]],
      time: [null, [Validators.required]],
    });
  }

  public onSearch(): void {
    this.logger.log('FindTicketComponent::onSearch');

    this.notFound = false;
    const validity = this.formUtilsService.checkValidity(this.form);
    const errors = this.formUtilsService.getFormErrors(this.form);

    this.logger.log('validity', validity);
    this.logger.log('errors', errors);
    this.logger.log('form.value', this.form.value);

    if (!validity) {
      return;
    }
    this.busy = this.violationTicketService.searchTicket(this.form.value).subscribe(res => res);
  }
}

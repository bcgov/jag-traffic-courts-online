import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ViewportService } from '@core/services/viewport.service';
import { Subscription } from 'rxjs';
import moment from 'moment';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { Ticket } from '@shared/models/ticket.model';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { ActivatedRoute, Router } from '@angular/router';
import { RouteUtils } from '@core/utils/route-utils.class';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';

export const MINIMUM_AGE = 18;

@Component({
  selector: 'app-part-a',
  templateUrl: './part-a.component.html',
  styleUrls: ['./part-a.component.scss'],
})
export class PartAComponent extends BaseDisputeFormPage implements OnInit {
  public busy: Subscription;
  public maxDateOfBirth: moment.Moment;

  private MINIMUM_AGE = 18;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    private viewportService: ViewportService,
    private service: DisputeResourceService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {
    super(route, router, formBuilder);

    this.maxDateOfBirth = moment().subtract(this.MINIMUM_AGE, 'years');
  }

  public ngOnInit(): void {
    this.service.ticket$.subscribe((ticket: Ticket) => {
      this.formStep2.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.formStep2)) {
      this.service.ticket$.next({
        ...this.service.ticket,
        ...this.formStep2.value,
      });

      this.routeNext(RouteUtils.currentRoutePath(this.router.url));
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }

  public onBack() {
    this.routeBack(RouteUtils.currentRoutePath(this.router.url));
  }

  public get isMobile(): boolean {
    return this.viewportService.isMobile;
  }

  public get homePhone(): FormControl {
    return this.formStep2.get('homePhone') as FormControl;
  }

  public get workPhone(): FormControl {
    return this.formStep2.get('workPhone') as FormControl;
  }

  public get birthdate(): FormControl {
    return this.formStep2.get('birthdate') as FormControl;
  }
}

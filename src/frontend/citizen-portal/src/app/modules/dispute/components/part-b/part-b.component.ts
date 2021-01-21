import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewportService } from '@core/services/viewport.service';
import { Subscription } from 'rxjs';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { Ticket } from '@shared/models/ticket.model';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { ActivatedRoute, Router } from '@angular/router';
import { RouteUtils } from '@core/utils/route-utils.class';
import { FormUtilsService } from '@core/services/form-utils.service';
import { UtilsService } from '@core/services/utils.service';
import { LoggerService } from '@core/services/logger.service';

@Component({
  selector: 'app-part-b',
  templateUrl: './part-b.component.html',
  styleUrls: ['./part-b.component.scss'],
})
export class PartBComponent extends BaseDisputeFormPage implements OnInit {
  public busy: Subscription;

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
  }

  public ngOnInit(): void {
    this.service.ticket$.subscribe((ticket: Ticket) => {
      this.formStep3.patchValue(ticket);
    });
  }

  public onSubmit(): void {
    if (this.formUtilsService.checkValidity(this.formStep3)) {
      this.service.ticket$.next({
        ...this.service.ticket,
        ...this.formStep3.value,
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
}

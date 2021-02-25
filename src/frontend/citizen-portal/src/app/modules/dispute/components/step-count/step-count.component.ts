import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { FormUtilsService } from '@core/services/form-utils.service';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { ViewportService } from '@core/services/viewport.service';
import { RouteUtils } from '@core/utils/route-utils.class';
import { BaseDisputeFormPage } from '@dispute/classes/BaseDisputeFormPage';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Ticket } from '@shared/models/ticket.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-step-count',
  templateUrl: './step-count.component.html',
  styleUrls: ['./step-count.component.scss'],
})
export class StepCountComponent extends BaseDisputeFormPage implements OnInit {
  @Input() public stepper: MatStepper;
  @Input() public step: any;

  public busy: Subscription;
  public formGroupCount: FormGroup;
  public nextBtnLabel: string;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    private viewportService: ViewportService,
    private formUtilsService: FormUtilsService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {
    super(route, router, formBuilder, disputeService, disputeResource);
    this.nextBtnLabel = 'Next';
  }

  public ngOnInit(): void {
    this.formGroupCount = this.formCounts.at(this.step.value) as FormGroup;
  }

  public onSubmit(): void {
    console.log('formCounts length', this.formCounts.length);

    // this.formCounts.forEach((cnt) => {
    // });

    let steps = this.disputeService.steps$.value;
    const exists = steps.some((step) => step.pageName === 3);
    if (!exists) {
      steps.splice(steps.length - 1, 0, {
        title: 'Court',
        value: null,
        pageName: 3,
      });
      this.disputeService.steps$.next(steps);
    }

    // '({count1} and {count1} != "A") or ({count2} and {count2} != "A") or ({count3} and {count3} != "A")',

    if (this.formUtilsService.checkValidity(this.formGroupCount)) {
      // this.disputeService.ticket$.next({
      //   ...this.disputeService.ticket,
      //   ...this.formCounts.value,
      // });

      this.stepper.next();
    } else {
      this.utilsService.scrollToErrorSection();
    }
  }
  public onBack() {
    this.stepper.previous();
  }

  public get isMobile(): boolean {
    return this.viewportService.isMobile;
  }

  public get count(): FormControl {
    return this.formGroupCount.get('count') as FormControl;
  }

  public get count1A1(): FormControl {
    return this.formGroupCount.get('count1A1') as FormControl;
  }

  public get count1A2(): FormControl {
    return this.formGroupCount.get('count1A2') as FormControl;
  }

  public get count1B1(): FormControl {
    return this.formGroupCount.get('count1B1') as FormControl;
  }

  public get count1B2(): FormControl {
    return this.formGroupCount.get('count1B2') as FormControl;
  }
}

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Dispute } from '@shared/models/dispute.model';
import { SurveyJsRoutes } from '@survey/survey-js.routes';

@Component({
  selector: 'app-dispute-list',
  templateUrl: './dispute-list.component.html',
  styleUrls: ['./dispute-list.component.scss'],
})
export class DisputeListComponent implements OnInit {
  public disputes: Dispute[];
  public dispute: Dispute;

  public columnsToDisplay: string[] = [
    'violationTicketNumber',
    'violationDate',
    'violationTime',
  ];

  constructor(
    private route: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService
  ) {}

  ngOnInit(): void {
    // this.disputeResource.getAllDisputes().subscribe((response) => {
    //   this.disputes = response;
    // });
    // this.disputeResource.getDispute().subscribe((response) => {
    //   this.dispute = response;
    // });
  }

  // public onSelect(dispute: Dispute): void {
  //   this.disputeService.dispute$.next(dispute);
  //   this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.STEPPER)]);
  // }

  // public onSelectSurvey(dispute: Dispute): void {
  //   this.disputeService.dispute$.next(dispute);
  //   this.route.navigate([SurveyJsRoutes.routePath(SurveyJsRoutes.HOME)]);
  // }
}

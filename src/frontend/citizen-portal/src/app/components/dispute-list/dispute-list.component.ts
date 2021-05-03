import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { TicketDispute } from '@shared/models/ticketDispute.model';

@Component({
  selector: 'app-dispute-list',
  templateUrl: './dispute-list.component.html',
  styleUrls: ['./dispute-list.component.scss'],
})
export class DisputeListComponent implements OnInit {
  public disputes: TicketDispute[];
  public dispute: TicketDispute;

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
    // this.busy = this.disputeResource.getAllDisputes().subscribe((response) => {
    //   this.disputes = response;
    // });
    // this.busy = this.disputeResource.getDispute().subscribe((response) => {
    //   this.dispute = response;
    // });
  }

  // public onSelect(dispute: Dispute): void {
  //   this.disputeService.dispute$.next(dispute);
  //   this.route.navigate([AppRoutes.routePath(AppRoutes.STEPPER)]);
  // }

  // public onSelectSurvey(dispute: Dispute): void {
  //   this.disputeService.dispute$.next(dispute);
  //   this.route.navigate([SurveyJsRoutes.routePath(SurveyJsRoutes.HOME)]);
  // }
}

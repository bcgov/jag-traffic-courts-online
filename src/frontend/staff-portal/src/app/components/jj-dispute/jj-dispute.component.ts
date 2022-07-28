import { Component, EventEmitter, Input, OnInit, Output, Inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { JJDisputeService } from '../../services/jj-dispute.service';
import { JJDispute } from '../../api/model/jJDispute.model';
import { Subscription } from 'rxjs';
import { JJDisputedCount, JJDisputeStatus } from 'app/api/model/models';

@Component({
  selector: 'app-jj-dispute',
  templateUrl: './jj-dispute.component.html',
  styleUrls: ['./jj-dispute.component.scss']
})
export class JJDisputeComponent implements OnInit {
  @Input() public jjDisputeInfo: JJDispute
  @Output() public backTicketList: EventEmitter<any> = new EventEmitter();
  public isMobile: boolean;
  public busy: Subscription;
  public lastUpdatedJJDispute: JJDispute;
  public todayDate: Date = new Date();
  public retrieving: boolean = true;
  public violationDate: string = "";
  public violationTime: string = "";
  public timeToPayCountsHeading: string = "";
  public fineReductionCountsHeading: string = "";

  constructor(
    protected route: ActivatedRoute,
    private utilsService: UtilsService,
    public mockConfigService: MockConfigService,
    private datePipe: DatePipe,
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    @Inject(Router) private router,
  ) {
    const today = new Date();
    this.isMobile = this.utilsService.isMobile();
  }

  public ngOnInit() {
    this.getJJDispute();
  }

  public onSubmit(): void {
    this.lastUpdatedJJDispute.status = JJDisputeStatus.Confirmed;  // Send to VTC Staff for review
    this.busy = this.jjDisputeService.putJJDispute(this.lastUpdatedJJDispute.ticketNumber, this.lastUpdatedJJDispute).subscribe((response: JJDispute) => {
      this.lastUpdatedJJDispute = response;
      this.logger.info(
        'JJDisputeComponent::putJJDispute response',
        response
      );
      this.onBack();
    });
  }

  public onSave(): void {
    // Update status to in progress unless status is set to review in which case do not change
    if (this.lastUpdatedJJDispute.status !== JJDisputeStatus.Review)  this.lastUpdatedJJDispute.status = JJDisputeStatus.InProgress;
    this.busy = this.jjDisputeService.putJJDispute(this.lastUpdatedJJDispute.ticketNumber, this.lastUpdatedJJDispute).subscribe((response: JJDispute) => {
      this.lastUpdatedJJDispute = response;
      this.logger.info(
        'JJDisputeComponent::putJJDispute response',
        response
      );
    });
  }

  // get dispute by id
  getJJDispute(): void {
    this.logger.log('JJDisputeComponent::getJJDispute');

    this.busy = this.jjDisputeService.getJJDispute(this.jjDisputeInfo.ticketNumber).subscribe((response: JJDispute) => {
      this.retrieving = false;
      this.logger.info(
        'JJDisputeComponent::getJJDispute response',
        response
      );

      this.lastUpdatedJJDispute = response;

      // set violation date and time
      let violationDate = this.lastUpdatedJJDispute.violationDate.split("T");
      this.violationDate = violationDate[0];
      this.violationTime = violationDate[1].split(":")[0] + ":" + violationDate[1].split(":")[1];

      // set up headings for written reasons
      this.lastUpdatedJJDispute.jjDisputedCounts.forEach(disputedCount => {
        if (disputedCount.requestTimeToPay == true) this.timeToPayCountsHeading += "Count " + disputedCount.count.toString() + ", ";
        if (disputedCount.requestReduction == true) this.fineReductionCountsHeading += "Count " + disputedCount.count.toString() + ", ";
      });
      if (this.timeToPayCountsHeading.length > 0) {
        this.timeToPayCountsHeading = this.timeToPayCountsHeading.substring(0, this.timeToPayCountsHeading.lastIndexOf(","));
      }
      if (this.fineReductionCountsHeading.length > 0) {
        this.fineReductionCountsHeading = this.fineReductionCountsHeading.substring(0, this.fineReductionCountsHeading.lastIndexOf(","));
      }
    });

  }

  getJJDisputedCount(count: number) {
    return this.lastUpdatedJJDispute.jjDisputedCounts.filter(x => x.count == count)[0];
  }

  // get from child component
  updateFinalDispositionCount(updatedJJDisputedCount: JJDisputedCount) {
    this.lastUpdatedJJDispute.jjDisputedCounts.forEach(jjDisputedCount => {
      if (jjDisputedCount.count == updatedJJDisputedCount.count) {
        jjDisputedCount = updatedJJDisputedCount;
      }
    });
  }

  public onBack() {
    this.backTicketList.emit();
  }
}



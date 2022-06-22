import { Component, EventEmitter, Input, Output, OnInit, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { JJDisputeService, JJFinalDispositionCount } from '../../services/jj-dispute.service';
import { JJDispute } from '../../api/model/jJDispute.model';
import { Subscription } from 'rxjs';
import { DisputedCount, ViolationTicketCount } from 'app/api';

@Component({
  selector: 'app-jj-count',
  templateUrl: './jj-count.component.html',
  styleUrls: ['./jj-count.component.scss']
})
export class JJCountComponent implements OnInit {
  @Input() public jjDisputeInfo: JJDispute
  // @Input() public disputedCount: DisputedCount;
  @Input() public violationTicketCount: ViolationTicketCount;
  // @Input() public finalDispositionCount: JJFinalDispositionCount;
  // @Output() public finalDispositionCountUpdate: EventEmitter<any> = new EventEmitter();
  public isMobile: boolean;
  public busy: Subscription;
  public todayDate: Date = new Date();
  public violationDate: string = "";
  
  constructor(
    protected route: ActivatedRoute,
    private utilsService: UtilsService,
    public mockConfigService: MockConfigService,
    private jjDisputeService: JJDisputeService,
    private logger: LoggerService,
    @Inject(Router) private router,
    ) {
      const today = new Date();
      this.isMobile = this.utilsService.isMobile();
    }

    ngOnInit(): void {
      this.violationDate = this.jjDisputeInfo.violationDate.split("T")[0];
    }

    addThirtyDays(initialDate: string): Date {
      var futureDate = new Date(initialDate);
      futureDate.setDate(futureDate.getDate() + 30);
      return futureDate;
    }
  }



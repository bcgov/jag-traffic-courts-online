import { Component, EventEmitter, Input, OnInit, Output, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { JJDisputeView, JJDisputeService } from '../../services/jj-dispute.service';
import { JJDispute } from '../../api/model/jJDispute.model';
import { Subscription } from 'rxjs';
import { ViolationTicket, DisputedCountPlea } from 'app/api';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmReasonDialogComponent } from '@shared/dialogs/confirm-reason-dialog/confirm-reason-dialog.component';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { ViolationTicketService } from 'app/services/violation-ticket.service';
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
  public lastUpdatedJJDispute: JJDisputeView;
  public todayDate: Date = new Date();
  public retrieving: boolean = true;
  public violationDate: string = "";
  public violationTime: string = "";
  public timeToPayCountsHeading: string = "";
  public fineReductionCountsHeading: string = "";
  
  constructor(
    protected route: ActivatedRoute,
    private dialog: MatDialog,
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
    this.getDispute();
  }

  public onSubmit(): void {
  }

  public onSave(): void {
  }

  public onReject(): void {
  }

  // get dispute by id
  getDispute(): void {
    this.logger.log('JJDisputeComponent::getDispute');

    this.busy = this.jjDisputeService.getJJDispute(this.jjDisputeInfo.ticketNumber).subscribe((response: JJDispute) => {
      this.retrieving = false;
      this.logger.info(
        'JJDisputeComponent::getDispute response',
        response
      );

      this.lastUpdatedJJDispute = response;
      this.mockData(); // TODO : get rid of this when data arrives

      // set violation date and time
      let violationDate = this.lastUpdatedJJDispute.violationDate.split("T");
      this.violationDate = violationDate[0];
      this.violationTime = violationDate[1].split(":")[0] + ":" + violationDate[1].split(":")[1];

      // set up headings for written reasons
      this.lastUpdatedJJDispute.disputedCounts.forEach(disputedCount => {
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

  public mockData() {
    // mock fields since we don't have any data yet
    this.lastUpdatedJJDispute.violationTicket = {} as ViolationTicket;
    this.lastUpdatedJJDispute.violationTicket.givenNames = "Francois Robert";
    this.lastUpdatedJJDispute.violationTicket.surname = "Hodge";
    this.lastUpdatedJJDispute.violationTicket.address = "1298 Mapleview Place";
    this.lastUpdatedJJDispute.violationTicket.organizationLocation = "Delta Police";

    this.lastUpdatedJJDispute.surname = "Hodge";
    this.lastUpdatedJJDispute.givenNames = "Francois Robert";
    this.lastUpdatedJJDispute.city = "Vancouver";
    this.lastUpdatedJJDispute.address = "1298 Mapleview Place";
    this.lastUpdatedJJDispute.driversLicenceProvince = "BC";
    this.lastUpdatedJJDispute.postalCode = "V56 3X7";
    this.lastUpdatedJJDispute.birthdate = "03/15/1976";
    this.lastUpdatedJJDispute.driversLicenceNumber = "6455895";
    this.lastUpdatedJJDispute.fineReductionReason = "I have lost my job and I am looking for another one";
    this.lastUpdatedJJDispute.timeToPayReason = "N/A";

    this.lastUpdatedJJDispute.disputedCounts = [];
    this.lastUpdatedJJDispute.disputedCounts.push({
      plea: DisputedCountPlea.Guilty,
      count: 1,
      requestTimeToPay: false,
      requestReduction: true,
      appearInCourt: false
    });
    this.lastUpdatedJJDispute.disputedCounts.push({
      plea: DisputedCountPlea.Guilty,
      count: 2,
      requestTimeToPay: false,
      requestReduction: true,
      appearInCourt: false
    });
    this.lastUpdatedJJDispute.disputedCounts.push({
      plea: DisputedCountPlea.Guilty,
      count: 3,
      requestTimeToPay: false,
      requestReduction: true,
      appearInCourt: false
    });

    this.lastUpdatedJJDispute.violationTicket.violationTicketCounts = [];
    this.lastUpdatedJJDispute.violationTicket.violationTicketCounts.push({
      count: 1,
      section: "92.1(1) MVA - Fail to stop resulting in pursuit",
      ticketedAmount: 380
    });
    this.lastUpdatedJJDispute.violationTicket.violationTicketCounts.push({
      count: 2,
      section: "146(5) MVA - Speed against area sign",
      ticketedAmount: 196
    });
    this.lastUpdatedJJDispute.violationTicket.violationTicketCounts.push({
      count: 3,
      section: "92.1(1) MVA - Fail to stop resulting in pursuit",
      ticketedAmount: 380
    });
  }

  public onBack() {
    window.location.reload();
  }
}



import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Dispute } from 'app/api';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-ticket-request',
  templateUrl: './ticket-request.component.html',
  styleUrls: ['./ticket-request.component.scss']
})
export class TicketRequestComponent implements OnInit {
  @Input() disputeInfo: Dispute;
  public collapseObj: any = {
    contactInformation: true
  }
  public form: FormGroup;

  constructor(
    protected formBuilder: FormBuilder,
    private violationTicketService: ViolationTicketService
  ) {
    this.form = this.formBuilder.group({
      timeToPayReason: null,
      fineReductionReason: null,
      disputedCount1: this.formBuilder.group({
        plea: null,
        requestType: null,
        appearInCourt: null,
        section: null,
        description: null,
        ticketedAmount: null
      }),
      disputedCount2: this.formBuilder.group({
        plea: null,
        requestType: null,
        appearInCourt: null,
        section: null,
        description: null,
        ticketedAmount: null
      }),
      disputedCount3: this.formBuilder.group({
        plea: null,
        requestType: null,
        appearInCourt: null,
        section: null,
        description: null,
        ticketedAmount: null
      }),
    });
  }

  setDisputedCount(count: number) {
    // set disputed count information
    let disputedCount = this.disputeInfo.disputedCounts.filter(x => x.count == count)[0];
    let violationTicketCount = this.disputeInfo.violationTicket.violationTicketCounts.filter(x => x.count == count)[0];
    if (disputedCount) {
      this.form.get('disputedCount' + count.toString()).get('plea').setValue(disputedCount.plea);
      let requestType = disputedCount.requestTimeToPay == true ? "Time to pay" : "";
      requestType = requestType.concat(disputedCount.requestTimeToPay == true && disputedCount.requestReduction == true ? " + " : "");
      requestType = requestType.concat(disputedCount.requestReduction == true ? "Fine reduction" : "");
      this.form.get('disputedCount' + count.toString()).get('requestType').setValue(requestType);
    }    
    if (violationTicketCount) {
      this.form.get('disputedCount' + count.toString()).get('section').setValue(this.violationTicketService.getLegalParagraphing(violationTicketCount));
      this.form.get('disputedCount' + count.toString()).get('description').setValue(violationTicketCount.description);
      this.form.get('disputedCount' + count.toString()).get('ticketedAmount').setValue(violationTicketCount.ticketedAmount);
    }  
  }

  ngOnInit(): void {
    this.form.patchValue(this.disputeInfo);
    this.setDisputedCount(1);
    this.setDisputedCount(2);
    this.setDisputedCount(3);
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

}

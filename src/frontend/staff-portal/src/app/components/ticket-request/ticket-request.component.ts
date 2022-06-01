import { Component, OnInit, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
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
      disputedCounts: this.formBuilder.array([])
    });
  }

  setDisputedCount(count: number) {
    // set disputed count information
    let disputedCount = this.disputeInfo.disputedCounts.filter(x => x.count == count)[0];
    let violationTicketCount = this.disputeInfo.violationTicket.violationTicketCounts.filter(x => x.count == count)[0];

    let requestType = "";
    if (disputedCount) {
      requestType = disputedCount.requestTimeToPay == true ? "Time to pay" : "";
      requestType = requestType.concat(disputedCount.requestTimeToPay == true && disputedCount.requestReduction == true ? " + " : "");
      requestType = requestType.concat(disputedCount.requestReduction == true ? "Fine reduction" : "");
    }

    const disputedCountForm = this.formBuilder.group({
      count: count,
      requestType: requestType,
      section: violationTicketCount? this.violationTicketService.getLegalParagraphing(violationTicketCount) : undefined,
      description: violationTicketCount?.description,
      ticketedAmount: violationTicketCount?.ticketedAmount,
      appearInCourt: disputedCount?.appearInCourt
    });

    if (violationTicketCount.description) this.disputedCounts.push(disputedCountForm);
  }

  get disputedCounts() {
    return this.form.controls["disputedCounts"] as FormArray;
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

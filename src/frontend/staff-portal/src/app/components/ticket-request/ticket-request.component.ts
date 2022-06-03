import { Component, OnInit, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Dispute, DisputedCount, DisputedCountPlea } from 'app/api';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-ticket-request',
  templateUrl: './ticket-request.component.html',
  styleUrls: ['./ticket-request.component.scss']
})
export class TicketRequestComponent implements OnInit {
  @Input() disputeInfo: Dispute;
  public countsActions: any;
  public collapseObj: any = {
    contactInformation: true
  }
  public form: FormGroup;
  public countFormFields = {
    plea: [null],
    count: [null],
    requestTimeToPay: [null],
    requestReduction: [null],
    appearInCourt: [null, [Validators.required]],
    notAppearInCourt: [null, [Validators.required]],
    courtGuilty: [null, [Validators.required]],
    courtNotGuilty: [null, [Validators.required]],
    noCourtGuilty: [null, [Validators.required]],
    noCourtNotGuilty: [null, [Validators.required]]
  }

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
      if (disputedCount.plea == 'GUILTY' && disputedCount.appearInCourt == false) {
        requestType = disputedCount.requestTimeToPay == true ? "Time to pay" : "";
        requestType = requestType.concat(disputedCount.requestTimeToPay == true && disputedCount.requestReduction == true ? " + " : "");
        requestType = requestType.concat(disputedCount.requestReduction == true ? "Fine reduction" : "");
      } else if (disputedCount.plea == 'GUILTY' && disputedCount.appearInCourt == true) {
        requestType = 'Time to pay and/or fine reduction';
      } else if (disputedCount.plea == 'NOT_GUILTY') {
        requestType = "Dispute offence";
      }
    }

    const disputedCountForm = this.formBuilder.group({
      count: count,
      requestType: requestType,
      plea: disputedCount?.plea,
      requestTimeToPay: disputedCount?.requestTimeToPay,
      requestReduction: disputedCount?.requestReduction,
      section: violationTicketCount ? this.violationTicketService.getLegalParagraphing(violationTicketCount) : undefined,
      description: violationTicketCount?.description,
      ticketedAmount: violationTicketCount?.ticketedAmount,
      appearInCourt: disputedCount?.appearInCourt,
      notAppearInCourt: !(disputedCount?.appearInCourt)
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

    this.countsActions = this.getCountsActions(this.disputeInfo.disputedCounts);
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

  public getCountsActions(counts: DisputedCount[]): any {
    let countsActions: any = {};

    let fields = Object.keys(this.countFormFields);
    let toCountStr = (arr: DisputedCount[]) => arr.map(i => "Count " + i.count).join(", ");
    fields.forEach(field => {
      if (counts && counts.length > 0) {
        countsActions[field] = toCountStr(counts.filter(i => i[field]));
      } else {
        countsActions[field] = [];
      }
    });
    countsActions.notAppearInCourt = counts.filter(i => i.appearInCourt === false).map(i => "Count " + i.count).join(", ");
    countsActions.courtGuilty = toCountStr(counts.filter(i => i.plea === DisputedCountPlea.Guilty && i.appearInCourt == true));
    countsActions.courtNotGuilty = toCountStr(counts.filter(i => i.plea === DisputedCountPlea.NotGuilty && i.appearInCourt == true));
    countsActions.noCourtGuilty = toCountStr(counts.filter(i => i.plea === DisputedCountPlea.Guilty && i.appearInCourt == false));
    countsActions.noCourtNotGuilty = toCountStr(counts.filter(i => i.plea === DisputedCountPlea.NotGuilty && i.appearInCourt == false));
    console.log(countsActions);
    return countsActions;
  }
}

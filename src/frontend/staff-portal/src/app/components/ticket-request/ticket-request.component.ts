import { Component, OnInit, Input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DisputeCount, DisputeCountPleaCode, DisputeCountRequestCourtAppearance, DisputeCountRequestReduction, DisputeCountRequestTimeToPay, DisputeRepresentedByLawyer } from 'app/api';
import { DisputeExtended } from 'app/services/dispute.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-ticket-request',
  templateUrl: './ticket-request.component.html',
  styleUrls: ['./ticket-request.component.scss']
})
export class TicketRequestComponent implements OnInit {
  @Input() disputeInfo: DisputeExtended;
  public countsActions: any;
  public collapseObj: any = {
    contactInformation: true
  }
  public form: FormGroup;
  public countFormFields = {
    pleaCode: [null],
    count: [null],
    requestTimeToPay: [null],
    requestReduction: [null],
    requestCourtAppearance: [null, [Validators.required]],
    notrequestCourtAppearance: [null, [Validators.required]],
    courtGuilty: [null, [Validators.required]],
    courtNotGuilty: [null, [Validators.required]],
    noCourtGuilty: [null, [Validators.required]],
    noCourtNotGuilty: [null, [Validators.required]]
  }
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public RequestCourtAppearance = DisputeCountRequestCourtAppearance;
  public PleaCode = DisputeCountPleaCode;
  public RequestTimeToPay = DisputeCountRequestTimeToPay;
  public RequestReduction = DisputeCountRequestReduction;

  constructor(
    protected formBuilder: FormBuilder,
    private violationTicketService: ViolationTicketService
  ) {
    this.form = this.formBuilder.group({
      timeToPayReason: null,
      fineReductionReason: null,
      disputeCounts: this.formBuilder.array([])
    });
  }

  setDisputeCount(count: number) {
    // set disputed count information
    let disputeCount = this.disputeInfo.disputeCounts.filter(x => x.countNo == count)[0];
    let violationTicketCount = this.disputeInfo.violationTicket.violationTicketCounts.filter(x => x.countNo == count)[0];

    let requestType = "";
    if (disputeCount) {
      if (disputeCount.pleaCode === this.PleaCode.G && disputeCount.requestCourtAppearance === this.RequestCourtAppearance.N) {
        requestType = disputeCount.requestTimeToPay === this.RequestTimeToPay.Y ? "Time to pay" : "";
        requestType = requestType.concat(disputeCount.requestTimeToPay === this.RequestTimeToPay.Y && disputeCount.requestReduction === this.RequestReduction.Y ? " + " : "");
        requestType = requestType.concat(disputeCount.requestReduction === this.RequestReduction.Y ? "Fine reduction" : "");
      } else if (disputeCount.pleaCode === this.PleaCode.G && disputeCount.requestCourtAppearance === this.RequestCourtAppearance.Y) {
        requestType = 'Time to pay and/or fine reduction';
      } else if (disputeCount.pleaCode === this.PleaCode.N) {
        requestType = "Dispute offence";
      }
    }

    const disputeCountForm = this.formBuilder.group({
      count: count,
      requestType: requestType,
      Code: disputeCount?.pleaCode,
      requestTimeToPay: disputeCount?.requestTimeToPay,
      requestReduction: disputeCount?.requestReduction,
      section: violationTicketCount ? this.violationTicketService.getLegalParagraphing(violationTicketCount) : undefined,
      description: violationTicketCount?.description,
      ticketedAmount: violationTicketCount?.ticketedAmount,
      requestCourtAppearance: disputeCount?.requestCourtAppearance,
      notrequestCourtAppearance: !(disputeCount?.requestCourtAppearance)
    });

    if (violationTicketCount?.description) this.disputeCounts.push(disputeCountForm);
  }

  get disputeCounts() {
    return this.form.controls["disputeCounts"] as FormArray;
  }

  ngOnInit(): void {
    this.form.patchValue(this.disputeInfo);
    this.setDisputeCount(1);
    this.setDisputeCount(2);
    this.setDisputeCount(3);

    this.countsActions = this.getCountsActions(this.disputeInfo.disputeCounts);
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

  public getCountsActions(counts: DisputeCount[]): any {
    let countsActions: any = {};

    let fields = Object.keys(this.countFormFields);
    let toCountStr = (arr: DisputeCount[]) => arr.map(i => "Count " + i.countNo).join(", ");
    fields.forEach(field => {
      if (counts && counts.length > 0) {
        countsActions[field] = toCountStr(counts.filter(i => i[field]));
      } else {
        countsActions[field] = [];
      }
    });
    countsActions.notrequestCourtAppearance = counts.filter(i => i.requestCourtAppearance === this.RequestCourtAppearance.N).map(i => "Count " + i.countNo).join(", ");
    countsActions.courtGuilty = toCountStr(counts.filter(i => i.pleaCode === this.PleaCode.G && i.requestCourtAppearance === this.RequestCourtAppearance.Y));
    countsActions.courtNotGuilty = toCountStr(counts.filter(i => i.pleaCode === this.PleaCode.N && i.requestCourtAppearance === this.RequestCourtAppearance.Y));
    countsActions.noCourtGuilty = toCountStr(counts.filter(i => i.pleaCode === this.PleaCode.G && i.requestCourtAppearance === this.RequestCourtAppearance.N));
    countsActions.noCourtNotGuilty = toCountStr(counts.filter(i => i.pleaCode === this.PleaCode.N && i.requestCourtAppearance == this.RequestCourtAppearance.N));
    return countsActions;
  }
}

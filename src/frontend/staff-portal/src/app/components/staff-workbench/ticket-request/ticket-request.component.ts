import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DisputeCount, DisputeCountPleaCode, DisputeRequestCourtAppearanceYn, DisputeCountRequestReduction, DisputeCountRequestTimeToPay, DisputeRepresentedByLawyer, Language } from 'app/api';
import { Dispute } from 'app/services/dispute.service';
import { LookupsService } from 'app/services/lookups.service';
import { ViolationTicketService } from 'app/services/violation-ticket.service';

@Component({
  selector: 'app-ticket-request',
  templateUrl: './ticket-request.component.html',
  styleUrls: ['./ticket-request.component.scss']
})
export class TicketRequestComponent implements OnInit, OnChanges {
  @Input() disputeInfo: Dispute;
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
    Guilty: [null, [Validators.required]],
    NotGuilty: [null, [Validators.required]],
  }
  public RepresentedByLawyer = DisputeRepresentedByLawyer;
  public RequestCourtAppearance = DisputeRequestCourtAppearanceYn;
  public PleaCode = DisputeCountPleaCode;
  public RequestTimeToPay = DisputeCountRequestTimeToPay;
  public RequestReduction = DisputeCountRequestReduction;

  constructor(
    protected formBuilder: FormBuilder,
    private violationTicketService: ViolationTicketService,
    public lookups: LookupsService
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
      // TCVP-2622 if all three variables are 'N', we can extrapolate that this count is "skipped". 
      //   We need to extrapolate since "skipped" is not captured or saved in the database.
      if (this.isSkipped(disputeCount)) {
        requestType = "No action at this time";
      }
      else if (disputeCount.pleaCode === this.PleaCode.G && this.disputeInfo.requestCourtAppearanceYn === this.RequestCourtAppearance.N) {
        requestType = disputeCount.requestTimeToPay === this.RequestTimeToPay.Y ? "Plead guilty and request time to pay with written reasons" : "";
        requestType = requestType.concat(disputeCount.requestTimeToPay === this.RequestTimeToPay.Y && disputeCount.requestReduction === this.RequestReduction.Y ? " + " : "");
        requestType = requestType.concat(disputeCount.requestReduction === this.RequestReduction.Y ? "Plead guilty and request a fine reduction with written reasons" : "");
      } else if (disputeCount.pleaCode === this.PleaCode.G && this.disputeInfo.requestCourtAppearanceYn === this.RequestCourtAppearance.Y) {
        requestType = 'Plead guilty and request a fine reduction and/or time to pay on this count in court';
      } else if (disputeCount.pleaCode === this.PleaCode.N) {
        requestType = "Dispute the charge";
      }
    }

    const disputeCountForm = this.formBuilder.group({
      count: count,
      requestType: requestType,
      pleaCode: disputeCount?.pleaCode,
      requestTimeToPay: disputeCount?.requestTimeToPay,
      requestReduction: disputeCount?.requestReduction,
      section: violationTicketCount ? this.violationTicketService.getLegalParagraphing(violationTicketCount) : undefined,
      description: violationTicketCount?.description,
      ticketedAmount: violationTicketCount?.ticketedAmount,
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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.disputeInfo.currentValue && changes.disputeInfo.previousValue) {
      this.disputeCounts.clear();
      this.ngOnInit();
    }
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

  public getCountsActions(counts: DisputeCount[]): any {
    let countsActions: any = {};
    let toCountStr = (arr: DisputeCount[]) => arr.map(i => "Count " + i.countNo).join(", ");
    countsActions.Guilty = toCountStr(counts.filter(i => i.pleaCode === this.PleaCode.G && !this.isSkipped(i)));
    countsActions.NotGuilty = toCountStr(counts.filter(i => i.pleaCode === this.PleaCode.N && !this.isSkipped(i)));
    countsActions.requestReduction = toCountStr(counts.filter(i => i.requestReduction === this.RequestReduction.Y && !this.isSkipped(i)));
    countsActions.requestTimeToPay = toCountStr(counts.filter(i => i.requestTimeToPay === this.RequestTimeToPay.Y && !this.isSkipped(i)));
    return countsActions;
  }

  // TCVP-2622 "skipped" is an extrapolated property based on requestCourtAppearance, requestReduction, and requestTimeToPay all equal 'N'
  private isSkipped(disputeCount: DisputeCount): boolean {
    return disputeCount?.requestCourtAppearance === this.RequestCourtAppearance.N
      && disputeCount?.requestReduction === this.RequestReduction.N
      && disputeCount?.requestTimeToPay === this.RequestTimeToPay.N
  }

}

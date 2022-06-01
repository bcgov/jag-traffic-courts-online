import { Component, OnInit, Input } from '@angular/core';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Dispute } from 'app/api';

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
  ) {
    this.form = this.formBuilder.group({
      timeToPayReason: null,
      fineReductionReason: null
    });
    this.form.patchValue(this.disputeInfo);
  }

  ngOnInit(): void {
  }
  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }

}

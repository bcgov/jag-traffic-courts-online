import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { JJDispute } from 'app/api';

@Component({
  selector: 'app-dispute-decision-info',
  templateUrl: './dispute-decision-info.component.html',
  styleUrls: ['./dispute-decision-info.component.scss']
})
export class DisputeDecisionInfoComponent implements OnInit {
  @Output() public backInbox: EventEmitter<any> = new EventEmitter();
  @Input() public jjDisputeInfo: JJDispute;

  constructor() { }

  ngOnInit(): void {
    console.log("Dispute Decision Details");
  }

  public onBack() {
    this.backInbox.emit();
  }
}

import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { JJDispute } from 'app/services/jj-dispute.service';

@Component({
  selector: 'app-dispute-decision-info',
  templateUrl: './dispute-decision-info.component.html',
  styleUrls: ['./dispute-decision-info.component.scss']
})
export class DisputeDecisionInfoComponent implements OnInit {
  @Output() public backInbox: EventEmitter<any> = new EventEmitter();
  @Input() public jjDisputeInfo: JJDispute;

  constructor(
  ) { }

  ngOnInit(): void {
  }

  public onBack() {
    this.backInbox.emit();
  }
}

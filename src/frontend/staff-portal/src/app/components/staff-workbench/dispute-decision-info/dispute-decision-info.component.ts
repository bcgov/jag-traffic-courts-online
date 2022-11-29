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

  public printDispute: boolean = true;
  public printUploadedDocuments: boolean = true;
  public printFileHistory: boolean = true;
  public printFileRemarks: boolean = true;

  constructor(
  ) { }

  ngOnInit(): void {
  }

  public onBack() {
    this.backInbox.emit();
  }

  public goTo(id: string) {
    document.getElementById(id)?.scrollIntoView();
  }
}

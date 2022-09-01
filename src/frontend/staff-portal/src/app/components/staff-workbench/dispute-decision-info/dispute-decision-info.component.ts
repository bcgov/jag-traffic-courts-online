import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-dispute-decision-info',
  templateUrl: './dispute-decision-info.component.html',
  styleUrls: ['./dispute-decision-info.component.scss']
})
export class DisputeDecisionInfoComponent implements OnInit {
  @Output() public backTicketList: EventEmitter<any> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  public onBack() {
    this.backTicketList.emit();
  }


}

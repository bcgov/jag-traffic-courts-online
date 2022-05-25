import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-ticket-request',
  templateUrl: './ticket-request.component.html',
  styleUrls: ['./ticket-request.component.scss']
})
export class TicketRequestComponent implements OnInit {
  public collapseObj: any = {
    contactInformation: true
 }
  constructor() { }

  ngOnInit(): void {
  }
  public handleCollapse(name: string) {
    this.collapseObj[name]= !this.collapseObj[name]
  }

}

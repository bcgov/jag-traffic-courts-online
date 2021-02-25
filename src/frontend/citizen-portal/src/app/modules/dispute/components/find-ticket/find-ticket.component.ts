import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
})
export class FindTicketComponent {
  constructor(
    private route: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService
  ) {}

  public onSearch(): void {
    this.disputeResource.getTicket().subscribe((response) => {
      console.log('ON SEARCH', response);

      this.disputeService.ticket$.next(response);

      // let steps = [];
      // steps.push({ title: 'Review', value: null, pageName: 1 });

      // let index = 0;
      // response.counts.forEach((cnt) => {
      //   steps.push({
      //     title: 'Count #' + cnt.countNo,
      //     description: cnt.description,
      //     value: index,
      //     pageName: 2,
      //   });
      //   index++;
      // });

      // steps.push({ title: 'Court', value: null, pageName: 3 });
      // steps.push({ title: 'Overview', value: null, pageName: 5 });
      // this.disputeService.steps$.next(steps);

      this.route.navigate(['/dispute/stepper']);
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
})
export class FindTicketComponent implements OnInit {
  public busy: Subscription;
  public form: FormGroup;

  constructor(
    private route: Router,
    private formBuilder: FormBuilder,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService
  ) {}

  public ngOnInit(): void {
    this.form = this.formBuilder.group({
      ticketNumber: ['EZ02000460', [Validators.required]],
      time: ['09:54', [Validators.required]],
    });
  }

  public onSearch(): void {
    const source = timer(1000);
    this.busy = source.subscribe((val) => {
      this.disputeResource.getDispute().subscribe((response) => {
        this.disputeService.dispute$.next(response);

        const formParams = { ...this.form.value };
        this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.SUMMARY)], {
          queryParams: formParams,
        });
      });
    });
  }

  public onRsbcSearch(): void {
    const queryParams = {
      ticketNumber: 'EZ02000460',
      time: '09:54',
    };

    this.disputeResource.getRsiTicket(queryParams).subscribe((response) => {
      this.disputeService.ticket$.next(response);
      this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.DISPLAY)]);
    });
  }
}

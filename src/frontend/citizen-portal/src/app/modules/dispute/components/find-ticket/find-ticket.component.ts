import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-find-ticket',
  templateUrl: './find-ticket.component.html',
  styleUrls: ['./find-ticket.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FindTicketComponent implements OnInit {
  public busy: Subscription;
  public form: FormGroup;

  public notFound = false;

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
    this.notFound = false;

    const formParams = { ...this.form.value };
    this.busy = this.disputeResource
      .getTicket(formParams)
      .subscribe((response) => {
        console.log('response', response);
        this.disputeService.ticket$.next(response);

        if (response) {
          this.route.navigate(
            [DisputeRoutes.routePath(DisputeRoutes.SUMMARY)],
            {
              queryParams: formParams,
            }
          );
        } else {
          this.notFound = true;
        }
      });
  }
}

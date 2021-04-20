import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Ticket } from '@shared/models/ticket.model';

export interface IBaseDisputeFormPage {
  form: FormGroup;
  // ticketDispute: TicketDispute;
  ticket: Ticket;
}

export abstract class BaseDisputeFormPage implements IBaseDisputeFormPage {
  public form: FormGroup;
  public ticket: Ticket;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService
  ) {}

  /**
   * @description
   * Patch the form with dispute information.
   */
  protected patchForm(): void {
    // Store a local copy of the dispute for views
    // this.ticketDispute = this.disputeService.ticketDispute;
    this.ticket = this.disputeService.ticket;

    // Attempt to patch the form if not already patched
    // this.disputeFormStateService.setForm(this.ticketDispute); //this.ticketDispute?.offence.dispute);
    // this.disputeFormStateService.setForm(this.ticket);
  }
}

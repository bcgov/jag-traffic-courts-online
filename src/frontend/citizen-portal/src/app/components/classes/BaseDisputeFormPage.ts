import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';

export interface IBaseDisputeFormPage {
  form: FormGroup;
  ticket: TicketDisputeView;
}

export abstract class BaseDisputeFormPage implements IBaseDisputeFormPage {
  public form: FormGroup;
  public ticket: TicketDisputeView;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    protected formBuilder: FormBuilder,
    protected disputeService: DisputeService,
    protected disputeResource: DisputeResourceService,
    protected disputeFormStateService: DisputeFormStateService
  ) { }

  /**
   * @description
   * Patch the form with dispute information.
   */
  protected patchForm(): void {
    // Store a local copy of the dispute for views
    this.ticket = this.disputeService.ticket;

    // Attempt to patch the form if not already patched
    this.disputeFormStateService.setForm(this.ticket);
  }
}

import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DisputeFormMode } from '@shared/enums/dispute-form-mode';
import { DisputeService, DisputantContactInformationFormGroup } from 'app/services/dispute.service';
import { DisputeStore } from 'app/store';

@Component({
  selector: 'app-update-dispute-contact',
  templateUrl: './update-dispute-contact.component.html',
  styleUrls: ['./update-dispute-contact.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class UpdateDisputeContactComponent implements OnInit {
  mode: DisputeFormMode = DisputeFormMode.UPDATEDISPUTANT;
  form: DisputantContactInformationFormGroup;

  private state: DisputeStore.State;
  private disputantFormFields = this.disputeService.disputantFormFields;

  constructor(
    private formBuilder: FormBuilder,
    private disputeService: DisputeService,
    private store: Store,
  ) {
  }

  ngOnInit(): void {
    this.disputeService.checkStoredDispute().subscribe(found => {
      if (found) {
        this.form = this.formBuilder.group({
          ...this.disputantFormFields,
        });
        this.form.reset();

        this.store.select(DisputeStore.Selectors.State).subscribe(state => {
          this.state = state;
        })
      }
    })
  }

  updateContact() {
    let input = { uuid: this.state.result.identifier, payload: this.form.value };
    this.store.dispatch(DisputeStore.Actions.UpdateContact(input));
  }

  back(): void {
    this.disputeService.goToUpdateDisputeLanding(this.state.params);
  }
}

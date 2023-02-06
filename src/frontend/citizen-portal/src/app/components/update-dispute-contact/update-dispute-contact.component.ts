import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { DisputeFormMode } from '@shared/enums/dispute-form-mode';
import { DisputeService } from 'app/services/dispute.service';
import { DisputantContactInformationFormGroup } from 'app/services/notice-of-dispute.service';
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

  constructor(
    private disputeService: DisputeService,
    private store: Store,
  ) {
  }

  ngOnInit(): void {
    this.disputeService.checkStoredDispute().subscribe(found => {
      if (found) {
        // this.form = this.formBuilder.group({
        //   ...this.disputantFormFields,
        // });
        // this.form.reset();
        // console.log(found, this.form);

        this.form = this.disputeService.getDisputantForm();
        this.store.select(DisputeStore.Selectors.State).subscribe(state => {
          this.state = state;
        })
      }
    })
  }

  updateContact() {
    this.store.dispatch(DisputeStore.Actions.UpdateContact({ payload: this.form.value }));
  }

  back(): void {
    this.disputeService.goToUpdateDisputeLanding(this.state.params);
  }
}

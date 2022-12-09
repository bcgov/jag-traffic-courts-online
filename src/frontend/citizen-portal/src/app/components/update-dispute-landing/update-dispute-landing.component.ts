import { Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { JJDisputeStatus } from 'app/api';
import { DisputeService } from 'app/services/dispute.service';
import { DisputeStore, disputeStateSelector } from 'app/store';

@Component({
  selector: 'app-update-dispute-landing',
  templateUrl: './update-dispute-landing.component.html',
  styleUrls: ['./update-dispute-landing.component.scss'],
})

export class UpdateDisputeLandingComponent implements OnInit {
  private data: DisputeStore.StateData;
  public isEditable: boolean = true;

  constructor(
    private disputeService: DisputeService,
    private store: Store,
  ) {
  }

  ngOnInit(): void {
    // subscrible to all changes
    this.store.pipe(select(disputeStateSelector)).subscribe(state => {
      if (state.data) {
        this.data = state.data;
        this.isEditable = !state.data?.dispute?.jjdispute_status || state.data?.dispute?.jjdispute_status === JJDisputeStatus.New;
      }

      if (!state.loading && !state.data?.dispute) {
        this.store.dispatch(DisputeStore.Actions.Search({}))
      }
    })
  }

  checkStatus(): void {
    this.disputeService.showDisputeStatus(this.data);
  }
}

import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { select, Store } from '@ngrx/store';
import { JJDisputeStatus } from 'app/api';
import { AppRoutes } from 'app/app.routes';
import { DisputeService } from 'app/services/dispute.service';
import { DisputeStore } from 'app/store';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-update-dispute-landing',
  templateUrl: './update-dispute-landing.component.html',
  styleUrls: ['./update-dispute-landing.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class UpdateDisputeLandingComponent implements OnInit {
  private state: DisputeStore.State;
  public isEditable: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(
    private disputeService: DisputeService,
    private store: Store,
    private router: Router,
  ) {
  }

  ngOnInit(): void {
    this.disputeService.checkStoredDispute().subscribe(found => {
      if (found) {
        // subscrible to all changes
        this.store.pipe(select(DisputeStore.Selectors.State)).subscribe(state => {
          if (state.result) {
            this.state = state;
            let dispute = state.result;
            if (dispute && !dispute.jjdispute_status || dispute?.jjdispute_status === JJDisputeStatus.New) {
              this.isEditable.next(true);
            } else {
              this.isEditable.next(false);
            }
          }
        })
      }
    })
  }

  checkStatus(): void {
    this.disputeService.showDisputeStatus(this.state);
  }

  goToUpdateDisputeAuth(): void {
    this.router.navigate([AppRoutes.disputePath(AppRoutes.UPDATE_DISPUTE_AUTH)], {
      queryParams: this.state.params,
    })
  }
}

import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { JJDisputeHearingType, JJDisputeStatus } from 'app/api';
import { AppConfigService } from 'app/services/app-config.service';
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
  private nonEditableStatus = [JJDisputeStatus.Cancelled, JJDisputeStatus.Concluded];
  public isEditable: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public bcServicesCardInfoLink: string;

  constructor(
    private appConfigService: AppConfigService,
    private disputeService: DisputeService,
    private store: Store,
  ) {
    this.bcServicesCardInfoLink = this.appConfigService.bcServicesCardInfoLink;
  }

  ngOnInit(): void {
    this.disputeService.checkStoredDispute().subscribe(found => {
      if (found) {
        // subscrible to all changes
        this.store.pipe(select(DisputeStore.Selectors.State)).subscribe(state => {
          if (state.result) {
            this.state = state;
            let dispute = state.result;
            if (dispute && !dispute.jjdispute_status || dispute.jjdispute_status === JJDisputeStatus.Unknown
              || (!this.nonEditableStatus.includes(dispute.jjdispute_status) && dispute.hearing_type === JJDisputeHearingType.CourtAppearance)) {
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

  goToUpdateDispute(): void {
    this.disputeService.goToUpdateDispute(this.state.params);
  }

  goToUpdateDisputeContact(): void {
    this.disputeService.goToUpdateDisputeContact(this.state.params);
  }
}

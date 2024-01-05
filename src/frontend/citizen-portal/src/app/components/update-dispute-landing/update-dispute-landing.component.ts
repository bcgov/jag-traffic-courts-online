import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { JJDisputeHearingType, JJDisputeStatus, DisputeStatus } from 'app/api';
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
  private nonEditableStatus = [JJDisputeStatus.Cancelled, JJDisputeStatus.Concluded, DisputeStatus.Concluded, DisputeStatus.Cancelled, DisputeStatus.Rejected];
  public isEditable: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public bcServicesCardInfoLink: string;
  public isEmailVerified: boolean = false;

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
        // subscribe to all changes
        this.store.select((DisputeStore.Selectors.State)).subscribe(state => {
          if (state.result) {
            this.state = state;
            let dispute = state.result;
            if (!dispute) this.isEditable.next(false); // no dispute found
            else if (this.nonEditableStatus.includes(dispute.dispute_status)) this.isEditable.next(false); // OCCAM dispute done
            else if (this.nonEditableStatus.includes(dispute.jjdispute_status)) this.isEditable.next(false); // JJ Dispute over
            else if (dispute.hearing_type === JJDisputeHearingType.WrittenReasons && dispute.jjdispute_status) this.isEditable.next(false); // written reasons and has a jj workbench status
            else this.isEditable.next(true); // otherwise allow editing

            this.isEmailVerified = dispute?.is_email_verified ? true : false;
            if (!this.isEmailVerified) {
              this.disputeService.openDisputantEmailNotVerifiedDialog();
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

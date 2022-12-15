import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { DisputeService } from 'app/services/dispute.service';
import { DisputeStore } from 'app/store';

@Component({
  selector: 'app-update-dispute-auth',
  templateUrl: './update-dispute-auth.component.html',
  styleUrls: ['./update-dispute-auth.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class UpdateDisputeAuthComponent implements OnInit {
  constructor(
    private disputeService: DisputeService,
  ) {
  }

  ngOnInit(): void {
    this.disputeService.checkStoredDispute().subscribe(() => { });
  }
}

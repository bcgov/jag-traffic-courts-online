import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { ConfigService } from '@config/config.service';
import { Dispute, DisputeService } from '../../../services/dispute.service';
import { DisputeUpdateRequestUpdateType, DisputeUpdateRequestStatus2 } from 'app/api';
import { DisputantUpdateRequest } from '../../../services/dispute.service';
import { Observable, forkJoin, map } from 'rxjs';
import { ToastService } from '@core/services/toast.service';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';

@Component({
  selector: 'app-update-request-info',
  templateUrl: './update-request-info.component.html',
  styleUrls: ['./update-request-info.component.scss', '../../../app.component.scss']
})
export class UpdateRequestInfoComponent implements OnInit {
  @Input() disputeInfo: Dispute;
  @Output() backInbox: EventEmitter<any> = new EventEmitter();
  initialDisputeValues: Dispute;
  retrieving: boolean = true;
  violationDate: string = "";
  infoHeight: number = window.innerHeight - 150; // less size of other fixed elements
  violationTime: string = "";
  conflict: boolean = false;
  collapseObj: any = {
    contactInformation: true
  }
  disputeUpdateRequests: DisputantUpdateRequest[] = [];
  RequestUpdateType = DisputeUpdateRequestUpdateType;
  RequestUpdateStatus = DisputeUpdateRequestStatus2;

  constructor(
    private config: ConfigService,
    private disputeService: DisputeService,
    private dialog: MatDialog,
    private toastService: ToastService,
    private logger: LoggerService,
  ) {
  }

  ngOnInit() {
    this.getDispute();
  }

  onSubmit(): void {
    // process accepts and rejects
    let observables: Observable<any>[] = [];
    this.disputeUpdateRequests.forEach(disputeUpdateRequest => {
      if (disputeUpdateRequest.status === this.RequestUpdateStatus.Pending) {
        if (disputeUpdateRequest.newStatus === this.RequestUpdateStatus.Accepted) {
          observables.push(this.disputeService.acceptDisputeUpdateRequest(disputeUpdateRequest.disputeUpdateRequestId).pipe(
            map(response => {
              let updateRequest = this.disputeUpdateRequests.filter(i => i.disputeUpdateRequestId === response).shift();
              if (updateRequest) {
                updateRequest.status = this.RequestUpdateStatus.Accepted;
              }
              return response;
            })
          ));
        } else if (disputeUpdateRequest.newStatus === this.RequestUpdateStatus.Rejected) {
          observables.push(this.disputeService.rejectDisputeUpdateRequest(disputeUpdateRequest.disputeUpdateRequestId).pipe(
            map(response => {
              let updateRequest = this.disputeUpdateRequests.filter(i => i.disputeUpdateRequestId === response).shift();
              if (updateRequest) {
                updateRequest.status = this.RequestUpdateStatus.Rejected;
              }
              return response;
            })
          ));
        }
      }
    })
    forkJoin(observables).subscribe({
      next: (response) => {
        const data: DialogOptions = {
          titleKey: "Disputant Update Requests",
          icon: "ok",
          actionType: "green",
          messageKey:
          "Your acceptances & rejections have been queued.  Allow a few minutes for them to take effect.",
          actionTextKey: "Ok",
          cancelHide: true
        };
        this.dialog.open(ConfirmDialogComponent, { data }).afterClosed()
          .subscribe((action: any) => {
            this.onBack();
          });
      },
      error: (err) => {
        this.toastService.openErrorToast("There are one or more error(s) when saving. Please review the change(s) and try again.");
      }
    });
  }

  // get dispute
  getDispute(): void {
    this.logger.log('UpdateRequestInfoComponent::getDispute');

    this.disputeService.getDispute(this.disputeInfo.disputeId).subscribe((response: Dispute) => {
      this.logger.info('UpdateRequestInfoComponent::getDispute response', response);

      this.initialDisputeValues = response;

      // set violation date and time
      let tempViolationDate = this.initialDisputeValues?.issuedTs?.split("T");
      if (tempViolationDate) {
        this.violationDate = tempViolationDate[0];
        this.violationTime = tempViolationDate[1].split(":")[0] + ":" + tempViolationDate[1].split(":")[1];
      }

      if (this.retrieving === true) this.getDisputeUpdateRequests(this.initialDisputeValues.disputeId);
    });
  }

  getDisputeUpdateRequests(disputeId: number) {
    this.logger.log('UpdateRequestInfoComponent::getDisputeUpdateRequests');

    this.disputeService.getDisputeUpdateRequests(disputeId).subscribe((response) => {
      this.retrieving = false;
      this.logger.info(
        'UpdateRequestInfoComponent::getDisputeUpdateRequests response',
        response
      );

      this.disputeUpdateRequests = response;

      // sort oldest to newest
      this.disputeUpdateRequests = this.disputeUpdateRequests.sort((a, b) => { if (b.createdTs < a.createdTs) return -1 });
    });
  }

  // respond to status changes from child component
  onDisputantUpdateRequestStatusChange(disputantUpdateRequest: DisputantUpdateRequest) {
    this.disputeUpdateRequests.forEach(disputeUpdateRequest => {
      if (disputeUpdateRequest.disputeUpdateRequestId === disputantUpdateRequest.disputeUpdateRequestId) disputeUpdateRequest.status = disputantUpdateRequest.status;
    });
  }

  getFormattedAddress(): string {
    let addresString = `${this.initialDisputeValues.addressLine1}`;

    if (this.initialDisputeValues.addressLine2) addresString += `, ${this.initialDisputeValues.addressLine2}`;
    if (this.initialDisputeValues.addressLine3) addresString += `, ${this.initialDisputeValues.addressLine3}`;
    if (this.initialDisputeValues.addressCity) addresString += ` ${this.initialDisputeValues.addressCity}`;

    if (this.initialDisputeValues.addressProvinceCountryId) {
      let prov = this.config.provincesAndStates.filter(x => x.provId === this.initialDisputeValues.addressProvinceCountryId && x.provSeqNo === this.initialDisputeValues.addressProvinceSeqNo);
      if (prov.length > 0) addresString += ` ${prov[0].provAbbreviationCd}`;
    } else if (this.initialDisputeValues.addressProvince) {
      addresString += ` ${this.initialDisputeValues.addressProvince}`;
    }

    if (this.initialDisputeValues.addressCountryId) {
      let country = this.config.countries.filter(x => x.ctryId === this.initialDisputeValues.addressCountryId);
      if (country.length > 0) addresString += ` ${country[0].ctryLongNm}`;
    }

    if (this.initialDisputeValues.postalCode) addresString += ` ${this.initialDisputeValues.postalCode}`;

    return addresString;
  }

  onBack() {
    this.backInbox.emit();
  }
}

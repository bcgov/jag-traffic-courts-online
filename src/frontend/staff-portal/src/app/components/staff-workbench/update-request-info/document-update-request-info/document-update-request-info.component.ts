import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { Dispute } from '../../../../services/dispute.service';
import { DisputantUpdateRequest } from '../../../../services/dispute.service';
import { LoggerService } from '@core/services/logger.service';
import { DisputeUpdateRequestStatus2 } from 'app/api';
import { JJDisputeService } from 'app/services/jj-dispute.service';

@Component({
  selector: 'app-document-update-request-info',
  templateUrl: './document-update-request-info.component.html',
  styleUrls: ['./document-update-request-info.component.scss', '../../../../app.component.scss']
})
export class DocumentUpdateRequestInfoComponent implements OnInit {
  @Input() public disputeInfo: Dispute;
  @Input() public disputantUpdateRequest!: DisputantUpdateRequest;
  @Output() public disputantUpdateRequestStatusChange: EventEmitter<DisputantUpdateRequest> = new EventEmitter<DisputantUpdateRequest>();
  public updateRequested: documentsUpdateJSON;
  public requestReadable: boolean = null;
  public UpdateRequestStatus = DisputeUpdateRequestStatus2;

  constructor(
    private logger: LoggerService,
    private jjDisputeService: JJDisputeService

  ) {
  }

  ngOnInit() {
    this.logger.log('DocumentUpdateRequestInfoComponent::Init', this.disputantUpdateRequest);

    try {
      this.updateRequested = JSON.parse(this.disputantUpdateRequest.updateJson);
      this.requestReadable = true;
    }
    catch (ex) {
      // Just dont crash, fail gracefully
      this.requestReadable = false;
    }
  }

  // emit status change to parent control
  statusChange(event) {
    this.disputantUpdateRequestStatusChange.emit(this.disputantUpdateRequest);
  }


  onGetFile(documentId: string) {
    this.jjDisputeService.getFileBlob(documentId).subscribe(result => {
      // TODO: remove the custom function here and replace with generated api call once staff API method
      // has proper response type documented in swagger json
      if (result != null) {
        var url = URL.createObjectURL(result);
        window.open(url);
      } else alert("File contents not found");
    });
  }

}

export interface documentsUpdateJSON {
  UploadedDocuments?: UploadDocumentRequestJSON[];
}

export interface UploadDocumentRequestJSON {
  DocumentId: string;
  DocumentType: string;
}

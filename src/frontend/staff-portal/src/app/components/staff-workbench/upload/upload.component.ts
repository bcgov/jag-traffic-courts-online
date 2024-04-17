import { Component, Input } from '@angular/core';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { DocumentService } from 'app/api/api/document.service';
import { FileMetadata } from 'app/api/model/fileMetadata.model';
import { Dispute } from 'app/services/dispute.service';
import { JJDisputeService } from 'app/services/jj-dispute.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class UploadComponent {

  @Input() disputeInfo: Dispute;
  public countsActions: any;
  public collapseObj: any = {
    upload: true
  }
  fileTypeToUpload: string = "Certified Extract";
  filesToUpload: any[] = [];

  constructor(
    private dialog: MatDialog,
    private documentService: DocumentService,
    private jjDisputeService: JJDisputeService,
  ) {
  }

  public handleCollapse(name: string) {
    this.collapseObj[name] = !this.collapseObj[name]
  }
  
  onRemoveFile(fileId: string, fileName: string) {
    const data: DialogOptions = {
      titleKey: "Remove File?",
      messageKey: "Are you sure you want to delete file " + fileName + "?",
      actionTextKey: "Delete",
      actionType: "warn",
      cancelTextKey: "Cancel",
      icon: "delete"
    };
    this.dialog.open(ConfirmDialogComponent, { data, width: "40%" }).afterClosed()
      .subscribe((action: any) => {
        if (action) {
          this.documentService.apiDocumentDelete(fileId).subscribe(any => {            
            this.disputeInfo.fileData = this.disputeInfo.fileData.filter(x => x.fileId !== fileId);
          });
        }
    });
  }

  onGetFile(fileId: string) {
    this.jjDisputeService.getFileBlob(fileId).subscribe(result => {
      if (result != null) {
        var url = URL.createObjectURL(result);
        window.open(url);
      } else alert("File contents not found");
    });
  }

  onUpload(files: FileList) {
    if (files.length <= 0) return;
    this.documentService.apiDocumentPost(this.disputeInfo.noticeOfDisputeGuid, this.fileTypeToUpload, files[0], 
      null).subscribe(fileId => {
        let item: FileMetadata = { fileId: fileId, fileName: files[0].name, virusScanStatus: "waiting for virus scan..." };
        this.disputeInfo.fileData.push(item);
    });
  }
}

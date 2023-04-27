import {
  AfterViewInit,
  Component, Inject
} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-ticket-image-dialog',
  templateUrl: './ticket-image-dialog.component.html',
  styleUrls: ['./ticket-image-dialog.component.scss'],
})
export class TicketImageDialogComponent implements AfterViewInit {
  // to hide bug: mat-expansion-panel animates expanded to closed when nested in a mat-dialog
  public visiblePanel = false;
  public imageToShow: any;

  constructor(
    private dialogRef: MatDialogRef<TicketImageDialogComponent>,
    @Inject(MAT_DIALOG_DATA) data) {
      this.imageToShow = data.imageToShow;
    //
  }

  public ngAfterViewInit(): void {
    // timeout required to avoid the dreaded 'ExpressionChangedAfterItHasBeenCheckedError'
    setTimeout(() => (this.visiblePanel = true));
  }
}

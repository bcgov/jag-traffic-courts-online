import {
  Component, Inject
} from '@angular/core';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA} from "@angular/material/legacy-dialog";

@Component({
  selector: 'app-ticket-image-dialog',
  templateUrl: './ticket-image-dialog.component.html',
  styleUrls: ['./ticket-image-dialog.component.scss'],
})
export class TicketImageDialogComponent {
  // to hide bug: mat-expansion-panel animates expanded to closed when nested in a mat-dialog
  public imageToShow: any;

  constructor(
    @Inject(MAT_DIALOG_DATA) data) {
      this.imageToShow = data.imageToShow; // pass in ticket image
  }
}

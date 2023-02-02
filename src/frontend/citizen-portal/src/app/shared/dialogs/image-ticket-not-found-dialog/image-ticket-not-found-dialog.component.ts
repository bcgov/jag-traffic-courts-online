import { Component, Inject, ChangeDetectionStrategy} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { DialogDefaultOptions } from '../dialog-default-options.model';
import { DialogOptions } from '../dialog-options.model';
import { DIALOG_DEFAULT_OPTION } from '../dialogs-properties.provider';
import { DialogContentOutput } from '../dialog-output.model';
import { ImageRequirementsDialogComponent } from '../image-requirements-dialog/image-requirements-dialog.component';
@Component({
  selector: 'app-image-ticket-not-found-dialog',
  templateUrl: './image-ticket-not-found-dialog.component.html',
  styleUrls: ['./image-ticket-not-found-dialog.component.scss']
})
export class ImageTicketNotFoundDialogComponent {

  public options: DialogOptions;
  public dialogContentOutput: DialogContentOutput<any>;

  constructor(
    public dialogRef: MatDialogRef<ImageTicketNotFoundDialogComponent>,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public customOptions: DialogOptions,
    @Inject(DIALOG_DEFAULT_OPTION) public defaultOptions: DialogDefaultOptions
  ) {
    this.options =
      typeof customOptions === 'string'
        ? this.getOptions(defaultOptions[customOptions]())
        : this.getOptions(customOptions);

    this.dialogContentOutput = null;
  }

  public onConfirm(): void {
    const response =
      this.dialogContentOutput !== null
        ? { output: this.dialogContentOutput }
        : true;
    this.dialogRef.close(response);
  }
  public onViewImageRequirements(): void {
    this.dialog.open(ImageRequirementsDialogComponent, {
      width: '600px',
    });
  }
  private getOptions(dialogOptions: DialogOptions): DialogOptions {
    const options: DialogOptions = {
      actionType: 'primary',
      actionTextKey: 'Confirm',
      cancelTextKey: 'Cancel',
      cancelHide: false,
      actionHide: false,
      ...dialogOptions,
    };
    

    return {
      icon: options.actionType === 'warn' ? 'warning' : 'help',
      ...options,
    };
  }

}

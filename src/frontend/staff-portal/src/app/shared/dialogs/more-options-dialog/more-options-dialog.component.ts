import { Component, Inject, ChangeDetectionStrategy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { DialogDefaultOptions } from '../dialog-default-options.model';
import { DialogOptions } from '../dialog-options.model';
import { DIALOG_DEFAULT_OPTION } from '../dialogs-properties.provider';
import { DialogContentOutput } from '../dialog-output.model';

@Component({
  selector: 'more-options-dialog',
  templateUrl: './more-options-dialog.component.html',
  styleUrls: ['./more-options-dialog.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MoreOptionsDialogComponent {
  public options: DialogOptions;
  public dialogContentOutput: DialogContentOutput<any>;

  constructor(
    public dialogRef: MatDialogRef<MoreOptionsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public customOptions: DialogOptions,
    @Inject(DIALOG_DEFAULT_OPTION) public defaultOptions: DialogDefaultOptions
  ) {
    this.options =
      typeof customOptions === 'string'
        ? this.getOptions(defaultOptions[customOptions]())
        : this.getOptions(customOptions);

    this.dialogContentOutput = null;
  }

  public onRequireCourtHearing(): void {
    const response =
      this.dialogContentOutput !== null
        ? { output: this.dialogContentOutput }
        : "Require Court Hearing";
    this.dialogRef.close(response);
  }

  public onChangeOfPlea(): void {
    const response =
      this.dialogContentOutput !== null
        ? { output: this.dialogContentOutput }
        : "Change Of Plea";
    this.dialogRef.close(response);
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

import { Component, Inject, ChangeDetectionStrategy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { DialogDefaultOptions } from '../dialog-default-options.model';
import { DialogOptions } from '../dialog-options.model';
import { DIALOG_DEFAULT_OPTION } from '../dialogs-properties.provider';
import { DialogContentOutput } from '../dialog-output.model';

@Component({
  selector: 'app-confirm-reason-dialog',
  templateUrl: './confirm-reason-dialog.component.html',
  styleUrls: ['./confirm-reason-dialog.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmReasonDialogComponent {
  public options: DialogOptions;
  public dialogContentOutput: DialogContentOutput<any>;
  public reasonForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<ConfirmReasonDialogComponent>,
    protected fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public customOptions: DialogOptions,
    @Inject(DIALOG_DEFAULT_OPTION) public defaultOptions: DialogDefaultOptions
  ) {
    this.options =
      typeof customOptions === 'string'
        ? this.getOptions(defaultOptions[customOptions]())
        : this.getOptions(customOptions);

    this.dialogContentOutput = null;

    this.reasonForm = this.fb.group({
      reason: [this.options.message, Validators.maxLength(256)]
    })
  }

  public onConfirm(): void {
    const response =
      this.dialogContentOutput !== null
        ? { output: this.dialogContentOutput }
        : { output: { response: true, reason: this.reasonForm.get('reason').value }};
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

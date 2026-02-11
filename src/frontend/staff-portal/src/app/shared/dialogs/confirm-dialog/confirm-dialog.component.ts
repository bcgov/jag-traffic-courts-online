import { Component, Inject, ChangeDetectionStrategy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { DialogDefaultOptions } from '../dialog-default-options.model';
import { DialogOptions } from '../dialog-options.model';
import { DIALOG_DEFAULT_OPTION } from '../dialogs-properties.provider';
import { DialogContentOutput } from '../dialog-output.model';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmDialogComponent {
  public options: DialogOptions;
  public dialogContentOutput: DialogContentOutput<any>;
  jjIDIR: string;
  currentDate: Date = new Date();

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public customOptions: DialogOptions,
    @Inject(DIALOG_DEFAULT_OPTION) public defaultOptions: DialogDefaultOptions,
    private authService: AuthService
  ) {
    this.options =
      typeof customOptions === 'string'
        ? this.getOptions(defaultOptions[customOptions]())
        : this.getOptions(customOptions);

    this.dialogContentOutput = null;

    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.jjIDIR = userProfile.idir;
      }
    });
  }

  public onConfirm(): void {
    const response =
      this.dialogContentOutput !== null
        ? { output: this.dialogContentOutput }
        : true;
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

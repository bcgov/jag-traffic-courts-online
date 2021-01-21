import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private duration: number;

  constructor(private snackBar: MatSnackBar) {
    this.duration = 3000; // ms
  }

  /**
   * @description
   * Opens a toast to display success messages.
   */
  public openSuccessToast(
    message: string,
    action: string = null,
    config: MatSnackBarConfig = null
  ) {
    // extraClasses: ['success']
    const defaultConfig: MatSnackBarConfig = {
      ...config,
      duration: this.duration,
    };
    this.openToast(message, action, defaultConfig);
  }

  /**
   * @description
   * Opens a toast to display error messages.
   */
  public openErrorToast(
    message: string,
    action: string = null,
    config: MatSnackBarConfig = null
  ) {
    // extraClasses: ['danger']
    const defaultConfig: MatSnackBarConfig = {
      ...config,
      duration: this.duration,
    };
    this.openToast(message, action, defaultConfig);
  }

  private openToast(
    message: string,
    action: string = null,
    config: MatSnackBarConfig
  ): void {
    this.snackBar.open(message, action, config);
  }
}

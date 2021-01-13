import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastService } from '@core/services/toast.service';
import { ConfirmDialogComponent } from '@shared/dialogs/confirm-dialog/confirm-dialog.component';
import { DialogOptions } from '@shared/dialogs/dialog-options.model';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'Traffic Court';
  public busy: Subscription;

  constructor(private dialog: MatDialog, private toastService: ToastService) {}

  public onDialog() {
    const data: DialogOptions = {
      title: 'Test',
      // imageSrc: '/assets/gov_bc_logo.png',
      actionText: 'Done',
      message: 'This is a test.',
      cancelHide: true,
    };
    this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe((response: boolean) => {
        if (response) {
          console.log('done');
        }
      });
  }

  public onToast() {
    this.toastService.openSuccessToast('Information has been saved');
  }

  public onTestBusy() {
    const source = timer(5000);
    console.log('start');
    this.busy = source.subscribe((val) => {
      console.log('end', val);
    });
  }

  public onConfirmDialog() {
    const data: DialogOptions = {
      title: 'Approve Test',
      message: 'Are you sure you want to approve this test?',
      actionText: 'Approve Test',
    };
    this.busy = this.dialog
      .open(ConfirmDialogComponent, { data })
      .afterClosed()
      .subscribe(() => {
        console.log('done');
      });
  }
}

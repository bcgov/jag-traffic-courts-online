import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-ticket-payment-dialog',
  templateUrl: './ticket-payment-dialog.component.html',
  styleUrls: ['./ticket-payment-dialog.component.scss'],
})
export class TicketPaymentDialogComponent implements OnInit {
  constructor(public dialogRef: MatDialogRef<TicketPaymentDialogComponent>) {}

  public ngOnInit(): void {
    //
  }

  public onProceedWithPayment(): void {
    const response = true;
    this.dialogRef.close(response);
  }
}

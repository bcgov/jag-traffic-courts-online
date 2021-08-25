import {
  AfterContentInit,
  AfterViewInit,
  Component,
  OnInit,
} from '@angular/core';

@Component({
  selector: 'app-ticket-example-dialog',
  templateUrl: './ticket-example-dialog.component.html',
  styleUrls: ['./ticket-example-dialog.component.scss'],
})
export class TicketExampleDialogComponent implements OnInit, AfterViewInit {
  // to hide bug: mat-expansion-panel animates expanded to closed when nested in a mat-dialog
  public visiblePanel = false;

  constructor() {
    //
  }

  public ngOnInit(): void {
    //
  }

  public ngAfterViewInit(): void {
    // timeout required to avoid the dreaded 'ExpressionChangedAfterItHasBeenCheckedError'
    setTimeout(() => (this.visiblePanel = true));
  }
}

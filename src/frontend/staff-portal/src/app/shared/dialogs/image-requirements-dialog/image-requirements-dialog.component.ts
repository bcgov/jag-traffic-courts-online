import {
  AfterContentInit,
  AfterViewInit,
  Component,
  OnInit,
} from '@angular/core';

@Component({
  selector: 'app-image-requirements-dialog',
  templateUrl: './image-requirements-dialog.component.html',
  styleUrls: ['./image-requirements-dialog.component.scss'],
})
export class ImageRequirementsDialogComponent implements OnInit, AfterViewInit {
  // to hide bug: mat-expansion-panel animates expanded to closed when nested in a mat-dialog
  public visiblePanel = false;

  constructor() {
    //
  }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  public ngOnInit(): void {
    //
  }

  public ngAfterViewInit(): void {
    // timeout required to avoid the dreaded 'ExpressionChangedAfterItHasBeenCheckedError'
    setTimeout(() => (this.visiblePanel = true));
  }
}

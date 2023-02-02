import { AfterViewInit, Component, ElementRef, Input, ViewChild } from '@angular/core';

@Component({
  selector: 'app-page-header',
  templateUrl: './page-header.component.html',
  styleUrls: ['./page-header.component.scss'],
})
export class PageHeaderComponent implements AfterViewInit {
  @Input() showHrOverride: boolean = null;
  @ViewChild("headerWrapper") private headerWrapper: ElementRef
  @ViewChild("subHeaderWrapper") private subHeaderWrapper: ElementRef
  showHr: boolean = true;

  ngAfterViewInit(): void {
    setTimeout(() => { // changes after view init, prevent warning
      if (this.showHrOverride === null) {
        if (this.headerWrapper.nativeElement?.childNodes?.length > 0
          || this.subHeaderWrapper.nativeElement?.childNodes?.length > 0) {
          this.showHr = true;
        } else {
          this.showHr = false;
        }
      } else {
        this.showHr = this.showHrOverride;
      }
    }, 0)
  }
}

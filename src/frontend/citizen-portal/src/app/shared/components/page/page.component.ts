import { Component, Input } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  styleUrls: ['./page.component.scss'],
})
export class PageComponent {
  @Input() public busy: Subscription;
  @Input() public mode: 'default' | 'full';

  constructor() {
    this.mode = 'default';
  }
}

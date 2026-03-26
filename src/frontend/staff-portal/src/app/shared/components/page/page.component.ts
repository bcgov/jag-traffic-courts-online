import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  styleUrls: ['./page.component.scss'],
  standalone: false,
})
export class PageComponent {
  @Input() public mode: 'default' | 'full';

  constructor(
  ) {
    this.mode = 'default';
  }
}

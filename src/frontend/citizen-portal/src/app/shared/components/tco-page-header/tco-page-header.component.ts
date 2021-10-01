import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-tco-page-header',
  templateUrl: './tco-page-header.component.html',
  styleUrls: ['./tco-page-header.component.scss'],
})
export class TcoPageHeaderComponent {

  @Input() showHorizontalRule = true;
}

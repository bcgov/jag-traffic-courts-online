import { Component, Input, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-stepper-page',
  templateUrl: './stepper-page.component.html',
  styleUrls: ['./stepper-page.component.scss'],
})
export class StepperPageComponent {
  @Input() public busy: Subscription;
  @Input() public mode: 'default' | 'full';

  constructor() {
    this.mode = 'default';
  }
}

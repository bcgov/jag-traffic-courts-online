import { Component, Input, Output, EventEmitter } from '@angular/core';

import { Subscription } from 'rxjs';

@Component({
  selector: 'app-busy-overlay',
  templateUrl: './busy-overlay.component.html',
  styleUrls: ['./busy-overlay.component.scss'],
})
export class BusyOverlayComponent {
  @Input() public busy: Subscription;
  @Output() public started: EventEmitter<boolean>;
  @Output() public stopped: EventEmitter<boolean>;

  constructor() {
    this.started = new EventEmitter();
    this.stopped = new EventEmitter();
  }

  /**
   * @description
   * Indicate the busy overlay is displayed.
   */
  public onBusyStart(event: boolean): void {
    this.started.emit(event);
  }

  /**
   * @description
   * Indicate the busy overlay is hidden.
   */
  public onBusyStop(event: boolean): void {
    this.stopped.emit(event);
  }
}

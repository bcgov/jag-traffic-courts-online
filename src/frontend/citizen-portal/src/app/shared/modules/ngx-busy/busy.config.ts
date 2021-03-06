import { BusyConfig } from 'ng-busy';

import { BusyOverlayMessageComponent } from './busy-overlay-message/busy-overlay-message.component';

export const busyConfig: BusyConfig = {
  backdrop: true,
  template: BusyOverlayMessageComponent,
  delay: 50,
  minDuration: 600,
  disableAnimation: true,
  wrapperClass: 'ng-busy',
  templateNgStyle: {},
  message: '',
};

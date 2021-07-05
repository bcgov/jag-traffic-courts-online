import { Injectable } from '@angular/core';

export interface ISnowplowWindow extends Window {
  snowplow: (...args) => void;
}

function getWindow(): any {
  return window;
}

@Injectable({
  providedIn: 'root',
})
export class WindowRefService {
  /**
   * @description
   * Get a reference to the native window object.
   */
  get nativeWindow(): ISnowplowWindow {
    return getWindow();
  }
}

import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { SortDirection } from '@angular/material/sort';

import { WindowRefService } from './window-ref.service';

export type SortWeight = -1 | 0 | 1;

@Injectable({
  providedIn: 'root',
})
export class UtilsService {
  private window: Window;

  constructor(
    @Inject(DOCUMENT) private document: Document,
    private windowRef: WindowRefService
  ) {
    this.window = this.windowRef.nativeWindow;
  }

  /**
   * @description
   * Scroll to the top of the mat-sidenav container.
   */
  public scrollTop() {
    // const contentContainer = this.document.querySelector('.app') || this.window;
    // contentContainer.scroll({ top: 0, left: 0, behavior: 'smooth' });
    this.window.scroll({ top: 0, left: 0, behavior: 'smooth' });
  }

  /**
   * @description
   * Scroll to have the element in view.
   */
  public scrollTo(el: Element): void {
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }

  /**
   * @description
   * Scroll to a material form field that is invalid, and if contained
   * within a <section> scroll to the section instead.
   */
  public scrollToErrorSection(): void {
    let errorElement = document.querySelector('mat-form-field.ng-invalid');

    if (!errorElement) {
      errorElement = document.querySelector('mat-radio-group.ng-invalid');
    }

    if (!errorElement) {
      errorElement = document.querySelector('mat-checkbox.ng-invalid');
    }

    if (errorElement) {
      const section = errorElement.closest('section');
      const element = section == null ? errorElement : section;
      this.scrollTo(element);
    } else {
      this.scrollTop();
    }
  }

  /**
   * @description
   * Checks if the browser is Internet Explorer, or pre-Chromium
   * Edge.
   *
   * The reversed attribute of ordered lists is not supported in IE or
   * pre-Chromium Edge, but has been supported in all other browsers
   * forevers!!!
   */
  public isIEOrPreChromiumEdge(): boolean {
    return !('reversed' in document.createElement('ol'));
  }

  /**
   * @description
   * Generic sorting of a JSON object by key.
   */
  public sortByKey<T>(
    a: { [key: string]: any },
    b: { [key: string]: any },
    key: string
  ): SortWeight {
    return this.sort<T>(a[key], b[key]);
  }

  /**
   * @description
   * Generic sorting of a JSON object by direction.
   */
  public sortByDirection<T>(
    a: T,
    b: T,
    direction: SortDirection = 'asc',
    withTrailingNull: boolean = true
  ): SortWeight {
    let result: SortWeight;

    if (a === null && withTrailingNull) {
      result = -1;
    } else if (b === null && withTrailingNull) {
      result = 1;
    } else {
      result = this.sort(a, b);
    }

    if (direction === 'desc') {
      result *= -1;
    }

    return result;
  }

  /**
   * @description
   * Generic sorting of a JSON object by key.
   */
  public sort<T>(a: T, b: T): SortWeight {
    return a > b ? 1 : a < b ? -1 : 0;
  }
}

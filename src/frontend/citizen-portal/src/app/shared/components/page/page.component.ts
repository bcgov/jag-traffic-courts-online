import { Component, Input } from '@angular/core';
import { LoadingStore } from '@core/store';
import { select, Store } from '@ngrx/store';
import { filter, Subscription } from 'rxjs';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  styleUrls: ['./page.component.scss'],
})
export class PageComponent {
  @Input() public mode: 'default' | 'full';
  public busy: Subscription;

  constructor(
    private store: Store
  ) {
    this.mode = 'default';
    this.store.pipe(select(LoadingStore.Selectors.IsLoading), filter(i => !!i)).subscribe(() => {
      if (!this.busy || this.busy.closed) {
        this.busy = this.store.select(LoadingStore.Selectors.IsLoading).subscribe(isLoading => {
          if (!isLoading) {
            this.busy.unsubscribe();
          }
        });
      }
    })
  }
}

import { Component, Input, OnDestroy } from '@angular/core';
import { LoadingStore } from '@core/store';
import { Store } from '@ngrx/store';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
  styleUrls: ['./page.component.scss'],
})
export class PageComponent implements OnDestroy {
  @Input() public mode: 'default' | 'full';

  private subscriptions: Subscription[] = [];
  public busy: Subscription;

  constructor(
    private store: Store
  ) {
    this.mode = 'default';
    let busySubscription = this.store.select(LoadingStore.Selectors.IsLoading).pipe(filter(i => !!i)).subscribe(() => {
      if (!this.busy || this.busy.closed) {
        this.busy = this.store.select(LoadingStore.Selectors.IsLoading).subscribe(isLoading => {
          if (!isLoading) {
            this.busy.unsubscribe();
          }
        });
      }
    });
    this.subscriptions.push(busySubscription);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(i => i.unsubscribe());
  }
}

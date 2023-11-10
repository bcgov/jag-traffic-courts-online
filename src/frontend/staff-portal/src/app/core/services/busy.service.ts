import { Injectable, OnDestroy } from '@angular/core';
import { LoadingStore } from '@core/store';
import { Store } from '@ngrx/store';
import { BehaviorSubject, Observable, Subscription, filter } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BusyService implements OnDestroy {
  private subscriptions: Subscription[] = [];

  // TODO: Investigate if there is a better solution or it should be putting inside store.
  // FIXME: Can't use in multiple location now
  private _busy: BehaviorSubject<Subscription> = new BehaviorSubject<Subscription>(null);

  constructor(
    private store: Store
  ) {
    let busySubscription = this.store.select(LoadingStore.Selectors.IsLoading).pipe(filter(i => !!i)).subscribe(() => {
      if (!this.busy || this.busy.closed) {
        this._busy.next(this.store.select(LoadingStore.Selectors.IsLoading).subscribe(isLoading => {
          if (!isLoading) {
            this.busy.unsubscribe();
          }
        }));
      }
    });
    this.subscriptions.push(busySubscription);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(i => i.unsubscribe());
  }

  public get busy$(): Observable<Subscription> {
    return this._busy.asObservable();
  }

  public get busy(): Subscription {
    return this._busy.value;
  }
}

import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpInterceptor, HttpResponse } from '@angular/common/http';
import { catchError, tap } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { LoadingStore } from '@core/store';

@Injectable({ providedIn: "root" })
export class LoadingInterceptor implements HttpInterceptor {
  // TODO: add urls here to suppress the loading overlay, such as autocomplete
  private slientLoadingUrls: string[] = [];

  constructor(
    private store: Store
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    let isSlient = this.isSlient(request.url);
    if (!isSlient) {
      this.store.dispatch(LoadingStore.Actions.Add());
    }

    return next.handle(request).pipe(
      tap(event => {
        if (event instanceof HttpResponse && event && !isSlient) {
          this.store.dispatch(LoadingStore.Actions.Remove());
        }
      }),
      catchError(err => {
        if (!isSlient) {
          this.store.dispatch(LoadingStore.Actions.Remove());
        }
        throw err;
      })
    );
  }

  private isSlient(requestUrl): boolean {
    return this.slientLoadingUrls.map(url => requestUrl.indexOf(url) === 0 ? true : false).filter(i => i).length > 0;
  }
}
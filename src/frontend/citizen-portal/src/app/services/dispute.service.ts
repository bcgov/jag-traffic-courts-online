import { Injectable } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { Store } from "@ngrx/store";
import { DisputeNotFoundDialogComponent } from "@shared/dialogs/dispute-not-found-dialog/dispute-not-found-dialog.component";
import { DisputeStatusDialogComponent } from "@shared/dialogs/dispute-status-dialog/dispute-status-dialog.component";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { DisputesService, SearchDisputeResult } from "app/api";
import { AppRoutes } from "app/app.routes";
import { DisputeStore } from "app/store";
import { map, Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class DisputeService {
  constructor(
    private dialog: MatDialog,
    private disputesService: DisputesService,
    private route: ActivatedRoute,
    private router: Router,
    private store: Store,
  ) {
  }

  checkStoredDispute(suppressRedirect?: boolean): Observable<boolean> {
    return this.store.select(DisputeStore.Selectors.State).pipe(
      map(state => {
        let found = false;
        if (!state.loading) {
          if (!state.result && !state.params) {
            let params = <QueryParamsForSearch>this.route.snapshot.queryParams;
            if (!params?.ticketNumber || !params.time) {
              if (!suppressRedirect) {
                this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND_DISPUTE)]);
              }
            } else {
              this.store.dispatch(DisputeStore.Actions.Search({ params }));
            }
          } else if (!state.result && state.params) {
            this.store.dispatch(DisputeStore.Actions.Search({}));
          }
          else {
            found = true;
          }
        }
        return found;
      })
    )
  }

  searchDispute(params: QueryParamsForSearch): Observable<SearchDisputeResult> {
    return this.disputesService.apiDisputesSearchGet(params.ticketNumber, params.time)
      .pipe(
        map((response: SearchDisputeResult) => {
          return response;
        })
      );
  }

  openDisputeNotFoundDialog() {
    return this.dialog.open(DisputeNotFoundDialogComponent, { width: '600px' });
  }

  showDisputeStatus(data: DisputeStore.State): void {
    this.dialog.open(DisputeStatusDialogComponent, {
      width: "60vw",
      data: data
    })
  }
}

export enum StatusStepType {
  SUBMITTED = "Submitted",
  PROCESSING = "Processing",
  SCHEDULED = "Hearing Scheduled",
  CONFIRMED = "Decision Made",
}

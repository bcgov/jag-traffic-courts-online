import { Injectable } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { DisputeStatusDialogComponent } from "@shared/dialogs/dispute-status-dialog/dispute-status-dialog.component";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { DisputesService, SearchDisputeResult, DisputeStatus } from "app/api";
import { DisputeStore } from "app/store";
import { map, Observable} from "rxjs";

@Injectable({
  providedIn: "root",
})
export class DisputeService {
  constructor(
    private dialog: MatDialog,
    private disputesService: DisputesService,
  ) {
  }

  searchDispute(params: QueryParamsForSearch): Observable<SearchDisputeResult> {
    return this.disputesService.apiDisputesSearchGet(params.ticketNumber, params.time)
      .pipe(
        map((response: SearchDisputeResult) => {
          return response;
        })
      );
  }

  showDisputeStatus(data: DisputeStore.StateData): void {
    this.dialog.open(DisputeStatusDialogComponent, {
      width: "60vw",
      data: data
    })
  }
}

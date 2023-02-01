import { Injectable } from "@angular/core";
import { FormGroup, AbstractControl } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { Store } from "@ngrx/store";
import { DisputeNotFoundDialogComponent } from "@shared/dialogs/dispute-not-found-dialog/dispute-not-found-dialog.component";
import { DisputeStatusDialogComponent } from "@shared/dialogs/dispute-status-dialog/dispute-status-dialog.component";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { DisputantContactInformation as DisputantContactInformationBase, DisputesService, SearchDisputeResult } from "app/api";
import { AppRoutes } from "app/app.routes";
import { DisputeStore } from "app/store";
import { map, Observable, of } from "rxjs";
import { NoticeOfDisputeService } from "./notice-of-dispute.service";

@Injectable({
  providedIn: "root",
})
export class DisputeService {
  disputantFormKeys: DisputantContactInformationKeys[] = [
    "disputant_surname",
    "disputant_given_names",
    "contact_given_names",
    "contact_type",
    "contact_law_firm_name",
    "address",
    "address_city",
    "address_province",
    "address_province_country_id",
    "address_province_seq_no",
    "address_country_id",
    "postal_code",
    "home_phone_number",
    "email_address",
  ]

  // copy setting from NoticeOfDisputeService instead of create a similar one
  disputantFormFields: DisputantContactInformationFormControls = {};

  constructor(
    private dialog: MatDialog,
    private noticeOfDisputesService: NoticeOfDisputeService,
    private disputesService: DisputesService,
    private route: ActivatedRoute,
    private router: Router,
    private store: Store,
  ) {
    this.disputantFormKeys.forEach(key => { // copy from NoticeOfDisputeService
      this.disputantFormFields[key] = this.noticeOfDisputesService.ticketFormFields[key];
    })
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
    return this.disputesService.apiDisputesSearchGet(params.ticketNumber, params.time);
  }

  updateDisputeContact(guid: string, input: DisputantContactInformation) {
    let payload = { ...input }; // state data cannot be changed, need to create a new one
    payload = this.noticeOfDisputesService.splitDisputantGivenNames(payload);  // break disputant names into first, second, third
    payload = this.noticeOfDisputesService.splitContactGivenNames(payload);  // break disputant names into first, second, third
    payload = this.noticeOfDisputesService.splitAddressLines(payload); // break address into line 1,2,3 by comma
    return this.disputesService.apiDisputeGuidHashContactPut(guid, payload);
  }

  openDisputeNotFoundDialog() {
    return this.dialog.open(DisputeNotFoundDialogComponent, { width: '600px' });
  }

  showDisputeStatus(state: DisputeStore.State): void {
    this.dialog.open(DisputeStatusDialogComponent, {
      width: "60vw",
      data: state
    })
  }

  goToUpdateDisputeLanding(params: QueryParamsForSearch): void {
    this.router.navigate([AppRoutes.disputePath(AppRoutes.UPDATE_DISPUTE)], {
      queryParams: params,
    });
  }

  goToUpdateDisputeContact(params: QueryParamsForSearch): void {
    this.router.navigate([AppRoutes.disputePath(AppRoutes.UPDATE_DISPUTE_CONTACT)], {
      queryParams: params,
    })
  }
}

export enum StatusStepType {
  SUBMITTED = "Submitted",
  PROCESSING = "Processing",
  SCHEDULED = "Hearing Scheduled",
  CONFIRMED = "Decision Made",
}

export interface DisputantContactInformation extends DisputantContactInformationBase {
  disputant_given_names?: string;
  contact_given_names?: string;
  address?: string;
}

export type DisputantContactInformationKeys = keyof DisputantContactInformation;
export type DisputantContactInformationFormControls = {
  [key in DisputantContactInformationKeys]?: AbstractControl;
}
export interface DisputantContactInformationFormGroup extends FormGroup {
  value: DisputantContactInformation;
  controls: DisputantContactInformationFormControls;
}

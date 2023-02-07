import { Injectable } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { Store } from "@ngrx/store";
import { DisputeNotFoundDialogComponent } from "@shared/dialogs/dispute-not-found-dialog/dispute-not-found-dialog.component";
import { DisputeStatusDialogComponent } from "@shared/dialogs/dispute-status-dialog/dispute-status-dialog.component";
import { QueryParamsForSearch } from "@shared/models/query-params-for-search.model";
import { DisputesService, SearchDisputeResult } from "app/api";
import { AppRoutes } from "app/app.routes";
import { AuthStore } from "app/auth/store";
import { DisputeStore } from "app/store";
import { cloneDeep } from "lodash";
import { map, Observable } from "rxjs";
import { DisputantContactInformation, DisputantContactInformationFormConfigs, DisputantContactInformationKeys, NoticeOfDispute, NoticeOfDisputeFormGroup, NoticeOfDisputeService } from "./notice-of-dispute.service";

@Injectable({
  providedIn: "root",
})
export class DisputeService {
  disputantFormKeys: DisputantContactInformationKeys[] = [
    "disputant_surname",
    "disputant_given_names",
    "contact_given_names",
    "contact_surname",
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

  // copy from NoticeOfDisputeService instead of create a similar one
  disputantFormConfigs: DisputantContactInformationFormConfigs = {};

  constructor(
    private dialog: MatDialog,
    private noticeOfDisputesService: NoticeOfDisputeService,
    private disputesService: DisputesService,
    private route: ActivatedRoute,
    private router: Router,
    private store: Store,
  ) {
    this.disputantFormConfigs = cloneDeep(this.noticeOfDisputesService.noticeOfDisputeFormConfigs);
    Object.keys(this.disputantFormConfigs).forEach(key => {
      if ((this.disputantFormKeys as String[]).indexOf(key) < 0) {
        delete this.disputantFormConfigs[key];
      }
    })
  }

  getDisputantForm(): NoticeOfDisputeFormGroup {
    return this.noticeOfDisputesService.getNoticeOfDisputeForm({}, this.disputantFormConfigs);
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

  getDispute(token: string): Observable<NoticeOfDispute> {
    return this.disputesService.apiDisputesGuidHashGet(token).pipe(
      map((noticeOfDispute: NoticeOfDispute) => {
        noticeOfDispute.disputant_given_names = [noticeOfDispute.disputant_given_name1, noticeOfDispute.disputant_given_name2, noticeOfDispute.disputant_given_name3].filter(i => i).join(" ");
        noticeOfDispute.lawyer_full_name = [noticeOfDispute.lawyer_given_name1, noticeOfDispute.lawyer_given_name2, noticeOfDispute.lawyer_given_name3, noticeOfDispute.lawyer_surname].filter(i => i).join(" ");
        noticeOfDispute.address = [noticeOfDispute.address_line1, noticeOfDispute.address_line2, noticeOfDispute.address_line3].filter(i => i).join(",");
        return noticeOfDispute;
      }))
  }

  updateDisputeContact(guid: string, input: DisputantContactInformation) {
    let payload = this.disputantContactToServer(input);
    return this.disputesService.apiDisputeGuidHashContactPut(guid, payload);
  }

  updateDispute(guid: string, input: NoticeOfDispute) {
    let payload = this.disputeToServer(input);
    console.log(payload);
    return this.disputesService.apiDisputesGuidHashPut(guid, payload);
  }

  private disputantContactToServer(input: DisputantContactInformation): DisputantContactInformation {
    let payload = { ...input }; // state data cannot be changed, need to create a new one
    payload = this.noticeOfDisputesService.splitDisputantGivenNames(payload);  // break disputant names into first, second, third
    payload = this.noticeOfDisputesService.splitContactGivenNames(payload);  // break disputant names into first, second, third
    payload = this.noticeOfDisputesService.splitAddressLines(payload); // break address into line 1,2,3 by comma
    return payload;
  }

  private disputeToServer(input: NoticeOfDispute): NoticeOfDispute {
    let payload: NoticeOfDispute = this.disputantContactToServer(input);
    payload = this.noticeOfDisputesService.splitLawyerNames(payload);
    return payload;
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
    this.router.navigate([AppRoutes.disputePath(AppRoutes.UPDATE_DISPUTE_LANDING)], {
      queryParams: params,
    });
  }

  goToUpdateDisputeContact(params: QueryParamsForSearch): void {
    this.router.navigate([AppRoutes.disputePath(AppRoutes.UPDATE_DISPUTE_CONTACT)], {
      queryParams: params,
    })
  }

  goToUpdateDispute(params: QueryParamsForSearch): void {
    this.store.dispatch(AuthStore.Actions.Authorize({ redirectUrl: this.getUpdateDisputeUrl(params) }));
  }

  private getUpdateDisputeUrl(params: QueryParamsForSearch): string {
    let queryString = Object.keys(params).map(function (k) {
      return encodeURIComponent(k) + '=' + encodeURIComponent(params[k])
    }).join('&');
    return AppRoutes.disputePath(AppRoutes.UPDATE_DISPUTE) + "?" + queryString;
  }
}

export enum StatusStepType {
  SUBMITTED = "Submitted",
  PROCESSING = "Processing",
  SCHEDULED = "Hearing Scheduled",
  CONFIRMED = "Decision Made",
}

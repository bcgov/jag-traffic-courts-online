import { Component, OnInit, ViewChild} from '@angular/core';
import { filter, Observable, Subscription } from 'rxjs';
import { FormControl } from '@angular/forms';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { JJDisputeService, JJDispute } from 'app/services/jj-dispute.service';
import { MatTab } from '@angular/material/tabs';
import { AppState } from 'app/store';
import { select, Store } from '@ngrx/store';
import * as JJDisputeStore from "app/store/jj-dispute";

@Component({
  selector: 'app-jj-workbench-dashboard',
  templateUrl: './jj-workbench-dashboard.component.html',
  styleUrls: ['./jj-workbench-dashboard.component.scss'],
})
export class JjWorkbenchDashboardComponent implements OnInit {
  @ViewChild("DCF") private dcfTab: MatTab;
  userProfile: KeycloakProfile = {};
  busy: Subscription;
  
  data$: Observable<JJDispute[]>;
  showDispute: boolean = false;
  tabSelected = new FormControl(0);
  jjPage: string = "WR Assignments";
  jjAdminRole: boolean = false;
  jjDisputeInfo: JJDispute;
  isInfoEditable: boolean = false;

  constructor(
    private authService: AuthService,
    private jjDisputeService: JJDisputeService,
    private store: Store<AppState>
  ) {
  }

  ngOnInit() {
    this.authService.userProfile$.subscribe(userProfile => {
      if (userProfile) {
        this.userProfile = userProfile;
        this.jjAdminRole = this.authService.checkRole("admin-judicial-justice");
      }
    })
    this.data$ = this.store.pipe(select(state => state.jjDispute.data), filter(i => !!i));
    // this.store.dispatch(JJDisputeStore.Actions.Get());
  }

  changeJJDispute(jjDispute: JJDispute) {
    this.isInfoEditable = !this.dcfTab.isActive;
    this.jjDisputeInfo = jjDispute;
    this.showDispute = true;
  }

  backInbox() {
    this.showDispute = false;
    this.jjDisputeService.refreshDisputes.emit();
  }
}

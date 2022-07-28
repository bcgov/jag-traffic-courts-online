import { Component, OnInit, ViewChild, AfterViewInit, Output, EventEmitter, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { JJDisputeService } from 'app/services/jj-dispute.service';
import { JJDispute } from '../../api/model/jJDispute.model';
import { LoggerService } from '@core/services/logger.service';
import { Subscription } from 'rxjs';
import { JJDisputeStatus } from 'app/api';
import { JwtHelperService } from '@auth0/angular-jwt';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-jj-dispute-decision-inbox',
  templateUrl: './jj-dispute-decision-inbox.component.html',
  styleUrls: ['./jj-dispute-decision-inbox.component.scss'],
})
export class JJDisputeDecisionInboxComponent implements OnInit, AfterViewInit {
  @Output() public jjPage: EventEmitter<string> = new EventEmitter();
  @Input() public ddIDIR: string;

  busy: Subscription;
  jjDisputeInfo: JJDispute;
  public isLoggedIn = false;
  public jwtHelper: JwtHelperService
  public ddRole: boolean = false;
  public fullName: string = "Loading...";
  data = [] as JJDisputeView[];
  showDispute: boolean = false;
  dataSource = new MatTableDataSource();
  @ViewChild(MatSort) sort = new MatSort();
  displayedColumns: string[] = [
    "ticketNumber",
    "jjDecisionDate",
    "jjAssignedTo",
    "violationDate",
    "disputantName",
    "courthouseLocation",
    "vtcAssignedTo"
    ];

  constructor(
    public jjDisputeService: JJDisputeService,
    private oidcSecurityService: OidcSecurityService,
    private logger: LoggerService,
  ) {

    // check for dd role
    this.oidcSecurityService.checkAuth().subscribe(({ isAuthenticated }) => {
      if (isAuthenticated) {
        this.isLoggedIn = isAuthenticated;

        // decode the token to get its payload
        const tokenPayload = this.jwtHelper.decodeToken(this.oidcSecurityService.getAccessToken());
        if (tokenPayload) {
          let resource_access = tokenPayload.resource_access["tco-staff-portal"];
          if (resource_access) {
            let roles = resource_access.roles;
            if (roles) roles.forEach(role => {

              if (role === "vtc-user") { // TODO USE role name for jj Admin
                this.ddRole = true;
              }
            });
          }
        }

        this.fullName = this.oidcSecurityService.getUserData()?.name;
        this.ddIDIR = this.oidcSecurityService.getUserData()?.preferred_username;
      }
    });
  }

  ngOnInit(): void {
    this.getAll();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  backTicketList(element) {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage.emit("Dispute Details");
    else this.jjPage.emit("Inbox");
    this.jjDisputeInfo = element;
    if (!this.showDispute) this.getAll();  // refresh list
  }

  backTicketpage() {
    this.showDispute = !this.showDispute;
    if (this.showDispute) this.jjPage.emit("Dispute Details");
    else this.jjPage.emit("Inbox");
    if (!this.showDispute) this.getAll(); // refresh list
  }

  getAll(): void {
    this.logger.log('JJDisputeDecisionInboxComponent::getAllDisputes');

    this.jjDisputeService.getJJDisputes().subscribe((response: JJDisputeView[]) => {
      // filter jj disputes only show those in CONFIRMED status
      this.data = response.filter(x => x.status == JJDisputeStatus.Confirmed);
      this.dataSource.data = this.data;

      this.data.forEach(x => {
        x.jjAssignedToName = this.jjDisputeService.jjList.filter(y => y.idir === x.jjAssignedTo)[0]?.name;
        x.vtcAssignedToName = this.jjDisputeService.jjList.filter(y => y.idir === x.vtcAssignedTo)[0]?.name;
      });

      // initially sort by decision date within status
      this.dataSource.data = this.dataSource.data.sort((a: JJDispute, b: JJDispute) =>
        {
          if (a.jjDecisionDate > b.jjDecisionDate) { return 1; } else { return -1; }
        });
    });
  }
}
export interface JJDisputeView extends JJDispute {
  vtcAssignedToName: string;
  jjAssignedToName: string;
}

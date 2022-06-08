import { Component, Input } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
@Component({
  selector: 'app-tco-page-header',
  templateUrl: './tco-page-header.component.html',
  styleUrls: ['./tco-page-header.component.scss', '../../../app.component.scss'],
})
export class TcoPageHeaderComponent {
  todayDate: Date = new Date();
  RegionName: string = "Fraser Valley Region";
  fullName: string = "Loading...";

  constructor ( private oidcSecurityService: OidcSecurityService) {

    oidcSecurityService.userData$.subscribe( (userInfo: any) => {
      if (userInfo && userInfo.userData && userInfo.userData.name) this.fullName = userInfo.userData.name;
    });

  }
  

  @Input() showHorizontalRule = true;
}

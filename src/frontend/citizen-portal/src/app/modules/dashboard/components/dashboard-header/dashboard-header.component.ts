import {
  Component,
  ChangeDetectionStrategy,
  Output,
  EventEmitter,
  Input,
  OnInit,
  ChangeDetectorRef,
} from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { KeycloakService } from 'keycloak-angular';

@Component({
  selector: 'app-dashboard-header',
  templateUrl: './dashboard-header.component.html',
  styleUrls: ['./dashboard-header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardHeaderComponent implements OnInit {
  public username: string;
  @Input() public isMobile: boolean;
  @Input() public hasMobileSidemenu: boolean;
  @Output() public toggle: EventEmitter<void>;

  constructor(
    protected keycloakService: KeycloakService,
    private cdRef: ChangeDetectorRef,
    protected logger: LoggerService
  ) {
    this.hasMobileSidemenu = true;
    this.toggle = new EventEmitter<void>();
  }

  public ngOnInit(): Promise<void> {
    try {
      this.keycloakService.isLoggedIn().then((result) => {
        if (result) {
          this.keycloakService.loadUserProfile().then((profile) => {
            this.username = `${profile.firstName} ${profile.lastName}`;
            this.cdRef.detectChanges();
          });
        }
      });
    } catch (e) {
      this.logger.error('Failed to load user details', e);
    }
    return Promise.resolve();
  }

  public toggleSidenav(): void {
    this.toggle.emit();
  }

  public onLogout(): Promise<void> {
    this.keycloakService.logout(
      `${window.location.protocol}//${window.location.host}`
    );
    return Promise.resolve();
  }
}

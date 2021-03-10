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
import { User } from 'app/modules/auth/models/user.model';
import { AuthService } from 'app/modules/auth/services/auth.service';

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
    protected authService: AuthService,
    private cdRef: ChangeDetectorRef,
    protected logger: LoggerService
  ) {
    this.hasMobileSidemenu = true;
    this.toggle = new EventEmitter<void>();
  }

  public ngOnInit() {
    this.authService.getUser$().subscribe(({ firstName, lastName }: User) => {
      this.username = `${firstName} ${lastName}`;
    });
    this.cdRef.detectChanges();
  }

  public toggleSidenav(): void {
    this.toggle.emit();
  }

  public onLogout(): Promise<void> {
    this.authService.logout(
      `${window.location.protocol}//${window.location.host}`
    );
    return Promise.resolve();
  }
}

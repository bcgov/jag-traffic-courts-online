import {
  Component,
  ChangeDetectionStrategy,
  Output,
  EventEmitter,
  Input,
  OnInit,
} from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { User } from '@shared/models/user.model';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class HeaderComponent implements OnInit {
  public fullName: string;
  @Input() public isMobile: boolean;
  @Input() public hasMobileSidemenu: boolean;
  @Output() public toggle: EventEmitter<void>;

  constructor(
    protected authService: AuthService,
    protected logger: LoggerService
  ) {
    this.hasMobileSidemenu = false;
    this.toggle = new EventEmitter<void>();
  }

  public async ngOnInit() {
    const authenticated = await this.authService.isLoggedIn();
    if (authenticated) {
      this.authService.getUser$().subscribe((user: User) => {
        this.fullName = `${user?.firstName} ${user?.lastName}`;
      });
    }
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

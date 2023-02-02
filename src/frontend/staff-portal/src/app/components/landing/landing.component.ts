import { OnInit, Component, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { AppRoutes } from 'app/app.routes';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss', '../../app.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LandingComponent implements OnInit {
  public isLoggedIn = false;

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {
  }

  public ngOnInit() {
    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      this.isLoggedIn = isLoggedIn;
      if (this.isLoggedIn) {
        this.authService.userProfile$.subscribe(() => {
          this.router.navigate([this.authService.getRedirectUrl()]);
        })
      }
    })
  }

  public login() {
    this.authService.login();
  }
}

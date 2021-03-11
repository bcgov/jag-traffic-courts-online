import {
  ComponentFixture,
  fakeAsync,
  flush,
  TestBed,
} from '@angular/core/testing';
import { CoreModule } from '@core/core.module';
import { LoggerService } from '@core/services/logger.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { NgxProgressModule } from '@shared/modules/ngx-progress/ngx-progress.module';
import { AuthService } from 'app/modules/auth/services/auth.service';
import { EMPTY, Observable, of } from 'rxjs';
import { MockAuthService } from 'tests/mocks/mock-auth.service';
import { DashboardHeaderComponent } from './dashboard-header.component';

describe('DashboardHeaderComponent', () => {
  let component: DashboardHeaderComponent;
  let authService: AuthService;
  let fixture: ComponentFixture<DashboardHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxMaterialModule, NgxProgressModule, CoreModule],
      declarations: [DashboardHeaderComponent],
      providers: [
        DashboardHeaderComponent,
        { provide: AuthService, useClass: MockAuthService },
        LoggerService,
      ],
    })
      .compileComponents()
      .then(() => {
        fixture = TestBed.createComponent(DashboardHeaderComponent);
      });

    component = TestBed.inject(DashboardHeaderComponent);
    authService = TestBed.inject(AuthService);
  });

  it('should not have username after construction', () => {
    expect(component.fullName).toBeUndefined();
  });

  it('should have username after Angular calls ngOnInit', fakeAsync(() => {
    component.ngOnInit();
    fixture.detectChanges();
    expect(component.fullName).toContain('mockFirstName mockLastName');
    flush();
  }));

  it('username undefined after Angular calls ngOnInit and user not loggedIn', () => {
    spyOn(authService, 'getUser$').and.returnValue(EMPTY);
    component.ngOnInit();
    expect(component.fullName).toBeUndefined();
  });

  it('should call logout after angular call logout', () => {
    spyOn(authService, 'logout').and.callThrough();
    component.onLogout().then(() => {
      expect(component.fullName).toBeUndefined();
      expect(authService.logout).toHaveBeenCalled();
    });
  });
});

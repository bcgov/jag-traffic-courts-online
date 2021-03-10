import { ChangeDetectorRef } from '@angular/core';
import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';
import { CoreModule } from '@core/core.module';
import { LoggerService } from '@core/services/logger.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { NgxProgressModule } from '@shared/modules/ngx-progress/ngx-progress.module';
import { KeycloakService } from 'keycloak-angular';
import { MockKeycloakService } from 'tests/mocks/mock-keycloak.service';
import { DashboardHeaderComponent } from './dashboard-header.component';

describe('DashboardHeaderComponent', () => {
  let component: DashboardHeaderComponent;
  let testBedKeycloak: KeycloakService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxMaterialModule, NgxProgressModule, CoreModule],
      declarations: [DashboardHeaderComponent],
      providers: [
        DashboardHeaderComponent,
        { provide: KeycloakService, useClass: MockKeycloakService },
        LoggerService,
        ChangeDetectorRef,
      ],
    }).compileComponents();

    component = TestBed.inject(DashboardHeaderComponent);
    testBedKeycloak = TestBed.inject(KeycloakService);
  });

  it('should not have username after construction', () => {
    expect(component.username).toBeUndefined();
  });

  it('should have username after Angular calls ngOnInit', () => {
    component.ngOnInit().then(() => {
      expect(component.username).toContain('mockfirstname mocklastname');
    });
  });

  it('username undefined after Angular calls ngOnInit and user not loggedIn', () => {
    spyOn(testBedKeycloak, 'isLoggedIn').and.returnValue(
      Promise.resolve(false)
    );
    component.ngOnInit().then(() => {
      expect(component.username).toBeUndefined;
    });
  });

  it('should call logout after angular call logout', () => {
    spyOn(testBedKeycloak, 'logout').and.callThrough();
    component.onLogout().then(() => {
      expect(component.username).toBeUndefined;
      expect(testBedKeycloak.logout).toHaveBeenCalled();
    });
  });
});

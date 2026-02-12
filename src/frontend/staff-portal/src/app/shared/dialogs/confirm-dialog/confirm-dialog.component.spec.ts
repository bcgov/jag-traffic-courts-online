import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';

import { ConfirmDialogComponent } from './confirm-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService, KeycloakProfile } from 'app/services/auth.service';
import { of } from 'rxjs';

describe('ConfirmDialogComponent', () => {
  let component: ConfirmDialogComponent;
  let fixture: ComponentFixture<ConfirmDialogComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        imports: [NgxMaterialModule, HttpClientTestingModule, TranslateModule.forRoot()],
        declarations: [ConfirmDialogComponent],
        providers: [
          {
            provide: MatDialogRef,
            useValue: {
              close: (dialogResult: any) => {},
            },
          },
          {
            provide: MAT_DIALOG_DATA,
            useValue: {},
          },
          {
            provide: AuthService,
            useValue: {
              userProfile$: of({ idir: "TEST" } as KeycloakProfile), 
            }
          },
        ],
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';

import { ConfirmReasonDialogComponent } from './confirm-reason-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

describe('ConfirmReasonDialogComponent', () => {
  let component: ConfirmReasonDialogComponent;
  let fixture: ComponentFixture<ConfirmReasonDialogComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        imports: [NgxMaterialModule, TranslateModule.forRoot()],
        declarations: [ConfirmReasonDialogComponent],
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
        ],
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmReasonDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

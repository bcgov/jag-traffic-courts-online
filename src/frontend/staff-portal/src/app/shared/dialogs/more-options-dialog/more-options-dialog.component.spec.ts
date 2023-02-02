import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';

import { MoreOptionsDialogComponent } from './more-options-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

describe('ConfirmDialogComponent', () => {
  let component: MoreOptionsDialogComponent;
  let fixture: ComponentFixture<MoreOptionsDialogComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        imports: [NgxMaterialModule, TranslateModule.forRoot()],
        declarations: [MoreOptionsDialogComponent],
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
    fixture = TestBed.createComponent(MoreOptionsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

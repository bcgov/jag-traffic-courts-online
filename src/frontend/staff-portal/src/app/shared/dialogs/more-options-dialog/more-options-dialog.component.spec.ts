import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';

import { MoreOptionsDialogComponent } from './more-options-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

describe('ConfirmDialogComponent', () => {
  let component: MoreOptionsDialogComponent;
  let fixture: ComponentFixture<MoreOptionsDialogComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        imports: [NgxMaterialModule],
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
          provideTranslateService({
            loader: provideTranslateHttpLoader({ prefix: './assets/i18n/', suffix: '.json'}),
            extend: true,
          })
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

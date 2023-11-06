import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatLegacyDialog as MatDialog, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';

import { DisputeNotFoundDialogComponent } from './dispute-not-found-dialog.component';

describe('DisputeNotFoundDialogComponent', () => {
  let component: DisputeNotFoundDialogComponent;
  let fixture: ComponentFixture<DisputeNotFoundDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DisputeNotFoundDialogComponent],
      providers: [
        {
          provide: MatDialog,
          useValue: {
            close: (dialogResult: any) => { },
          },
        },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {},
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeNotFoundDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

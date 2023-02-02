import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DisputeStatusDialogComponent } from './dispute-status-dialog.component';

describe('DisputeStatusDialogComponent', () => {
  let component: DisputeStatusDialogComponent;
  let fixture: ComponentFixture<DisputeStatusDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DisputeStatusDialogComponent],
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
    fixture = TestBed.createComponent(DisputeStatusDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

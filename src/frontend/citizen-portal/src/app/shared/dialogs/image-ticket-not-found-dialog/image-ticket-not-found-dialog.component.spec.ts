import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatLegacyDialog as MatDialog, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef } from '@angular/material/legacy-dialog';

import { ImageTicketNotFoundDialogComponent } from './image-ticket-not-found-dialog.component';

describe('ImageTicketNotFoundDialogComponent', () => {
  let component: ImageTicketNotFoundDialogComponent;
  let fixture: ComponentFixture<ImageTicketNotFoundDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImageTicketNotFoundDialogComponent ],
      providers: [
        {
          provide: MatDialogRef,
          useValue: {
            close: (dialogResult: any) => {},
          },
        },
        {
          provide: MatDialog,
          useValue: {
            close: (dialogResult: any) => {},
          },
        },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {},
        },
      ],
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImageTicketNotFoundDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatLegacyDialog as MatDialog, MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';

import { TicketInformationDialogComponent } from './ticket-information-dialog.component';

describe('TicketInformationDialogComponent', () => {
  let component: TicketInformationDialogComponent;
  let fixture: ComponentFixture<TicketInformationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TicketInformationDialogComponent],
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
    fixture = TestBed.createComponent(TicketInformationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

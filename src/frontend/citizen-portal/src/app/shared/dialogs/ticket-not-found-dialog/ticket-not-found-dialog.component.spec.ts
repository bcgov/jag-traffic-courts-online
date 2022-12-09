import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { TicketNotFoundDialogComponent } from './ticket-not-found-dialog.component';

describe('TicketNotFoundDialogComponent', () => {
  let component: TicketNotFoundDialogComponent;
  let fixture: ComponentFixture<TicketNotFoundDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TicketNotFoundDialogComponent],
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
    fixture = TestBed.createComponent(TicketNotFoundDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

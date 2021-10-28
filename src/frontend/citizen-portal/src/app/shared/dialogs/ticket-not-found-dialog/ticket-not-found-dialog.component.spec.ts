import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketNotFoundDialogComponent } from './ticket-not-found-dialog.component';

describe('TicketNotFoundDialogComponent', () => {
  let component: TicketNotFoundDialogComponent;
  let fixture: ComponentFixture<TicketNotFoundDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TicketNotFoundDialogComponent ]
    })
    .compileComponents();
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

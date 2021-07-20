import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketExampleDialogComponent } from './ticket-example-dialog.component';

describe('TicketExampleDialogComponent', () => {
  let component: TicketExampleDialogComponent;
  let fixture: ComponentFixture<TicketExampleDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TicketExampleDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketExampleDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

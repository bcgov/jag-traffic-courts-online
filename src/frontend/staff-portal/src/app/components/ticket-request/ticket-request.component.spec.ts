import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketRequestComponent } from './ticket-request.component';

describe('TicketRequestComponent', () => {
  let component: TicketRequestComponent;
  let fixture: ComponentFixture<TicketRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TicketRequestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

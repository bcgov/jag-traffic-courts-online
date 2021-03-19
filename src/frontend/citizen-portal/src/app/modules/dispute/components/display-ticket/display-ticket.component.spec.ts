import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayTicketComponent } from './display-ticket.component';

describe('DisplayTicketComponent', () => {
  let component: DisplayTicketComponent;
  let fixture: ComponentFixture<DisplayTicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DisplayTicketComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

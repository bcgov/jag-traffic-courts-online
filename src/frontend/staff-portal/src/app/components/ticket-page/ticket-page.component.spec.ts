import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketPageComponent } from './ticket-page.component';

describe('TicketPageComponent', () => {
  let component: TicketPageComponent;
  let fixture: ComponentFixture<TicketPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TicketPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

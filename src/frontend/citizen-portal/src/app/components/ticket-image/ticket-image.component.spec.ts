import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketImageComponent } from './ticket-image.component';

describe('TicketImageComponent', () => {
  let component: TicketImageComponent;
  let fixture: ComponentFixture<TicketImageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TicketImageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

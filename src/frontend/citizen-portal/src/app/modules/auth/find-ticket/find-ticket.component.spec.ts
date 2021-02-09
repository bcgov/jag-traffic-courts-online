import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FindTicketComponent } from './find-ticket.component';

describe('FindTicketComponent', () => {
  let component: FindTicketComponent;
  let fixture: ComponentFixture<FindTicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FindTicketComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FindTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { CurrencyPipe } from '@angular/common';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShellTicketComponent } from './shell-ticket.component';

describe('ShellTicketComponent', () => {
  let component: ShellTicketComponent;
  let fixture: ComponentFixture<ShellTicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ShellTicketComponent],
      providers: [CurrencyPipe],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ShellTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

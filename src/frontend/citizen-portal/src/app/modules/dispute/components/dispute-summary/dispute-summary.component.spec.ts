import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisputeSummaryComponent } from './dispute-summary.component';

describe('DisputeSummaryComponent', () => {
  let component: DisputeSummaryComponent;
  let fixture: ComponentFixture<DisputeSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisputeSummaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

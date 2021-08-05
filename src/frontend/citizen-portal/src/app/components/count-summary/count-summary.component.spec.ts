import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountSummaryComponent } from './count-summary.component';

describe('CountSummaryComponent', () => {
  let component: CountSummaryComponent;
  let fixture: ComponentFixture<CountSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CountSummaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CountSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

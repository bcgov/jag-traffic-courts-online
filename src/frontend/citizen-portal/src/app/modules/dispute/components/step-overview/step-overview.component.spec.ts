import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepOverviewComponent } from './step-overview.component';

describe('StepOverviewComponent', () => {
  let component: StepOverviewComponent;
  let fixture: ComponentFixture<StepOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StepOverviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

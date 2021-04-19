import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisputeAllStepperComponent } from './dispute-all-stepper.component';

describe('DisputeAllStepperComponent', () => {
  let component: DisputeAllStepperComponent;
  let fixture: ComponentFixture<DisputeAllStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisputeAllStepperComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeAllStepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

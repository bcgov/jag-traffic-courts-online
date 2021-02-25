import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepCourtComponent } from './step-court.component';

describe('StepCourtComponent', () => {
  let component: StepCourtComponent;
  let fixture: ComponentFixture<StepCourtComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StepCourtComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepCourtComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

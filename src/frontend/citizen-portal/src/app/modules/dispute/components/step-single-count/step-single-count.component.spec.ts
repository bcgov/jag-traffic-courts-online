import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepSingleCountComponent } from './step-single-count.component';

describe('StepSingleCountComponent', () => {
  let component: StepSingleCountComponent;
  let fixture: ComponentFixture<StepSingleCountComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StepSingleCountComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepSingleCountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

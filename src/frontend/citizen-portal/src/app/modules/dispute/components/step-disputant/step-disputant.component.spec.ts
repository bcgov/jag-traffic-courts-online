import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepDisputantComponent } from './step-disputant.component';

describe('StepDisputantComponent', () => {
  let component: StepDisputantComponent;
  let fixture: ComponentFixture<StepDisputantComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StepDisputantComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepDisputantComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

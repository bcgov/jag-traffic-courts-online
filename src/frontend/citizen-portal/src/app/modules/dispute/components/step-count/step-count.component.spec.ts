import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepCountComponent } from './step-count.component';

describe('StepCountComponent', () => {
  let component: StepCountComponent;
  let fixture: ComponentFixture<StepCountComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StepCountComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepCountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

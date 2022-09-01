import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisputeDecisionInfoComponent } from './dispute-decision-info.component';

describe('DisputeDecisionInfoComponent', () => {
  let component: DisputeDecisionInfoComponent;
  let fixture: ComponentFixture<DisputeDecisionInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisputeDecisionInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeDecisionInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

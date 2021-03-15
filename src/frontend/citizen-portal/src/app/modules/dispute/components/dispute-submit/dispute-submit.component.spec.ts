import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisputeSubmitComponent } from './dispute-submit.component';

describe('DisputeSubmitComponent', () => {
  let component: DisputeSubmitComponent;
  let fixture: ComponentFixture<DisputeSubmitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisputeSubmitComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeSubmitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisputeDashboardComponent } from './dispute-dashboard.component';

describe('DisputeDashboardComponent', () => {
  let component: DisputeDashboardComponent;
  let fixture: ComponentFixture<DisputeDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisputeDashboardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

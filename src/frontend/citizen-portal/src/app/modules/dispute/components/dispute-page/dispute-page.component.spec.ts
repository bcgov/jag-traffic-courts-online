import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisputePageComponent } from './dispute-page.component';

describe('DisputePageComponent', () => {
  let component: DisputePageComponent;
  let fixture: ComponentFixture<DisputePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisputePageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

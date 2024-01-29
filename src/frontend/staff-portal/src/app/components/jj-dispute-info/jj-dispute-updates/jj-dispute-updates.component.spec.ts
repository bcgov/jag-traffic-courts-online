import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JjDisputeUpdatesComponent } from './jj-dispute-updates.component';

describe('JjDisputeUpdatesComponent', () => {
  let component: JjDisputeUpdatesComponent;
  let fixture: ComponentFixture<JjDisputeUpdatesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [JjDisputeUpdatesComponent]
    });
    fixture = TestBed.createComponent(JjDisputeUpdatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

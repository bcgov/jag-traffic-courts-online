import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisputeFooterComponent } from './dispute-footer.component';

describe('DisputeFooterComponent', () => {
  let component: DisputeFooterComponent;
  let fixture: ComponentFixture<DisputeFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisputeFooterComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResolutionFooterComponent } from './resolution-footer.component';

describe('ResolutionFooterComponent', () => {
  let component: ResolutionFooterComponent;
  let fixture: ComponentFixture<ResolutionFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ResolutionFooterComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResolutionFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

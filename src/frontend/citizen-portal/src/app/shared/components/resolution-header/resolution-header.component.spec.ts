import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResolutionHeaderComponent } from './resolution-header.component';

describe('ResolutionHeaderComponent', () => {
  let component: ResolutionHeaderComponent;
  let fixture: ComponentFixture<ResolutionHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ResolutionHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResolutionHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

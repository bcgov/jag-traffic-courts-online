import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TcoPageHeaderComponent } from './tco-page-header.component';

describe('TcoPageHeaderComponent', () => {
  let component: TcoPageHeaderComponent;
  let fixture: ComponentFixture<TcoPageHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TcoPageHeaderComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TcoPageHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

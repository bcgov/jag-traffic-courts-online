import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountFormFieldComponent } from './count-form-field.component';

describe('CountFormFieldComponent', () => {
  let component: CountFormFieldComponent;
  let fixture: ComponentFixture<CountFormFieldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CountFormFieldComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CountFormFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

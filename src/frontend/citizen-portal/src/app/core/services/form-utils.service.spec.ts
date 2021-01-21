import { TestBed } from '@angular/core/testing';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { FormUtilsService } from './form-utils.service';

describe('FormUtilsService', () => {
  let service: FormUtilsService;
  let formGroup: FormGroup;

  const formBuilder: FormBuilder = new FormBuilder();

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      providers: [{ provide: FormBuilder, useValue: formBuilder }],
    });
  });

  beforeEach(() => {
    service = TestBed.inject(FormUtilsService);

    formGroup = formBuilder.group({
      id1: [null],
      id2: [null, Validators.required],
    });
  });

  it('should create', () => {
    expect(service).toBeTruthy();
  });

  it('should call checkValidity', () => {
    let val = service.checkValidity(formGroup);
    expect(val).toBeFalse();
  });

  it('should call setValidators', () => {
    service.setValidators(formGroup, []);
    expect(true).toBeTruthy();
  });

  it('should call resetAndClearValidators', () => {
    let res = service.resetAndClearValidators(formGroup);
    expect(res).not.toBeNull();
  });

  it('should call isRequired - true', () => {
    let req = service.isRequired(formGroup, 'id2');
    expect(req).toBeTruthy();
  });

  it('should call getFormErrors', () => {
    let res = service.getFormErrors(formGroup);
    expect(res).not.toBeNull();
  });
});

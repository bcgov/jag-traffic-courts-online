import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

import { DisputeFormStateService } from './dispute-form-state.service';

describe('DisputeFormStateService', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        RouterTestingModule,
        HttpClientTestingModule,
      ],
      providers: [],
    })
  );

  it('should create', () => {
    const service: DisputeFormStateService = TestBed.inject(
      DisputeFormStateService
    );
    expect(service).toBeTruthy();
  });
});

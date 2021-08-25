import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

import { DisputeFormStateService } from './dispute-form-state.service';
@Component({ selector: 'app-test-blank', template: `` })
class BlankComponent {}

describe('DisputeFormStateService', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
        HttpClientTestingModule,
      ],
      declarations: [BlankComponent],
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

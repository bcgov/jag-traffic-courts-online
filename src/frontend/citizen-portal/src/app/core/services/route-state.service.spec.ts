import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { RouteStateService } from './route-state.service';
@Component({ selector: 'app-test-blank', template: `` })
class BlankComponent {}

describe('RouteStateService', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
      ],
      declarations: [BlankComponent],
    })
  );

  it('should create', () => {
    const service: RouteStateService = TestBed.inject(RouteStateService);
    expect(service).toBeTruthy();
  });
});

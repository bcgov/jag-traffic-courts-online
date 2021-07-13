import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { FeatureFlagGuard } from './feature-flag.guard';

describe('FeatureFlagGuard', () => {
  let guard: FeatureFlagGuard;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
    });
    guard = TestBed.inject(FeatureFlagGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});

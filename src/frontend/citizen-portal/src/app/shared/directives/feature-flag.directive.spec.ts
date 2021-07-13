import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FeatureFlagDirective } from './feature-flag.directive';

describe('FeatureFlagDirective', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      declarations: [FeatureFlagDirective],
    }).compileComponents();
  });

  it('should create an instance', () => {
    const directive = new FeatureFlagDirective(null, null, null);
    expect(directive).toBeTruthy();
  });
});

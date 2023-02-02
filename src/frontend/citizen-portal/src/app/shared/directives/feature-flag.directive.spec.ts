import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FeatureFlagDirective } from './feature-flag.directive';
import { Component } from '@angular/core';
@Component({ selector: 'app-test-blank', template: `` })
class BlankComponent {}

describe('FeatureFlagDirective', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
      ],
      declarations: [FeatureFlagDirective, BlankComponent],
    }).compileComponents();
  });

  it('should create an instance', () => {
    const directive = new FeatureFlagDirective(null, null, null);
    expect(directive).toBeTruthy();
  });
});

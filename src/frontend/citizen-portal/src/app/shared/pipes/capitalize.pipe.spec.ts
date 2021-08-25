import { Component } from '@angular/core';
import { TestBed, waitForAsync } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { CapitalizePipe } from './capitalize.pipe';

@Component({ selector: 'test-blank', template: `` })
class BlankComponent {}

describe('CapitalizePipe', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
      ],
      declarations: [CapitalizePipe, BlankComponent],
    }).compileComponents();
  });

  it('create an instance', () => {
    const pipe = new CapitalizePipe();
    expect(pipe).toBeTruthy();
  });

  it('should capitalize a word', () => {
    const pipe = new CapitalizePipe();
    const after = pipe.transform('test one');
    expect(after).toBe('Test one');
  });

  it('should capitalize all words', () => {
    const pipe = new CapitalizePipe();
    const after = pipe.transform('test one', true);
    expect(after).toBe('Test One');
  });

  it('should handle null', () => {
    const pipe = new CapitalizePipe();
    const after = pipe.transform(null);
    expect(after).toBeNull();
  });
});

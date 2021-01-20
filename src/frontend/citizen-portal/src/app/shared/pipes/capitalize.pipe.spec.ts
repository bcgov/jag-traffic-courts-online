import { TestBed, waitForAsync } from '@angular/core/testing';
import { CapitalizePipe } from './capitalize.pipe';

describe('CapitalizePipe', () => {
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

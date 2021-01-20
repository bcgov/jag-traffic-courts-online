import { TestBed, inject, waitForAsync } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { PostalPipe } from './postal.pipe';

describe('PostalPipe', () => {
  it('create an instance', () => {
    const pipe = new PostalPipe();
    expect(pipe).toBeTruthy();
  });

  it('should return value', () => {
    const pipe = new PostalPipe();
    const after = pipe.transform('v6p2e6');
    expect(after).toBe('V6P 2E6');
  });

  it('should handle null', () => {
    const pipe = new PostalPipe();
    const after = pipe.transform(null);
    expect(after).toBe('');
  });
});

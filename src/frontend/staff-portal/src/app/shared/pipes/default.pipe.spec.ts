import { DefaultPipe } from './default.pipe';

describe('DefaultPipe', () => {
  it('create an instance', () => {
    const pipe = new DefaultPipe();
    expect(pipe).toBeTruthy();
  });

  it('should return value', () => {
    const pipe = new DefaultPipe();
    const after = pipe.transform('test');
    expect(after).toBe('test');
  });

  it('should handle null', () => {
    const pipe = new DefaultPipe();
    const after = pipe.transform(null);
    expect(after).toBe('-');
  });
});

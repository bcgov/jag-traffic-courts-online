import { ReplacePipe } from './replace.pipe';

describe('ReplacePipe', () => {
  it('create an instance', () => {
    const pipe = new ReplacePipe();
    expect(pipe).toBeTruthy();
  });

  it('should return value', () => {
    const pipe = new ReplacePipe();
    const after = pipe.transform('abcdefabc', 'abc', 'ghi');
    expect(after).toBe('ghidefghi');
  });

  it('should handle null', () => {
    const pipe = new ReplacePipe();
    const after = pipe.transform(null, null, null);
    expect(after).toBeNull();
  });
});

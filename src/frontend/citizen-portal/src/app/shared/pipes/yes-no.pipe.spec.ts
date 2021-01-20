import { YesNoPipe } from './yes-no.pipe';

describe('YesNoPipe', () => {
  it('create an instance', () => {
    const pipe = new YesNoPipe();
    expect(pipe).toBeTruthy();
  });

  it('should return Yes', () => {
    const pipe = new YesNoPipe();
    const after = pipe.transform('abc');
    expect(after).toBe('Yes');
  });

  it('should return No', () => {
    const pipe = new YesNoPipe();
    const after = pipe.transform(null, true);
    expect(after).toBe('No');
  });

  it('should handle null', () => {
    const pipe = new YesNoPipe();
    const after = pipe.transform(null);
    expect(after).toBe('');
  });
});

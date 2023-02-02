import { PhonePipe } from './phone.pipe';

describe('PhonePipe', () => {
  it('create an instance', () => {
    const pipe = new PhonePipe();
    expect(pipe).toBeTruthy();
  });

  it('should return value', () => {
    const pipe = new PhonePipe();
    const after = pipe.transform('2501234567');
    expect(after).toBe('(250) 123-4567');
  });

  it('should handle null', () => {
    const pipe = new PhonePipe();
    const after = pipe.transform(null);
    expect(after).toBeNull();
  });
});

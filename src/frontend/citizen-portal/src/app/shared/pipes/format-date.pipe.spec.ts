import { FormatDatePipe } from './format-date.pipe';

describe('DatePipe', () => {
  it('create an instance', () => {
    const pipe = new FormatDatePipe();
    expect(pipe).toBeTruthy();
  });

  it('should format date', () => {
    const dateStr = '2020-01-01';
    const pipe = new FormatDatePipe();
    const after = pipe.transform(dateStr);
    expect(after).toBe('1 Jan 2020');
  });

  it('should handle null', () => {
    const pipe = new FormatDatePipe();
    const after = pipe.transform(null);
    expect(after).toBeNull();
  });
});

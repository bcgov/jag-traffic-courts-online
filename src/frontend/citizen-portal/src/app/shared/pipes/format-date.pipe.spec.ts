import { FormatDatePipe } from './format-date.pipe';

describe('DatePipe', () => {
  it('create an instance', () => {
    const pipe = new FormatDatePipe();
    expect(pipe).toBeTruthy();
  });

  it('should format date1', () => {
    const dateStr = '2020-01-01';
    const pipe = new FormatDatePipe();
    const after = pipe.transform(dateStr);
    expect(after).toBe('Jan 1 2020');
  });

  it('should format date2', () => {
    const dateStr = '2020 01 01';
    const pipe = new FormatDatePipe();
    const after = pipe.transform(dateStr);
    expect(after).toBe('Jan 1 2020');
  });

  it('should format date3', () => {
    const dateStr = 'Jan 1 2020';
    const pipe = new FormatDatePipe();
    const after = pipe.transform(dateStr);
    expect(after).toBe('Jan 1 2020');
  });

  it('should fail format date1', () => {
    const dateStr = 'a-1-1';
    const pipe = new FormatDatePipe();
    const after = pipe.transform(dateStr);
    expect(after).toBe('a-1-1');
  });

  it('should fail format date2', () => {
    const dateStr = 'a 1 1';
    const pipe = new FormatDatePipe();
    const after = pipe.transform(dateStr);
    expect(after).toBe('a 1 1');
  });

  it('should fail format date3', () => {
    const dateStr = 'test';
    const pipe = new FormatDatePipe();
    const after = pipe.transform(dateStr);
    expect(after).toBe('test');
  });

  it('should handle null', () => {
    const pipe = new FormatDatePipe();
    const after = pipe.transform(null);
    expect(after).toBeNull();
  });
});

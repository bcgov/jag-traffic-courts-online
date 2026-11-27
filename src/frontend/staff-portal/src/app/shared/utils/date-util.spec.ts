import { DateUtil } from './date-util';

describe('DateUtil', () => {
  describe('toLocalDateTimeString', () => {
    it('formats an afternoon time as local 24-hour time with no offset', () => {
      // 1:50 PM local, the time of day the TCVP-3602 save failure was reported at
      expect(DateUtil.toLocalDateTimeString(new Date(2026, 10, 26, 13, 50, 7))).toBe('2026-11-26T13:50:07');
    });

    it('zero-pads single-digit months, days, hours, minutes and seconds', () => {
      expect(DateUtil.toLocalDateTimeString(new Date(2026, 0, 2, 3, 4, 5))).toBe('2026-01-02T03:04:05');
    });

    it('returns null for an invalid date', () => {
      expect(DateUtil.toLocalDateTimeString(new Date('not a date'))).toBeNull();
    });

    it('returns null when there is no date', () => {
      expect(DateUtil.toLocalDateTimeString(null)).toBeNull();
      expect(DateUtil.toLocalDateTimeString(undefined)).toBeNull();
    });
  });
});

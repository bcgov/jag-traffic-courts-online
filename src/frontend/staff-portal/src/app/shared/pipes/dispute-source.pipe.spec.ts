import { DisputeSourcePipe, isCitizenSubmitted } from './dispute-source.pipe';

describe('DisputeSourcePipe', () => {
  const pipe = new DisputeSourcePipe();

  it('create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('should return Disputant for the Citizen Portal', () => {
    expect(pipe.transform('CITZ', 'OCCAMORDS')).toBe('Disputant');
  });

  it('should return STAFF with the user for the Staff Portal', () => {
    expect(pipe.transform('STAF', 'DAVEKAY')).toBe('STAFF (DAVEKAY)');
  });

  it('should return STAFF without a user when none is known', () => {
    expect(pipe.transform('STAF', null)).toBe('STAFF');
  });

  it('should return JUSTIN for JUSTIN', () => {
    expect(pipe.transform('JSTN', 'SOMEUSER')).toBe('JUSTIN');
  });

  it('should fall back to the created-by user when there is no source', () => {
    expect(pipe.transform(null, 'OCCAMORDS')).toBe('Disputant');
    expect(pipe.transform(undefined, 'DAVEKAY')).toBe('DAVEKAY');
    expect(pipe.transform('UNKNOWN', 'DAVEKAY')).toBe('DAVEKAY');
    expect(pipe.transform(null, null)).toBe('');
  });
});

describe('isCitizenSubmitted', () => {
  it('should use the source when there is one', () => {
    expect(isCitizenSubmitted('CITZ', 'DAVEKAY')).toBeTrue();
    expect(isCitizenSubmitted('STAF', 'OCCAMORDS')).toBeFalse();
    expect(isCitizenSubmitted('JSTN', null)).toBeFalse();
  });

  it('should fall back to the created-by user when there is no source', () => {
    expect(isCitizenSubmitted(null, 'OCCAMORDS')).toBeTrue();
    expect(isCitizenSubmitted('UNKNOWN', 'OCCAMORDS')).toBeTrue();
    expect(isCitizenSubmitted(undefined, 'DAVEKAY')).toBeFalse();
  });
});

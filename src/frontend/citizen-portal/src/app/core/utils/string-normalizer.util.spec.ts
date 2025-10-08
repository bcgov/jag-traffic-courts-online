import { StringNormalizer } from './string-normalizer.util';

describe('StringNormalizer', () => {

  describe('normalizeCharacters', () => {
    it('should handle null and undefined inputs', () => {
      expect(StringNormalizer.normalizeCharacters(null as any)).toBeNull();
      expect(StringNormalizer.normalizeCharacters(undefined as any)).toBeUndefined();
      expect(StringNormalizer.normalizeCharacters('')).toBe('');
    });

    it('should normalize Unicode apostrophes to ASCII', () => {
      const inputs = [
        '\u2019',  // Right single quotation mark
        '\u2018',  // Left single quotation mark
        '\u201B',  // Single high-reversed-9 quotation mark
        '\u2032',  // Prime
        '\u02B9',  // Modifier letter prime
        '\u02C8',  // Modifier letter vertical line
        '\u0060',  // Grave accent (backtick)
        '\u00B4',  // Acute accent
      ];

      inputs.forEach(input => {
        expect(StringNormalizer.normalizeCharacters(input)).toBe("'");
      });
    });

    it('should normalize Unicode quotes to ASCII', () => {
      const inputs = [
        '\u201C',  // Left double quotation mark
        '\u201D',  // Right double quotation mark
        '\u201F',  // Double high-reversed-9 quotation mark
        '\u2033',  // Double prime
        '\u2036',  // Reversed double prime
        '\u301D',  // Reversed double prime quotation mark
        '\u301E',  // Double prime quotation mark
      ];

      inputs.forEach(input => {
        expect(StringNormalizer.normalizeCharacters(input)).toBe('"');
      });
    });

    it('should normalize Unicode dashes to ASCII hyphen', () => {
      const inputs = [
        '\u2013',  // En dash
        '\u2014',  // Em dash
        '\u2212',  // Minus sign
        '\u2012',  // Figure dash
        '\u207B',  // Superscript minus
      ];

      inputs.forEach(input => {
        expect(StringNormalizer.normalizeCharacters(input)).toBe('-');
      });
    });

    it('should normalize ellipsis to three periods', () => {
      expect(StringNormalizer.normalizeCharacters('\u2026')).toBe('...');
    });

    it('should normalize various spaces to regular space or remove them', () => {
      const spaceInputs = [
        '\u00A0',  // Non-breaking space
        '\u2009',  // Thin space
        '\u200A',  // Hair space
        '\u2002',  // En space
        '\u2003',  // Em space
        '\u2004',  // Three-per-em space
        '\u2005',  // Four-per-em space
        '\u2006',  // Six-per-em space
        '\u2007',  // Figure space
        '\u2008',  // Punctuation space
      ];

      spaceInputs.forEach(input => {
        expect(StringNormalizer.normalizeCharacters(input)).toBe(' ');
      });

      // Zero-width characters should be removed
      expect(StringNormalizer.normalizeCharacters('\u200B')).toBe('');
      expect(StringNormalizer.normalizeCharacters('\u200C')).toBe('');
      expect(StringNormalizer.normalizeCharacters('\u200D')).toBe('');
    });

    it('should normalize fullwidth characters', () => {
      expect(StringNormalizer.normalizeCharacters('\uFF07')).toBe("'");
      expect(StringNormalizer.normalizeCharacters('\uFF02')).toBe('"');
      expect(StringNormalizer.normalizeCharacters('\uFF0D')).toBe('-');
    });

    it('should handle complex strings with multiple normalizations', () => {
      const input = 'John\u2019s \u201Cquoted\u201D text\u2014with dashes\u2026';
      const expected = 'John\'s "quoted" text-with dashes...';
      expect(StringNormalizer.normalizeCharacters(input)).toBe(expected);
    });

    it('should preserve regular ASCII characters', () => {
      const input = 'Hello World! 123 ABC abc';
      expect(StringNormalizer.normalizeCharacters(input)).toBe(input);
    });
  });

  describe('isASCII', () => {
    it('should handle null and undefined inputs', () => {
      expect(StringNormalizer.isASCII(null as any)).toBe(true);
      expect(StringNormalizer.isASCII(undefined as any)).toBe(true);
      expect(StringNormalizer.isASCII('')).toBe(true);
    });

    it('should return true for ASCII-only strings', () => {
      const asciiStrings = [
        'Hello World',
        '123456789',
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
        '!"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~',
        ' \t\n\r',
      ];

      asciiStrings.forEach(str => {
        expect(StringNormalizer.isASCII(str)).toBe(true);
      });
    });

    it('should return false for strings with non-ASCII characters', () => {
      const nonAsciiStrings = [
        'Café',
        'Naïve',
        'résumé',
        'Москва',
        '北京',
        'José',
        '\u2019',  // Right single quotation mark
        '\u00A0',  // Non-breaking space
      ];

      nonAsciiStrings.forEach(str => {
        expect(StringNormalizer.isASCII(str)).toBe(false);
      });
    });
  });

  describe('getNonASCIICharacters', () => {
    it('should handle null and undefined inputs', () => {
      expect(StringNormalizer.getNonASCIICharacters(null as any)).toEqual([]);
      expect(StringNormalizer.getNonASCIICharacters(undefined as any)).toEqual([]);
      expect(StringNormalizer.getNonASCIICharacters('')).toEqual([]);
    });

    it('should return empty array for ASCII-only strings', () => {
      expect(StringNormalizer.getNonASCIICharacters('Hello World 123')).toEqual([]);
    });

    it('should return non-ASCII characters', () => {
      expect(StringNormalizer.getNonASCIICharacters('Café')).toEqual(['é']);
      expect(StringNormalizer.getNonASCIICharacters('José María')).toEqual(['é', 'í']);
      expect(StringNormalizer.getNonASCIICharacters('Hello\u2019world')).toEqual(['\u2019']);
    });

    it('should not duplicate characters', () => {
      expect(StringNormalizer.getNonASCIICharacters('ééé')).toEqual(['é']);
      expect(StringNormalizer.getNonASCIICharacters('José José')).toEqual(['é']);
    });
  });

  describe('normalizeAndValidate', () => {
    it('should return normalized string and validation result', () => {
      const input = 'John\u2019s "test"';
      const result = StringNormalizer.normalizeAndValidate(input);

      expect(result.normalized).toBe('John\'s "test"');
      expect(result.isValid).toBe(true);
      expect(result.nonASCIICharacters).toEqual([]);
    });

    it('should handle strings that remain invalid after normalization', () => {
      const input = 'Café with \u2019quotes\u2019';
      const result = StringNormalizer.normalizeAndValidate(input);

      expect(result.normalized).toBe('Café with \'quotes\'');
      expect(result.isValid).toBe(false);
      expect(result.nonASCIICharacters).toEqual(['é']);
    });

    it('should handle empty and null inputs', () => {
      const emptyResult = StringNormalizer.normalizeAndValidate('');
      expect(emptyResult.normalized).toBe('');
      expect(emptyResult.isValid).toBe(true);
      expect(emptyResult.nonASCIICharacters).toEqual([]);

      const nullResult = StringNormalizer.normalizeAndValidate(null as any);
      expect(nullResult.normalized).toBeNull();
      expect(nullResult.isValid).toBe(true);
      expect(nullResult.nonASCIICharacters).toEqual([]);
    });
  });

  describe('getValidationErrorMessage', () => {
    it('should return empty string for empty array', () => {
      expect(StringNormalizer.getValidationErrorMessage([])).toBe('');
    });

    it('should return singular message for single character', () => {
      const message = StringNormalizer.getValidationErrorMessage(['é']);
      expect(message).toBe('The following character is not allowed: \'é\'. Please use standard English characters only.');
    });

    it('should return plural message for multiple characters', () => {
      const message = StringNormalizer.getValidationErrorMessage(['é', 'ñ', 'ü']);
      expect(message).toBe('The following characters are not allowed: \'é\', \'ñ\', \'ü\'. Please use standard English characters only.');
    });
  });

  describe('integration tests', () => {
    it('should handle real-world name input scenarios', () => {
      const testCases = [
        {
          input: 'O\u2019Connor',  // O'Connor with Unicode apostrophe
          expectedNormalized: 'O\'Connor',
          expectedValid: true
        },
        {
          input: 'Smith\u2014Brown',  // Smith-Brown with em dash
          expectedNormalized: 'Smith-Brown',
          expectedValid: true
        },
        {
          input: '"Hello"\u2026',  // "Hello" with Unicode quotes and ellipsis
          expectedNormalized: '"Hello"...',
          expectedValid: true
        },
        {
          input: 'José María',  // Name with accented characters
          expectedNormalized: 'José María',
          expectedValid: false
        },
        {
          input: 'John\u00A0Smith',  // Name with non-breaking space
          expectedNormalized: 'John Smith',
          expectedValid: true
        }
      ];

      testCases.forEach(testCase => {
        const result = StringNormalizer.normalizeAndValidate(testCase.input);
        expect(result.normalized).toBe(testCase.expectedNormalized);
        expect(result.isValid).toBe(testCase.expectedValid);
      });
    });

    it('should handle empty and whitespace-only strings correctly', () => {
      const testCases = ['', ' ', '\t', '\n', '\r\n'];
      
      testCases.forEach(testCase => {
        const result = StringNormalizer.normalizeAndValidate(testCase);
        expect(result.isValid).toBe(true);
        expect(result.nonASCIICharacters).toEqual([]);
      });
    });
  });
});

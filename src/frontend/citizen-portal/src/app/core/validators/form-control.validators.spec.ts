import { FormControl } from '@angular/forms';
import { FormControlValidators } from './form-control.validators';

describe('FormControlValidators', () => {

  describe('asciiOnly', () => {
    it('should return null for empty values', () => {
      const control = new FormControl('');
      expect(FormControlValidators.asciiOnly(control)).toBeNull();

      control.setValue(null);
      expect(FormControlValidators.asciiOnly(control)).toBeNull();

      control.setValue(undefined);
      expect(FormControlValidators.asciiOnly(control)).toBeNull();
    });

    it('should return null for ASCII-only strings', () => {
      const asciiValues = [
        'Hello World',
        'John Smith',
        'test@example.com',
        '123456789',
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
        '!"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~'
      ];

      asciiValues.forEach(value => {
        const control = new FormControl(value);
        expect(FormControlValidators.asciiOnly(control)).toBeNull();
      });
    });

    it('should normalize Unicode characters and return null if result is ASCII', () => {
      const testCases = [
        {
          input: 'O\u2019Connor',  // O'Connor with Unicode apostrophe
          expectedNormalized: 'O\'Connor'
        },
        {
          input: 'Smith\u2014Brown',  // Smith-Brown with em dash
          expectedNormalized: 'Smith-Brown'
        },
        {
          input: '"Hello"\u2026',  // "Hello" with Unicode quotes and ellipsis
          expectedNormalized: '"Hello"...'
        },
        {
          input: 'John\u00A0Smith',  // Name with non-breaking space
          expectedNormalized: 'John Smith'
        }
      ];

      testCases.forEach(testCase => {
        const control = new FormControl(testCase.input);
        const result = FormControlValidators.asciiOnly(control);
        
        expect(result).toBeNull();
        
        // Check that the value was normalized (after setTimeout)
        setTimeout(() => {
          expect(control.value).toBe(testCase.expectedNormalized);
        }, 0);
      });
    });

    it('should return error for non-ASCII characters that cannot be normalized', () => {
      const nonAsciiValues = [
        'Café',
        'José María',
        'résumé',
        'Москва',
        '北京'
      ];

      nonAsciiValues.forEach(value => {
        const control = new FormControl(value);
        const result = FormControlValidators.asciiOnly(control);
        
        expect(result).not.toBeNull();
        expect(result?.asciiOnly).toBeDefined();
        expect(result?.asciiOnly?.message).toBeDefined();
        expect(result?.asciiOnly?.invalidCharacters).toBeDefined();
        expect(result?.asciiOnly?.invalidCharacters?.length).toBeGreaterThan(0);
      });
    });

    it('should handle mixed cases with normalization and remaining non-ASCII', () => {
      const input = 'José\u2019s café';  // José's café with Unicode apostrophe
      const control = new FormControl(input);
      const result = FormControlValidators.asciiOnly(control);
      
      expect(result).not.toBeNull();
      expect(result?.asciiOnly).toBeDefined();
      expect(result?.asciiOnly?.invalidCharacters).toContain('é');
      
      // Check that apostrophe was normalized but accented characters remain
      setTimeout(() => {
        expect(control.value).toBe('José\'s café');
      }, 0);
    });

    it('should provide meaningful error messages', () => {
      const control = new FormControl('Café');
      const result = FormControlValidators.asciiOnly(control);
      
      expect(result).not.toBeNull();
      expect(result?.asciiOnly?.message).toContain('character is not allowed');
      expect(result?.asciiOnly?.message).toContain('é');
      expect(result?.asciiOnly?.message).toContain('standard English characters');
    });
  });

  describe('asciiOnlyStrict', () => {
    it('should return null for empty values', () => {
      const control = new FormControl('');
      expect(FormControlValidators.asciiOnlyStrict(control)).toBeNull();

      control.setValue(null);
      expect(FormControlValidators.asciiOnlyStrict(control)).toBeNull();

      control.setValue(undefined);
      expect(FormControlValidators.asciiOnlyStrict(control)).toBeNull();
    });

    it('should return null for ASCII-only strings', () => {
      const asciiValues = [
        'Hello World',
        'John Smith',
        'test@example.com',
        '123456789'
      ];

      asciiValues.forEach(value => {
        const control = new FormControl(value);
        expect(FormControlValidators.asciiOnlyStrict(control)).toBeNull();
      });
    });

    it('should return error for any non-ASCII characters without normalization', () => {
      const nonAsciiValues = [
        'O\u2019Connor',  // Unicode apostrophe (would be normalized in asciiOnly)
        'Smith\u2014Brown',  // Em dash (would be normalized in asciiOnly)
        'Café',
        'José María',
        '\u00A0',  // Non-breaking space (would be normalized in asciiOnly)
      ];

      nonAsciiValues.forEach(value => {
        const control = new FormControl(value);
        const result = FormControlValidators.asciiOnlyStrict(control);
        
        expect(result).not.toBeNull();
        expect(result?.asciiOnlyStrict).toBeDefined();
        expect(result?.asciiOnlyStrict?.message).toBeDefined();
        expect(result?.asciiOnlyStrict?.invalidCharacters).toBeDefined();
        expect(result?.asciiOnlyStrict?.invalidCharacters?.length).toBeGreaterThan(0);
        
        // Ensure the original value is not modified
        expect(control.value).toBe(value);
      });
    });

    it('should provide meaningful error messages', () => {
      const control = new FormControl('Café');
      const result = FormControlValidators.asciiOnlyStrict(control);
      
      expect(result).not.toBeNull();
      expect(result?.asciiOnlyStrict?.message).toContain('character is not allowed');
      expect(result?.asciiOnlyStrict?.message).toContain('é');
      expect(result?.asciiOnlyStrict?.message).toContain('standard English characters');
    });

    it('should handle multiple non-ASCII characters', () => {
      const control = new FormControl('José María');
      const result = FormControlValidators.asciiOnlyStrict(control);
      
      expect(result).not.toBeNull();
      expect(result?.asciiOnlyStrict?.invalidCharacters).toContain('é');
      expect(result?.asciiOnlyStrict?.invalidCharacters).toContain('í');
      expect(result?.asciiOnlyStrict?.message).toContain('characters are not allowed');
    });
  });

  describe('integration with other validators', () => {
    it('should work with existing alpha validator', () => {
      const control = new FormControl('Café123');
      
      // Should fail alpha validator (contains numbers)
      const alphaResult = FormControlValidators.alpha(control);
      expect(alphaResult).not.toBeNull();
      expect(alphaResult?.alpha).toBeDefined();
      
      // Should also fail ASCII validator (contains accented character)
      const asciiResult = FormControlValidators.asciiOnlyStrict(control);
      expect(asciiResult).not.toBeNull();
      expect(asciiResult?.asciiOnlyStrict).toBeDefined();
    });

    it('should work with alphanumeric validator', () => {
      const control = new FormControl('Test123');
      
      // Should pass alphanumeric validator
      const alphanumericResult = FormControlValidators.alphanumeric(control);
      expect(alphanumericResult).toBeNull();
      
      // Should also pass ASCII validator
      const asciiResult = FormControlValidators.asciiOnlyStrict(control);
      expect(asciiResult).toBeNull();
    });
  });

  describe('real-world scenarios', () => {
    it('should handle common name input scenarios', () => {
      const testCases = [
        {
          input: "O'Connor",
          strictShouldPass: true,
          normalizingShouldPass: true,
          description: 'Regular ASCII apostrophe'
        },
        {
          input: 'O\u2019Connor',
          strictShouldPass: false,
          normalizingShouldPass: true,
          description: 'Unicode apostrophe (should be normalized)'
        },
        {
          input: 'José',
          strictShouldPass: false,
          normalizingShouldPass: false,
          description: 'Accented character (cannot be normalized)'
        },
        {
          input: 'Smith-Brown',
          strictShouldPass: true,
          normalizingShouldPass: true,
          description: 'Regular ASCII hyphen'
        },
        {
          input: 'Smith\u2014Brown',
          strictShouldPass: false,
          normalizingShouldPass: true,
          description: 'Em dash (should be normalized)'
        }
      ];

      testCases.forEach(testCase => {
        const strictControl = new FormControl(testCase.input);
        const normalizingControl = new FormControl(testCase.input);
        
        const strictResult = FormControlValidators.asciiOnlyStrict(strictControl);
        const normalizingResult = FormControlValidators.asciiOnly(normalizingControl);
        
        if (testCase.strictShouldPass) {
          expect(strictResult).toBeNull();
        } else {
          expect(strictResult).not.toBeNull();
        }
        
        if (testCase.normalizingShouldPass) {
          expect(normalizingResult).toBeNull();
        } else {
          expect(normalizingResult).not.toBeNull();
        }
      });
    });
  });
});

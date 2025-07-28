/**
 * String normalization utility for handling non-ASCII characters in user input fields.
 * 
 * Implements a two-phase approach:
 * Phase 1: Normalize common problematic characters to ASCII equivalents
 * Phase 2: Validate that no non-ASCII characters remain after normalization
 */
export class StringNormalizer {

  /**
   * Character mapping for normalization - maps Unicode characters to ASCII equivalents
   */
  private static readonly CHARACTER_MAP: Map<string, string> = new Map([
    // Unicode apostrophes/quotes to ASCII apostrophe
    ['\u2019', "'"],  // U+2019 Right single quotation mark
    ['\u2018', "'"],  // U+2018 Left single quotation mark
    ['\u201B', "'"],  // U+201B Single high-reversed-9 quotation mark
    ['\u2032', "'"],  // U+2032 Prime
    ['\u02B9', "'"],  // U+02B9 Modifier letter prime
    ['\u02C8', "'"],  // U+02C8 Modifier letter vertical line
    ['\u0060', "'"],  // U+0060 Grave accent (backtick)
    ['\u00B4', "'"],  // U+00B4 Acute accent

    // Unicode quotes to ASCII double quote
    ['\u201C', '"'],  // U+201C Left double quotation mark
    ['\u201D', '"'],  // U+201D Right double quotation mark
    ['\u201F', '"'],  // U+201F Double high-reversed-9 quotation mark
    ['\u2033', '"'],  // U+2033 Double prime
    ['\u2036', '"'],  // U+2036 Reversed double prime
    ['\u301D', '"'],  // U+301D Reversed double prime quotation mark
    ['\u301E', '"'],  // U+301E Double prime quotation mark

    // En/em dashes to regular hyphen
    ['\u2013', '-'],  // U+2013 En dash
    ['\u2014', '-'],  // U+2014 Em dash
    ['\u2212', '-'],  // U+2212 Minus sign
    ['\u2012', '-'],  // U+2012 Figure dash
    ['\u207B', '-'],  // U+207B Superscript minus

    // Ellipsis to three periods
    ['\u2026', '...'], // U+2026 Horizontal ellipsis

    // Non-breaking and special spaces to regular space
    ['\u00A0', ' '],  // U+00A0 Non-breaking space
    ['\u2009', ' '],  // U+2009 Thin space
    ['\u200A', ' '],  // U+200A Hair space
    ['\u2002', ' '],  // U+2002 En space
    ['\u2003', ' '],  // U+2003 Em space
    ['\u2004', ' '],  // U+2004 Three-per-em space
    ['\u2005', ' '],  // U+2005 Four-per-em space
    ['\u2006', ' '],  // U+2006 Six-per-em space
    ['\u2007', ' '],  // U+2007 Figure space
    ['\u2008', ' '],  // U+2008 Punctuation space
    ['\u200B', ''],   // U+200B Zero width space (remove completely)
    ['\u200C', ''],   // U+200C Zero width non-joiner
    ['\u200D', ''],   // U+200D Zero width joiner

    // Additional common problematic characters
    ['\uFF07', "'"],  // U+FF07 Fullwidth apostrophe
    ['\uFF02', '"'],  // U+FF02 Fullwidth quotation mark
    ['\uFF0D', '-'],  // U+FF0D Fullwidth hyphen-minus
  ]);

  /**
   * Phase 1: Normalize common problematic Unicode characters to ASCII equivalents
   * 
   * @param input - The string to normalize
   * @returns The normalized string with problematic characters replaced
   */
  public static normalizeCharacters(input: string): string {
    if (!input || typeof input !== 'string') {
      return input;
    }

    let normalized = input;

    // Apply character mappings
    this.CHARACTER_MAP.forEach((replacement, unicode) => {
      normalized = normalized.replace(new RegExp(unicode, 'g'), replacement);
    });

    try {
      normalized = normalized.normalize('NFC');
    } catch (e) {
      // Fallback if normalize is not supported
      console.warn('Unicode normalization not supported in this environment');
    }

    return normalized;
  }

  /**
   * Phase 2: Validate that the string contains only ASCII characters
   * 
   * @param input - The string to validate
   * @returns true if the string contains only ASCII characters, false otherwise
   */
  public static isASCII(input: string): boolean {
    if (!input || typeof input !== 'string') {
      return true; // Empty or non-string values are considered valid
    }

    // ASCII characters are in the range 0-127
    return /^[\x00-\x7F]*$/.test(input);
  }

  /**
   * Get all non-ASCII characters from a string for debugging purposes
   * 
   * @param input - The string to analyze
   * @returns Array of non-ASCII characters found in the string
   */
  public static getNonASCIICharacters(input: string): string[] {
    if (!input || typeof input !== 'string') {
      return [];
    }

    const nonASCII: string[] = [];
    for (let i = 0; i < input.length; i++) {
      const char = input[i];
      const charCode = char.charCodeAt(0);
      if (charCode > 127) {
        if (!nonASCII.includes(char)) {
          nonASCII.push(char);
        }
      }
    }
    return nonASCII;
  }

  /**
   * Complete normalization and validation process
   * 
   * @param input - The string to process
   * @returns Object containing the normalized string and validation result
   */
  public static normalizeAndValidate(input: string): {
    normalized: string;
    isValid: boolean;
    nonASCIICharacters: string[];
  } {
    const normalized = this.normalizeCharacters(input);
    const isValid = this.isASCII(normalized);
    const nonASCIICharacters = isValid ? [] : this.getNonASCIICharacters(normalized);

    return {
      normalized,
      isValid,
      nonASCIICharacters
    };
  }

  /**
   * Get a user-friendly error message for validation failures
   * 
   * @param nonASCIICharacters - Array of non-ASCII characters found
   * @returns User-friendly error message
   */
  public static getValidationErrorMessage(nonASCIICharacters: string[]): string {
    if (nonASCIICharacters.length === 0) {
      return '';
    }

    const chars = nonASCIICharacters.map(char => `'${char}'`).join(', ');
    const plural = nonASCIICharacters.length > 1;
    
    return `The following ${plural ? 'characters are' : 'character is'} not allowed: ${chars}. Please use standard English characters only.`;
  }
}

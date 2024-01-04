export class StringUtils {
    /**
     * Compares two strings in a null-safe manner.
     *
     * @param {string} str1 - The first string to compare.
     * @param {string} str2 - The second string to compare.
     * @returns {boolean} True if the strings are equal (case-insensitive), false otherwise.
     */
    static nullSafeCompare(str1: string, str2: string): boolean {
        return (str1 || "").toUpperCase() === (str2 || "").toUpperCase();
    }
}
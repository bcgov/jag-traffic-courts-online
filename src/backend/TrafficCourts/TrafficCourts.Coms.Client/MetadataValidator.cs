namespace TrafficCourts.Coms.Client;

public static class MetadataValidator
{
    public static IEnumerable<char> InvalidMetadataKeyCharacters
    {
        get
        {
            // any US-ASCII control character - (octets 0 - 31) and DEL (127)
            for (int c = 0; c <= 31; c++)
            {
                yield return (char)c;
            }

            yield return (char)127;

            // separators - formatting same as RFC https://www.rfc-editor.org/rfc/rfc2616#section-4.2
            yield return ',';
            yield return ';';
            yield return ':';
            yield return '\\';
            yield return '"';

            yield return '/';
            yield return '[';
            yield return ']';
            yield return '?';
            yield return '=';

            yield return '{';
            yield return '}';
            //yield return ' '; // included in control 0-31
            //yield return '\t'; // included in control 0-31

        }
    }

    /// <summary>
    /// Validates the meta data collection
    /// </summary>
    /// <param name="items"></param>
    /// <exception cref="MetadataInvalidKeyException">A key contains an invalid character</exception>
    /// <exception cref="MetadataTooLongException">The total length of the metadata is too long</exception>
    public static void Validate(IDictionary<string, string>? items)
    {
        // no metadata is ok
        if (items is null)
        {
            return;
        }

        ValidateKeys(items);
        ValidateLength(items);
    }

    private static void ValidateKeys(IDictionary<string, string> items)
    {
        foreach (var item in items)
        {
            string key = item.Key;

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new MetadataInvalidKeyException(key);
            }

            // make sure the metadata key is valid against valid http header spec
            // https://www.rfc-editor.org/rfc/rfc2616#section-4.2
            if (key.Any(c => char.IsControl(c) ||
                c == '(' || c == ')' || c == '<' || c == '>' || c == '@' ||
                c == ',' || c == ';' || c == ':' || c == '\\' || c == '"' ||
                c == '/' || c == '[' || c == ']' || c == '?' || c == '=' ||
                c == '{' || c == '}' || c == ' ' || c == '\t'))
            {
                throw new MetadataInvalidKeyException(key);
            }
        }
    }

    private static void ValidateLength(IDictionary<string, string> items)
    {
        int prefixLength = "x-amz-meta-".Length;

        // see https://github.com/bcgov/common-object-management-service/wiki/Metadata-Tag#metadata
        int length = 0;
        foreach (var item in items)
        {
            length += prefixLength;
            length += item.Key.Length; // ValidateKeys already checked if null or empty
            if (item.Value is not null)
            {
                length += item.Value.Length;
            }
        }

        // note this is not totally correct because there are internal "user" meta data items
        if (length > 2048)
        {
            throw new MetadataTooLongException();
        }
    }
}

public static class TagValidator
{
    public static void Validate(Dictionary<string, string>? items)
    {
        if (items is null)
        {
            return;
        }

        // see https://github.com/bcgov/common-object-management-service/wiki/Metadata-Tag#tag
        if (items.Count > 10)
        {
            throw new TooManyTagsException(items.Count);
        }

        foreach (var item in items)
        {
            if (item.Key.Length > 128) throw new TagKeyTooLongException(item.Key);
            if (item.Value is not null && item.Value.Length > 256) throw new TagValueTooLongException(item.Key, item.Value);
        }
    }
}
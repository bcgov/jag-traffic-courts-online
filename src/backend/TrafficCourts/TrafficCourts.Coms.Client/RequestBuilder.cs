using System.Text;

namespace TrafficCourts.Coms.Client;

/// <summary>
/// Formats COMS parameters
/// </summary>
internal static class RequestBuilder
{
    /// <summary>
    /// Appends the Query-ObjectId parameters
    /// </summary>
    /// <param name="urlBuilder">The <see cref="StringBuilder"/> to append the query string to.</param>
    /// <param name="name">The name of the query string parameter.</param>
    /// <param name="ids">The values</param>
    /// <exception cref="ArgumentNullException"><paramref name="urlBuilder"/> is null.</exception>
    public static void AppendQueryObjectId(StringBuilder urlBuilder, IList<Guid>? ids)
    {
        if (urlBuilder is null)
        {
            throw new ArgumentNullException(nameof(urlBuilder));
        }

        if (ids is null || ids.Count == 0)
        {
            // nothing to append
            return;
        }

        urlBuilder.Append("objId=");

        for (int i = 0; i < ids.Count; i++)
        {
            if (i > 0)
            {
                urlBuilder.Append(',');
            }

            urlBuilder.Append(ids[i].ToString("d"));
        }

        urlBuilder.Append('&');
    }

    /// <summary>
    /// Appends the Query-TagSet parameters
    /// </summary>
    /// <param name="urlBuilder"></param>
    /// <param name="tags"></param>
    /// <exception cref="ArgumentNullException"><paramref name="urlBuilder"/> is null.</exception>
    /// <exception cref="TooManyTagsException"><paramref name="tags"/> There are more than 10 tags.</exception>
    /// <exception cref="TagKeyEmptyException"><paramref name="tags"/> one of the keys is an empty string.</exception>
    /// <exception cref="TagKeyTooLongException"><paramref name="tags"/> one of the keys is too long.</exception>
    /// <exception cref="TagValueTooLongException"><paramref name="tags"/> one of the key values is too long.</exception>
    public static void AppendQueryTagSet(StringBuilder urlBuilder, IDictionary<string, string>? tags)
    {
        if (urlBuilder is null)
        {
            throw new ArgumentNullException(nameof(urlBuilder));
        }

        if (tags is null)
        {
            return;
        }

        if (tags.Count > 10)
        {
            throw new TooManyTagsException(tags.Count);
        }

        foreach (var tag in tags)
        {
            string key = tag.Key ?? string.Empty;
            string value = tag.Value ?? string.Empty;

            if (key.Length == 0) throw new TagKeyEmptyException(key);
            if (key.Length > 128) throw new TagKeyTooLongException(key);
            if (value.Length > 256) throw new TagValueTooLongException(key, value);

            urlBuilder
                .Append(Uri.EscapeDataString($"tagset[{key}]"))
                .Append('=')
                .Append(Uri.EscapeDataString(value))
                .Append('&');
        }
    }

    /// <summary>
    /// Appends the Header-Metadata parameters to the request message
    /// </summary>
    /// <param name="request"></param>
    /// <param name="metadata"></param>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is null.</exception>
    /// <exception cref="MetadataInvalidKeyException"><paramref name="metadata"/> contains keys with invalid  values.</exception>
    public static void AppendHeaderMetadata(HttpRequestMessage request, IDictionary<string, string>? metadata)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (metadata is null)
        {
            return;
        }

        MetadataValidator.Validate(metadata);

        foreach (var item in metadata)
        {
            string key = item.Key;
            request.Headers.TryAddWithoutValidation($"x-amz-meta-{key.ToLower()}", item.Value);
        }
    }
}

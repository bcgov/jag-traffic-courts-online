using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace BCGov.VirusScan.Api.Http;

public static class HttpRequestExtensions
{
    public static async IAsyncEnumerable<FileMultipartSection?> GetFormFileSectionsAsync(this HttpRequest request, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        // example usage: 
        // 
        // await foreach (var section in Request.GetFormFileSectionsAsync(cancellationToken))
        // {
        //   ...
        // }

        ArgumentNullException.ThrowIfNull(request);

        string boundary = request.GetMultipartBoundary();
        Stream body = request.Body;

        var reader = new MultipartReader(boundary, body);

        MultipartSection? section;

        while ((section = await reader.ReadNextSectionAsync(cancellation)) is not null)
        {
            var contentDispositionHeader = section.GetContentDispositionHeader();

            if (contentDispositionHeader?.IsFileDisposition() is true)
            {
                yield return section.AsFileSection();
            }
            else if (contentDispositionHeader?.IsFormDisposition() is true)
            {
                // do not expect form disposition
            }
        }
    }
}

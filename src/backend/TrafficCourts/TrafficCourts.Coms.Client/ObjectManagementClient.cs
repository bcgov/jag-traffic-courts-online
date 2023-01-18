using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace TrafficCourts.Coms.Client;

public partial class ObjectManagementClient : IObjectManagementClient
{
    public async Task<IList<ObjectMetadata>> GetObjectMetadataAsync(IList<Guid> ids, CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/metadata?");

        RequestBuilder.AppendQueryObjectId(urlBuilder_, ids);
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new HttpRequestMessage())
            {
                request_.Method = HttpMethod.Get;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);

                var response_ = await client_.SendAsync(request_, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;

                try
                {
                    var headers_ = Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync(response_, ComsJsonContext.Default.ObjectMetadataCollection, headers_, cancellationToken);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else
                    if (status_ == 401)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseUnauthorized>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<ResponseUnauthorized>("Unauthorized", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    if (status_ == 403)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseForbidden>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<ResponseForbidden>("Forbidden", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseError>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<ResponseError>("Internal Server Error", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                }
                finally
                {
                    if (disposeResponse_)
                        response_.Dispose();
                }
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }

    public async Task<ObjectMetadata?> GetObjectMetadataAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await GetObjectMetadataAsync(new[] { id }, cancellationToken).ConfigureAwait(false);

        if (response.Count == 0)
        {            
            return null; // not found
        }

        return response[0];
    }

    private async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(
        HttpResponseMessage response, 
        JsonTypeInfo<T> typeInfo,
        IReadOnlyDictionary<string, IEnumerable<string>> headers,
        CancellationToken cancellationToken)
    {
        if (response == null || response.Content == null)
        {
            return new ObjectResponseResult<T>(default(T)!, string.Empty);
        }

        if (ReadResponseAsString)
        {
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                var typedBody = JsonSerializer.Deserialize(responseText, typeInfo);
                return new ObjectResponseResult<T>(typedBody!, responseText);
            }
            catch (JsonException exception)
            {
                var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
            }
        }
        else
        {
            try
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var typedBody = await JsonSerializer.DeserializeAsync<T>(responseStream, typeInfo, cancellationToken).ConfigureAwait(false);
                    return new ObjectResponseResult<T>(typedBody!, string.Empty);
                }
            }
            catch (JsonException exception)
            {
                var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
            }
        }
    }
}

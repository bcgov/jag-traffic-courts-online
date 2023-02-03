#nullable enable
namespace TrafficCourts.Coms.Client;

public partial class ObjectManagementClient
{
    private string _baseUrl = "/api/v1";
    private System.Net.Http.HttpClient _httpClient;
    private System.Lazy<System.Text.Json.JsonSerializerOptions> _settings;

    public ObjectManagementClient(System.Net.Http.HttpClient httpClient)
    {
        _httpClient = httpClient;
        _settings = new System.Lazy<System.Text.Json.JsonSerializerOptions>(CreateSerializerSettings);
    }

    private System.Text.Json.JsonSerializerOptions CreateSerializerSettings()
    {
        var settings = new System.Text.Json.JsonSerializerOptions();
        UpdateJsonSerializerSettings(settings);
        return settings;
    }

    public string BaseUrl
    {
        get { return _baseUrl; }
        set { _baseUrl = value; }
    }

    protected System.Text.Json.JsonSerializerOptions JsonSerializerSettings { get { return _settings.Value; } }

    partial void UpdateJsonSerializerSettings(System.Text.Json.JsonSerializerOptions settings);

    partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
    partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
    partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Creates new objects
    /// </summary>
    /// <remarks>
    /// Create object(s) in the configured object storage. If COMS is running in either 'OIDC' or 'Full' mode, any objects created with OIDC user authentication will have all object permissions assigned to them by default.
    /// </remarks>
    /// <param name="x_amz_meta_*">An arbitrary metadata key/value pair. Must contain the Public-amz-meta- prefix to be valid. Multiple metadata pairs can be defined. keys must be unique and will be converted to lowercase.</param>
    /// <param name="tagset*">Tags for the object, defined as a Key/Value tag. The query must be formatted in deepObject style notation, where a tag-set made out of multiple tags would be encoded something similar to `tagset[Public]=a&amp;tagset[y]=b`. Only one value can exist for a given tag key.</param>
    /// <param name="anyKey">This endpoint can accept an arbitrary number of form-data keys. There must be at least one key present, and every key must be unique. All keys shall contain a binary representation of the file to upload. In the response, each successfully uploaded file shall contain a 'fieldName' property corresponding to your custom defined keys.</param>
    /// <returns>Returns an array of created object data</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<System.Collections.Generic.List<Anonymous>> CreateObjectsAsync(IReadOnlyDictionary<string, string>? meta, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object?");

        RequestBuilder.AppendQueryTagSet(urlBuilder_, tags);
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                RequestBuilder.AppendHeaderMetadata(request_, meta);
                var boundary_ = System.Guid.NewGuid().ToString();
                var content_ = new System.Net.Http.MultipartFormDataContent(boundary_);
                content_.Headers.Remove("Content-Type");
                content_.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary_);

                if (anyKey == null)
                    throw new System.ArgumentNullException("anyKey");
                else
                {
                    var content_anyKey_ = new System.Net.Http.StreamContent(anyKey.Data);
                    if (!string.IsNullOrEmpty(anyKey.ContentType))
                        content_anyKey_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(anyKey.ContentType);
                    content_.Add(content_anyKey_, "anyKey", anyKey.FileName ?? "anyKey");
                }
                request_.Content = content_;
                request_.Method = HttpMethod.Post;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 201)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<System.Collections.Generic.List<Anonymous>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Search for objects
    /// </summary>
    /// <remarks>
    /// Returns a list of objects matching all search criteria across all known versions of objects. If the request is BasicAuth authenticated, all search and filtered results will appear. However, If the request is BearerAuth authenticated, only objects that the user has at least one permission associated with, will appear in addition to their filtering parameters.
    /// </remarks>
    /// <param name="meta">An arbitrary metadata key/value pair. Must contain the Public-amz-meta- prefix to be valid. Multiple metadata pairs can be defined. keys must be unique and will be converted to lowercase.</param>
    /// <param name="objIds">Uuid or array of uuids representing the object</param>
    /// <param name="path">The canonical S3 path string of the object</param>
    /// <param name="active">Boolean on active status</param>
    /// <param name="deleteMarker">Boolean on object version DeleteMarker status</param>
    /// <param name="latest">Boolean on object version is latest</param>
    /// <param name="public">Boolean on public status</param>
    /// <param name="mimeType">The object MIME Type</param>
    /// <param name="name">the `name` metadata for the object Typically a descriptive title or original filename</param>
    /// <param name="tags">Tags for the object, defined as a Key/Value tag. The query must be formatted in deepObject style notation, where a tag-set made out of multiple tags would be encoded something similar to `tagset[Public]=a&amp;tagset[y]=b`. Only one value can exist for a given tag key.</param>
    /// <returns>Returns and array of objects</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<System.Collections.Generic.List<DBObject>> SearchObjectsAsync(IReadOnlyDictionary<string, string>? meta, IList<Guid>? objIds, string? path, bool? active, bool? deleteMarker, bool? latest, bool? @public, string? mimeType, string? name, IReadOnlyDictionary<string, string>? tags, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object?");

        RequestBuilder.AppendQueryObjectId(urlBuilder_, objIds);

        if (path != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("path") + "=").Append(System.Uri.EscapeDataString(ConvertToString(path, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (active != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("active") + "=").Append(System.Uri.EscapeDataString(ConvertToString(active, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (deleteMarker != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("deleteMarker") + "=").Append(System.Uri.EscapeDataString(ConvertToString(deleteMarker, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (latest != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("latest") + "=").Append(System.Uri.EscapeDataString(ConvertToString(latest, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (@public != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("public") + "=").Append(System.Uri.EscapeDataString(ConvertToString(@public, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (mimeType != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("mimeType") + "=").Append(System.Uri.EscapeDataString(ConvertToString(mimeType, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (name != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }

        RequestBuilder.AppendQueryTagSet(urlBuilder_, tags);
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                RequestBuilder.AppendHeaderMetadata(request_, meta);
                request_.Method = HttpMethod.Get;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<System.Collections.Generic.List<DBObject>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Returns object headers
    /// </summary>
    /// <remarks>
    /// Returns S3 and COMS headers for a specific object
    /// </remarks>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="versionId">Request a specified version</param>
    /// <returns>Returns object headers</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task HeadObjectAsync(System.Guid objId, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = HttpMethod.Head;

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Returns the object
    /// </summary>
    /// <remarks>
    /// Returns the object as either a direct binary stream, an HTTP 201 containing a direct, temporary pre-signed S3 object URL location, or an HTTP 302 redirect to a direct, temporary pre-signed S3 object URL location.
    /// </remarks>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="download">Download mode behavior. Default behavior (undefined) will yield an HTTP 302 redirect to the S3 bucket via presigned URL. If `proxy` is specified, the object contents will be available proxied through COMS. If `url` is specified, expect an HTTP 201 cotaining the presigned URL as a JSON string in the response.</param>
    /// <param name="expiresIn">How many seconds the pre-signed URL should remain valid for</param>
    /// <param name="versionId">Request a specified version</param>
    /// <returns>Returns the object</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<FileResponse> ReadObjectAsync(System.Guid objId, DownloadMode? download, int? expiresIn, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        if (download != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("download") + "=").Append(System.Uri.EscapeDataString(ConvertToString(download, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (expiresIn != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("expiresIn") + "=").Append(System.Uri.EscapeDataString(ConvertToString(expiresIn, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = HttpMethod.Get;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/octet-stream"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200 || status_ == 206)
                    {
                        var responseStream_ = response_.Content == null ? System.IO.Stream.Null : await response_.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        var fileResponse_ = new FileResponse(status_, headers_, responseStream_, null, response_);
                        disposeClient_ = false; disposeResponse_ = false; // response and client are disposed by FileResponse
                        return fileResponse_;
                    }
                    else
                    if (status_ == 201)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<string>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<string>("Returns a Presigned S3 URL", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                    }
                    else
                    if (status_ == 302)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("Returns a temporary pre-signed S3 object URL location header", status_, responseText_, headers_, null);
                    }
                    else
                    if (status_ == 304)
                    {
                        string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new ApiException("Not Modified", status_, responseText_, headers_, null);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Updates an Object
    /// </summary>
    /// <remarks>
    /// Updates the object in the configured object storage. If the object storage supports versioning, a new version will be generated instead of overwriting the existing contents.
    /// </remarks>
    /// <param name="x_amz_meta_*">An arbitrary metadata key/value pair. Must contain the x-amz-meta- prefix to be valid. Multiple metadata pairs can be defined. keys must be unique and will be converted to lowercase.</param>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="tagset*">Tags for the object, defined as a Key/Value tag. The query must be formatted in deepObject style notation, where a tag-set made out of multiple tags would be encoded something similar to `tagset[Public]=a&amp;tagset[y]=b`. Only one value can exist for a given tag key.</param>
    /// <param name="anyKey">This endpoint will accept only one arbitrary form-data key. That key shall contain a binary representation of the file to upload. In the response, the successfully uploaded file shall contain a 'fieldName' property corresponding to your custom defined key.</param>
    /// <returns>Returns the updated object data</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<Response> UpdateObjectAsync(IReadOnlyDictionary<string, string>? meta, System.Guid objId, IReadOnlyDictionary<string, string>? tags, FileParameter anyKey, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        RequestBuilder.AppendQueryTagSet(urlBuilder_, tags);
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                RequestBuilder.AppendHeaderMetadata(request_, meta);
                var boundary_ = System.Guid.NewGuid().ToString();
                var content_ = new System.Net.Http.MultipartFormDataContent(boundary_);
                content_.Headers.Remove("Content-Type");
                content_.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary_);

                if (anyKey == null)
                    throw new System.ArgumentNullException("anyKey");
                else
                {
                    var content_anyKey_ = new System.Net.Http.StreamContent(anyKey.Data);
                    if (!string.IsNullOrEmpty(anyKey.ContentType))
                        content_anyKey_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(anyKey.ContentType);
                    content_.Add(content_anyKey_, "anyKey", anyKey.FileName ?? "anyKey");
                }
                request_.Content = content_;
                request_.Method = HttpMethod.Post;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<Response>(response_, headers_, cancellationToken).ConfigureAwait(false);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Deletes an object or a version of object
    /// </summary>
    /// <remarks>
    /// Deletes the specified object (or version) from S3. If the object storage supports versioning, precise S3 version stack manipulation is supported, including soft-deletion and soft-restore. Hard-deletions on S3 are also supported. For more details on general S3 version behavior, visit https://docs.aws.amazon.com/AmazonS3/latest/userguide/DeletingObjectVersions.html
    /// </remarks>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="versionId">delete a specified version</param>
    /// <returns>Object or version was deleted from object storage and COMS database</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<ResponseObjectDeleted> DeleteObjectAsync(System.Guid objId, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = HttpMethod.Delete;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseObjectDeleted>(response_, headers_, cancellationToken).ConfigureAwait(false);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Returns the object version history
    /// </summary>
    /// <remarks>
    /// Returns an array of an object's version history
    /// </remarks>
    /// <param name="objId">Uuid of an object</param>
    /// <returns>Returns an array of versions for a specific object</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<S3VersionList> ListObjectVersionAsync(System.Guid objId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}/versions");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = HttpMethod.Get;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<S3VersionList>(response_, headers_, cancellationToken).ConfigureAwait(false);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Sets the public flag of an object
    /// </summary>
    /// <remarks>
    /// Toggles the public property for an object. Sets public to false if public query parameter is not specified.
    /// </remarks>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="public">Boolean on public status</param>
    /// <returns>Returns the object information</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<DBObject> TogglePublicAsync(System.Guid objId, bool? @public, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}/public?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        if (@public != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("public") + "=").Append(System.Uri.EscapeDataString(ConvertToString(@public, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = HttpMethod.Patch;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<DBObject>(response_, headers_, cancellationToken).ConfigureAwait(false);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Adds metadata to an object
    /// </summary>
    /// <remarks>
    /// Creates a copy and new version of the object with the given metadata added to the object. Multiple Key/Value pairs can be provided in the header for the metadata.
    /// </remarks>
    /// <param name="meta">An arbitrary metadata key/value pair. Must contain the x-amz-meta- prefix to be valid. Multiple metadata pairs can be defined. keys must be unique and will be converted to lowercase.</param>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="versionId">Request a specified version</param>
    /// <returns>Accepted and no content</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task AddMetadataAsync(IReadOnlyDictionary<string, string>? meta, System.Guid objId, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}/metadata?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                RequestBuilder.AppendHeaderMetadata(request_, meta);

                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = HttpMethod.Patch;

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
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
                    if (status_ == 422)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseValidationError>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<ResponseValidationError>("The server was unable to process the contained instructions. Generally validation error(s).", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
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

    /// <summary>
    /// Replaces metadata of an object
    /// </summary>
    /// <remarks>
    /// Creates a copy and new version of the object with the given metadata replacing the existing. Multiple Key/Value pairs can be provided in the header for the metadata.
    /// </remarks>
    /// <param name="meta">An arbitrary metadata key/value pair. Must contain the x-amz-meta- prefix to be valid. Multiple metadata pairs can be defined. keys must be unique and will be converted to lowercase.</param>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="versionId">Request a specified version</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Accepted and no content</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task ReplaceMetadataAsync(IReadOnlyDictionary<string, string>? meta, System.Guid objId, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}/metadata?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                RequestBuilder.AppendHeaderMetadata(request_, meta);
                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = HttpMethod.Put;

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
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
                    if (status_ == 422)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseValidationError>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<ResponseValidationError>("The server was unable to process the contained instructions. Generally validation error(s).", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Delete metadata of an object.
    /// </summary>
    /// <remarks>
    /// Creates a copy and new version of the object with the given metadata removed. Multiple Key/Value pairs can be provided in the header for the metadata. If no metadata headers are given then all metadata will be removed. Metadata headers `name` and `id` are mandatory and will always persist.
    /// </remarks>
    /// <param name="meta">An arbitrary metadata key/value pair. Must contain the x-amz-meta- prefix to be valid. Multiple metadata pairs can be defined. keys must be unique and will be converted to lowercase.</param>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="versionId">Request a specified version</param>
    /// <returns>Accepted and no content</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task DeleteMetadataAsync(IReadOnlyDictionary<string, string>? meta, System.Guid objId, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}/metadata?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                RequestBuilder.AppendHeaderMetadata(request_, meta);
                request_.Method = HttpMethod.Delete;

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Returns a list of matching metadata for objects
    /// </summary>
    /// <remarks>
    /// Gets a metadata matching the given objects. Multiple Key/Value pairs can be provided in the headers to narrow down results. If none are provided the full set of metadata will be returned.
    /// </remarks>
    /// <param name="x_amz_meta_*">An arbitrary metadata key/value pair. Must contain the x-amz-meta- prefix to be valid. Multiple metadata pairs can be defined. keys must be unique and will be converted to lowercase.</param>
    /// <param name="objId">Uuid or array of uuids representing the object</param>
    /// <returns>Returns an array of objects with matching key/value pairs.</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<System.Collections.Generic.ICollection<Anonymous2>> FetchMetadataForObjectAsync(IReadOnlyDictionary<string, string>? meta, System.Guid? objId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/metadata?");
        if (objId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("objId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                RequestBuilder.AppendHeaderMetadata(request_, meta);
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<System.Collections.Generic.ICollection<Anonymous2>>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        return objectResponse_.Object;
                    }
                    else
                    if (status_ == 422)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseValidationError>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<ResponseValidationError>("The server was unable to process the contained instructions. Generally validation error(s).", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Adds tags to an object
    /// </summary>
    /// <remarks>
    /// Adds a specified set of tags to the object. Multiple Key/Value pairs can be provided in the query.
    /// </remarks>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="tagset*">Tags for the object, defined as a Key/Value tag. The query must be formatted in deepObject style notation, where a tag-set made out of multiple tags would be encoded something similar to `tagset[Public]=a&amp;tagset[y]=b`. Only one value can exist for a given tag key.</param>
    /// <param name="versionId">Request a specified version</param>
    /// <returns>Accepted and no content</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task AddTaggingAsync(System.Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}/tagging?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        RequestBuilder.AppendQueryTagSet(urlBuilder_, tags);

        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = HttpMethod.Patch;

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
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
                    if (status_ == 422)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseValidationError>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<ResponseValidationError>("The server was unable to process the contained instructions. Generally validation error(s).", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Replaces tags of an object
    /// </summary>
    /// <remarks>
    /// Replace the existing tag-set of an object with the set of given tags. Multiple Key/Value pairs can be provided in the query.
    /// </remarks>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="tagset*">Tags for the object, defined as a Key/Value tag. The query must be formatted in deepObject style notation, where a tag-set made out of multiple tags would be encoded something similar to `tagset[Public]=a&amp;tagset[y]=b`. Only one value can exist for a given tag key.</param>
    /// <param name="versionId">Request a specified version</param>
    /// <returns>Accepted and no content</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task ReplaceTaggingAsync(System.Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}/tagging?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        RequestBuilder.AppendQueryTagSet(urlBuilder_, tags);
        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = HttpMethod.Put;

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
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
                    if (status_ == 422)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<ResponseValidationError>(response_, headers_, cancellationToken).ConfigureAwait(false);
                        if (objectResponse_.Object == null)
                        {
                            throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                        }
                        throw new ApiException<ResponseValidationError>("The server was unable to process the contained instructions. Generally validation error(s).", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Delete tags of an object.
    /// </summary>
    /// <remarks>
    /// Removes the specified set of tags from the object. Multiple Key/Value pairs can be provided in the query. All tags in the tag-set will be removed from the object if no tags are specified.
    /// </remarks>
    /// <param name="objId">Uuid of an object</param>
    /// <param name="tagset*">Tags for the object, defined as a Key/Value tag. The query must be formatted in deepObject style notation, where a tag-set made out of multiple tags would be encoded something similar to `tagset[Public]=a&amp;tagset[y]=b`. Only one value can exist for a given tag key.</param>
    /// <param name="versionId">Request a specified version</param>
    /// <returns>Accepted and no content</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task DeleteTaggingAsync(System.Guid objId, IReadOnlyDictionary<string, string>? tags, string? versionId, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/object/{objId}/tagging?");
        urlBuilder_.Replace("{objId}", System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture)));
        RequestBuilder.AppendQueryTagSet(urlBuilder_, tags);
        if (versionId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("versionId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(versionId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = HttpMethod.Delete;

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 204)
                    {
                        return;
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

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Search for object permissions
    /// </summary>
    /// <remarks>
    /// Returns an array of permissions meeting the filtering parameters provided
    /// </remarks>
    /// <param name="bucketId">Uuid or array of uuids representing the bucket</param>
    /// <param name="objId">Uuid or array of uuids representing the object</param>
    /// <param name="userId">Uuid or array of uuids representing the user</param>
    /// <param name="permCode">the permission type</param>
    /// <returns>Returns an array of objectId/userId/permCode triplets that match the provided parameters</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async System.Threading.Tasks.Task<System.Collections.Generic.ICollection<DBObjectPermission>> ObjectSearchPermissionsAsync(System.Guid? bucketId, System.Guid? objId, System.Guid? userId, PermCode? permCode, System.Threading.CancellationToken cancellationToken)
    {
        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/permission/object?");
        if (bucketId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("bucketId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(bucketId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (objId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("objId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(objId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (userId != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("userId") + "=").Append(System.Uri.EscapeDataString(ConvertToString(userId, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        if (permCode != null)
        {
            urlBuilder_.Append(System.Uri.EscapeDataString("permCode") + "=").Append(System.Uri.EscapeDataString(ConvertToString(permCode, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
        }
        urlBuilder_.Length--;

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = HttpMethod.Get;
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                PrepareRequest(client_, request_, urlBuilder_);

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                PrepareRequest(client_, request_, url_);

                var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                var disposeResponse_ = true;
                try
                {
                    var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                    if (response_.Content != null && response_.Content.Headers != null)
                    {
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;
                    }

                    ProcessResponse(client_, response_);

                    var status_ = (int)response_.StatusCode;
                    if (status_ == 200)
                    {
                        var objectResponse_ = await ReadObjectResponseAsync<System.Collections.Generic.List<DBObjectPermission>>(response_, headers_, cancellationToken).ConfigureAwait(false);
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

    protected struct ObjectResponseResult<T>
    {
        public ObjectResponseResult(T responseObject, string responseText)
        {
            this.Object = responseObject;
            this.Text = responseText;
        }

        public T Object { get; }

        public string Text { get; }
    }

    public bool ReadResponseAsString { get; set; }

    protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
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
                var typedBody = System.Text.Json.JsonSerializer.Deserialize<T>(responseText, JsonSerializerSettings);
                return new ObjectResponseResult<T>(typedBody!, responseText);
            }
            catch (System.Text.Json.JsonException exception)
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
                    var typedBody = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(responseStream, JsonSerializerSettings, cancellationToken).ConfigureAwait(false);
                    return new ObjectResponseResult<T>(typedBody!, string.Empty);
                }
            }
            catch (System.Text.Json.JsonException exception)
            {
                var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
            }
        }
    }

    private string ConvertToString(object? value, System.Globalization.CultureInfo cultureInfo)
    {
        if (value == null)
        {
            return "";
        }

        if (value is System.Enum)
        {
            var name = System.Enum.GetName(value.GetType(), value);
            if (name != null)
            {
                var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                if (field != null)
                {
                    var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute))
                        as System.Runtime.Serialization.EnumMemberAttribute;
                    if (attribute != null)
                    {
                        return attribute.Value != null ? attribute.Value : name;
                    }
                }

                var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                return converted == null ? string.Empty : converted;
            }
        }
        else if (value is bool)
        {
            return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
        }
        else if (value is byte[])
        {
            return System.Convert.ToBase64String((byte[])value);
        }
        else if (value.GetType().IsArray)
        {
            var array = System.Linq.Enumerable.OfType<object>((System.Array)value);
            return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
        }

        var result = System.Convert.ToString(value, cultureInfo);
        return result == null ? "" : result;
    }
}

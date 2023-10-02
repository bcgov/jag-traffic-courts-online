using System.Text.Json.Nodes;

namespace TrafficCourts.Cdogs.Client
{

    public partial class DocumentGenerationClient
    {
        public async Task<FileResponse> UploadTemplateAndRenderReportAsync<T>(TemplateRenderObject<T> body, CancellationToken cancellationToken)
        {
            if (body == null)
                throw new System.ArgumentNullException("body");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/template/render");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var settings = _settings.Value;
                    settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    var json_ = Newtonsoft.Json.JsonConvert.SerializeObject(body, settings);
                    var content_ = new System.Net.Http.StringContent(json_);
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
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
                        if (status_ == 400)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<BadRequest>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new DocumentGenerationApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new DocumentGenerationApiException<BadRequest>("Request is missing content or is malformed", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 405)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<MethodNotAllowed>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new DocumentGenerationApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new DocumentGenerationApiException<MethodNotAllowed>("Method not allowed", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationError>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new DocumentGenerationApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new DocumentGenerationApiException<ValidationError>("The server was unable to process the contained instructions. Generally validation error(s).", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<Error>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new DocumentGenerationApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new DocumentGenerationApiException<Error>("Unexpected error", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
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
        public async Task<FileResponse> UploadTemplateAndRenderReportAsync(JsonNode data, Options options, Template template, CancellationToken cancellationToken)
        {
            if (data == null)
                throw new System.ArgumentNullException("data");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/template/render");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var settings = _settings.Value;
                    settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    TemplateRenderObject renderObject = new TemplateRenderObject
                    {
                        Options = options,
                        Template = template
                    };
                    var json_ = Newtonsoft.Json.JsonConvert.SerializeObject(renderObject, settings);
                    json_ = json_.Remove(json_.Length - 1, 1) + ", \"data\":" + data.ToJsonString() + "}";
                    var content_ = new System.Net.Http.StringContent(json_);
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
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
                        if (status_ == 400)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<BadRequest>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new DocumentGenerationApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new DocumentGenerationApiException<BadRequest>("Request is missing content or is malformed", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 405)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<MethodNotAllowed>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new DocumentGenerationApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new DocumentGenerationApiException<MethodNotAllowed>("Method not allowed", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationError>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new DocumentGenerationApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new DocumentGenerationApiException<ValidationError>("The server was unable to process the contained instructions. Generally validation error(s).", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<Error>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new DocumentGenerationApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new DocumentGenerationApiException<Error>("Unexpected error", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
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
    }
}

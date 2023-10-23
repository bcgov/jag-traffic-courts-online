namespace TrafficCourts.Cdogs.Client
{
    public partial interface IDocumentGenerationClient
    {
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Generate document from inline Template
        /// </summary>
        /// <remarks>
        /// This endpoint accepts a document template and a set (or multiple sets) of substitution variables and merges them into the document.
        /// </remarks>
        /// <param name="body">Fields required to generate a document</param>
        /// <returns>Returns the supplied document with variables merged in</returns>
        /// <exception cref="DocumentGenerationApiException">A server side error occurred.</exception>
        Task<FileResponse> UploadTemplateAndRenderReportAsync<T>(TemplateRenderObject<T> body, CancellationToken cancellationToken);
    }
}

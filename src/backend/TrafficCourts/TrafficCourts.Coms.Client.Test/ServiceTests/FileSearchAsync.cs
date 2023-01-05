using AutoFixture;
using Moq;

namespace TrafficCourts.Coms.Client.Test.ServiceTests;

public class FileSearchAsync : ObjectManagementServiceTest
{
    [Fact]
    public async Task should_pass_parameters_to_client()
    {
        FileSearchParameters parameters = _fixture.Create<FileSearchParameters>();
        CancellationTokenSource cts = new CancellationTokenSource();

        // setup return value
        SetupSearchObjectsReturn(new List<DBObject>());

        ObjectManagementService sut = GetService();

        // act
        List<FileSearchResult> results = await sut.FileSearchAsync(parameters, cts.Token);

        // assert
        // verify the client called with correct values
        _mockClient.Verify(_ =>
            _.SearchObjectsAsync(
                It.Is<IDictionary<string, string>?>((actual) => actual == parameters.Metadata),
                It.Is<IList<Guid>?>((actual) => actual == parameters.Ids),
                It.Is<string?>((actual) => actual == parameters.Path),
                It.Is<bool?>((actual) => actual == parameters.Active),
                It.Is<bool?>((actual) => actual == parameters.Public),
                It.Is<string?>((actual) => actual == parameters.MimeType),
                It.Is<string?>((actual) => actual == parameters.Name),
                It.Is<IDictionary<string, string>?>((actual) => actual == parameters.Tags),
                It.Is<CancellationToken>((actual) => actual == cts.Token)
            ));
    }

    [Fact]
    public async Task should_fetch_metadata_for_each_result()
    {
        Guid expected = Guid.NewGuid();

        FileSearchParameters parameters = _fixture.Create<FileSearchParameters>();
        CancellationTokenSource cts = new CancellationTokenSource();

        // setup return value
        SetupSearchObjectsReturn(new List<DBObject> { new DBObject { Id = expected } });

        IReadOnlyDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        SetupReadObjectReturn(new FileResponse(200, headers, GetRandomStream(), null, null));

        ObjectManagementService sut = GetService();

        // act
        List<FileSearchResult> results = await sut.FileSearchAsync(parameters, cts.Token);

        // verify the client called with correct values
        _mockClient.Verify(_ => _.SearchObjectsAsync(
                It.Is<IDictionary<string, string>?>((actual) => actual == parameters.Metadata),
                It.Is<IList<Guid>?>((actual) => actual == parameters.Ids),
                It.Is<string?>((actual) => actual == parameters.Path),
                It.Is<bool?>((actual) => actual == parameters.Active),
                It.Is<bool?>((actual) => actual == parameters.Public),
                It.Is<string?>((actual) => actual == parameters.MimeType),
                It.Is<string?>((actual) => actual == parameters.Name),
                It.Is<IDictionary<string, string>?>((actual) => actual == parameters.Tags),
                It.Is<CancellationToken>((actual) => actual == cts.Token)
            ));

        // should have read the object to get the expected found object
        _mockClient.Verify(_ => _.ReadObjectAsync(
             It.Is<Guid>((actual) => actual == expected),
             It.Is<DownloadMode?>((actual) => actual == DownloadMode.Url),
             It.Is<int?>((actual) => actual == null),
             It.Is<string?>((actual) => actual == null),
            It.Is<CancellationToken>((actual) => actual == cts.Token)
        ));
    }


    private void SetupSearchObjectsReturn(List<DBObject> values)
    {
        _mockClient.Setup(_ => _.SearchObjectsAsync(
            It.IsAny<IDictionary<string, string>?>(),
            It.IsAny<IList<Guid>?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<IDictionary<string, string>?>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(() => values);
    }


}

﻿using AutoFixture;
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
        SetupGetObjectMetadataAsync(Array.Empty<Anonymous2>());

        ObjectManagementService sut = GetService();

        // act
        IList<FileSearchResult> results = await sut.FileSearchAsync(parameters, cts.Token);

        // assert
        // verify the client called with correct values
        _mockClient.Verify(_ =>
            _.SearchObjectsAsync(
                It.Is<IReadOnlyDictionary<string, string>?>((actual) => actual == parameters.Metadata),
                It.Is<IList<Guid>?>((actual) => actual == parameters.Ids),
                It.Is<Guid?>((actual) => actual == parameters.BucketId),
                It.Is<string?>((actual) => actual == parameters.Path),
                It.Is<bool?>((actual) => actual == parameters.Active),
                It.Is<bool?>((actual) => actual == parameters.DeleteMarker),
                It.Is<bool?>((actual) => actual == parameters.Latest),
                It.Is<bool?>((actual) => actual == parameters.Public),
                It.Is<string?>((actual) => actual == parameters.MimeType),
                It.Is<string?>((actual) => actual == parameters.Name),
                It.Is<IReadOnlyDictionary<string, string>?>((actual) => actual == parameters.Tags),
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
        SetupGetObjectMetadataAsync(new Anonymous2[] { new Anonymous2 { ObjectId = expected } });
        SetupGetObjectTagsAsync(new Anonymous3[] { new Anonymous3 { ObjectId = expected } });

        ObjectManagementService sut = GetService();

        // act
        IList<FileSearchResult> results = await sut.FileSearchAsync(parameters, cts.Token);

        // verify the client called with correct values
        _mockClient.Verify(_ => _.SearchObjectsAsync(
                It.Is<IReadOnlyDictionary<string, string>?>((actual) => actual == parameters.Metadata),
                It.Is<IList<Guid>?>((actual) => actual == parameters.Ids),
                It.Is<Guid?>((actual) => actual == parameters.BucketId),
                It.Is<string?>((actual) => actual == parameters.Path),
                It.Is<bool?>((actual) => actual == parameters.Active),
                It.Is<bool?>((actual) => actual == parameters.DeleteMarker),
                It.Is<bool?>((actual) => actual == parameters.Latest),
                It.Is<bool?>((actual) => actual == parameters.Public),
                It.Is<string?>((actual) => actual == parameters.MimeType),
                It.Is<string?>((actual) => actual == parameters.Name),
                It.Is<IReadOnlyDictionary<string, string>?>((actual) => actual == parameters.Tags),
                It.Is<CancellationToken>((actual) => actual == cts.Token)
            ));

        // should have read the object to get the expected found object
        _mockClient.Verify(_ => _.FetchMetadataAsync(
            It.Is<IList<Guid>>((ids) => ids.Count == 1 && ids[0] == expected),
            It.Is<IReadOnlyDictionary<string, string>>(meta => meta == null),
            It.Is<CancellationToken>((cancellationToken) => cancellationToken == cts.Token)
        ));
    }

    private void SetupSearchObjectsReturn(List<DBObject> values)
    {
        _mockClient.Setup(_ => _.SearchObjectsAsync(
            It.IsAny<IReadOnlyDictionary<string, string>?>(),
            It.IsAny<IList<Guid>?>(),
            It.IsAny<Guid?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<bool?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<IReadOnlyDictionary<string, string>?>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(() => values);
    }
}

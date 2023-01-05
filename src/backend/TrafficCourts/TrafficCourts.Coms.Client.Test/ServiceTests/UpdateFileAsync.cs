using Moq;

namespace TrafficCourts.Coms.Client.Test.ServiceTests;

public class UpdateFileAsync : ObjectManagementServiceTest
{
    [Fact]
    public async Task should_call_client_with_correct_parameters()
    {
        // setup expected file data
        Guid expectedId = Guid.NewGuid();
        Stream expectedStream = GetRandomStream();
        string expectedFilename = $"filename-{Guid.NewGuid().ToString()[..6]}.pdf";
        string expectedContentType = "application/pdf";

        File file = new(data: expectedStream, fileName: expectedFilename, contentType: expectedContentType);

        // simulate success
        _mockClient.Setup(_ => _.UpdateObjectAsync(
            It.IsAny<IDictionary<string, string>?>(),
            It.IsAny<Guid>(),
            It.IsAny<IDictionary<string, string>?>(),
            It.IsAny<FileParameter>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                return new Response();
            });

        CancellationTokenSource cts = new CancellationTokenSource();

        ObjectManagementService sut = GetService();

        // act
        await sut.UpdateFileAsync(expectedId, file, cts.Token);

        // assert
        // verify the client called with correct values
        _mockClient.Verify(_ =>
            _.UpdateObjectAsync(
                It.Is<IDictionary<string, string>>((_) => Equal(file.Metadata, _)),
                It.Is<Guid>((_) => _ == expectedId),
                It.Is<IDictionary<string, string>>((_) => Equal(file.Tags, _)),
                It.Is<FileParameter>((_) => _.Data == expectedStream && _.FileName == expectedFilename && _.ContentType == expectedContentType),
                It.Is<CancellationToken>((cancellationToken) => cancellationToken == cts.Token))
            );
    }

}

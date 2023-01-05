using Moq;

namespace TrafficCourts.Coms.Client.Test.ServiceTests;

public class CreateFileAsync : ObjectManagementServiceTest
{
    [Fact]
    public async Task should_throw_if_file_is_null()
    {
        // create strict client to ensure its operations are not called
        ObjectManagementService sut = GetService();

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.CreateFileAsync(null!, CancellationToken.None));
        Assert.Equal("file", actual.ParamName);
    }

    [Fact]
    public async Task should_throw_if_file_data_is_null()
    {
        File file = new(data: null!);

        // create strict client to ensure its operations are not called
        _mockClient = new Moq.Mock<IObjectManagementClient>(Moq.MockBehavior.Strict);
        ObjectManagementService sut = GetService();
        var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateFileAsync(file, CancellationToken.None));
        Assert.Equal("file", actual.ParamName);
    }

    [Fact]
    public async Task should_throw_if_metadata_is_too_long()
    {
        File file = new(data: GetRandomStream());

        // metadata prefix is "x-amz-meta-" which is 10 characters long
        file.Metadata.Add("x", new string('y', 2048 - 1 - 10));

        // create strict client to ensure its operations are not called
        _mockClient = new Moq.Mock<IObjectManagementClient>(Moq.MockBehavior.Strict);
        ObjectManagementService sut = GetService();
        var actual = await Assert.ThrowsAsync<MetadataTooLongException>(() => sut.CreateFileAsync(file, CancellationToken.None));
    }

    [Theory]
    [MemberData(nameof(InvalidHttpHeaderCharacters))]
    public async Task should_throw_if_metadata_key_has_invalid_characters(char invalidChar)
    {
        File file = new(data: GetRandomStream());

        file.Metadata.Add(invalidChar.ToString(), "y");

        // create strict client to ensure its operations are not called
        _mockClient = new Moq.Mock<IObjectManagementClient>(Moq.MockBehavior.Strict);
        ObjectManagementService sut = GetService();
        var actual = await Assert.ThrowsAsync<MetadataInvalidKeyException>(() => sut.CreateFileAsync(file, CancellationToken.None));        
    }

    public static IEnumerable<object[]> InvalidHttpHeaderCharacters()
    {
        foreach (var c in MetadataValidator.InvalidMetadataKeyCharacters)
        {
            yield return new object[] { c };
        }
    }

    [Fact]
    public async Task should_call_client_with_correct_parameters()
    {
        // setup expected file data
        Stream expectedStream = GetRandomStream();
        string expectedFilename = $"filename-{Guid.NewGuid().ToString()[..6]}.pdf";
        string expectedContentType = "application/pdf";

        File file = new(data: expectedStream, fileName: expectedFilename, contentType: expectedContentType);

        // setup expected return value
        Guid expectedId = Guid.NewGuid();

        // simulate success
        _mockClient.Setup(_ => _.CreateObjectsAsync(
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<IDictionary<string, string>>(),
            It.IsAny<FileParameter>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                return new List<Anonymous> { new Anonymous { Id = expectedId } };
            });

        CancellationTokenSource cts = new CancellationTokenSource();

        ObjectManagementService sut = GetService();

        // act
        var actual = await sut.CreateFileAsync(file, cts.Token);
        
        // assert
        Assert.Equal(expectedId, actual);

        // verify the client called with correct values
        _mockClient.Verify(_ =>
            _.CreateObjectsAsync(
                It.Is<IDictionary<string, string>>((_) => Equal(new Dictionary<string, string>(), _)),
                It.Is<IDictionary<string, string>>((_) => Equal(new Dictionary<string, string>(), _)),
                It.Is<FileParameter>((_) => _.Data == expectedStream && _.FileName == expectedFilename && _.ContentType == expectedContentType),
                It.Is<CancellationToken>((cancellationToken) => cancellationToken == cts.Token))
            );
    }

    // TODO: add test when create returns 0 items
    // TODO: add test when create returns more than 1 item

    private static bool Equal(IDictionary<string, string> expected, IDictionary<string, string> actual)
    {
        if (expected.Count != actual.Count)
        { 
            return false; 
        }

        foreach (var item in expected)
        {
            if (!actual.ContainsKey(item.Key) || actual[item.Key] != item.Value) 
            { 
                return false; 
            }
        }

        return true;
    }
}

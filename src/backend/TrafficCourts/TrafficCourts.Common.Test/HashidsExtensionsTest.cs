using HashidsNet;
using Moq;
using System;
using Xunit;

namespace TrafficCourts.Common.Test;

public class HashidsExtensionsTest
{
    [Fact]
    public void TryDecodeGuid_null_hashids_object_throws_argument_null_exception()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => HashidsExtensions.TryDecodeGuid(null!, "", out Guid result));
        Assert.Equal("hashids", actual.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r")]
    public void null_or_whitespace_returns_null(string value)
    {
        // create strict mock cause we do not expect any methods to be called
        var mockIHashids = new Mock<IHashids>(MockBehavior.Strict);
        var actual = HashidsExtensions.TryDecodeGuid(mockIHashids.Object, value, out Guid result);
        Assert.False(actual);
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void decodes_correctly_formatted_guid()
    {
        var expected = Guid.NewGuid();

        var mockIHashids = new Mock<IHashids>(MockBehavior.Strict);
        mockIHashids.Setup(_ => _.DecodeHex(It.IsAny<string>())).Returns(expected.ToString("n"));

        var actual = HashidsExtensions.TryDecodeGuid(mockIHashids.Object, "input-doesnt-matter-cause-we-mock-the-IHashids", out Guid result);

        Assert.True(actual);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void returns_false_if_decoded_hex_is_not_correct_length()
    {
        var hex = Guid.NewGuid().ToString("n");

        var mockIHashids = new Mock<IHashids>(MockBehavior.Strict);
        mockIHashids.Setup(_ => _.DecodeHex(It.IsAny<string>())).Returns(hex[0..16]); // only part of the guid

        var actual = HashidsExtensions.TryDecodeGuid(mockIHashids.Object, "input-doesnt-matter-cause-we-mock-the-IHashids", out Guid value);

        Assert.False(actual);
        Assert.Equal(Guid.Empty, value);
    }

    [Fact]
    public void EncodeGuid_null_hashids_object_throws_argument_null_exception()
    {
        var actual = Assert.Throws<ArgumentNullException>(() => HashidsExtensions.EncodeGuid(null!, Guid.Empty));
        Assert.Equal("hashids", actual.ParamName);
    }

    [Fact]
    public void encodes_guid_correctly()
    {
        var input = Guid.NewGuid();
        var expected = input.ToString("n");

        var mockIHashids = new Mock<IHashids>(MockBehavior.Strict);
        mockIHashids.Setup(_ => _.EncodeHex(It.Is<string>(value => value == expected))).Returns(expected);

        // this will throw if the value passed to EncodeHex is not formatted correctly
        var actaul = HashidsExtensions.EncodeGuid(mockIHashids.Object, input);

    }
}

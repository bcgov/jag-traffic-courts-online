using System.Text;

namespace TrafficCourts.Coms.Client.Test
{
    public class RequestBuilderTest
    {
        [Fact]
        public void appending_header_metadata_should_throw_if_stringbuilder_is_null()
        {
            HttpRequestMessage request = null!;
            var actual = Assert.Throws<ArgumentNullException>(() => RequestBuilder.AppendHeaderMetadata(request, null));
            Assert.Equal("request", actual.ParamName);
        }

        [Theory]
        [MemberData(nameof(InvalidHttpHeaderCharacters))]
        public void appending_header_metadata_should_throw_if_metadata_keys_contains_invalid_characters(char invalidHeaderChar)
        {
            HttpRequestMessage request = new();

            string expected = new  string(invalidHeaderChar, 1);

            Dictionary<string, string> metadata = new Dictionary<string, string>
            {
                { expected, "value" }
            };

            var actual = Assert.Throws<MetadataInvalidKeyException>(() => RequestBuilder.AppendHeaderMetadata(request, metadata));
            Assert.Equal(expected, actual.Key);
        }

        public static IEnumerable<object[]> InvalidHttpHeaderCharacters()
        {
            foreach (var c in MetadataValidator.InvalidMetadataKeyCharacters)
            {
                yield return new object[] { c };
            }
        }

        [Fact]
        public void appending_query_object_ids_should_throw_if_stringbuilder_is_null()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => RequestBuilder.AppendQueryObjectId(null!, null));
            Assert.Equal("urlBuilder", actual.ParamName);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        public void appending_query_object_ids_should_format_each_id_correctly(int count)
        {
            // generate the number of items
            List<Guid> ids = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList();

            // should be objId=X,Y,Z&
            var expected = "objId=" + ids.Aggregate("", (current, next) => current.Length == 0 ? next.ToString("d") : current + "," + next.ToString("d")) + "&";

            StringBuilder buffer = new StringBuilder();
            RequestBuilder.AppendQueryObjectId(buffer, ids);

            var actual = buffer.ToString();
            Assert.Equal(expected, actual);
            Assert.EndsWith("&", actual); // redundant from above, but want to make it explict it should end with the parameter pair separator
        }

        [Fact]
        public void appending_query_object_ids_with_null_list_should_not_append_to_buffer()
        {
            List<Guid>? ids = null;

            StringBuilder buffer = new StringBuilder();
            RequestBuilder.AppendQueryObjectId(buffer, ids);

            var actual = buffer.ToString();
            Assert.Equal(0, actual.Length);
        }

        [Fact]
        public void appending_query_object_ids_with_empty_list_should_not_append_to_buffer()
        {
            List<Guid> ids = new();

            StringBuilder buffer = new StringBuilder();
            RequestBuilder.AppendQueryObjectId(buffer, ids);

            var actual = buffer.ToString();
            Assert.Equal(0, actual.Length);
        }


        [Fact]
        public void appending_query_tag_set_should_throw_if_stringbuilder_is_null()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => RequestBuilder.AppendQueryTagSet(null!, null));
            Assert.Equal("urlBuilder", actual.ParamName);
        }

        [Fact]
        public void appending_query_tag_with_empty_tags_should_not_append_to_buffer()
        {
            StringBuilder buffer = new StringBuilder();
            Dictionary<string, string>? values = new Dictionary<string, string>();
            RequestBuilder.AppendQueryTagSet(buffer, values);

            var actual = buffer.ToString();
            Assert.Equal(0, actual.Length);
        }

        [Fact]
        public void appending_query_tag_with_null_tags_should_not_append_to_buffer()
        {
            StringBuilder buffer = new StringBuilder();

            Dictionary<string, string>? values = null;
            RequestBuilder.AppendQueryTagSet(buffer, values);

            var actual = buffer.ToString();
            Assert.Equal(0, actual.Length);
        }

        [Fact]
        public void appending_query_tag_should_throw_if_tag_key_is_empty()
        {
            string expected = string.Empty;
            Dictionary<string, string>? values = new Dictionary<string, string>()
            {
                { expected, "1" }
            };

            StringBuilder buffer = new StringBuilder();
            var actual = Assert.Throws<TagKeyEmptyException>(() => RequestBuilder.AppendQueryTagSet(buffer, values));
            Assert.Equal(expected, actual.Key);
        }

        [Fact]
        public void appending_query_tag_should_throw_if_there_are_too_many_tags()
        {
            Dictionary<string, string>? values = new Dictionary<string, string>();

            // add 11 items - max is 10
            for (int i = 1; i <= 11; i++)
            {
                values.Add(i.ToString(), string.Empty);
            }

            StringBuilder buffer = new StringBuilder();
            var actual = Assert.Throws<TooManyTagsException>(() => RequestBuilder.AppendQueryTagSet(buffer, values));
        }

        [Fact]
        public void appending_query_tag_with_tags_should_append_to_buffer()
        {
            StringBuilder buffer = new StringBuilder();
            Dictionary<string, string>? values = new Dictionary<string, string>()
            {
                { "a", "1" },
                { "b", "2" }
            };

            // [ -> %5B
            // ] -> %5D
            string[] expected = new string[]
            {
                "tagset%5Ba%5D=1",
                "tagset%5Bb%5D=2"
            };

            RequestBuilder.AppendQueryTagSet(buffer, values);

            var actual = buffer.ToString();
            var segments = actual.Split('&');

            foreach (var segment in segments)
            {
                Assert.Contains(segment, actual);
            }

            Assert.EndsWith("&", actual);
        }

        [Fact]
        public void appending_query_tag_should_throw_if_tag_key_is_too_long()
        {
            string expected = new string('a', 129);
            Dictionary<string, string>? values = new Dictionary<string, string>()
            {
                { expected, "1" }
            };

            StringBuilder buffer = new StringBuilder();
            var actual = Assert.Throws<TagKeyTooLongException>(() => RequestBuilder.AppendQueryTagSet(buffer, values));

            Assert.Equal(expected, actual.Key);
        }

        [Fact]
        public void appending_query_tag_should_throw_if_tag_value_is_too_long()
        {
            string expected = new string('a', 257);
            Dictionary<string, string>? values = new Dictionary<string, string>()
            {
                { "a", expected }
            };

            StringBuilder buffer = new StringBuilder();
            var actual = Assert.Throws<TagValueTooLongException>(() => RequestBuilder.AppendQueryTagSet(buffer, values));

            Assert.Equal("a", actual.Key);
            Assert.Equal(expected, actual.Value);
        }
    }
}

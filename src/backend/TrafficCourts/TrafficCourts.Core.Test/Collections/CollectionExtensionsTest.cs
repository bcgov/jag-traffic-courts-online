using AutoFixture;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

using TrafficCourts.Collections;

namespace TrafficCourts.Core.Test.Collections;

public class CollectionExtensionsTest
{
    private readonly List<Model> _items;

    public CollectionExtensionsTest()
    {
        Fixture fixture = new Fixture();

        _items = fixture.CreateMany<Model>(10).ToList();
    }

    [Fact]
    public void throws_invalid_operation_exception_on_invalid_path()
    {
        var items = _items.AsQueryable();
        var actual = Assert.Throws<InvalidOperationException>(() => items.OrderBy("Name.First"));
    }

    [Fact]
    public void can_order_by_regular_property()
    {
        can_order_by_property<int>(_ => _.Id, nameof(Model.Id));
    }

    [Fact]
    public void can_order_by_regular_property_with_different_case()
    {
        can_order_by_property<int>(_ => _.Id, nameof(Model.Id).ToUpper());
        can_order_by_property<int>(_ => _.Id, nameof(Model.Id).ToLower());

        // lower case already handled by the json property attributes
        can_order_by_property<string>(_ => _.Name, nameof(Model.Name).ToUpper());
        can_order_by_property<string>(_ => _.Description, nameof(Model.Description).ToUpper());
    }

    [Fact]
    public void can_order_by_system_text_json_property()
    {
        can_order_by_property<string>(_ => _.Name, "name");
    }

    [Fact]
    public void can_order_by_newtonsoft_json_property()
    {
        can_order_by_property<string>(_ => _.Description, "description");
    }

    [Fact]
    public void can_order_then_by_regular_property()
    {
        can_order_then_by_property<int>(_ => _.Id, "Id");
    }

    [Fact]
    public void can_order_then_by_system_text_json_property()
    {
        can_order_then_by_property<string>(_ => _.Name, "name");
    }

    [Fact]
    public void can_order_then_by_newtonsoft_json_property()
    {
        can_order_then_by_property<string>(_ => _.Description, "description");
    }

    [Fact]
    public void can_order_by_regular_property_descending()
    {
        can_order_by_property_descending<int>(_ => _.Id, "Id");
    }

    [Fact]
    public void can_order_by_system_text_json_property_descending()
    {
        can_order_by_property_descending<string>(_ => _.Name, "name");
    }

    [Fact]
    public void can_order_by_newtonsoft_json_property_descending()
    {
        can_order_by_property_descending<string>(_ => _.Description, "description");
    }

    [Fact]
    public void can_order_then_by_regular_property_descending()
    {
        can_order_then_by_property_descending<int>(_ => _.Id, "Id");
    }

    [Fact]
    public void can_order_then_by_system_text_json_property_descending()
    {
        can_order_then_by_property_descending<string>(_ => _.Name, "name");
    }

    [Fact]
    public void can_order_then_by_newtonsoft_json_property_descending()
    {
        can_order_then_by_property_descending<string>(_ => _.Description, "description");
    }

    private void can_order_by_property<TKey>(Func<Model, TKey> keySelector, string name)
    {
        var items = _items;

        var expected = items.OrderBy(keySelector).ToList();

        var actual = items.AsQueryable().OrderBy(name).ToList();

        Assert.Equal(expected, actual);
    }

    private void can_order_then_by_property<TKey>(Expression<Func<Model, TKey>> keySelector, string name)
    {
        var items = _items.AsQueryable().OrderBy(_ => 0);

        var expected = items.ThenBy(keySelector).ToList();

        var actual = items.ThenBy(name).ToList();

        Assert.Equal(expected, actual);
    }

    private void can_order_by_property_descending<TKey>(Func<Model, TKey> keySelector, string name)
    {
        var items = _items;

        var expected = items.OrderByDescending(keySelector).ToList();

        var actual = items.AsQueryable().OrderByDescending(name).ToList();

        Assert.Equal(expected, actual);
    }
    private void can_order_then_by_property_descending<TKey>(Expression<Func<Model, TKey>> keySelector, string name)
    {
        var items = _items.AsQueryable().OrderBy(_ => 0);

        var expected = items.ThenByDescending(keySelector).ToList();

        var actual = items.ThenByDescending(name).ToList();

        Assert.Equal(expected, actual);
    }


    [ExcludeFromCodeCoverage]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Model
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = string.Empty;

        private string DebuggerDisplay => $"{Id}: {Name} - {Description}";
    }
}

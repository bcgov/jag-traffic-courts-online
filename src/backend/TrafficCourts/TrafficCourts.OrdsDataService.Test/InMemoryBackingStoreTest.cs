using TrafficCourts.OrdsDataService.Occam;

namespace TrafficCourts.OrdsDataService.Test;

public class InMemoryBackingStoreTests
{
    [Fact]
    public void Set_ShouldNotMarkItemDirtyIfTheValueWasntModified()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key = "testKey";
        var value = "testValue";
        store.Set(key, value);
        store.Dirty = false;

        // Act
        store.Set(key, value);

        // Assert
        Assert.False(store.Dirty);
    }

    [Fact]
    public void Set_ShouldAddNewItem_WhenKeyDoesNotExist()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key = "testKey";
        var value = "testValue";

        // Act
        store.Set(key, value);

        // Assert
        var result = store.Get<string>(key);
        Assert.Equal(value, result);
    }

    [Fact]
    public void Set_ShouldUpdateItem_WhenKeyExists()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key = "testKey";
        var initialValue = "initialValue";
        var updatedValue = "updatedValue";

        store.Set(key, initialValue);

        // Act
        store.Set(key, updatedValue);

        // Assert
        Assert.True(store.Dirty);
        var result = store.Get<string>(key);
        Assert.Equal(updatedValue, result);
    }

    [Fact]
    public void Set_ShouldMarkItemAsDirty_WhenAlwaysDirtyIsTrue()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key = "testKey";
        var value = "testValue";

        // Act
        store.Set(key, value, alwaysDirty: true);

        // Assert
        Assert.True(store.Dirty);
        var result = store.Get<string>(key);
        Assert.Equal(value, result);
    }

    [Fact]
    public void Get_ShouldReturnDefault_WhenKeyDoesNotExist()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key = "nonExistentKey";

        // Act
        var result = store.Get<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Dirty_ShouldReturnTrue_WhenAnyItemIsDirty()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key = "testKey";
        var value = "testValue";

        store.Set(key, value);

        // Act
        var isDirty = store.Dirty;

        // Assert
        Assert.True(isDirty);
    }

    [Fact]
    public void Dirty_ShouldReturnFalse_WhenNoItemIsDirty()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key = "testKey";
        var value = "testValue";

        store.Set(key, value);
        store.Dirty = false;

        // Act
        var isDirty = store.Dirty;

        // Assert
        Assert.False(isDirty);
    }

    [Fact]
    public void Dirty_ShouldReturnTrue_WhenAlwaysDirtyIsTrue()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key = "testKey";
        var value = "testValue";

        store.Set(key, value, alwaysDirty: true);

        // Act
        var isDirty = store.Dirty;

        // Assert
        Assert.True(isDirty);
    }



    [Fact]
    public void ToDictionary_ShouldReturnOnlyDirtyItems()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key1 = "key1";
        var value1 = "value1";
        var key2 = "key2";
        var value2 = "value2";

        store.Set(key1, value1);
        store.Set(key2, value2);
        store.Dirty = false;
        store.Set(key1, "newValue1");

        // Act
        var dictionary = store.ToDictionary();

        // Assert
        Assert.Single(dictionary);
        Assert.Equal("newValue1", dictionary[key1]);
    }

    [Fact]
    public void ToDictionary_ShouldIncludeAlwaysDirtyItems()
    {
        // Arrange
        var store = new InMemoryBackingStore();
        var key1 = "key1";
        var value1 = "value1";
        var key2 = "key2";
        var value2 = "value2";

        store.Set(key1, value1, alwaysDirty: true);
        store.Set(key2, value2, alwaysDirty: false);

        // Act
        var dictionary = store.ToDictionary();

        // Assert
        Assert.Equal(2, dictionary.Count);
        Assert.Equal(value1, dictionary[key1]);
        Assert.Equal(value2, dictionary[key2]);
    }

    [Fact]
    public void Set_ShouldThrowArgumentNullException_WhenKeyIsNull()
    {
        // Arrange
        var store = new InMemoryBackingStore();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => store.Set(null, "value"));
    }

    [Fact]
    public void Get_ShouldThrowArgumentNullException_WhenKeyIsNull()
    {
        // Arrange
        var store = new InMemoryBackingStore();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => store.Get<string>(null));
    }
}

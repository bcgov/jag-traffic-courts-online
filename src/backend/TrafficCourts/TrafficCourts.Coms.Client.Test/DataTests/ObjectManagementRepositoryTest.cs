using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TrafficCourts.Coms.Client.Data;
using TrafficCourts.Coms.Client.Data.Models;

using ObjectManagementContext = TrafficCourts.Coms.Client.Data.ObjectManagementContext;

namespace TrafficCourts.Coms.Client.Test.DataTests;

public class ObjectManagementRepositoryTest
{
    private ObjectManagementContext _context;

    public ObjectManagementRepositoryTest()
    {
        var contextOptions = new DbContextOptionsBuilder<ObjectManagementContext>()
            .UseInMemoryDatabase("ObjectManagementContext")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;

        _context = new ObjectManagementContext(contextOptions);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        CreateDatabaseEntities(_context);

        _context.SaveChanges();

    }

    /// <summary>
    /// Test the GetObjectMetadata and GetObjectTags operations.
    /// </summary>
    [Fact]
    public void get_metadata_and_tags_without_version()
    {
        var sut = new ObjectManagementRepository(_context);

        var id = _context.Objects.Select(_ => _.Id).First();

        var metadata = sut.GetObjectMetadata(id);
        var tags = sut.GetObjectTags(id);

        var tag = Assert.Single(tags);
        var metadatum = Assert.Single(metadata);

        Assert.Equal(tag.Value, metadatum.Value);
    }

    [Fact]
    public void get_metadata_and_tags_with_version()
    {
        var sut = new ObjectManagementRepository(_context);

        var id = _context.Objects.Select(_ => _.Id).First();

        var metadata = sut.GetObjectMetadata(id, Guid.NewGuid().ToString());
        var tags = sut.GetObjectTags(id, Guid.NewGuid().ToString());

        Assert.Empty(tags);
        Assert.Empty(metadata);
    }

    [Fact]
    public void guid_empty_shoud_not_return_tags()
    {
        var sut = new ObjectManagementRepository(_context);
        var tags = sut.GetObjectTags(Guid.Empty);
        Assert.Empty(tags);
    }

    [Fact]
    public void guid_empty_shoud_not_return_any_metadata()
    {
        var sut = new ObjectManagementRepository(_context);
        var metadata = sut.GetObjectMetadata(Guid.Empty);
        Assert.Empty(metadata);
    }

    private Guid NextId => Guid.NewGuid();

    private void CreateDatabaseEntities(ObjectManagementContext context)
    {
        // create tags and metadata
        for (int i = 1; i <= 10; i++)
        {
            context.Tags.Add(new Tag { Key = $"Tag{i}", Value = $"Value{i}" });
            context.Metadata.Add(new Data.Models.Metadata { Key = $"Metadata{i}", Value = $"Value{i}" });
        }

        context.SaveChanges();

        for (int i = 1; i <= 5; i++)
        {
            Guid versionId = NextId;
            Guid objectId = NextId;
            string createdBy = NextId.ToString("d");
            string updatedBy = NextId.ToString("d");

            var obj = new Data.Models.Object { Id = objectId, Path = "/", Public = false, Active = true, CreatedBy = createdBy, UpdatedBy = updatedBy };
            var ver = new Data.Models.Version { Id = versionId, ObjectId = objectId, MimeType = "application/pdf", DeleteMarker = false, CreatedBy = createdBy, UpdatedBy = updatedBy };

            context.Objects.Add(obj);
            context.Versions.Add(ver);

            // link this version to a tag
            var tagN = context.Tags.Single(_ => _.Key == $"Tag{i}");
            var metaN = context.Metadata.Single(_ => _.Key == $"Metadata{i}");

            VersionTag versionTag = new VersionTag { Version = ver, Tag = tagN, CreatedBy = createdBy, UpdatedBy = updatedBy };
            VersionMetadatum versionMetadatum = new VersionMetadatum { Version = ver, Metadata = metaN, CreatedBy = createdBy, UpdatedBy = updatedBy };

            context.VersionTags.Add(versionTag);
            context.VersionMetadata.Add(versionMetadatum);
        }

        context.SaveChanges();
    }


}

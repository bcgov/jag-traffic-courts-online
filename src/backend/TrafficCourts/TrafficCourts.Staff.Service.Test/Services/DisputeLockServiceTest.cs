using System;
using System.Collections.Generic;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Services;

public class DisputeLockServiceTest
{
    [Fact]
    public void GetLockTest()
    {
        // Arrange
        var disputeLockService = new DisputeLockService();

        // Act
        var result = disputeLockService.GetLock("ticketNumber", "username");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ticketNumber", result.TicketNumber);
        Assert.Equal("username", result.Username);
        Assert.True(DateTime.UtcNow < result.ExpiryTimeUtc);
    }

    [Fact]
    public void GetLockTest_LockIsInUseException()
    {
        // Arrange
        var disputeLockService = new DisputeLockService();
        var result = disputeLockService.GetLock("ticketNumber", "username");

        // Act
        var exception = Assert.Throws<LockIsInUseException>(() => disputeLockService.GetLock("ticketNumber", "anotherUsername"));

        // Assert
        Assert.Equal("username", exception.Username);
        Assert.Equal(result, exception.Lock);
    }

    [Fact]
    public void RefreshLockTest()
    {
        // Arrange
        var disputeLockService = new DisputeLockService();
        var result = disputeLockService.GetLock("ticketNumber", "username");

        // Act
        var refreshResult = disputeLockService.RefreshLock(result!.LockId!, "username");

        // Assert
        Assert.NotNull(refreshResult);
        Assert.True(DateTime.UtcNow < refreshResult);
    }

    [Fact]
    public void RefreshLockTest_LockIsInUseException()
    {
        // Arrange
        var disputeLockService = new DisputeLockService();
        var result = disputeLockService.GetLock("ticketNumber", "username");

        // Act
        var exception = Assert.Throws<LockIsInUseException>(() => disputeLockService.RefreshLock(result!.LockId!, "anotherUsername"));

        // Assert
        Assert.Equal("username", exception.Username);
        Assert.Equal(result, exception.Lock);
    }

    [Fact]
    public void RefreshLockTest_LockNotFound()
    {
        // Arrange
        var disputeLockService = new DisputeLockService();

        // Act
        var result = disputeLockService.RefreshLock("lockId", "username");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ReleaseLockTest()
    {
        // Arrange
        var disputeLockService = new DisputeLockService();
        var result = disputeLockService.GetLock("ticketNumber", "username");

        // Act
        disputeLockService.ReleaseLock(result!.LockId!);

        // Assert
        Assert.Empty(disputeLockService.GetLocks());
    }

    [Fact]
    public void ReleaseLockTest_LockNotFound()
    {
        // Arrange
        var disputeLockService = new DisputeLockService();

        // Act
        var exception = Assert.Throws<KeyNotFoundException>(() => disputeLockService.ReleaseLock("lockId"));

        // Assert
        Assert.Equal("Lock with id lockId not found", exception.Message);
    }
}

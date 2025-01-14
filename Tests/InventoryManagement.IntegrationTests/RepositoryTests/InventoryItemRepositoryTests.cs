using FluentAssertions;

using InventoryManagement.Api.Features.InventoryItems;
using InventoryManagement.Api.Infrastructure.Database.Repositories;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.IntegrationTests.RepositoryTests;
public class InventoryItemRepositoryTests : BaseRepositoryIntegrationTest
{
    [Test]
    public async Task AddItem_OnNewItem_ShouldCreateNewItemInDatabase()
    {
        // Arrange
        InventoryItem newItem = InventoryItem.Create(@"SampleItem1", @"SampleItem", @"M");
        InventoryItemRepository repository = new(DbContext);

        await DbContext.InventoryItems.ExecuteDeleteAsync();

        // Act
        repository.CreateNewItem(newItem);
        await DbContext.SaveChangesAsync();

        // Assert
        InventoryItem? result = await DbContext.InventoryItems.FirstOrDefaultAsync(i => i.InventoryItemId == newItem.InventoryItemId);

        result.Should().NotBeNull();
        result?.InventoryItemId.Should().Be(newItem.InventoryItemId);
        result?.ItemName.Should().Be(newItem.ItemName);
        result?.MeasurementUnit.Should().Be(newItem.MeasurementUnit);
    }

    [Test]
    public async Task DeleteItem_OnDeletingExistingItem_ShouldRemoveItemFromDatabase()
    {
        // Arrange
        InventoryItem sampleItem = InventoryItem.Create(@"SampleItem1", @"SampleItem", @"M");
        InventoryItemRepository repository = new(DbContext);

        await DbContext.InventoryItems.ExecuteDeleteAsync();
        DbContext.InventoryItems.Add(sampleItem);
        await DbContext.SaveChangesAsync();

        // Act
        repository.DeleteItem(sampleItem);
        await DbContext.SaveChangesAsync();

        // Assert
        bool isAnyItemInDb = await DbContext.InventoryItems.AnyAsync();
        isAnyItemInDb.Should().BeFalse();
    }

    [TestCase(@"Sample2")]
    [TestCase(@"12345")]
    [TestCase(@"15Sample")]
    public async Task IsItemIdInUse_WithAIdNotInUse_ShouldReturnFalse(string itemIdNotInUse)
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        InventoryItem sampleItem = InventoryItem.Create(@"SampleItem1", @"SampleItem", @"M");
        InventoryItemRepository repository = new(DbContext);

        await DbContext.InventoryItems.ExecuteDeleteAsync(tokenSource.Token);
        DbContext.InventoryItems.Add(sampleItem);
        await DbContext.SaveChangesAsync(tokenSource.Token);

        // Act
        bool isInUse = await repository.IsInventoryItemIdInUseAsync(itemIdNotInUse, tokenSource.Token);

        // Assert
        isInUse.Should().BeFalse();
    }

    [Test]
    public async Task IsItemIdInUse_WithAIdInUse_ShouldReturnTrue()
    {
        // Arrange
        const string itemId = @"SampleItem001";
        using CancellationTokenSource tokenSource = new();
        InventoryItem sampleItem = InventoryItem.Create(itemId, @"SampleItem", @"M");
        InventoryItemRepository repository = new(DbContext);

        await DbContext.InventoryItems.ExecuteDeleteAsync(tokenSource.Token);
        DbContext.InventoryItems.Add(sampleItem);
        await DbContext.SaveChangesAsync(tokenSource.Token);

        // Act
        bool isInUse = await repository.IsInventoryItemIdInUseAsync(itemId, tokenSource.Token);

        // Assert
        isInUse.Should().BeTrue();
    }

    [Test]
    public async Task GetInventoryItemById_WithIdOfAnExistingItemInDatabase_ShouldReturnItem()
    {
        // Arrange
        const string itemId = @"SampleItem001";
        using CancellationTokenSource tokenSource = new();
        InventoryItem sampleItem = InventoryItem.Create(itemId, @"SampleItem", @"M");
        InventoryItemRepository repository = new(DbContext);

        await DbContext.InventoryItems.ExecuteDeleteAsync(tokenSource.Token);
        DbContext.InventoryItems.Add(sampleItem);
        await DbContext.SaveChangesAsync(tokenSource.Token);

        // Act
        InventoryItem? item = await repository.GetInventoryItemByIdAsync(itemId, tokenSource.Token);

        // Assert
        item.Should().NotBeNull();
        item?.InventoryItemId.Should().Be(itemId);
    }

    [TestCase(@"Sample2")]
    [TestCase(@"12345")]
    [TestCase(@"15Sample")]
    public async Task GetInventoryItemById_WithIdOfNonExistingItemInDatabase_ShouldReturnNull(string itemIdOfNonExistingItem)
    {
        // Arrange
        using CancellationTokenSource tokenSource = new();
        InventoryItem sampleItem = InventoryItem.Create(@"SampleItem001", @"SampleItem", @"M");
        InventoryItemRepository repository = new(DbContext);

        await DbContext.InventoryItems.ExecuteDeleteAsync(tokenSource.Token);
        DbContext.InventoryItems.Add(sampleItem);
        await DbContext.SaveChangesAsync(tokenSource.Token);

        // Act
        InventoryItem? item = await repository.GetInventoryItemByIdAsync(itemIdOfNonExistingItem, tokenSource.Token);

        // Assert
        item.Should().BeNull();
    }
}

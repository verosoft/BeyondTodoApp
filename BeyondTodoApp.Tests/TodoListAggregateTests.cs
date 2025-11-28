using BeyondTodoDomain;
using BeyondTodoDomain.Entities;
using BeyondTodoDomain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeyondTodoApp.Tests;

public class TodoListAggregateTests
{
    private readonly Mock<ITodoListRepository> _repositoryMock;
    private readonly Mock<ILogger<TodoListAggregate>> _loggerMock;
    private readonly TodoListAggregate _aggregate;

    public TodoListAggregateTests()
    {
        _repositoryMock = new Mock<ITodoListRepository>();
        _loggerMock = new Mock<ILogger<TodoListAggregate>>();
        _aggregate = new TodoListAggregate(_repositoryMock.Object, _loggerMock.Object);

        // Setup default valid categories for most tests
        var validCategories = new List<string> { "Work", "Home", "Personal" };
        _repositoryMock.Setup(r => r.GetAllCategories()).Returns(validCategories);
    }

    [Fact]
    public void AddItem_WithValidCategory_ShouldAddItemToList()
    {
        // Arrange
        var id = 1;
        var title = "New Task";
        var description = "Task Description";
        var category = "Work";

        // Act
        _aggregate.AddItem(id, title, description, category);

        // Assert
        var items = _aggregate.GetItemsForPersistence();
        Assert.Single(items);
        var item = items.First();
        Assert.Equal(id, item.Id);
        Assert.Equal(title, item.Title);
        Assert.Equal(description, item.Description);
        Assert.Equal(category, item.Category);
    }

    [Fact]
    public void AddItem_WithInvalidCategory_ShouldThrowException()
    {
        // Arrange
        var invalidCategory = "InvalidCategory";

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _aggregate.AddItem(1, "Title", "Desc", invalidCategory));
        Assert.Equal($"La categoría '{invalidCategory}' no es válida.", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Fact]
    public void UpdateItem_ExistingItem_ShouldUpdateDescription()
    {
        // Arrange
        var id = 1;
        var initialDescription = "Initial";
        var updatedDescription = "Updated";
        _aggregate.AddItem(id, "Title", initialDescription, "Work");

        // Act
        _aggregate.UpdateItem(id, updatedDescription);

        // Assert
        var item = _aggregate.GetItemsForPersistence().First();
        Assert.Equal(updatedDescription, item.Description);
    }

    [Fact]
    public void UpdateItem_NonExistentItem_ShouldDoNothing()
    {
        // Arrange
        _aggregate.AddItem(1, "Title", "Description", "Work");

        // Act
        _aggregate.UpdateItem(2, "Some update");

        // Assert
        var item = _aggregate.GetItemsForPersistence().First();
        Assert.Equal("Description", item.Description); // Unchanged
    }

    [Fact]
    public void UpdateItem_CompletedItem_ShouldThrowException()
    {
        // Arrange
        var id = 1;
        _aggregate.AddItem(id, "Title", "Description", "Work");
        _aggregate.RegisterProgression(id, DateTime.Now, 50m);
        _aggregate.RegisterProgression(id, DateTime.Now, 50m);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _aggregate.UpdateItem(id, "New Description"));
        Assert.Equal($"Error actualizando item {id}", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }


    [Fact]
    public void RemoveItem_ExistingItem_ShouldBeRemoved()
    {
        // Arrange
        _aggregate.AddItem(1, "Title", "Description", "Work");

        // Act
        _aggregate.RemoveItem(1);

        // Assert
        Assert.Empty(_aggregate.GetItemsForPersistence());
    }

    [Fact]
    public void RemoveItem_NonExistentItem_ShouldDoNothing()
    {
        // Arrange
        _aggregate.AddItem(1, "Title", "Description", "Work");

        // Act
        _aggregate.RemoveItem(2);

        // Assert
        Assert.Single(_aggregate.GetItemsForPersistence());
    }

    [Fact]
    public void RemoveItem_CompletedItem_ShouldThrowException()
    {
        // Arrange
        var id = 1;
        _aggregate.AddItem(id, "Title", "Description", "Work");
        _aggregate.RegisterProgression(id, DateTime.Now, 50m);
        _aggregate.RegisterProgression(id, DateTime.Now, 50m);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _aggregate.RemoveItem(id));
        Assert.Equal($"Error eliminando item {id}", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
       
    }

    [Fact]
    public void RegisterProgression_ExistingItem_ShouldAddProgress()
    {
        // Arrange
        var id = 1;
        _aggregate.AddItem(id, "Title", "Description", "Work");
        var progressDate = DateTime.UtcNow;
        var progressPercent = 50m;

        // Act
        _aggregate.RegisterProgression(id, progressDate, progressPercent);

        // Assert
        var item = _aggregate.GetItemsForPersistence().First();
        Assert.Single(item.Progressions);
        var progress = item.Progressions.First();
        Assert.Equal(progressDate, progress.Date);
        Assert.Equal(progressPercent, progress.Percent);
    }

    [Fact]
    public void RegisterProgression_NonExistentItem_ShouldDoNothing()
    {
        // Act
        _aggregate.RegisterProgression(1, DateTime.Now, 50m);

        // Assert
        Assert.Empty(_aggregate.GetItemsForPersistence());
    }
    
    [Fact]
    public void RegisterProgression_CompletedItem_ShouldThrowException()
    {
        // Arrange
        var id = 1;
        _aggregate.AddItem(id, "Title", "Description", "Work");
        _aggregate.RegisterProgression(id, DateTime.Now, 50m);
         _aggregate.RegisterProgression(id, DateTime.Now, 50m);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _aggregate.RegisterProgression(id, DateTime.Now, 10m));
        Assert.Equal($"Error registrando progresión en item {id}", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
       
    }

    [Fact]
    public void GetItemsForPersistence_AfterAddingItems_ShouldReturnReadOnlyList()
    {
        // Arrange
        _aggregate.AddItem(1, "Title 1", "Desc 1", "Work");
        _aggregate.AddItem(2, "Title 2", "Desc 2", "Home");

        // Act
        var items = _aggregate.GetItemsForPersistence();

        // Assert
        Assert.Equal(2, items.Count);
        Assert.IsAssignableFrom<IReadOnlyList<TodoItem>>(items);
    }

    [Fact]
    public void GetItemsForPersistence_NoItems_ShouldReturnEmptyReadOnlyList()
    {
        // Act
        var items = _aggregate.GetItemsForPersistence();

        // Assert
        Assert.Empty(items);
        Assert.IsAssignableFrom<IReadOnlyList<TodoItem>>(items);
    }
}

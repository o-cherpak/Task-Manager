using Microsoft.EntityFrameworkCore;
using TaskManager.Exceptions;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Tests;

public class TodoManagerTest : IDisposable
{
    private readonly TodoManager _tm;
    private readonly TodoDbContext _db;

    public TodoManagerTest()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new TodoDbContext(options);
        _tm = new TodoManager(_db);
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }

    [Theory]
    [InlineData("MyNewTitleForTesting")]
    [InlineData("BlackBlack")]
    [InlineData("WhiteYellow")]
    [InlineData("Orange")]
    [InlineData("Dinner")]
    public void AddTodoItemTest(string title)
    {
        var todo = _tm.AddTodoItem(title);
        var todoWithHighPriority = _tm.AddTodoItem(title, TodoPriority.High);

        Assert.Equal(TodoStatus.Pending, todo.Status);
        Assert.Equal(TodoPriority.High, todoWithHighPriority.Priority);
        Assert.Equal(title, todo.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void AddTodoItem_EmptyTitleTest(string title)
    {
        Assert.Throws<ArgumentException>(() => _tm.AddTodoItem(title));
    }

    [Theory]
    [InlineData("White")]
    [InlineData("Orange")]
    [InlineData("Marimbo")]
    [InlineData("Lunch")]
    public void SetCompleteTest(string title)
    {
        var todo1 = _tm.AddTodoItem(title);
        var todo2 = _tm.AddTodoItem(title);

        _tm.SetComplete(todo1.Id);
        var todo1Updated = _tm.GetTodoById(todo1.Id);
        var todo2Updated = _tm.GetTodoById(todo2.Id);

        Assert.Equal(TodoStatus.Done, todo1Updated.Status);
        Assert.Equal(TodoStatus.Pending, todo2Updated.Status);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000001")]
    [InlineData("00000000-0000-0000-0000-000000000002")]
    [InlineData("00000000-0000-0000-0000-000000000003")]
    public void SetComplete_InvalidIdTest(Guid id)
    {
        Assert.Throws<TodoNotFoundException>(() => _tm.SetComplete(id));
    }

    [Fact]
    public void GetTodoById_AfterDeleteTest()
    {
        var title = "Banana";
        var todo = _tm.AddTodoItem(title);

        _tm.Delete(todo.Id);

        Assert.Throws<TodoNotFoundException>(() => _tm.GetTodoById(todo.Id));
    }

    [Fact]
    public void Delete_ExistingItemTest()
    {
        var todoItem1 = _tm.AddTodoItem("todo1");
        var todoItem2 = _tm.AddTodoItem("todo2");

        _tm.Delete(todoItem1.Id);
        _tm.Delete(todoItem2.Id);

        Assert.True(!_tm.GetAll().Any());
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000001")]
    [InlineData("00000000-0000-0000-0000-000000000002")]
    [InlineData("00000000-0000-0000-0000-000000000003")]
    public void Delete_InvalidIdTest(Guid id)
    {
        Assert.Throws<TodoNotFoundException>(() => _tm.Delete(id));
    }

    [Fact]
    public void GetActiveTest()
    {
        var todo1 = _tm.AddTodoItem("todo1");
        _tm.AddTodoItem("todo2");
        _tm.AddTodoItem("todo3");

        _tm.SetComplete(todo1.Id);

        Assert.True(_tm.GetActive().Count() == 2);
    }

    [Fact]
    void GetByPriorityTest()
    {
        var todoItem1 = _tm.AddTodoItem("todo1");
        var todoItem2 = _tm.AddTodoItem("todo2", TodoPriority.High);
        var todoItem3 = _tm.AddTodoItem("todo3", TodoPriority.High);

        var result = _tm.GetPriority(TodoPriority.High).ToArray();

        Assert.Equal([todoItem2, todoItem3], result);
    }
}
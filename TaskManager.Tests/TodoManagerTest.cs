using TaskManager.Exceptions;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Tests;

public class TodoManagerTest
{
    private readonly TodoManager _tm;

    public TodoManagerTest()
    {
        _tm = new TodoManager();
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

        Assert.Equal(TodoStatus.Pending, todo.Status);
        Assert.Equal(title, todo.Title);
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

    [Fact]
    public void GetTodoById()
    {
        var title = "Banana";
        var todo = _tm.AddTodoItem(title);

        _tm.Delete(todo.Id);

        Assert.Throws<TodoNotFoundException>(() =>
        {
            var upatedTodo = _tm.GetTodoById(todo.Id);
            _tm.GetTodoById(upatedTodo.Id);
        });
    }
}
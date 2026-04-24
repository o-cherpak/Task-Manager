using TaskManager.Exceptions;
using TaskManager.Models;

namespace TaskManager.Services;

public class TodoManager
{
    private List<TodoItem> _todoItems = new List<TodoItem>();

    public void LoadTodoItems(List<TodoItem> todoItems)
    {
        _todoItems = todoItems;
    }

    public TodoItem AddTodoItem(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.", title);
        }

        TodoItem todoItem = new TodoItem(
            Guid.NewGuid(),
            title,
            TodoStatus.Pending,
            DateTime.Now
        );

        _todoItems.Add(todoItem);

        return todoItem;
    }

    public TodoItem GetTodoById(Guid id)
    {
        return _todoItems.FirstOrDefault(todo => todo.Id == id)
               ?? throw new TodoNotFoundException(id);
    }

    public IEnumerable<TodoItem> GetAll()
    {
        return _todoItems.OrderBy(todo => todo.CreatedAt);
    }

    public IEnumerable<TodoItem> GetActive()
    {
        return _todoItems.Where(todo => todo.Status != TodoStatus.Done);
    }

    public void SetComplete(Guid id)
    {
        var index = _todoItems.FindIndex(todo => todo.Id == id);

        if (index == -1) throw new TodoNotFoundException(id);
        _todoItems[index] = _todoItems[index] with { Status = TodoStatus.Done };
    }

    public void Delete(Guid id)
    {
        TodoItem todo;

        todo = _todoItems.FirstOrDefault(t => t.Id == id)
               ?? throw new TodoNotFoundException(id);

        _todoItems.Remove(todo);
    }
}
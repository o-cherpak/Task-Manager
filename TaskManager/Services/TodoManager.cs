using TaskManager.Models;

namespace TaskManager.Services;

public class TodoManager
{
    private List<TodoItem> _todoItems = new List<TodoItem>();

    public TodoItem AddTodoItem(string title)
    {
        TodoItem todoItem = new TodoItem(
            Guid.NewGuid(),
            title,
            TodoStatus.InProgress,
            DateTime.Now
        );

        _todoItems.Add(todoItem);

        return todoItem;
    }

    public IOrderedEnumerable<TodoItem> GetAll()
    {
        return _todoItems.OrderBy(todo => todo.CreatedAt);
    }

    public IEnumerable<TodoItem> GetActive()
    {
        return _todoItems.Where(todo => todo.Status != TodoStatus.Done);
    }

    public void SetComplete(Guid id)
    {
        var todo = _todoItems.FirstOrDefault(todo => todo.Id == id);

        if (todo is null) return;

        todo = todo with { Status = TodoStatus.Done };
    }

    public void Delete(Guid id)
    {
        var todo = _todoItems.FirstOrDefault(todo => todo.Id == id);

        if (todo is null) return;

        _todoItems.Remove(todo);
    }
}
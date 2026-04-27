using TaskManager.Exceptions;
using TaskManager.Models;

namespace TaskManager.Services;

public class TodoManager
{
    private readonly TodoDbContext _db;

    public TodoManager(TodoDbContext db)
    {
        _db = db;
    }

    public TodoItem AddTodoItem(string title, TodoPriority priority = TodoPriority.Low)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty.", title);
        }

        TodoItem todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Status = TodoStatus.Pending,
            Priority = priority,
            CreatedAt = DateTime.UtcNow
        };

        _db.TodoItems.Add(todoItem);
        _db.SaveChanges();

        return todoItem;
    }

    public TodoItem GetTodoById(Guid id)
    {
        return _db.TodoItems.FirstOrDefault(todo => todo.Id == id)
               ?? throw new TodoNotFoundException(id);
    }

    public IEnumerable<TodoItem> GetAll()
    {
        return _db.TodoItems.OrderBy(todo => todo.CreatedAt);
    }

    public IEnumerable<TodoItem> GetActive()
    {
        return _db.TodoItems.Where(todo => todo.Status != TodoStatus.Done);
    }

    public IEnumerable<TodoItem> GetPriority(TodoPriority priority)
    {
        return _db.TodoItems.Where(todo => todo.Priority == priority);
    }

    public void SetComplete(Guid id)
    {
        var todoItem = _db.TodoItems.FirstOrDefault(todo => todo.Id == id)
                       ?? throw new TodoNotFoundException(id);

        todoItem.Status = TodoStatus.Done;
        _db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        TodoItem todo;

        todo = _db.TodoItems.FirstOrDefault(t => t.Id == id)
               ?? throw new TodoNotFoundException(id);

        _db.TodoItems.Remove(todo);
        _db.SaveChanges();
    }
}
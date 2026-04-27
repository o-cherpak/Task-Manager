using Microsoft.EntityFrameworkCore;
using TaskManager;
using TaskManager.Exceptions;
using TaskManager.Models;

namespace TaskManager.Services;

public class TodoManager
{
    //Todos
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
        var todoItem = GetTodoById(id);

        todoItem.Status = TodoStatus.Done;
        _db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var todo = GetTodoById(id);

        _db.TodoItems.Remove(todo);
        _db.SaveChanges();
    }

    public void SetDeadline(Guid id, DateTime deadline)
    {
        var todo = GetTodoById(id);

        todo.DeadLine = deadline;
        _db.SaveChanges();
    }

    public IEnumerable<TodoItem> GetAllOverdue()
    {
        return _db.TodoItems
            .Where(t => t.DeadLine < DateTime.UtcNow
                        && t.Status != TodoStatus.Done);
    }

    //Categories

    public Category AddCategory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", name);
        }

        Category category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Todos = new List<TodoItem>()
        };

        _db.Categories.Add(category);
        _db.SaveChanges();
        return category;
    }

    public Category GetCategoryById(Guid id)
    {
        return _db.Categories.FirstOrDefault(category => category.Id == id)
               ?? throw new InvalidOperationException("Category not found");
    }

    public void AssignCategory(Guid todoId, Guid categoryId)
    {
        var todoItem = GetTodoById(todoId);
        var category = GetCategoryById(categoryId);

        todoItem.CategoryId = categoryId;
        todoItem.Category = category;
        
        _db.SaveChanges();
    }

    public IEnumerable<TodoItem> GetByCategory(Guid categoryId)
    {
        return _db.TodoItems.Where(t => t.CategoryId == categoryId)
            .Include(t => t.Category);
    }
}
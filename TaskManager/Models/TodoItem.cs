namespace TaskManager.Models;

public record TodoItem(
    Guid Id,
    string Title,
    TodoStatus Status,
    DateTime CreatedAt
);
namespace TaskManager.Models;

public class TodoItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TodoStatus Status { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeadLine { get; set; }
}
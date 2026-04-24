namespace TaskManager.Exceptions;

public class TodoNotFoundException : Exception
{
    public Guid Id { get; }

    public TodoNotFoundException(Guid id) : base($"Todo with id '{id}' was not found.")
    {
        Id = id;
    }
}
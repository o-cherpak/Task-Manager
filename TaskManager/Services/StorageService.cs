using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Models;

namespace TaskManager.Services;

public class StorageService
{
    private string todoPath = Path.Combine(AppContext.BaseDirectory, "todos.json");

    private JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true
    };

    public async Task SaveAsync(List<TodoItem> todoItems)
    {
        var json = JsonSerializer.Serialize(todoItems);

        await File.WriteAllTextAsync(todoPath, json);
    }

    public async Task<List<TodoItem>> LoadAsync()
    {
        if (!File.Exists(todoPath)) return new List<TodoItem>();

        var json = await File.ReadAllTextAsync(todoPath);

        return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
    }
}
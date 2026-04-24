using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Models;

namespace TaskManager.Services;

public class StorageService
{
    private readonly string _todoPath = Path.Combine(AppContext.BaseDirectory, "todos.json");

    private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true
    };

    public async Task SaveAsync(List<TodoItem> todoItems)
    {
        var json = JsonSerializer.Serialize(todoItems, _jsonOptions);

        await File.WriteAllTextAsync(_todoPath, json);
    }

    public async Task<List<TodoItem>> LoadAsync()
    {
        if (!File.Exists(_todoPath)) return new List<TodoItem>();

        var json = await File.ReadAllTextAsync(_todoPath);

        return JsonSerializer.Deserialize<List<TodoItem>>(json, _jsonOptions) ?? new List<TodoItem>();
    }
}
using BeyondTodoDomain.Entities;
using BeyondTodoDomain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BeyondTodoDomain;

public class TodoListAggregate(ITodoListRepository repository, ILogger<TodoListAggregate> logger) : ITodoList
{
    private readonly ITodoListRepository _repository = repository;
    private readonly ILogger<TodoListAggregate> _logger = logger;
    private readonly List<TodoItem> _items = [];

    public void AddItem(int id, string title, string description, string category)
    {
        var validCategories = _repository.GetAllCategories();
        if (!validCategories.Contains(category))
        {
            _logger.LogError("La categoría '{Category}' no es válida.", category);
            throw new Exception($"La categoría '{category}' no es válida.", new InvalidOperationException());
        }
        _items.Add(new TodoItem(id, title, description, category));
    }

    public void UpdateItem(int id, string description)
    {
        var item = _items.FirstOrDefault(i => i.Id == id) ?? throw new Exception($"El item con Id: {id}, no existe", new KeyNotFoundException());

        try
        {
            item.UpdateDescription(description);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando item {Id}", id);
            throw new Exception($"Error actualizando item {id}", new InvalidOperationException());

        }
    }

    public void RemoveItem(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id) ?? throw new Exception($"El item con Id: {id}, no existe", new KeyNotFoundException());
        try
        {
            item.ValidateModificationAllowed();
            _items.Remove(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando item {Id}", id);
            throw new Exception($"Error eliminando item {id}", new InvalidOperationException());

        }
    }

    public void RegisterProgression(int id, DateTime dateTime, decimal percent)
    {
        var item = _items.FirstOrDefault(i => i.Id == id) ?? throw new Exception($"El item con Id: {id}, no existe", new KeyNotFoundException());

        try { item.AddProgression(dateTime, percent); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando progresión en item {Id}", id);
            throw new Exception($"Error registrando progresión en item {id}", new InvalidOperationException());

        }
    }

    public void PrintItems()
    {
        foreach (var item in _items.OrderBy(i => i.Id))
        {
            Console.WriteLine($"{item.Id}) {item.Title} - {item.Description} ({item.Category}) Completed:{item.IsCompleted}");
            decimal accumulated = 0;
            foreach (var prog in item.Progressions)
            {
                accumulated += prog.Percent;
                int barLength = (int)(accumulated / 2);
                string bar = new string('O', barLength).PadRight(50);
                Console.WriteLine($"{prog.Date} - {accumulated}% |{bar}|");
            }
            Console.WriteLine();
        }
    }

    public IReadOnlyList<TodoItem> GetItemsForPersistence() => _items.AsReadOnly();

}
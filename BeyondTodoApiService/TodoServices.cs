using BeyondTodoDomain;
using BeyondTodoDomain.Entities;
using BeyondTodoDomain.Interfaces;

namespace BeyondTodoApiService;

public class TodoServices(ITodoListRepository todoListRepository, ITodoList todoListAggregate, ITodoListDataBaseRepository todoListDataBaseRepository) : ITodoService
{
    private readonly ITodoListRepository _todoListRepository = todoListRepository;
    private readonly ITodoList _todoListAggregate = todoListAggregate;

    private readonly ITodoListDataBaseRepository _todoListDataBaseRepository = todoListDataBaseRepository;

    public Result<bool> CreateNewTodoItem(string title, string description, string category)
    {
        int id = _todoListRepository.GetNextId();

        try
        {
            _todoListAggregate.AddItem(id, title, description, category);
            return Result<bool>.Success(true);

        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.ToString());
        }
    }

    public void DisplayItems()
    {
        Console.WriteLine("\n--- LISTA DE TODO ITEMS ACTUAL ---");
        _todoListAggregate.PrintItems();
        Console.WriteLine("---------------------------------");
    }

    public Result<IReadOnlyList<TodoItem>> GetAllTodos()
    {
        var allTodos = ((TodoListAggregate)_todoListAggregate).GetItemsForPersistence();
        return Result<IReadOnlyList<TodoItem>>.Success(allTodos);
    }

    public Result<bool> RegisterProgress(int id, DateTime dateTime, decimal percent)
    {
        try
        {
            _todoListAggregate.RegisterProgression(id, dateTime, percent);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.ToString());
        }
    }

    public Result<bool> RemoveTodoItem(int id)
    {
        try
        {
            _todoListAggregate.RemoveItem(id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.ToString());
        }
    }

    public void Save()
    {
        _todoListDataBaseRepository.Save((TodoListAggregate)_todoListAggregate);
    }

    public Result<bool> UpdateTodoDescription(int id, string newDescription)
    {
        try
        {
            _todoListAggregate.UpdateItem(id, newDescription);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.ToString());
        }
    }
}


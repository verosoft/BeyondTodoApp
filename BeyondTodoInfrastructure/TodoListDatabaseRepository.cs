using BeyondTodoDomain;
using BeyondTodoDomain.Entities;
using BeyondTodoDomain.Interfaces;

namespace BeyondTodoInfrastructure;

public class TodoListDatabaseRepository : ITodoListDataBaseRepository
{
    public void Save(TodoListAggregate todoListAggregate)
    {
        Console.WriteLine("\n--- Infraestructura: Guardando en Base de Datos ---");

        IReadOnlyList<TodoItem> itemsToSave = todoListAggregate.GetItemsForPersistence();

        foreach (var item in itemsToSave)
        {
            Console.WriteLine($"Guardando TodoItem ID: {item.Id}, Título: {item.Title}");
        }

        Console.WriteLine("--------------------------------------------------\n");


    }
}

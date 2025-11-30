using BeyondTodoDomain.Entities;
using BeyondTodoDomain.Interfaces;

namespace BeyondTodoInfrastructure;

public class TodoListRepository : ITodoListRepository
{
    private int _currentId = 1;

    public int GetNextId() => _currentId++;
    public List<string> GetAllCategories() => ["Work", "Personal", "Hobby"];

}

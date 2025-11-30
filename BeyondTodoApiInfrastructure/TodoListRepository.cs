using BeyondTodoDomain.Interfaces;

namespace BeyondTodoApiInfrastructure;

public class TodoListRepository : ITodoListRepository
{
    private int _currentId = 1;

    public int GetNextId() => _currentId++;
    public List<string> GetAllCategories() => ["Work", "Personal", "Hobby"];

}

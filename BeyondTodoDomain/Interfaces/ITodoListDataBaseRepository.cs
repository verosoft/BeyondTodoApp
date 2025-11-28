namespace BeyondTodoDomain.Interfaces;

public interface ITodoListDataBaseRepository
{
    void Save(TodoListAggregate todoListAggregate);
}

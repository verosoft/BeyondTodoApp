
using BeyondTodoDomain;

namespace BeyondTodoServices;

public interface ITodoService
{
    Result<bool> CreateNewTodoItem(string title, string description, string category);
    Result<bool> RegisterProgress(int id, DateTime dateTime, decimal percent);
    Result<bool> UpdateTodoDescription(int id, string newDescription);
    Result<bool> RemoveTodoItem(int id);
    void DisplayItems();

    void Save();
}

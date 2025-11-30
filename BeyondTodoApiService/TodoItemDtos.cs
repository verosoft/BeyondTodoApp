using System;

namespace BeyondTodoApiService;

public record Progression(DateTime Date, decimal Percent);
public class TodoItemDtos(int id, string title, string description, string category)
{
    public int Id { get; } = id;
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string Category { get; } = category;

    public IReadOnlyCollection<Progression> Progressions { get; set; } = [];

    public decimal CurrentPercent => Progressions.Sum(p => p.Percent);

    public bool IsCompleted => CurrentPercent == 100m;
}

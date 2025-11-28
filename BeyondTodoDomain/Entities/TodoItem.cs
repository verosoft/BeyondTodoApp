namespace BeyondTodoDomain.Entities;

public record Progression(DateTime Date, decimal Percent);

public class TodoItem(int id, string title, string description, string category)
{
    public int Id { get; } = id;
    public string Title { get; } = title;
    public string Description { get; private set; } = description;
    public string Category { get; } = category;

    private readonly List<Progression> _progressions = [];
    public IReadOnlyCollection<Progression> Progressions => _progressions.AsReadOnly();

    public bool IsCompleted => CurrentPercent == 100m;
    public decimal CurrentPercent => _progressions.Sum(p => p.Percent);

    public void UpdateDescription(string newDescription)
    {
        ValidateModificationAllowed();
        Description = newDescription;
    }

    public void AddProgression(DateTime date, decimal percent)
    {
        if (percent <= 0 || percent >= 100)
            throw new InvalidOperationException("El porcentaje debe ser mayor a 0 y menor a 100.");

        if (_progressions.Count > 0 && date <= _progressions.Max(p => p.Date))
            throw new InvalidOperationException("La fecha de la progresión debe ser posterior a la última registrada.");

        if (CurrentPercent + percent > 100)
            throw new InvalidOperationException($"Añadir {percent}% excedería el 100% total (Actual: {CurrentPercent}%).");

        _progressions.Add(new Progression(date, percent));
    }

    public void ValidateModificationAllowed()
    {
        if (CurrentPercent > 50)
            throw new InvalidOperationException("No se puede modificar o eliminar un item con más del 50% de progreso.");
    }
}
using Midterm.Domain.Enums;

namespace Midterm.Domain;

public abstract class BaseTask
{
    public delegate void TaskCompletedHandler(BaseTask completedTask);
    public event TaskCompletedHandler? OnTaskCompleted;

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsCompleted { get; private set; }

    public void CompleteTask()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;
        OnTaskCompleted?.Invoke(this);
    }
}
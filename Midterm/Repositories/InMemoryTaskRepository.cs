using Midterm.Domain;
using Midterm.Domain.Enums;

namespace Midterm.Repositories;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<BaseTask> _tasks;

    public InMemoryTaskRepository()
    {
        _tasks = new List<BaseTask>
        {
            new BugReportTask
            {
                Title = "Login page crashes on empty password",
                SeverityLevel = SeverityLevel.High
            },
            new BugReportTask
            {
                Title = "Tooltip misaligned in Safari",
                SeverityLevel = SeverityLevel.Low
            },
            new FeatureRequestTask
            {
                Title = "Add dark mode support",
                EstimatedHours = 12.5
            },
            new FeatureRequestTask
            {
                Title = "Implement CSV export",
                EstimatedHours = 8.0
            }
        };
    }

    public IEnumerable<BaseTask> GetAll() => _tasks.AsReadOnly();
    public BaseTask? GetById(Guid id) => _tasks.FirstOrDefault(t => t.Id == id);
    public void Add(BaseTask task) => _tasks.Add(task);
}
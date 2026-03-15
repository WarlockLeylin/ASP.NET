using Midterm.Domain;
using Midterm.Domain.Enums;

namespace Midterm.Services;

public static class TaskFilterService
{
    public static IEnumerable<BugReportTask> GetHighSeverityIncompleteBugs(
        IEnumerable<BaseTask> tasks) =>
        tasks
            .OfType<BugReportTask>()
            .Where(t => !t.IsCompleted && t.SeverityLevel == SeverityLevel.High)
            .OrderByDescending(t => t.CreatedAt);

    public static double GetTotalEstimatedHoursForIncompleteFeatures(
        IEnumerable<BaseTask> tasks) =>
        tasks
            .OfType<FeatureRequestTask>()
            .Where(t => !t.IsCompleted)
            .Sum(t => t.EstimatedHours);
}
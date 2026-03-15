using Midterm.Domain.Enums;

namespace Midterm.Domain;

public class BugReportTask : BaseTask
{
    public SeverityLevel SeverityLevel { get; set; }
}